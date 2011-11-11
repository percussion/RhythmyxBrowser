using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RhythmyxBrowser.RXWeb;

namespace RhythmyxBrowser
{
    public partial class RhythmyxBrowser : Form
    {

        private String m_address;
        private String m_port;
        private String m_user;
        private String m_password;
        private RXWeb.securitySOAP m_secSvc;
        private String m_rxSession;
        private contentSOAP m_contentSvc; 
        public RhythmyxBrowser()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerConnectDialog f = new ServerConnectDialog();

            DialogResult ret = f.ShowDialog();

            if (ret.Equals(DialogResult.OK))
            {
                connectToServer(f.ServerAddress,f.ServerPort,f.UserName,f.Password);
            }
        }

        private void connectToServer(String address, String port, String user, String password){

            m_secSvc = new global::RhythmyxBrowser.RXWeb.securitySOAP();

            m_secSvc.Url = "http://localhost:9992/Rhythmyx/webservices/securitySOAP"; // address + ":" + port + "/Rhythmyx/webservices/" + m_secSvc.Url;
       
            CookieContainer  cookies = new CookieContainer();
            m_secSvc.CookieContainer = cookies;

          
            RXWeb.LoginRequest loginReq = new RXWeb.LoginRequest();

            loginReq.Username = user;
            loginReq.Password = password;
            loginReq.LocaleCode = "en-us";
            loginReq.Community = "Enterprise_Investments";
            RXWeb.LoginResponse loginResp = m_secSvc.Login(loginReq);

            m_rxSession = loginResp.PSLogin.sessionId;
            m_secSvc.PSAuthenticationHeaderValue = new PSAuthenticationHeader();
            m_secSvc.PSAuthenticationHeaderValue.Session = m_rxSession;

            m_contentSvc = new contentSOAP();
            m_contentSvc.Url = "http://localhost:9992/Rhythmyx/webservices/contentSOAP";

            m_contentSvc.CookieContainer = m_secSvc.CookieContainer;
            m_contentSvc.PSAuthenticationHeaderValue = m_secSvc.PSAuthenticationHeaderValue;

            FindFolderChildrenRequest folderReq = new FindFolderChildrenRequest();

            folderReq.Folder = new FolderRef();
            folderReq.Folder.Id = 1;

            m_contentSvc.FindFolderChildrenCompleted+= new FindFolderChildrenCompletedEventHandler(this.m_contentSvc_FindFolderChildrenCompleted);
           // m_contentSvc.FindFolderChildrenAsync(folderReq);

           PSItemSummary[] items = m_contentSvc.FindFolderChildren(folderReq);

           addFolderChildren(items, null);

        }

        private void m_contentSvc_FindFolderChildrenCompleted(Object caller, FindFolderChildrenCompletedEventArgs args){

            addFolderChildren(args.Result, null);
        }
        
        private void addFolderChildren(PSItemSummary[] items, TreeNode parent)
        {
            foreach (PSItemSummary i in items)
            {
                TreeNode nd = new TreeNode();
                nd.Tag = i;
                nd.Text = i.name;

                if (i.ContentType == null || i.ContentType.id == 101)
                {
                    nd.ImageIndex = 1;
                    nd.SelectedImageIndex = 2;
                    nd.StateImageIndex = 2;
                }
                else
                {
                    nd.ImageIndex = 3;
                    nd.SelectedImageIndex = 3;
                    nd.StateImageIndex = 3;
                }


                if (parent !=null)
                    parent.Nodes.Add(nd);
                else
                    tvBrowser.Nodes.Add(nd);
                }

        }
        

        private void tvBrowser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Only populate if the node currently has no children.
            if (e.Node.Nodes.Count == 0)
            {
                FindFolderChildrenRequest folderReq = new FindFolderChildrenRequest();

                folderReq.Folder = new FolderRef();
                folderReq.Folder.Id = (e.Node.Tag as PSItemSummary).id;

                PSItemSummary[] items = m_contentSvc.FindFolderChildren(folderReq);
                
                addFolderChildren(items, e.Node);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox f = new AboutBox();

            f.ShowDialog();

        }

        private void tvBrowser_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.SelectedImageIndex == 3)
            {
                PSItemSummary ps = e.Node.Tag as PSItemSummary;


             
                assemblySOAP assSoap = new assemblySOAP();
                assSoap.CookieContainer = m_contentSvc.CookieContainer;
                assSoap.Url = "http://localhost:9992/Rhythmyx/webservices/assemblySOAP";
                assSoap.PSAuthenticationHeaderValue = m_contentSvc.PSAuthenticationHeaderValue;

                LoadAssemblyTemplatesRequest req= new LoadAssemblyTemplatesRequest();
                req.ContentType = ps.ContentType.name;
                
                PSAssemblyTemplate[] templates = assSoap.LoadAssemblyTemplates(req);

                SelectTemplateDialog fTemp = new SelectTemplateDialog();
                PSAssemblyTemplate selectedTemp;
                fTemp.Hide();
                fTemp.LoadTemplates(templates);
                if (fTemp.ShowDialog() == DialogResult.OK)
                {
                    selectedTemp = fTemp.Template;

                    //http://127.0.0.1:9992/Rhythmyx/assembler/render?sys_revision=2&sys_contentid=634&sys_folderid=524&sys_context=0&sys_siteid=301&sys_itemfilter=preview&sys_template=555
                    wbMain.Navigate("http://127.0.0.1:9992/Rhythmyx/assembler/render?sys_contentid=" + (ps.id & 0xFFFFFFFF) + "&sys_context=0&sys_siteid=301&sys_itemfilter=preview&sys_template=" + templates[0].id);
                }
              }
        }


        private static PSItem LoadItem(contentSOAP contentSvc, long id)
        {
            LoadItemsRequest req = new LoadItemsRequest();
            req.Id = new long[] { id };
            req.IncludeBinary = true;
            PSItem[] items = contentSvc.LoadItems(req);
            return items[0];
        }
    }
}
