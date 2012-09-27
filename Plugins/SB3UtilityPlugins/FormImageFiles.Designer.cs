namespace SB3Utility
{
	partial class FormImageFiles
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
			this.listViewImages = new System.Windows.Forms.ListView();
			this.imageFilesHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reopenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewImages
			// 
			this.listViewImages.AutoArrange = false;
			this.listViewImages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.imageFilesHeader});
			this.listViewImages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewImages.FullRowSelect = true;
			this.listViewImages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewImages.HideSelection = false;
			this.listViewImages.LabelWrap = false;
			this.listViewImages.Location = new System.Drawing.Point(0, 24);
			this.listViewImages.Name = "listViewImages";
			this.listViewImages.ShowGroups = false;
			this.listViewImages.ShowItemToolTips = true;
			this.listViewImages.Size = new System.Drawing.Size(292, 249);
			this.listViewImages.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewImages.TabIndex = 5;
			this.listViewImages.TabStop = false;
			this.listViewImages.UseCompatibleStateImageBehavior = false;
			this.listViewImages.View = System.Windows.Forms.View.Details;
			this.listViewImages.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewImages_ItemSelectionChanged);
			// 
			// menuStrip1
			// 
			this.menuStrip1.AllowMerge = false;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(292, 24);
			this.menuStrip1.TabIndex = 6;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reopenToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// reopenToolStripMenuItem
			// 
			this.reopenToolStripMenuItem.Name = "reopenToolStripMenuItem";
			this.reopenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.reopenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.reopenToolStripMenuItem.Text = "&Reopen";
			this.reopenToolStripMenuItem.Click += new System.EventHandler(this.reopenToolStripMenuItem_Click);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.closeToolStripMenuItem.Text = "&Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// FormImageFiles
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.listViewImages);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormImageFiles";
			this.Text = "Images";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.ListView listViewImages;
		private System.Windows.Forms.ColumnHeader imageFilesHeader;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reopenToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	}
}