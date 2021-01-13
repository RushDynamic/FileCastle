using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCastle
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        #region "Drag and drop"
        private void frmMain_Load(object sender, EventArgs e)
        {
            AllowDrop = true;
            DragEnter += new DragEventHandler(frmMain_DragEnter);
            DragDrop += new DragEventHandler(frmMain_DragDrop);
        }

        void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (!lbMain.Items.Contains(file)) {
                    lbMain.Items.Add(file);
                }
            }
        }
        #endregion
    }
}
