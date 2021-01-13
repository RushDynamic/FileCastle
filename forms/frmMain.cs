using FileCastle.service;
using FileCastle.constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCastle
{
    public partial class frmMain : Form
    {
        FileCastleService fileCastleService;
        
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

        private void BtnMain_Click(object sender, EventArgs e)
        {
            fileCastleService = new FileCastleService();
            if (lbMain.Items.Count > 0)
            {
                try
                {
                    List<string> filesToConsider = new List<string>();
                    if (lbMain.SelectedItems.Count > 0)
                    {
                        filesToConsider = lbMain.SelectedItems.Cast<string>().ToList();
                    }
                    else
                    {
                        filesToConsider = lbMain.Items.Cast<string>().ToList();
                    }
                    switch (fileCastleService.VerifyAllFiles(filesToConsider))
                    {
                        case Enums.Actions.Encrypt: //do stuff 
                            break;
                        case Enums.Actions.Decrypt: //do stuff 
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("An error has occured: {0}", ex.Message), "Error");
                }
            }
            else
            {
                MessageBox.Show("Please add some files/directories first.");
            }
        }
    }
}
