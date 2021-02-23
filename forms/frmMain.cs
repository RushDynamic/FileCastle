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
    // TODO: Add loading texts (like Discord)
    // TODO: Add custom fonts

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
                if (!lbMain.Items.Contains(file))
                {
                    lbMain.Items.Add(file);
                }
            }
        }
        #endregion
        private async void ProcessFiles(List<string> _filesToConsider, string _key, Tuple<Enums.Actions, long> _actionInfo)
        {
            progressBar.Visible = true;
            try
            {
                fileCastleService.FileProcessed += FileCastleService_FileProcessed;
                var progress = new Progress<int>(percent =>
                {
                    progressBar.Value = percent;
                });
                await Task.Run(() => fileCastleService.ProcessFiles(progress, _filesToConsider, _key, _actionInfo));
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("An error has occured: {0}", ex.Message), "Error");
            }
            finally
            {
                btnMain.Enabled = true;
                AllowDrop = true;
            }
        }

        private void FileCastleService_FileProcessed(object sender, string fileName)
        {
            if (lbMain.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    lbMain.Items.Remove(fileName);
                }));
            }
            else
            {
                lbMain.Items.Remove(fileName);
            }
        }

        #region "UI Components"
        private void BtnMain_Click(object sender, EventArgs e)
        {
            btnMain.Enabled = false;
            AllowDrop = false;
            //lblDropFilesHeading.Text = FileCastleConstants.LABEL_HEADING_WORKING;
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

                    ProcessFiles(filesToConsider, txtKey.Text, fileCastleService.VerifyAllFiles(filesToConsider));
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

        private void btnPassword_Click(object sender, EventArgs e)
        {
            txtKey.UseSystemPasswordChar = !txtKey.UseSystemPasswordChar;
        }

        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (lbMain.SelectedItems.Count > 0)
            {
                lbMain.Items.Remove(lbMain.SelectedItems[0]);
            }
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("Remove all imported files?", "FileCastle", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
            {
                lbMain.Items.Clear();
            }
        }
        #endregion
    }
}
