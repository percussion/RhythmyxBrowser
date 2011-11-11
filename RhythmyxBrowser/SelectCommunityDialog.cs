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
    public partial class SelectCommunityDialog : Form
    {

        public String CommunityName
        {
            get
            {
                return cboCommunity.SelectedItem.ToString();
            }
        }

        public SelectCommunityDialog()
        {
            InitializeComponent();
        }
    }
}
