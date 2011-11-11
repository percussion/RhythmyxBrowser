using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RhythmyxBrowser
{
    public partial class ServerConnectDialog : Form
    {

        public String ServerAddress
        {
            get { return txtServerAddress.Text; }
            set { txtServerAddress.Text = value; }
        }

        public String ServerPort
        {
            get { return txtPortNumber.Text; }
            set { txtPortNumber.Text = value; }
        }

        public String UserName
        {
            get { return txtUserName.Text; }
            set { txtUserName.Text = value; }
        }

        public String Password
        {
            get { return txtPassword.Text; }
            set { txtPassword.Text = value; }
        }

        public ServerConnectDialog()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {

        }
    }
}
