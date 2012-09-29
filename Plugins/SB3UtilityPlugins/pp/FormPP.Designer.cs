namespace SB3Utility
{
	partial class FormPP
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
			this.tabControlSubfiles = new System.Windows.Forms.TabControl();
			this.tabPageXXSubfiles = new System.Windows.Forms.TabPage();
			this.xxSubfilesList = new System.Windows.Forms.ListView();
			this.xxSubfilesListHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabPageXASubfiles = new System.Windows.Forms.TabPage();
			this.xaSubfilesList = new System.Windows.Forms.ListView();
			this.xaSubfilesListHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabPageImageSubfiles = new System.Windows.Forms.TabPage();
			this.imageSubfilesList = new System.Windows.Forms.ListView();
			this.imageSubfilesListHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabPageOtherSubfiles = new System.Windows.Forms.TabPage();
			this.otherSubfilesList = new System.Windows.Forms.ListView();
			this.otherSubfilesListHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.comboBoxFormat = new System.Windows.Forms.ComboBox();
			this.label41 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportPPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.reopenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.saveppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveppAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.subfilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportSubfilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.keepBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.tabControlSubfiles.SuspendLayout();
			this.tabPageXXSubfiles.SuspendLayout();
			this.tabPageXASubfiles.SuspendLayout();
			this.tabPageImageSubfiles.SuspendLayout();
			this.tabPageOtherSubfiles.SuspendLayout();
			this.panel1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlSubfiles
			// 
			this.tabControlSubfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlSubfiles.Controls.Add(this.tabPageXXSubfiles);
			this.tabControlSubfiles.Controls.Add(this.tabPageXASubfiles);
			this.tabControlSubfiles.Controls.Add(this.tabPageImageSubfiles);
			this.tabControlSubfiles.Controls.Add(this.tabPageOtherSubfiles);
			this.tabControlSubfiles.Location = new System.Drawing.Point(0, 29);
			this.tabControlSubfiles.Multiline = true;
			this.tabControlSubfiles.Name = "tabControlSubfiles";
			this.tabControlSubfiles.SelectedIndex = 0;
			this.tabControlSubfiles.Size = new System.Drawing.Size(280, 362);
			this.tabControlSubfiles.TabIndex = 123;
			this.tabControlSubfiles.TabStop = false;
			// 
			// tabPageXXSubfiles
			// 
			this.tabPageXXSubfiles.Controls.Add(this.xxSubfilesList);
			this.tabPageXXSubfiles.Location = new System.Drawing.Point(4, 22);
			this.tabPageXXSubfiles.Name = "tabPageXXSubfiles";
			this.tabPageXXSubfiles.Size = new System.Drawing.Size(272, 336);
			this.tabPageXXSubfiles.TabIndex = 0;
			this.tabPageXXSubfiles.Text = ".xx";
			this.tabPageXXSubfiles.UseVisualStyleBackColor = true;
			// 
			// xxSubfilesList
			// 
			this.xxSubfilesList.AutoArrange = false;
			this.xxSubfilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.xxSubfilesListHeader});
			this.xxSubfilesList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.xxSubfilesList.FullRowSelect = true;
			this.xxSubfilesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.xxSubfilesList.HideSelection = false;
			this.xxSubfilesList.LabelWrap = false;
			this.xxSubfilesList.Location = new System.Drawing.Point(0, 0);
			this.xxSubfilesList.Name = "xxSubfilesList";
			this.xxSubfilesList.ShowGroups = false;
			this.xxSubfilesList.ShowItemToolTips = true;
			this.xxSubfilesList.Size = new System.Drawing.Size(272, 336);
			this.xxSubfilesList.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.xxSubfilesList.TabIndex = 4;
			this.xxSubfilesList.TabStop = false;
			this.xxSubfilesList.UseCompatibleStateImageBehavior = false;
			this.xxSubfilesList.View = System.Windows.Forms.View.Details;
			this.xxSubfilesList.DoubleClick += new System.EventHandler(this.xxSubfilesList_DoubleClick);
			this.xxSubfilesList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.xxSubfilesList_KeyPress);
			// 
			// tabPageXASubfiles
			// 
			this.tabPageXASubfiles.Controls.Add(this.xaSubfilesList);
			this.tabPageXASubfiles.Location = new System.Drawing.Point(4, 22);
			this.tabPageXASubfiles.Name = "tabPageXASubfiles";
			this.tabPageXASubfiles.Size = new System.Drawing.Size(272, 336);
			this.tabPageXASubfiles.TabIndex = 2;
			this.tabPageXASubfiles.Text = ".xa";
			this.tabPageXASubfiles.UseVisualStyleBackColor = true;
			// 
			// xaSubfilesList
			// 
			this.xaSubfilesList.AutoArrange = false;
			this.xaSubfilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.xaSubfilesListHeader});
			this.xaSubfilesList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.xaSubfilesList.FullRowSelect = true;
			this.xaSubfilesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.xaSubfilesList.HideSelection = false;
			this.xaSubfilesList.LabelWrap = false;
			this.xaSubfilesList.Location = new System.Drawing.Point(0, 0);
			this.xaSubfilesList.Name = "xaSubfilesList";
			this.xaSubfilesList.ShowGroups = false;
			this.xaSubfilesList.ShowItemToolTips = true;
			this.xaSubfilesList.Size = new System.Drawing.Size(272, 336);
			this.xaSubfilesList.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.xaSubfilesList.TabIndex = 5;
			this.xaSubfilesList.TabStop = false;
			this.xaSubfilesList.UseCompatibleStateImageBehavior = false;
			this.xaSubfilesList.View = System.Windows.Forms.View.Details;
			this.xaSubfilesList.DoubleClick += new System.EventHandler(this.xaSubfilesList_DoubleClick);
			this.xaSubfilesList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.xaSubfilesList_KeyPress);
			// 
			// tabPageImageSubfiles
			// 
			this.tabPageImageSubfiles.Controls.Add(this.imageSubfilesList);
			this.tabPageImageSubfiles.Location = new System.Drawing.Point(4, 22);
			this.tabPageImageSubfiles.Name = "tabPageImageSubfiles";
			this.tabPageImageSubfiles.Size = new System.Drawing.Size(272, 336);
			this.tabPageImageSubfiles.TabIndex = 3;
			this.tabPageImageSubfiles.Text = "Img";
			this.tabPageImageSubfiles.UseVisualStyleBackColor = true;
			// 
			// imageSubfilesList
			// 
			this.imageSubfilesList.AutoArrange = false;
			this.imageSubfilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.imageSubfilesListHeader});
			this.imageSubfilesList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.imageSubfilesList.FullRowSelect = true;
			this.imageSubfilesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.imageSubfilesList.HideSelection = false;
			this.imageSubfilesList.LabelWrap = false;
			this.imageSubfilesList.Location = new System.Drawing.Point(0, 0);
			this.imageSubfilesList.Name = "imageSubfilesList";
			this.imageSubfilesList.ShowGroups = false;
			this.imageSubfilesList.ShowItemToolTips = true;
			this.imageSubfilesList.Size = new System.Drawing.Size(272, 336);
			this.imageSubfilesList.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.imageSubfilesList.TabIndex = 6;
			this.imageSubfilesList.TabStop = false;
			this.imageSubfilesList.UseCompatibleStateImageBehavior = false;
			this.imageSubfilesList.View = System.Windows.Forms.View.Details;
			this.imageSubfilesList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.imageSubfilesList_ItemSelectionChanged);
			// 
			// tabPageOtherSubfiles
			// 
			this.tabPageOtherSubfiles.Controls.Add(this.otherSubfilesList);
			this.tabPageOtherSubfiles.Location = new System.Drawing.Point(4, 22);
			this.tabPageOtherSubfiles.Name = "tabPageOtherSubfiles";
			this.tabPageOtherSubfiles.Size = new System.Drawing.Size(272, 336);
			this.tabPageOtherSubfiles.TabIndex = 1;
			this.tabPageOtherSubfiles.Text = "Other";
			this.tabPageOtherSubfiles.UseVisualStyleBackColor = true;
			// 
			// otherSubfilesList
			// 
			this.otherSubfilesList.AutoArrange = false;
			this.otherSubfilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.otherSubfilesListHeader});
			this.otherSubfilesList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.otherSubfilesList.FullRowSelect = true;
			this.otherSubfilesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.otherSubfilesList.HideSelection = false;
			this.otherSubfilesList.LabelWrap = false;
			this.otherSubfilesList.Location = new System.Drawing.Point(0, 0);
			this.otherSubfilesList.Name = "otherSubfilesList";
			this.otherSubfilesList.ShowGroups = false;
			this.otherSubfilesList.ShowItemToolTips = true;
			this.otherSubfilesList.Size = new System.Drawing.Size(272, 336);
			this.otherSubfilesList.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.otherSubfilesList.TabIndex = 5;
			this.otherSubfilesList.TabStop = false;
			this.otherSubfilesList.UseCompatibleStateImageBehavior = false;
			this.otherSubfilesList.View = System.Windows.Forms.View.Details;
			// 
			// comboBoxFormat
			// 
			this.comboBoxFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxFormat.DropDownHeight = 300;
			this.comboBoxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFormat.IntegralHeight = false;
			this.comboBoxFormat.Location = new System.Drawing.Point(46, 2);
			this.comboBoxFormat.Name = "comboBoxFormat";
			this.comboBoxFormat.Size = new System.Drawing.Size(234, 21);
			this.comboBoxFormat.TabIndex = 141;
			this.comboBoxFormat.TabStop = false;
			// 
			// label41
			// 
			this.label41.AutoSize = true;
			this.label41.Location = new System.Drawing.Point(1, 5);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(39, 13);
			this.label41.TabIndex = 142;
			this.label41.Text = "Format";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.comboBoxFormat);
			this.panel1.Controls.Add(this.tabControlSubfiles);
			this.panel1.Controls.Add(this.label41);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(279, 391);
			this.panel1.TabIndex = 143;
			// 
			// menuStrip1
			// 
			this.menuStrip1.AllowMerge = false;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.subfilesToolStripMenuItem,
            this.optionsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(279, 24);
			this.menuStrip1.TabIndex = 144;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportPPToolStripMenuItem,
            this.toolStripSeparator5,
            this.reopenToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveppToolStripMenuItem,
            this.saveppAsToolStripMenuItem,
            this.toolStripSeparator6,
            this.closeToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// exportPPToolStripMenuItem
			// 
			this.exportPPToolStripMenuItem.Name = "exportPPToolStripMenuItem";
			this.exportPPToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.exportPPToolStripMenuItem.Text = "&Export...";
			this.exportPPToolStripMenuItem.Click += new System.EventHandler(this.exportPPToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(166, 6);
			// 
			// reopenToolStripMenuItem
			// 
			this.reopenToolStripMenuItem.Name = "reopenToolStripMenuItem";
			this.reopenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.reopenToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.reopenToolStripMenuItem.Text = "&Reopen .pp";
			this.reopenToolStripMenuItem.Click += new System.EventHandler(this.reopenToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
			// 
			// saveppToolStripMenuItem
			// 
			this.saveppToolStripMenuItem.Name = "saveppToolStripMenuItem";
			this.saveppToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveppToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.saveppToolStripMenuItem.Text = "&Save .pp";
			this.saveppToolStripMenuItem.Click += new System.EventHandler(this.saveppToolStripMenuItem_Click);
			// 
			// saveppAsToolStripMenuItem
			// 
			this.saveppAsToolStripMenuItem.Name = "saveppAsToolStripMenuItem";
			this.saveppAsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.saveppAsToolStripMenuItem.Text = "Save .pp &As...";
			this.saveppAsToolStripMenuItem.Click += new System.EventHandler(this.saveppAsToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(166, 6);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.closeToolStripMenuItem.Text = "&Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// subfilesToolStripMenuItem
			// 
			this.subfilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportSubfilesToolStripMenuItem,
            this.toolStripSeparator4,
            this.addFilesToolStripMenuItem,
            this.toolStripSeparator2,
            this.removeToolStripMenuItem,
            this.toolStripSeparator3,
            this.renameToolStripMenuItem});
			this.subfilesToolStripMenuItem.Name = "subfilesToolStripMenuItem";
			this.subfilesToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.subfilesToolStripMenuItem.Text = "&Subfiles";
			// 
			// exportSubfilesToolStripMenuItem
			// 
			this.exportSubfilesToolStripMenuItem.Name = "exportSubfilesToolStripMenuItem";
			this.exportSubfilesToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.exportSubfilesToolStripMenuItem.Text = "&Export...";
			this.exportSubfilesToolStripMenuItem.Click += new System.EventHandler(this.exportSubfilesToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(126, 6);
			// 
			// addFilesToolStripMenuItem
			// 
			this.addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
			this.addFilesToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.addFilesToolStripMenuItem.Text = "&Add Files...";
			this.addFilesToolStripMenuItem.Click += new System.EventHandler(this.addFilesToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(126, 6);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.removeToolStripMenuItem.Text = "Re&move";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(126, 6);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.renameToolStripMenuItem.Text = "Re&name";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keepBackupToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.optionsToolStripMenuItem.Text = "&Options";
			// 
			// keepBackupToolStripMenuItem
			// 
			this.keepBackupToolStripMenuItem.Checked = true;
			this.keepBackupToolStripMenuItem.CheckOnClick = true;
			this.keepBackupToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.keepBackupToolStripMenuItem.Name = "keepBackupToolStripMenuItem";
			this.keepBackupToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.keepBackupToolStripMenuItem.Text = "Keep &Backup";
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.RestoreDirectory = true;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Multiselect = true;
			this.openFileDialog1.RestoreDirectory = true;
			// 
			// FormPP
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(279, 415);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormPP";
			this.Text = "FormPP";
			this.ToolTipText = "Blasdh";
			this.tabControlSubfiles.ResumeLayout(false);
			this.tabPageXXSubfiles.ResumeLayout(false);
			this.tabPageXASubfiles.ResumeLayout(false);
			this.tabPageImageSubfiles.ResumeLayout(false);
			this.tabPageOtherSubfiles.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControlSubfiles;
		private System.Windows.Forms.TabPage tabPageXXSubfiles;
		private System.Windows.Forms.ColumnHeader xxSubfilesListHeader;
		private System.Windows.Forms.TabPage tabPageXASubfiles;
		private System.Windows.Forms.ColumnHeader xaSubfilesListHeader;
		private System.Windows.Forms.TabPage tabPageImageSubfiles;
		private System.Windows.Forms.ListView imageSubfilesList;
		private System.Windows.Forms.ColumnHeader imageSubfilesListHeader;
		private System.Windows.Forms.TabPage tabPageOtherSubfiles;
		private System.Windows.Forms.ListView otherSubfilesList;
		private System.Windows.Forms.ColumnHeader otherSubfilesListHeader;
		private System.Windows.Forms.ComboBox comboBoxFormat;
		private System.Windows.Forms.Label label41;
		public System.Windows.Forms.ListView xxSubfilesList;
		public System.Windows.Forms.ListView xaSubfilesList;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveppToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveppAsToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem keepBackupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reopenToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem subfilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem exportPPToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportSubfilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;

	}
}