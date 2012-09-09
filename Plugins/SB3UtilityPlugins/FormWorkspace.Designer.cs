namespace SB3Utility
{
	partial class FormWorkspace
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.nodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStripSubmesh = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.targetPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.materialNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.worldCoordinatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStripMorphKeyframe = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.renameToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.treeView = new SB3Utility.TriStateTreeView();
			this.toolStripTextBoxTargetPosition = new SB3Utility.ToolStripEditTextBox();
			this.toolStripTextBoxMaterialName = new SB3Utility.ToolStripEditTextBox();
			this.toolStripEditTextBoxNewMorphKeyframeName = new SB3Utility.ToolStripEditTextBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.contextMenuStripSubmesh.SuspendLayout();
			this.contextMenuStripMorphKeyframe.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonRemove);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 334);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(227, 39);
			this.panel1.TabIndex = 6;
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonRemove.Location = new System.Drawing.Point(75, 9);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonRemove.TabIndex = 2;
			this.buttonRemove.TabStop = false;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(2, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(224, 26);
			this.label1.TabIndex = 7;
			this.label1.Text = "• Drag and drop from or to other trees.\r\n• Checkboxes may or may not have an effe" +
    "ct.";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(227, 33);
			this.panel2.TabIndex = 8;
			// 
			// menuStrip1
			// 
			this.menuStrip1.AllowMerge = false;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nodesToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(227, 24);
			this.menuStrip1.TabIndex = 9;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// nodesToolStripMenuItem
			// 
			this.nodesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem});
			this.nodesToolStripMenuItem.Name = "nodesToolStripMenuItem";
			this.nodesToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
			this.nodesToolStripMenuItem.Text = "Nodes";
			// 
			// expandAllToolStripMenuItem
			// 
			this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
			this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.expandAllToolStripMenuItem.Text = "Expand All";
			this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
			// 
			// collapseAllToolStripMenuItem
			// 
			this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
			this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.collapseAllToolStripMenuItem.Text = "Collapse All";
			this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
			// 
			// contextMenuStripSubmesh
			// 
			this.contextMenuStripSubmesh.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.targetPositionToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.materialNameToolStripMenuItem,
            this.worldCoordinatesToolStripMenuItem});
			this.contextMenuStripSubmesh.Name = "contextMenuStripSubmesh";
			this.contextMenuStripSubmesh.Size = new System.Drawing.Size(198, 114);
			this.contextMenuStripSubmesh.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripSubmesh_Opening);
			// 
			// targetPositionToolStripMenuItem
			// 
			this.targetPositionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxTargetPosition});
			this.targetPositionToolStripMenuItem.Name = "targetPositionToolStripMenuItem";
			this.targetPositionToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.targetPositionToolStripMenuItem.Text = "Target Position";
			// 
			// replaceToolStripMenuItem
			// 
			this.replaceToolStripMenuItem.Checked = true;
			this.replaceToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
			this.replaceToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.replaceToolStripMenuItem.Text = "Replace Original Submesh";
			this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
			// 
			// materialNameToolStripMenuItem
			// 
			this.materialNameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxMaterialName});
			this.materialNameToolStripMenuItem.Name = "materialNameToolStripMenuItem";
			this.materialNameToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.materialNameToolStripMenuItem.Text = "Material Name";
			// 
			// worldCoordinatesToolStripMenuItem
			// 
			this.worldCoordinatesToolStripMenuItem.Name = "worldCoordinatesToolStripMenuItem";
			this.worldCoordinatesToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.worldCoordinatesToolStripMenuItem.Text = "World Coordinates";
			this.worldCoordinatesToolStripMenuItem.Click += new System.EventHandler(this.worldCoordinatesToolStripMenuItem_Click);
			// 
			// contextMenuStripMorphKeyframe
			// 
			this.contextMenuStripMorphKeyframe.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToToolStripMenuItem});
			this.contextMenuStripMorphKeyframe.Name = "contextMenuStripMorphKeyframe";
			this.contextMenuStripMorphKeyframe.Size = new System.Drawing.Size(153, 48);
			this.contextMenuStripMorphKeyframe.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripMorphKeyframe_Opening);
			// 
			// renameToToolStripMenuItem
			// 
			this.renameToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripEditTextBoxNewMorphKeyframeName});
			this.renameToToolStripMenuItem.Name = "renameToToolStripMenuItem";
			this.renameToToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.renameToToolStripMenuItem.Text = "Rename to";
			// 
			// treeView
			// 
			this.treeView.AllowDrop = true;
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.HideSelection = false;
			this.treeView.Location = new System.Drawing.Point(0, 57);
			this.treeView.Name = "treeView";
			this.treeView.Size = new System.Drawing.Size(227, 277);
			this.treeView.TabIndex = 5;
			this.treeView.TabStop = false;
			this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
			this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
			this.treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
			this.treeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
			// 
			// toolStripTextBoxTargetPosition
			// 
			this.toolStripTextBoxTargetPosition.Name = "toolStripTextBoxTargetPosition";
			this.toolStripTextBoxTargetPosition.Size = new System.Drawing.Size(22, 21);
			this.toolStripTextBoxTargetPosition.AfterEditTextChanged += new System.EventHandler(this.toolStripTextBoxTargetPosition_AfterEditTextChanged);
			// 
			// toolStripTextBoxMaterialName
			// 
			this.toolStripTextBoxMaterialName.AcceptsReturn = true;
			this.toolStripTextBoxMaterialName.MaxLength = 64;
			this.toolStripTextBoxMaterialName.Name = "toolStripTextBoxMaterialName";
			this.toolStripTextBoxMaterialName.Size = new System.Drawing.Size(120, 21);
			this.toolStripTextBoxMaterialName.AfterEditTextChanged += new System.EventHandler(this.toolStripTextBoxMaterialName_AfterEditTextChanged);
			// 
			// toolStripEditTextBoxNewMorphKeyframeName
			// 
			this.toolStripEditTextBoxNewMorphKeyframeName.AcceptsReturn = true;
			this.toolStripEditTextBoxNewMorphKeyframeName.MaxLength = 64;
			this.toolStripEditTextBoxNewMorphKeyframeName.Name = "toolStripEditTextBoxNewMorphKeyframeName";
			this.toolStripEditTextBoxNewMorphKeyframeName.Size = new System.Drawing.Size(120, 21);
			this.toolStripEditTextBoxNewMorphKeyframeName.AfterEditTextChanged += new System.EventHandler(this.toolStripEditTextBoxNewMorphKeyframeName_AfterEditTextChanged);
			// 
			// FormWorkspace
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(227, 373);
			this.Controls.Add(this.treeView);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "FormWorkspace";
			this.Text = "Workspace";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.contextMenuStripSubmesh.ResumeLayout(false);
			this.contextMenuStripMorphKeyframe.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private TriStateTreeView treeView;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem nodesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem targetPositionToolStripMenuItem;
		private ToolStripEditTextBox toolStripTextBoxTargetPosition;
		private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem materialNameToolStripMenuItem;
		private ToolStripEditTextBox toolStripTextBoxMaterialName;
		private System.Windows.Forms.ToolStripMenuItem worldCoordinatesToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripSubmesh;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripMorphKeyframe;
		private System.Windows.Forms.ToolStripMenuItem renameToToolStripMenuItem;
		private ToolStripEditTextBox toolStripEditTextBoxNewMorphKeyframeName;

	}
}