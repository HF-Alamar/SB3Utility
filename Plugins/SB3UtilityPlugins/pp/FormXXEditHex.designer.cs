namespace SB3Utility
{
	partial class FormXXEditHex
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageXX = new System.Windows.Forms.TabPage();
			this.dataGridViewEditorXX = new SB3Utility.DataGridViewEditor();
			this.tabPageFrame = new System.Windows.Forms.TabPage();
			this.dataGridViewEditorFrame = new SB3Utility.DataGridViewEditor();
			this.tabPageMesh = new System.Windows.Forms.TabPage();
			this.dataGridViewEditorMesh = new SB3Utility.DataGridViewEditor();
			this.tabPageSubmesh = new System.Windows.Forms.TabPage();
			this.dataGridViewEditorSubmesh = new SB3Utility.DataGridViewEditor();
			this.tabPageMaterial = new System.Windows.Forms.TabPage();
			this.dataGridViewEditorMaterial = new SB3Utility.DataGridViewEditor();
			this.tabPageTexture = new System.Windows.Forms.TabPage();
			this.dataGridViewEditorTexture = new SB3Utility.DataGridViewEditor();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControl1.SuspendLayout();
			this.tabPageXX.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorXX)).BeginInit();
			this.tabPageFrame.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorFrame)).BeginInit();
			this.tabPageMesh.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorMesh)).BeginInit();
			this.tabPageSubmesh.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorSubmesh)).BeginInit();
			this.tabPageMaterial.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorMaterial)).BeginInit();
			this.tabPageTexture.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorTexture)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(44, 431);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPageXX);
			this.tabControl1.Controls.Add(this.tabPageFrame);
			this.tabControl1.Controls.Add(this.tabPageMesh);
			this.tabControl1.Controls.Add(this.tabPageSubmesh);
			this.tabControl1.Controls.Add(this.tabPageMaterial);
			this.tabControl1.Controls.Add(this.tabPageTexture);
			this.tabControl1.Location = new System.Drawing.Point(12, 31);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(695, 387);
			this.tabControl1.TabIndex = 6;
			// 
			// tabPageXX
			// 
			this.tabPageXX.Controls.Add(this.dataGridViewEditorXX);
			this.tabPageXX.Location = new System.Drawing.Point(4, 22);
			this.tabPageXX.Name = "tabPageXX";
			this.tabPageXX.Size = new System.Drawing.Size(687, 361);
			this.tabPageXX.TabIndex = 2;
			this.tabPageXX.Text = ".xx";
			this.tabPageXX.UseVisualStyleBackColor = true;
			// 
			// dataGridViewEditorXX
			// 
			this.dataGridViewEditorXX.AllowUserToAddRows = false;
			this.dataGridViewEditorXX.AllowUserToDeleteRows = false;
			this.dataGridViewEditorXX.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.dataGridViewEditorXX.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dataGridViewEditorXX.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewEditorXX.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewEditorXX.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewEditorXX.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewEditorXX.Name = "dataGridViewEditorXX";
			this.dataGridViewEditorXX.ShowRowIndex = true;
			this.dataGridViewEditorXX.Size = new System.Drawing.Size(687, 361);
			this.dataGridViewEditorXX.TabIndex = 0;
			// 
			// tabPageFrame
			// 
			this.tabPageFrame.Controls.Add(this.dataGridViewEditorFrame);
			this.tabPageFrame.Location = new System.Drawing.Point(4, 22);
			this.tabPageFrame.Name = "tabPageFrame";
			this.tabPageFrame.Size = new System.Drawing.Size(687, 361);
			this.tabPageFrame.TabIndex = 0;
			this.tabPageFrame.Text = "Frame";
			this.tabPageFrame.UseVisualStyleBackColor = true;
			// 
			// dataGridViewEditorFrame
			// 
			this.dataGridViewEditorFrame.AllowUserToAddRows = false;
			this.dataGridViewEditorFrame.AllowUserToDeleteRows = false;
			this.dataGridViewEditorFrame.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.dataGridViewEditorFrame.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dataGridViewEditorFrame.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewEditorFrame.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewEditorFrame.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewEditorFrame.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewEditorFrame.Name = "dataGridViewEditorFrame";
			this.dataGridViewEditorFrame.ShowRowIndex = true;
			this.dataGridViewEditorFrame.Size = new System.Drawing.Size(687, 361);
			this.dataGridViewEditorFrame.TabIndex = 1;
			// 
			// tabPageMesh
			// 
			this.tabPageMesh.Controls.Add(this.dataGridViewEditorMesh);
			this.tabPageMesh.Location = new System.Drawing.Point(4, 22);
			this.tabPageMesh.Name = "tabPageMesh";
			this.tabPageMesh.Size = new System.Drawing.Size(687, 361);
			this.tabPageMesh.TabIndex = 1;
			this.tabPageMesh.Text = "Mesh";
			this.tabPageMesh.UseVisualStyleBackColor = true;
			// 
			// dataGridViewEditorMesh
			// 
			this.dataGridViewEditorMesh.AllowUserToAddRows = false;
			this.dataGridViewEditorMesh.AllowUserToDeleteRows = false;
			this.dataGridViewEditorMesh.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.dataGridViewEditorMesh.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dataGridViewEditorMesh.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewEditorMesh.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewEditorMesh.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewEditorMesh.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewEditorMesh.Name = "dataGridViewEditorMesh";
			this.dataGridViewEditorMesh.ShowRowIndex = true;
			this.dataGridViewEditorMesh.Size = new System.Drawing.Size(687, 361);
			this.dataGridViewEditorMesh.TabIndex = 1;
			// 
			// tabPageSubmesh
			// 
			this.tabPageSubmesh.Controls.Add(this.dataGridViewEditorSubmesh);
			this.tabPageSubmesh.Location = new System.Drawing.Point(4, 22);
			this.tabPageSubmesh.Name = "tabPageSubmesh";
			this.tabPageSubmesh.Size = new System.Drawing.Size(687, 361);
			this.tabPageSubmesh.TabIndex = 5;
			this.tabPageSubmesh.Text = "Submesh";
			this.tabPageSubmesh.UseVisualStyleBackColor = true;
			// 
			// dataGridViewEditorSubmesh
			// 
			this.dataGridViewEditorSubmesh.AllowUserToAddRows = false;
			this.dataGridViewEditorSubmesh.AllowUserToDeleteRows = false;
			this.dataGridViewEditorSubmesh.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.dataGridViewEditorSubmesh.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dataGridViewEditorSubmesh.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewEditorSubmesh.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewEditorSubmesh.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewEditorSubmesh.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewEditorSubmesh.Name = "dataGridViewEditorSubmesh";
			this.dataGridViewEditorSubmesh.ShowRowIndex = true;
			this.dataGridViewEditorSubmesh.Size = new System.Drawing.Size(687, 361);
			this.dataGridViewEditorSubmesh.TabIndex = 2;
			// 
			// tabPageMaterial
			// 
			this.tabPageMaterial.Controls.Add(this.dataGridViewEditorMaterial);
			this.tabPageMaterial.Location = new System.Drawing.Point(4, 22);
			this.tabPageMaterial.Name = "tabPageMaterial";
			this.tabPageMaterial.Size = new System.Drawing.Size(687, 361);
			this.tabPageMaterial.TabIndex = 3;
			this.tabPageMaterial.Text = "Material";
			this.tabPageMaterial.UseVisualStyleBackColor = true;
			// 
			// dataGridViewEditorMaterial
			// 
			this.dataGridViewEditorMaterial.AllowUserToAddRows = false;
			this.dataGridViewEditorMaterial.AllowUserToDeleteRows = false;
			this.dataGridViewEditorMaterial.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.dataGridViewEditorMaterial.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dataGridViewEditorMaterial.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewEditorMaterial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewEditorMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewEditorMaterial.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewEditorMaterial.Name = "dataGridViewEditorMaterial";
			this.dataGridViewEditorMaterial.ShowRowIndex = true;
			this.dataGridViewEditorMaterial.Size = new System.Drawing.Size(687, 361);
			this.dataGridViewEditorMaterial.TabIndex = 1;
			// 
			// tabPageTexture
			// 
			this.tabPageTexture.Controls.Add(this.dataGridViewEditorTexture);
			this.tabPageTexture.Location = new System.Drawing.Point(4, 22);
			this.tabPageTexture.Name = "tabPageTexture";
			this.tabPageTexture.Size = new System.Drawing.Size(687, 361);
			this.tabPageTexture.TabIndex = 4;
			this.tabPageTexture.Text = "Texture";
			this.tabPageTexture.UseVisualStyleBackColor = true;
			// 
			// dataGridViewEditorTexture
			// 
			this.dataGridViewEditorTexture.AllowUserToAddRows = false;
			this.dataGridViewEditorTexture.AllowUserToDeleteRows = false;
			this.dataGridViewEditorTexture.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.dataGridViewEditorTexture.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dataGridViewEditorTexture.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewEditorTexture.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewEditorTexture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewEditorTexture.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewEditorTexture.Name = "dataGridViewEditorTexture";
			this.dataGridViewEditorTexture.ShowRowIndex = true;
			this.dataGridViewEditorTexture.Size = new System.Drawing.Size(687, 361);
			this.dataGridViewEditorTexture.TabIndex = 1;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(154, 431);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(719, 24);
			this.menuStrip1.TabIndex = 7;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// FormXXEditHex
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(719, 463);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.menuStrip1);
			this.DoubleBuffered = true;
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.MinimizeBox = false;
			this.Name = "FormXXEditHex";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Hex";
			this.tabControl1.ResumeLayout(false);
			this.tabPageXX.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorXX)).EndInit();
			this.tabPageFrame.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorFrame)).EndInit();
			this.tabPageMesh.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorMesh)).EndInit();
			this.tabPageSubmesh.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorSubmesh)).EndInit();
			this.tabPageMaterial.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorMaterial)).EndInit();
			this.tabPageTexture.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewEditorTexture)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TabPage tabPageFrame;
		private System.Windows.Forms.TabPage tabPageMesh;
		private System.Windows.Forms.TabPage tabPageXX;
		private System.Windows.Forms.TabPage tabPageMaterial;
		private System.Windows.Forms.TabPage tabPageTexture;
		private DataGridViewEditor dataGridViewEditorXX;
		private DataGridViewEditor dataGridViewEditorFrame;
		private DataGridViewEditor dataGridViewEditorMesh;
		private DataGridViewEditor dataGridViewEditorMaterial;
		private DataGridViewEditor dataGridViewEditorTexture;
		private System.Windows.Forms.TabPage tabPageSubmesh;
		private DataGridViewEditor dataGridViewEditorSubmesh;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
	}
}