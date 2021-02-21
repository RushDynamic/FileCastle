namespace FileCastle
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyInfo = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnPassword = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnMain = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lbMain = new System.Windows.Forms.ListBox();
            this.removeSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyInfo
            // 
            this.notifyInfo.Text = "notifyIcon1";
            this.notifyInfo.Visible = true;
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeSelectedToolStripMenuItem,
            this.removeAllToolStripMenuItem});
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(181, 70);
            // 
            // btnPassword
            // 
            this.btnPassword.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.btnPassword.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.btnPassword.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.btnPassword.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.btnPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPassword.Location = new System.Drawing.Point(547, 316);
            this.btnPassword.Name = "btnPassword";
            this.btnPassword.Size = new System.Drawing.Size(25, 23);
            this.btnPassword.TabIndex = 23;
            this.btnPassword.Text = "*";
            this.btnPassword.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnPassword.UseVisualStyleBackColor = true;
            this.btnPassword.Click += new System.EventHandler(this.btnPassword_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 299);
            this.progressBar.MarqueeAnimationSpeed = 30;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(560, 11);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 21;
            this.progressBar.Visible = false;
            // 
            // btnMain
            // 
            this.btnMain.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.btnMain.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.btnMain.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.btnMain.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.btnMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMain.Location = new System.Drawing.Point(12, 352);
            this.btnMain.Name = "btnMain";
            this.btnMain.Size = new System.Drawing.Size(560, 25);
            this.btnMain.TabIndex = 18;
            this.btnMain.Text = "Encrypt/Decrypt";
            this.btnMain.UseVisualStyleBackColor = true;
            this.btnMain.Click += new System.EventHandler(this.BtnMain_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Location = new System.Drawing.Point(54, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 25);
            this.label3.TabIndex = 17;
            this.label3.Text = "File Castle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(49, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(486, 50);
            this.label2.TabIndex = 16;
            this.label2.Text = "DROP FILES/FOLDERS HERE";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 319);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "Key";
            // 
            // txtKey
            // 
            this.txtKey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.txtKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKey.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtKey.Location = new System.Drawing.Point(44, 316);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(497, 23);
            this.txtKey.TabIndex = 13;
            this.txtKey.UseSystemPasswordChar = true;
            // 
            // lbMain
            // 
            this.lbMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lbMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbMain.ContextMenuStrip = this.menuMain;
            this.lbMain.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMain.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lbMain.FormattingEnabled = true;
            this.lbMain.ItemHeight = 20;
            this.lbMain.Location = new System.Drawing.Point(12, 91);
            this.lbMain.Name = "lbMain";
            this.lbMain.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbMain.Size = new System.Drawing.Size(560, 202);
            this.lbMain.TabIndex = 22;
            // 
            // removeSelectedToolStripMenuItem
            // 
            this.removeSelectedToolStripMenuItem.Name = "removeSelectedToolStripMenuItem";
            this.removeSelectedToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeSelectedToolStripMenuItem.Text = "Remove Selected";
            this.removeSelectedToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedToolStripMenuItem_Click);
            // 
            // removeAllToolStripMenuItem
            // 
            this.removeAllToolStripMenuItem.Name = "removeAllToolStripMenuItem";
            this.removeAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeAllToolStripMenuItem.Text = "Remove All";
            this.removeAllToolStripMenuItem.Click += new System.EventHandler(this.removeAllToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.ClientSize = new System.Drawing.Size(584, 389);
            this.Controls.Add(this.btnPassword);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnMain);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.lbMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Castle";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyInfo;
        private System.Windows.Forms.ContextMenuStrip menuMain;
        private System.Windows.Forms.Button btnPassword;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnMain;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.ListBox lbMain;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllToolStripMenuItem;
    }
}

