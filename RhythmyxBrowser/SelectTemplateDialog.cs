using System;
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
    public partial class SelectTemplateDialog : Form
    {

        public PSAssemblyTemplate Template
        {
            get { return cboTemplate.SelectedItem as PSAssemblyTemplate; }
        }

        public void LoadTemplates(PSAssemblyTemplate[] templates)
        {
            cboTemplate.Items.Clear();
            cboTemplate.ValueMember = "id";
            cboTemplate.DisplayMember = "label";
            cboTemplate.Items.AddRange(templates);
            

        }


        public SelectTemplateDialog()
        {
            InitializeComponent();
        }
    }
}
