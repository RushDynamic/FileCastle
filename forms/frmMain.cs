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
        enum Actions
        {
            Encrypt,
            Decrypt
        }
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

        private Actions VerifyAllFiles()
        {
            int encryptedFilesCount = 0;
            int filesCount;
            if (lbMain.SelectedItems.Count > 0)
            {
                filesCount = lbMain.SelectedItems.Count;
                foreach (string fileName in lbMain.SelectedItems)
                {
                    VerifyCurrentFile(fileName, ref encryptedFilesCount);
                }
            }
            else
            {
                filesCount = lbMain.Items.Count;
                foreach (string fileName in lbMain.Items)
                {
                    VerifyCurrentFile(fileName, ref encryptedFilesCount);
                }
            }
            if ((encryptedFilesCount > 0) && (encryptedFilesCount == filesCount))
            {
                return Actions.Decrypt;
            }
            else if (encryptedFilesCount == 0)
            {
                return Actions.Encrypt;
            }
            else
            {
                throw new Exception("Invalid selection of files.");
            }
        }

        private void VerifyCurrentFile(string _fileName, ref int _encryptedFilesCount)
        {
            if (Directory.Exists(_fileName))
            {
                DirectoryInfo dir = new DirectoryInfo(_fileName);
                var files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    VerifyCurrentFile(file.FullName, ref _encryptedFilesCount);
                }
            }
            else
            {
                if(new FileInfo(_fileName).Extension == ".castle")
                {
                    _encryptedFilesCount ++;
                }
            }
        }

        private void BtnMain_Click(object sender, EventArgs e)
        {
            if (lbMain.Items.Count > 0)
            {
                try
                {
                    switch(VerifyAllFiles())
                    {
                        case Actions.Decrypt: MessageBox.Show("Decryption"); break;
                        case Actions.Encrypt: MessageBox.Show("Encryption"); break;
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
