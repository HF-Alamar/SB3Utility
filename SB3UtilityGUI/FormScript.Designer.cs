namespace SB3Utility
{
	partial class FormScript
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
			this.richTextBoxScript = new System.Windows.Forms.RichTextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.stuffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.quickSaveSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runQuickSavedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.captureCommandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.autosaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quickSavedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// richTextBoxScript
			// 
			this.richTextBoxScript.DetectUrls = false;
			this.richTextBoxScript.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBoxScript.HideSelection = false;
			this.richTextBoxScript.Location = new System.Drawing.Point(0, 24);
			this.richTextBoxScript.Name = "richTextBoxScript";
			this.richTextBoxScript.Size = new System.Drawing.Size(292, 249);
			this.richTextBoxScript.TabIndex = 0;
			this.richTextBoxScript.Text = "";
			this.richTextBoxScript.WordWrap = false;
			// 
			// menuStrip1
			// 
			this.menuStrip1.AllowDrop = true;
			this.menuStrip1.AllowMerge = false;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stuffToolStripMenuItem,
            this.scriptToolStripMenuItem,
            this.quickSavedToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.ShowItemToolTips = true;
			this.menuStrip1.Size = new System.Drawing.Size(292, 24);
			this.menuStrip1.TabIndex = 199;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// stuffToolStripMenuItem
			// 
			this.stuffToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveAsToolStripMenuItem});
			this.stuffToolStripMenuItem.Name = "stuffToolStripMenuItem";
			this.stuffToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.stuffToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Image = global::SB3Utility.Properties.Resources.openToolStripMenuItem;
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(122, 6);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// scriptToolStripMenuItem
			// 
			this.scriptToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.runSelectedToolStripMenuItem,
            this.toolStripSeparator2,
            this.quickSaveSelectedToolStripMenuItem,
            this.runQuickSavedToolStripMenuItem,
            this.toolStripSeparator4,
            this.captureCommandsToolStripMenuItem,
            this.autosaveToolStripMenuItem,
            this.toolStripSeparator3,
            this.clearToolStripMenuItem});
			this.scriptToolStripMenuItem.Name = "scriptToolStripMenuItem";
			this.scriptToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.scriptToolStripMenuItem.Text = "&Script";
			// 
			// runToolStripMenuItem
			// 
			this.runToolStripMenuItem.Name = "runToolStripMenuItem";
			this.runToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.runToolStripMenuItem.Text = "&Run";
			this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
			// 
			// runSelectedToolStripMenuItem
			// 
			this.runSelectedToolStripMenuItem.Name = "runSelectedToolStripMenuItem";
			this.runSelectedToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.runSelectedToolStripMenuItem.Text = "R&un Selected";
			this.runSelectedToolStripMenuItem.Click += new System.EventHandler(this.runSelectedToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(168, 6);
			// 
			// quickSaveSelectedToolStripMenuItem
			// 
			this.quickSaveSelectedToolStripMenuItem.Name = "quickSaveSelectedToolStripMenuItem";
			this.quickSaveSelectedToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.quickSaveSelectedToolStripMenuItem.Text = "Quick &Save Selected";
			this.quickSaveSelectedToolStripMenuItem.Click += new System.EventHandler(this.quickSaveSelectedToolStripMenuItem_Click);
			// 
			// runQuickSavedToolStripMenuItem
			// 
			this.runQuickSavedToolStripMenuItem.Name = "runQuickSavedToolStripMenuItem";
			this.runQuickSavedToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.runQuickSavedToolStripMenuItem.Text = "Run &Quick Saved";
			this.runQuickSavedToolStripMenuItem.Click += new System.EventHandler(this.runQuickSavedToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(168, 6);
			// 
			// captureCommandsToolStripMenuItem
			// 
			this.captureCommandsToolStripMenuItem.Checked = true;
			this.captureCommandsToolStripMenuItem.CheckOnClick = true;
			this.captureCommandsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.captureCommandsToolStripMenuItem.Name = "captureCommandsToolStripMenuItem";
			this.captureCommandsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.captureCommandsToolStripMenuItem.Text = "Ca&pture Commands";
			// 
			// autosaveToolStripMenuItem
			// 
			this.autosaveToolStripMenuItem.Checked = true;
			this.autosaveToolStripMenuItem.CheckOnClick = true;
			this.autosaveToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.autosaveToolStripMenuItem.Name = "autosaveToolStripMenuItem";
			this.autosaveToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.autosaveToolStripMenuItem.Text = "&Autosave";
			this.autosaveToolStripMenuItem.CheckedChanged += new System.EventHandler(this.autosaveToolStripMenuItem_CheckedChanged);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(168, 6);
			// 
			// clearToolStripMenuItem
			// 
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.clearToolStripMenuItem.Text = "&Clear";
			this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
			// 
			// quickSavedToolStripMenuItem
			// 
			this.quickSavedToolStripMenuItem.Enabled = false;
			this.quickSavedToolStripMenuItem.Name = "quickSavedToolStripMenuItem";
			this.quickSavedToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
			this.quickSavedToolStripMenuItem.Text = "Quick Saved";
			this.quickSavedToolStripMenuItem.Visible = false;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.RestoreDirectory = true;
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.RestoreDirectory = true;
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 100;
			this.toolTip1.ReshowDelay = 100;
			// 
			// FormScript
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.richTextBoxScript);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormScript";
			this.Text = "FormScript";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem stuffToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem scriptToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem runSelectedToolStripMenuItem;
		public System.Windows.Forms.RichTextBox richTextBoxScript;
		private System.Windows.Forms.ToolStripMenuItem captureCommandsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolStripMenuItem autosaveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem quickSaveSelectedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem runQuickSavedToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem quickSavedToolStripMenuItem;
		private System.Windows.Forms.ToolTip toolTip1;


	}
}