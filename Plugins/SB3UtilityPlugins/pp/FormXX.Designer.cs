namespace SB3Utility
{
	partial class FormXX
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
			CustomDispose();

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
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reopenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.savexxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.savexxAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.keepBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.textBoxFormat = new SB3Utility.EditTextBox();
			this.buttonConvert = new System.Windows.Forms.Button();
			this.buttonEditHex = new System.Windows.Forms.Button();
			this.label26 = new System.Windows.Forms.Label();
			this.tabControlLists = new System.Windows.Forms.TabControl();
			this.tabPageObject = new System.Windows.Forms.TabPage();
			this.treeViewObjectTree = new System.Windows.Forms.TreeView();
			this.panelObjectTreeBottom = new System.Windows.Forms.Panel();
			this.buttonObjectTreeCollapse = new System.Windows.Forms.Button();
			this.buttonObjectTreeExpand = new System.Windows.Forms.Button();
			this.tabPageMesh = new System.Windows.Forms.TabPage();
			this.splitContainerMesh = new System.Windows.Forms.SplitContainer();
			this.listViewMesh = new System.Windows.Forms.ListView();
			this.meshlistHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainerMeshCrossRef = new System.Windows.Forms.SplitContainer();
			this.listViewMeshMaterial = new System.Windows.Forms.ListView();
			this.listViewMeshMaterialHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label68 = new System.Windows.Forms.Label();
			this.listViewMeshTexture = new System.Windows.Forms.ListView();
			this.listViewMeshTextureHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label70 = new System.Windows.Forms.Label();
			this.tabPageMaterial = new System.Windows.Forms.TabPage();
			this.splitContainerMaterial = new System.Windows.Forms.SplitContainer();
			this.listViewMaterial = new System.Windows.Forms.ListView();
			this.materiallistHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainerMaterialCrossRef = new System.Windows.Forms.SplitContainer();
			this.listViewMaterialMesh = new System.Windows.Forms.ListView();
			this.listViewMaterialMeshHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label71 = new System.Windows.Forms.Label();
			this.listViewMaterialTexture = new System.Windows.Forms.ListView();
			this.listViewMaterialTextureHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label72 = new System.Windows.Forms.Label();
			this.tabPageTexture = new System.Windows.Forms.TabPage();
			this.splitContainerTexture = new System.Windows.Forms.SplitContainer();
			this.listViewTexture = new System.Windows.Forms.ListView();
			this.texturelistHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainerTextureCrossRef = new System.Windows.Forms.SplitContainer();
			this.listViewTextureMesh = new System.Windows.Forms.ListView();
			this.listViewTextureMeshHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label73 = new System.Windows.Forms.Label();
			this.listViewTextureMaterial = new System.Windows.Forms.ListView();
			this.listViewTextureMaterialHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label74 = new System.Windows.Forms.Label();
			this.tabControlViews = new System.Windows.Forms.TabControl();
			this.tabPageFrameView = new System.Windows.Forms.TabPage();
			this.buttonFrameEditHex = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.buttonFrameMatrixApply = new System.Windows.Forms.Button();
			this.numericFrameMatrixRatio = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.numericFrameMatrixNumber = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.buttonFrameMatrixPaste = new System.Windows.Forms.Button();
			this.buttonFrameMatrixCopy = new System.Windows.Forms.Button();
			this.buttonFrameMatrixGrow = new System.Windows.Forms.Button();
			this.buttonFrameMatrixShrink = new System.Windows.Forms.Button();
			this.buttonFrameMatrixIdentity = new System.Windows.Forms.Button();
			this.buttonFrameMatrixInverse = new System.Windows.Forms.Button();
			this.buttonFrameMatrixCombined = new System.Windows.Forms.Button();
			this.label15 = new System.Windows.Forms.Label();
			this.textBoxFrameName2 = new SB3Utility.EditTextBox();
			this.tabControlFrameMatrix = new System.Windows.Forms.TabControl();
			this.tabPageFrameSRT = new System.Windows.Forms.TabPage();
			this.dataGridViewFrameSRT = new SB3Utility.DataGridViewEditor();
			this.tabPageFrameMatrix = new System.Windows.Forms.TabPage();
			this.dataGridViewFrameMatrix = new SB3Utility.DataGridViewEditor();
			this.buttonFrameMoveUp = new System.Windows.Forms.Button();
			this.buttonFrameRemove = new System.Windows.Forms.Button();
			this.buttonFrameMoveDown = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxFrameName = new SB3Utility.EditTextBox();
			this.tabPageBoneView = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.buttonBoneMatrixApply = new System.Windows.Forms.Button();
			this.numericBoneMatrixRatio = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.numericBoneMatrixNumber = new System.Windows.Forms.NumericUpDown();
			this.label12 = new System.Windows.Forms.Label();
			this.buttonBoneMatrixPaste = new System.Windows.Forms.Button();
			this.buttonBoneMatrixCopy = new System.Windows.Forms.Button();
			this.buttonBoneMatrixGrow = new System.Windows.Forms.Button();
			this.buttonBoneMatrixShrink = new System.Windows.Forms.Button();
			this.buttonBoneMatrixIdentity = new System.Windows.Forms.Button();
			this.buttonBoneMatrixInverse = new System.Windows.Forms.Button();
			this.tabControlBoneMatrix = new System.Windows.Forms.TabControl();
			this.tabPageBoneSRT = new System.Windows.Forms.TabPage();
			this.dataGridViewBoneSRT = new SB3Utility.DataGridViewEditor();
			this.tabPageBoneMatrix = new System.Windows.Forms.TabPage();
			this.dataGridViewBoneMatrix = new SB3Utility.DataGridViewEditor();
			this.buttonBoneRemove = new System.Windows.Forms.Button();
			this.buttonBoneCopy = new System.Windows.Forms.Button();
			this.buttonBoneGotoFrame = new System.Windows.Forms.Button();
			this.label25 = new System.Windows.Forms.Label();
			this.textBoxBoneName = new SB3Utility.EditTextBox();
			this.tabPageMeshView = new System.Windows.Forms.TabPage();
			this.buttonMeshNormals = new System.Windows.Forms.Button();
			this.buttonMeshEditHex = new System.Windows.Forms.Button();
			this.buttonMeshMinBones = new System.Windows.Forms.Button();
			this.MeshGotoFrame = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label28 = new System.Windows.Forms.Label();
			this.comboBoxMeshExportFormat = new System.Windows.Forms.ComboBox();
			this.buttonMeshExport = new System.Windows.Forms.Button();
			this.panelMeshExportOptionsFbx = new System.Windows.Forms.Panel();
			this.label13 = new System.Windows.Forms.Label();
			this.textBoxKeyframeRange = new SB3Utility.EditTextBox();
			this.checkBoxMeshExportFbxSkins = new System.Windows.Forms.CheckBox();
			this.checkBoxMeshExportFbxAllFrames = new System.Windows.Forms.CheckBox();
			this.panelMeshExportOptionsDefault = new System.Windows.Forms.Panel();
			this.panelMeshExportOptionsDirectX = new System.Windows.Forms.Panel();
			this.numericMeshExportDirectXTicksPerSecond = new System.Windows.Forms.NumericUpDown();
			this.numericMeshExportDirectXKeyframeLength = new System.Windows.Forms.NumericUpDown();
			this.label35 = new System.Windows.Forms.Label();
			this.label34 = new System.Windows.Forms.Label();
			this.panelMeshExportOptionsCollada = new System.Windows.Forms.Panel();
			this.checkBoxMeshExportColladaAllFrames = new System.Windows.Forms.CheckBox();
			this.panelMeshExportOptionsMqo = new System.Windows.Forms.Panel();
			this.checkBoxMeshExportMqoSingleFile = new System.Windows.Forms.CheckBox();
			this.checkBoxMeshExportMqoWorldCoords = new System.Windows.Forms.CheckBox();
			this.checkBoxMeshSkinned = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonMeshRemove = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dataGridViewMesh = new System.Windows.Forms.DataGridView();
			this.ColumnSubmeshVerts = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnSubmeshFaces = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnSubmeshMaterial = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.buttonSubmeshEdit = new System.Windows.Forms.Button();
			this.buttonSubmeshRemove = new System.Windows.Forms.Button();
			this.textBoxMeshName = new SB3Utility.EditTextBox();
			this.tabPageMaterialView = new System.Windows.Forms.TabPage();
			this.buttonTextureExternal = new System.Windows.Forms.Button();
			this.comboBoxMatTex4 = new System.Windows.Forms.ComboBox();
			this.comboBoxMatTex3 = new System.Windows.Forms.ComboBox();
			this.comboBoxMatTex2 = new System.Windows.Forms.ComboBox();
			this.comboBoxMatTex1 = new System.Windows.Forms.ComboBox();
			this.buttonMaterialRemove = new System.Windows.Forms.Button();
			this.buttonMaterialCopy = new System.Windows.Forms.Button();
			this.label17 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.textBoxMatSpecularPower = new SB3Utility.EditTextBox();
			this.textBoxMatEmissiveA = new SB3Utility.EditTextBox();
			this.textBoxMatEmissiveB = new SB3Utility.EditTextBox();
			this.textBoxMatEmissiveG = new SB3Utility.EditTextBox();
			this.textBoxMatEmissiveR = new SB3Utility.EditTextBox();
			this.textBoxMatSpecularA = new SB3Utility.EditTextBox();
			this.textBoxMatSpecularB = new SB3Utility.EditTextBox();
			this.textBoxMatSpecularG = new SB3Utility.EditTextBox();
			this.textBoxMatSpecularR = new SB3Utility.EditTextBox();
			this.textBoxMatAmbientA = new SB3Utility.EditTextBox();
			this.textBoxMatAmbientB = new SB3Utility.EditTextBox();
			this.textBoxMatAmbientG = new SB3Utility.EditTextBox();
			this.textBoxMatAmbientR = new SB3Utility.EditTextBox();
			this.textBoxMatDiffuseA = new SB3Utility.EditTextBox();
			this.textBoxMatDiffuseB = new SB3Utility.EditTextBox();
			this.textBoxMatDiffuseG = new SB3Utility.EditTextBox();
			this.textBoxMatDiffuseR = new SB3Utility.EditTextBox();
			this.textBoxMatName = new SB3Utility.EditTextBox();
			this.tabPageTextureView = new System.Windows.Forms.TabPage();
			this.buttonTextureAdd = new System.Windows.Forms.Button();
			this.panelTexturePic = new System.Windows.Forms.Panel();
			this.pictureBoxTexture = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonTextureExport = new System.Windows.Forms.Button();
			this.buttonTextureReplace = new System.Windows.Forms.Button();
			this.buttonTextureRemove = new System.Windows.Forms.Button();
			this.textBoxTexSize = new SB3Utility.EditTextBox();
			this.textBoxTexName = new SB3Utility.EditTextBox();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControlLists.SuspendLayout();
			this.tabPageObject.SuspendLayout();
			this.panelObjectTreeBottom.SuspendLayout();
			this.tabPageMesh.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMesh)).BeginInit();
			this.splitContainerMesh.Panel1.SuspendLayout();
			this.splitContainerMesh.Panel2.SuspendLayout();
			this.splitContainerMesh.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMeshCrossRef)).BeginInit();
			this.splitContainerMeshCrossRef.Panel1.SuspendLayout();
			this.splitContainerMeshCrossRef.Panel2.SuspendLayout();
			this.splitContainerMeshCrossRef.SuspendLayout();
			this.tabPageMaterial.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMaterial)).BeginInit();
			this.splitContainerMaterial.Panel1.SuspendLayout();
			this.splitContainerMaterial.Panel2.SuspendLayout();
			this.splitContainerMaterial.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMaterialCrossRef)).BeginInit();
			this.splitContainerMaterialCrossRef.Panel1.SuspendLayout();
			this.splitContainerMaterialCrossRef.Panel2.SuspendLayout();
			this.splitContainerMaterialCrossRef.SuspendLayout();
			this.tabPageTexture.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerTexture)).BeginInit();
			this.splitContainerTexture.Panel1.SuspendLayout();
			this.splitContainerTexture.Panel2.SuspendLayout();
			this.splitContainerTexture.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerTextureCrossRef)).BeginInit();
			this.splitContainerTextureCrossRef.Panel1.SuspendLayout();
			this.splitContainerTextureCrossRef.Panel2.SuspendLayout();
			this.splitContainerTextureCrossRef.SuspendLayout();
			this.tabControlViews.SuspendLayout();
			this.tabPageFrameView.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericFrameMatrixRatio)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericFrameMatrixNumber)).BeginInit();
			this.tabControlFrameMatrix.SuspendLayout();
			this.tabPageFrameSRT.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewFrameSRT)).BeginInit();
			this.tabPageFrameMatrix.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewFrameMatrix)).BeginInit();
			this.tabPageBoneView.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericBoneMatrixRatio)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericBoneMatrixNumber)).BeginInit();
			this.tabControlBoneMatrix.SuspendLayout();
			this.tabPageBoneSRT.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewBoneSRT)).BeginInit();
			this.tabPageBoneMatrix.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewBoneMatrix)).BeginInit();
			this.tabPageMeshView.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panelMeshExportOptionsFbx.SuspendLayout();
			this.panelMeshExportOptionsDirectX.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericMeshExportDirectXTicksPerSecond)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericMeshExportDirectXKeyframeLength)).BeginInit();
			this.panelMeshExportOptionsCollada.SuspendLayout();
			this.panelMeshExportOptionsMqo.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewMesh)).BeginInit();
			this.tabPageMaterialView.SuspendLayout();
			this.tabPageTextureView.SuspendLayout();
			this.panelTexturePic.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTexture)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.AllowMerge = false;
			this.menuStrip1.AutoSize = false;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
			this.menuStrip1.Size = new System.Drawing.Size(524, 18);
			this.menuStrip1.TabIndex = 117;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reopenToolStripMenuItem,
            this.toolStripSeparator1,
            this.savexxToolStripMenuItem,
            this.savexxAsToolStripMenuItem,
            this.toolStripSeparator6,
            this.closeToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 18);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// reopenToolStripMenuItem
			// 
			this.reopenToolStripMenuItem.Name = "reopenToolStripMenuItem";
			this.reopenToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.reopenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.reopenToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.reopenToolStripMenuItem.Text = "&Reopen .xx";
			this.reopenToolStripMenuItem.Click += new System.EventHandler(this.reopenToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
			// 
			// savexxToolStripMenuItem
			// 
			this.savexxToolStripMenuItem.Name = "savexxToolStripMenuItem";
			this.savexxToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.savexxToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.savexxToolStripMenuItem.Text = "&Save .xx";
			this.savexxToolStripMenuItem.Click += new System.EventHandler(this.savexxToolStripMenuItem_Click);
			// 
			// savexxAsToolStripMenuItem
			// 
			this.savexxAsToolStripMenuItem.Name = "savexxAsToolStripMenuItem";
			this.savexxAsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.savexxAsToolStripMenuItem.Text = "Save .xx &As...";
			this.savexxAsToolStripMenuItem.Click += new System.EventHandler(this.savexxAsToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(166, 6);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+F4";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.closeToolStripMenuItem.Text = "&Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keepBackupToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 18);
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
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 18);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.textBoxFormat);
			this.splitContainer1.Panel1.Controls.Add(this.buttonConvert);
			this.splitContainer1.Panel1.Controls.Add(this.buttonEditHex);
			this.splitContainer1.Panel1.Controls.Add(this.label26);
			this.splitContainer1.Panel1.Controls.Add(this.tabControlLists);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControlViews);
			this.splitContainer1.Size = new System.Drawing.Size(524, 518);
			this.splitContainer1.SplitterDistance = 259;
			this.splitContainer1.TabIndex = 116;
			this.splitContainer1.TabStop = false;
			// 
			// textBoxFormat
			// 
			this.textBoxFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBoxFormat.Location = new System.Drawing.Point(44, 494);
			this.textBoxFormat.Name = "textBoxFormat";
			this.textBoxFormat.ReadOnly = true;
			this.textBoxFormat.Size = new System.Drawing.Size(24, 20);
			this.textBoxFormat.TabIndex = 121;
			this.textBoxFormat.TabStop = false;
			this.textBoxFormat.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// buttonConvert
			// 
			this.buttonConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonConvert.Location = new System.Drawing.Point(81, 493);
			this.buttonConvert.Name = "buttonConvert";
			this.buttonConvert.Size = new System.Drawing.Size(75, 23);
			this.buttonConvert.TabIndex = 123;
			this.buttonConvert.TabStop = false;
			this.buttonConvert.Text = "Convert";
			this.buttonConvert.UseVisualStyleBackColor = true;
			this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
			// 
			// buttonEditHex
			// 
			this.buttonEditHex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEditHex.Location = new System.Drawing.Point(183, 493);
			this.buttonEditHex.Name = "buttonEditHex";
			this.buttonEditHex.Size = new System.Drawing.Size(75, 23);
			this.buttonEditHex.TabIndex = 122;
			this.buttonEditHex.TabStop = false;
			this.buttonEditHex.Text = "Edit Hex";
			this.buttonEditHex.UseVisualStyleBackColor = true;
			this.buttonEditHex.Click += new System.EventHandler(this.buttonEditHex_Click);
			// 
			// label26
			// 
			this.label26.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label26.AutoSize = true;
			this.label26.Location = new System.Drawing.Point(3, 497);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(39, 13);
			this.label26.TabIndex = 120;
			this.label26.Text = "Format";
			// 
			// tabControlLists
			// 
			this.tabControlLists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlLists.Controls.Add(this.tabPageObject);
			this.tabControlLists.Controls.Add(this.tabPageMesh);
			this.tabControlLists.Controls.Add(this.tabPageMaterial);
			this.tabControlLists.Controls.Add(this.tabPageTexture);
			this.tabControlLists.Location = new System.Drawing.Point(0, 0);
			this.tabControlLists.Name = "tabControlLists";
			this.tabControlLists.SelectedIndex = 0;
			this.tabControlLists.Size = new System.Drawing.Size(259, 490);
			this.tabControlLists.TabIndex = 119;
			this.tabControlLists.TabStop = false;
			// 
			// tabPageObject
			// 
			this.tabPageObject.Controls.Add(this.treeViewObjectTree);
			this.tabPageObject.Controls.Add(this.panelObjectTreeBottom);
			this.tabPageObject.Location = new System.Drawing.Point(4, 22);
			this.tabPageObject.Name = "tabPageObject";
			this.tabPageObject.Size = new System.Drawing.Size(251, 464);
			this.tabPageObject.TabIndex = 2;
			this.tabPageObject.Text = "Object Tree";
			this.tabPageObject.UseVisualStyleBackColor = true;
			// 
			// treeViewObjectTree
			// 
			this.treeViewObjectTree.AllowDrop = true;
			this.treeViewObjectTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewObjectTree.HideSelection = false;
			this.treeViewObjectTree.Location = new System.Drawing.Point(0, 0);
			this.treeViewObjectTree.Name = "treeViewObjectTree";
			this.treeViewObjectTree.Size = new System.Drawing.Size(251, 435);
			this.treeViewObjectTree.TabIndex = 36;
			this.treeViewObjectTree.TabStop = false;
			this.treeViewObjectTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewObjectTree_ItemDrag);
			this.treeViewObjectTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewObjectTree_AfterSelect);
			this.treeViewObjectTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewObjectTree_NodeMouseClick);
			this.treeViewObjectTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewObjectTree_DragDrop);
			this.treeViewObjectTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewObjectTree_DragEnter);
			this.treeViewObjectTree.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewObjectTree_DragOver);
			// 
			// panelObjectTreeBottom
			// 
			this.panelObjectTreeBottom.Controls.Add(this.buttonObjectTreeCollapse);
			this.panelObjectTreeBottom.Controls.Add(this.buttonObjectTreeExpand);
			this.panelObjectTreeBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelObjectTreeBottom.Location = new System.Drawing.Point(0, 435);
			this.panelObjectTreeBottom.Name = "panelObjectTreeBottom";
			this.panelObjectTreeBottom.Size = new System.Drawing.Size(251, 29);
			this.panelObjectTreeBottom.TabIndex = 37;
			// 
			// buttonObjectTreeCollapse
			// 
			this.buttonObjectTreeCollapse.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonObjectTreeCollapse.Location = new System.Drawing.Point(139, 4);
			this.buttonObjectTreeCollapse.Name = "buttonObjectTreeCollapse";
			this.buttonObjectTreeCollapse.Size = new System.Drawing.Size(75, 23);
			this.buttonObjectTreeCollapse.TabIndex = 136;
			this.buttonObjectTreeCollapse.TabStop = false;
			this.buttonObjectTreeCollapse.Text = "Collapse All";
			this.buttonObjectTreeCollapse.UseVisualStyleBackColor = true;
			this.buttonObjectTreeCollapse.Click += new System.EventHandler(this.buttonObjectTreeCollapse_Click);
			// 
			// buttonObjectTreeExpand
			// 
			this.buttonObjectTreeExpand.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonObjectTreeExpand.Location = new System.Drawing.Point(37, 4);
			this.buttonObjectTreeExpand.Name = "buttonObjectTreeExpand";
			this.buttonObjectTreeExpand.Size = new System.Drawing.Size(75, 23);
			this.buttonObjectTreeExpand.TabIndex = 135;
			this.buttonObjectTreeExpand.TabStop = false;
			this.buttonObjectTreeExpand.Text = "Expand All";
			this.buttonObjectTreeExpand.UseVisualStyleBackColor = true;
			this.buttonObjectTreeExpand.Click += new System.EventHandler(this.buttonObjectTreeExpand_Click);
			// 
			// tabPageMesh
			// 
			this.tabPageMesh.Controls.Add(this.splitContainerMesh);
			this.tabPageMesh.Location = new System.Drawing.Point(4, 22);
			this.tabPageMesh.Name = "tabPageMesh";
			this.tabPageMesh.Size = new System.Drawing.Size(251, 464);
			this.tabPageMesh.TabIndex = 0;
			this.tabPageMesh.Text = "Mesh";
			this.tabPageMesh.UseVisualStyleBackColor = true;
			// 
			// splitContainerMesh
			// 
			this.splitContainerMesh.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMesh.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMesh.Name = "splitContainerMesh";
			this.splitContainerMesh.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMesh.Panel1
			// 
			this.splitContainerMesh.Panel1.Controls.Add(this.listViewMesh);
			// 
			// splitContainerMesh.Panel2
			// 
			this.splitContainerMesh.Panel2.Controls.Add(this.splitContainerMeshCrossRef);
			this.splitContainerMesh.Size = new System.Drawing.Size(251, 464);
			this.splitContainerMesh.SplitterDistance = 264;
			this.splitContainerMesh.TabIndex = 3;
			this.splitContainerMesh.TabStop = false;
			// 
			// listViewMesh
			// 
			this.listViewMesh.AutoArrange = false;
			this.listViewMesh.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.meshlistHeader});
			this.listViewMesh.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewMesh.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewMesh.HideSelection = false;
			this.listViewMesh.LabelWrap = false;
			this.listViewMesh.Location = new System.Drawing.Point(0, 0);
			this.listViewMesh.Name = "listViewMesh";
			this.listViewMesh.Size = new System.Drawing.Size(251, 264);
			this.listViewMesh.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewMesh.TabIndex = 1;
			this.listViewMesh.TabStop = false;
			this.listViewMesh.UseCompatibleStateImageBehavior = false;
			this.listViewMesh.View = System.Windows.Forms.View.Details;
			this.listViewMesh.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewMesh_ItemSelectionChanged);
			// 
			// splitContainerMeshCrossRef
			// 
			this.splitContainerMeshCrossRef.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMeshCrossRef.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMeshCrossRef.Name = "splitContainerMeshCrossRef";
			this.splitContainerMeshCrossRef.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMeshCrossRef.Panel1
			// 
			this.splitContainerMeshCrossRef.Panel1.Controls.Add(this.listViewMeshMaterial);
			this.splitContainerMeshCrossRef.Panel1.Controls.Add(this.label68);
			// 
			// splitContainerMeshCrossRef.Panel2
			// 
			this.splitContainerMeshCrossRef.Panel2.Controls.Add(this.listViewMeshTexture);
			this.splitContainerMeshCrossRef.Panel2.Controls.Add(this.label70);
			this.splitContainerMeshCrossRef.Size = new System.Drawing.Size(251, 196);
			this.splitContainerMeshCrossRef.SplitterDistance = 94;
			this.splitContainerMeshCrossRef.TabIndex = 2;
			this.splitContainerMeshCrossRef.TabStop = false;
			// 
			// listViewMeshMaterial
			// 
			this.listViewMeshMaterial.AutoArrange = false;
			this.listViewMeshMaterial.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listViewMeshMaterialHeader});
			this.listViewMeshMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewMeshMaterial.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewMeshMaterial.HideSelection = false;
			this.listViewMeshMaterial.LabelWrap = false;
			this.listViewMeshMaterial.Location = new System.Drawing.Point(0, 13);
			this.listViewMeshMaterial.Name = "listViewMeshMaterial";
			this.listViewMeshMaterial.Size = new System.Drawing.Size(251, 81);
			this.listViewMeshMaterial.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewMeshMaterial.TabIndex = 2;
			this.listViewMeshMaterial.TabStop = false;
			this.listViewMeshMaterial.UseCompatibleStateImageBehavior = false;
			this.listViewMeshMaterial.View = System.Windows.Forms.View.Details;
			this.listViewMeshMaterial.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewMeshMaterial_ItemSelectionChanged);
			// 
			// label68
			// 
			this.label68.AutoSize = true;
			this.label68.Dock = System.Windows.Forms.DockStyle.Top;
			this.label68.Location = new System.Drawing.Point(0, 0);
			this.label68.Name = "label68";
			this.label68.Size = new System.Drawing.Size(77, 13);
			this.label68.TabIndex = 0;
			this.label68.Text = "Materials Used";
			// 
			// listViewMeshTexture
			// 
			this.listViewMeshTexture.AutoArrange = false;
			this.listViewMeshTexture.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listViewMeshTextureHeader});
			this.listViewMeshTexture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewMeshTexture.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewMeshTexture.HideSelection = false;
			this.listViewMeshTexture.LabelWrap = false;
			this.listViewMeshTexture.Location = new System.Drawing.Point(0, 13);
			this.listViewMeshTexture.Name = "listViewMeshTexture";
			this.listViewMeshTexture.Size = new System.Drawing.Size(251, 85);
			this.listViewMeshTexture.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewMeshTexture.TabIndex = 2;
			this.listViewMeshTexture.TabStop = false;
			this.listViewMeshTexture.UseCompatibleStateImageBehavior = false;
			this.listViewMeshTexture.View = System.Windows.Forms.View.Details;
			this.listViewMeshTexture.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewMeshTexture_ItemSelectionChanged);
			// 
			// label70
			// 
			this.label70.AutoSize = true;
			this.label70.Dock = System.Windows.Forms.DockStyle.Top;
			this.label70.Location = new System.Drawing.Point(0, 0);
			this.label70.Name = "label70";
			this.label70.Size = new System.Drawing.Size(76, 13);
			this.label70.TabIndex = 1;
			this.label70.Text = "Textures Used";
			// 
			// tabPageMaterial
			// 
			this.tabPageMaterial.Controls.Add(this.splitContainerMaterial);
			this.tabPageMaterial.Location = new System.Drawing.Point(4, 22);
			this.tabPageMaterial.Name = "tabPageMaterial";
			this.tabPageMaterial.Size = new System.Drawing.Size(251, 464);
			this.tabPageMaterial.TabIndex = 1;
			this.tabPageMaterial.Text = "Material";
			this.tabPageMaterial.UseVisualStyleBackColor = true;
			// 
			// splitContainerMaterial
			// 
			this.splitContainerMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMaterial.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMaterial.Name = "splitContainerMaterial";
			this.splitContainerMaterial.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMaterial.Panel1
			// 
			this.splitContainerMaterial.Panel1.Controls.Add(this.listViewMaterial);
			// 
			// splitContainerMaterial.Panel2
			// 
			this.splitContainerMaterial.Panel2.Controls.Add(this.splitContainerMaterialCrossRef);
			this.splitContainerMaterial.Size = new System.Drawing.Size(251, 464);
			this.splitContainerMaterial.SplitterDistance = 264;
			this.splitContainerMaterial.TabIndex = 4;
			this.splitContainerMaterial.TabStop = false;
			// 
			// listViewMaterial
			// 
			this.listViewMaterial.AutoArrange = false;
			this.listViewMaterial.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.materiallistHeader});
			this.listViewMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewMaterial.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewMaterial.HideSelection = false;
			this.listViewMaterial.LabelWrap = false;
			this.listViewMaterial.Location = new System.Drawing.Point(0, 0);
			this.listViewMaterial.Name = "listViewMaterial";
			this.listViewMaterial.Size = new System.Drawing.Size(251, 264);
			this.listViewMaterial.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewMaterial.TabIndex = 0;
			this.listViewMaterial.TabStop = false;
			this.listViewMaterial.UseCompatibleStateImageBehavior = false;
			this.listViewMaterial.View = System.Windows.Forms.View.Details;
			this.listViewMaterial.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewMaterial_ItemSelectionChanged);
			// 
			// splitContainerMaterialCrossRef
			// 
			this.splitContainerMaterialCrossRef.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMaterialCrossRef.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMaterialCrossRef.Name = "splitContainerMaterialCrossRef";
			this.splitContainerMaterialCrossRef.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMaterialCrossRef.Panel1
			// 
			this.splitContainerMaterialCrossRef.Panel1.Controls.Add(this.listViewMaterialMesh);
			this.splitContainerMaterialCrossRef.Panel1.Controls.Add(this.label71);
			// 
			// splitContainerMaterialCrossRef.Panel2
			// 
			this.splitContainerMaterialCrossRef.Panel2.Controls.Add(this.listViewMaterialTexture);
			this.splitContainerMaterialCrossRef.Panel2.Controls.Add(this.label72);
			this.splitContainerMaterialCrossRef.Size = new System.Drawing.Size(251, 196);
			this.splitContainerMaterialCrossRef.SplitterDistance = 94;
			this.splitContainerMaterialCrossRef.TabIndex = 2;
			this.splitContainerMaterialCrossRef.TabStop = false;
			// 
			// listViewMaterialMesh
			// 
			this.listViewMaterialMesh.AutoArrange = false;
			this.listViewMaterialMesh.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listViewMaterialMeshHeader});
			this.listViewMaterialMesh.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewMaterialMesh.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewMaterialMesh.HideSelection = false;
			this.listViewMaterialMesh.LabelWrap = false;
			this.listViewMaterialMesh.Location = new System.Drawing.Point(0, 13);
			this.listViewMaterialMesh.Name = "listViewMaterialMesh";
			this.listViewMaterialMesh.Size = new System.Drawing.Size(251, 81);
			this.listViewMaterialMesh.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewMaterialMesh.TabIndex = 3;
			this.listViewMaterialMesh.TabStop = false;
			this.listViewMaterialMesh.UseCompatibleStateImageBehavior = false;
			this.listViewMaterialMesh.View = System.Windows.Forms.View.Details;
			this.listViewMaterialMesh.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewMaterialMesh_ItemSelectionChanged);
			// 
			// label71
			// 
			this.label71.AutoSize = true;
			this.label71.Dock = System.Windows.Forms.DockStyle.Top;
			this.label71.Location = new System.Drawing.Point(0, 0);
			this.label71.Name = "label71";
			this.label71.Size = new System.Drawing.Size(84, 13);
			this.label71.TabIndex = 1;
			this.label71.Text = "Used In Meshes";
			// 
			// listViewMaterialTexture
			// 
			this.listViewMaterialTexture.AutoArrange = false;
			this.listViewMaterialTexture.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listViewMaterialTextureHeader});
			this.listViewMaterialTexture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewMaterialTexture.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewMaterialTexture.HideSelection = false;
			this.listViewMaterialTexture.LabelWrap = false;
			this.listViewMaterialTexture.Location = new System.Drawing.Point(0, 13);
			this.listViewMaterialTexture.Name = "listViewMaterialTexture";
			this.listViewMaterialTexture.Size = new System.Drawing.Size(251, 85);
			this.listViewMaterialTexture.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewMaterialTexture.TabIndex = 3;
			this.listViewMaterialTexture.TabStop = false;
			this.listViewMaterialTexture.UseCompatibleStateImageBehavior = false;
			this.listViewMaterialTexture.View = System.Windows.Forms.View.Details;
			this.listViewMaterialTexture.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewMaterialTexture_ItemSelectionChanged);
			// 
			// label72
			// 
			this.label72.AutoSize = true;
			this.label72.Dock = System.Windows.Forms.DockStyle.Top;
			this.label72.Location = new System.Drawing.Point(0, 0);
			this.label72.Name = "label72";
			this.label72.Size = new System.Drawing.Size(76, 13);
			this.label72.TabIndex = 1;
			this.label72.Text = "Textures Used";
			// 
			// tabPageTexture
			// 
			this.tabPageTexture.Controls.Add(this.splitContainerTexture);
			this.tabPageTexture.Location = new System.Drawing.Point(4, 22);
			this.tabPageTexture.Name = "tabPageTexture";
			this.tabPageTexture.Size = new System.Drawing.Size(251, 464);
			this.tabPageTexture.TabIndex = 3;
			this.tabPageTexture.Text = "Texture";
			this.tabPageTexture.UseVisualStyleBackColor = true;
			// 
			// splitContainerTexture
			// 
			this.splitContainerTexture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerTexture.Location = new System.Drawing.Point(0, 0);
			this.splitContainerTexture.Name = "splitContainerTexture";
			this.splitContainerTexture.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerTexture.Panel1
			// 
			this.splitContainerTexture.Panel1.Controls.Add(this.listViewTexture);
			// 
			// splitContainerTexture.Panel2
			// 
			this.splitContainerTexture.Panel2.Controls.Add(this.splitContainerTextureCrossRef);
			this.splitContainerTexture.Size = new System.Drawing.Size(251, 464);
			this.splitContainerTexture.SplitterDistance = 264;
			this.splitContainerTexture.TabIndex = 3;
			this.splitContainerTexture.TabStop = false;
			// 
			// listViewTexture
			// 
			this.listViewTexture.AutoArrange = false;
			this.listViewTexture.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.texturelistHeader});
			this.listViewTexture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewTexture.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewTexture.HideSelection = false;
			this.listViewTexture.LabelWrap = false;
			this.listViewTexture.Location = new System.Drawing.Point(0, 0);
			this.listViewTexture.Name = "listViewTexture";
			this.listViewTexture.Size = new System.Drawing.Size(251, 264);
			this.listViewTexture.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewTexture.TabIndex = 1;
			this.listViewTexture.TabStop = false;
			this.listViewTexture.UseCompatibleStateImageBehavior = false;
			this.listViewTexture.View = System.Windows.Forms.View.Details;
			this.listViewTexture.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewTexture_ItemSelectionChanged);
			// 
			// splitContainerTextureCrossRef
			// 
			this.splitContainerTextureCrossRef.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerTextureCrossRef.Location = new System.Drawing.Point(0, 0);
			this.splitContainerTextureCrossRef.Name = "splitContainerTextureCrossRef";
			this.splitContainerTextureCrossRef.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerTextureCrossRef.Panel1
			// 
			this.splitContainerTextureCrossRef.Panel1.Controls.Add(this.listViewTextureMesh);
			this.splitContainerTextureCrossRef.Panel1.Controls.Add(this.label73);
			// 
			// splitContainerTextureCrossRef.Panel2
			// 
			this.splitContainerTextureCrossRef.Panel2.Controls.Add(this.listViewTextureMaterial);
			this.splitContainerTextureCrossRef.Panel2.Controls.Add(this.label74);
			this.splitContainerTextureCrossRef.Size = new System.Drawing.Size(251, 196);
			this.splitContainerTextureCrossRef.SplitterDistance = 94;
			this.splitContainerTextureCrossRef.TabIndex = 2;
			this.splitContainerTextureCrossRef.TabStop = false;
			// 
			// listViewTextureMesh
			// 
			this.listViewTextureMesh.AutoArrange = false;
			this.listViewTextureMesh.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listViewTextureMeshHeader});
			this.listViewTextureMesh.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewTextureMesh.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewTextureMesh.HideSelection = false;
			this.listViewTextureMesh.LabelWrap = false;
			this.listViewTextureMesh.Location = new System.Drawing.Point(0, 13);
			this.listViewTextureMesh.Name = "listViewTextureMesh";
			this.listViewTextureMesh.Size = new System.Drawing.Size(251, 81);
			this.listViewTextureMesh.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewTextureMesh.TabIndex = 4;
			this.listViewTextureMesh.TabStop = false;
			this.listViewTextureMesh.UseCompatibleStateImageBehavior = false;
			this.listViewTextureMesh.View = System.Windows.Forms.View.Details;
			this.listViewTextureMesh.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewTextureMesh_ItemSelectionChanged);
			// 
			// label73
			// 
			this.label73.AutoSize = true;
			this.label73.Dock = System.Windows.Forms.DockStyle.Top;
			this.label73.Location = new System.Drawing.Point(0, 0);
			this.label73.Name = "label73";
			this.label73.Size = new System.Drawing.Size(84, 13);
			this.label73.TabIndex = 1;
			this.label73.Text = "Used In Meshes";
			// 
			// listViewTextureMaterial
			// 
			this.listViewTextureMaterial.AutoArrange = false;
			this.listViewTextureMaterial.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listViewTextureMaterialHeader});
			this.listViewTextureMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewTextureMaterial.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewTextureMaterial.HideSelection = false;
			this.listViewTextureMaterial.LabelWrap = false;
			this.listViewTextureMaterial.Location = new System.Drawing.Point(0, 13);
			this.listViewTextureMaterial.Name = "listViewTextureMaterial";
			this.listViewTextureMaterial.Size = new System.Drawing.Size(251, 85);
			this.listViewTextureMaterial.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewTextureMaterial.TabIndex = 4;
			this.listViewTextureMaterial.TabStop = false;
			this.listViewTextureMaterial.UseCompatibleStateImageBehavior = false;
			this.listViewTextureMaterial.View = System.Windows.Forms.View.Details;
			this.listViewTextureMaterial.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewTextureMaterial_ItemSelectionChanged);
			// 
			// label74
			// 
			this.label74.AutoSize = true;
			this.label74.Dock = System.Windows.Forms.DockStyle.Top;
			this.label74.Location = new System.Drawing.Point(0, 0);
			this.label74.Name = "label74";
			this.label74.Size = new System.Drawing.Size(89, 13);
			this.label74.TabIndex = 1;
			this.label74.Text = "Used In Materials";
			// 
			// tabControlViews
			// 
			this.tabControlViews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlViews.Controls.Add(this.tabPageFrameView);
			this.tabControlViews.Controls.Add(this.tabPageBoneView);
			this.tabControlViews.Controls.Add(this.tabPageMeshView);
			this.tabControlViews.Controls.Add(this.tabPageMaterialView);
			this.tabControlViews.Controls.Add(this.tabPageTextureView);
			this.tabControlViews.Location = new System.Drawing.Point(0, 0);
			this.tabControlViews.Name = "tabControlViews";
			this.tabControlViews.SelectedIndex = 0;
			this.tabControlViews.Size = new System.Drawing.Size(261, 518);
			this.tabControlViews.TabIndex = 133;
			// 
			// tabPageFrameView
			// 
			this.tabPageFrameView.Controls.Add(this.buttonFrameEditHex);
			this.tabPageFrameView.Controls.Add(this.groupBox3);
			this.tabPageFrameView.Controls.Add(this.label15);
			this.tabPageFrameView.Controls.Add(this.textBoxFrameName2);
			this.tabPageFrameView.Controls.Add(this.tabControlFrameMatrix);
			this.tabPageFrameView.Controls.Add(this.buttonFrameMoveUp);
			this.tabPageFrameView.Controls.Add(this.buttonFrameRemove);
			this.tabPageFrameView.Controls.Add(this.buttonFrameMoveDown);
			this.tabPageFrameView.Controls.Add(this.label4);
			this.tabPageFrameView.Controls.Add(this.textBoxFrameName);
			this.tabPageFrameView.Location = new System.Drawing.Point(4, 22);
			this.tabPageFrameView.Name = "tabPageFrameView";
			this.tabPageFrameView.Size = new System.Drawing.Size(253, 492);
			this.tabPageFrameView.TabIndex = 2;
			this.tabPageFrameView.Text = "Frame";
			this.tabPageFrameView.UseVisualStyleBackColor = true;
			// 
			// buttonFrameEditHex
			// 
			this.buttonFrameEditHex.Location = new System.Drawing.Point(88, 67);
			this.buttonFrameEditHex.Name = "buttonFrameEditHex";
			this.buttonFrameEditHex.Size = new System.Drawing.Size(75, 23);
			this.buttonFrameEditHex.TabIndex = 12;
			this.buttonFrameEditHex.Text = "Edit Hex";
			this.buttonFrameEditHex.UseVisualStyleBackColor = true;
			this.buttonFrameEditHex.Click += new System.EventHandler(this.buttonFrameEditHex_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.buttonFrameMatrixApply);
			this.groupBox3.Controls.Add(this.numericFrameMatrixRatio);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.numericFrameMatrixNumber);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.buttonFrameMatrixPaste);
			this.groupBox3.Controls.Add(this.buttonFrameMatrixCopy);
			this.groupBox3.Controls.Add(this.buttonFrameMatrixGrow);
			this.groupBox3.Controls.Add(this.buttonFrameMatrixShrink);
			this.groupBox3.Controls.Add(this.buttonFrameMatrixIdentity);
			this.groupBox3.Controls.Add(this.buttonFrameMatrixInverse);
			this.groupBox3.Controls.Add(this.buttonFrameMatrixCombined);
			this.groupBox3.Location = new System.Drawing.Point(0, 253);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(253, 149);
			this.groupBox3.TabIndex = 60;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Matrix Operations";
			// 
			// buttonFrameMatrixApply
			// 
			this.buttonFrameMatrixApply.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonFrameMatrixApply.Location = new System.Drawing.Point(67, 114);
			this.buttonFrameMatrixApply.Name = "buttonFrameMatrixApply";
			this.buttonFrameMatrixApply.Size = new System.Drawing.Size(112, 23);
			this.buttonFrameMatrixApply.TabIndex = 80;
			this.buttonFrameMatrixApply.Text = "Apply Changes";
			this.buttonFrameMatrixApply.UseVisualStyleBackColor = true;
			this.buttonFrameMatrixApply.Click += new System.EventHandler(this.buttonFrameMatrixApply_Click);
			// 
			// numericFrameMatrixRatio
			// 
			this.numericFrameMatrixRatio.DecimalPlaces = 2;
			this.numericFrameMatrixRatio.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numericFrameMatrixRatio.Location = new System.Drawing.Point(36, 52);
			this.numericFrameMatrixRatio.Name = "numericFrameMatrixRatio";
			this.numericFrameMatrixRatio.Size = new System.Drawing.Size(56, 20);
			this.numericFrameMatrixRatio.TabIndex = 68;
			this.numericFrameMatrixRatio.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 55);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(32, 13);
			this.label5.TabIndex = 178;
			this.label5.Text = "Ratio";
			// 
			// numericFrameMatrixNumber
			// 
			this.numericFrameMatrixNumber.Location = new System.Drawing.Point(50, 83);
			this.numericFrameMatrixNumber.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericFrameMatrixNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericFrameMatrixNumber.Name = "numericFrameMatrixNumber";
			this.numericFrameMatrixNumber.Size = new System.Drawing.Size(42, 20);
			this.numericFrameMatrixNumber.TabIndex = 74;
			this.numericFrameMatrixNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 87);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(45, 13);
			this.label6.TabIndex = 176;
			this.label6.Text = "Matrix #";
			// 
			// buttonFrameMatrixPaste
			// 
			this.buttonFrameMatrixPaste.Location = new System.Drawing.Point(179, 81);
			this.buttonFrameMatrixPaste.Name = "buttonFrameMatrixPaste";
			this.buttonFrameMatrixPaste.Size = new System.Drawing.Size(66, 23);
			this.buttonFrameMatrixPaste.TabIndex = 78;
			this.buttonFrameMatrixPaste.Text = "Paste";
			this.buttonFrameMatrixPaste.UseVisualStyleBackColor = true;
			this.buttonFrameMatrixPaste.Click += new System.EventHandler(this.buttonFrameMatrixPaste_Click);
			// 
			// buttonFrameMatrixCopy
			// 
			this.buttonFrameMatrixCopy.Location = new System.Drawing.Point(102, 81);
			this.buttonFrameMatrixCopy.Name = "buttonFrameMatrixCopy";
			this.buttonFrameMatrixCopy.Size = new System.Drawing.Size(66, 23);
			this.buttonFrameMatrixCopy.TabIndex = 76;
			this.buttonFrameMatrixCopy.Text = "Copy";
			this.buttonFrameMatrixCopy.UseVisualStyleBackColor = true;
			this.buttonFrameMatrixCopy.Click += new System.EventHandler(this.buttonFrameMatrixCopy_Click);
			// 
			// buttonFrameMatrixGrow
			// 
			this.buttonFrameMatrixGrow.Location = new System.Drawing.Point(102, 51);
			this.buttonFrameMatrixGrow.Name = "buttonFrameMatrixGrow";
			this.buttonFrameMatrixGrow.Size = new System.Drawing.Size(66, 23);
			this.buttonFrameMatrixGrow.TabIndex = 70;
			this.buttonFrameMatrixGrow.Text = "Grow";
			this.buttonFrameMatrixGrow.UseVisualStyleBackColor = true;
			this.buttonFrameMatrixGrow.Click += new System.EventHandler(this.buttonFrameMatrixGrow_Click);
			// 
			// buttonFrameMatrixShrink
			// 
			this.buttonFrameMatrixShrink.Location = new System.Drawing.Point(179, 51);
			this.buttonFrameMatrixShrink.Name = "buttonFrameMatrixShrink";
			this.buttonFrameMatrixShrink.Size = new System.Drawing.Size(66, 23);
			this.buttonFrameMatrixShrink.TabIndex = 72;
			this.buttonFrameMatrixShrink.Text = "Shrink";
			this.buttonFrameMatrixShrink.UseVisualStyleBackColor = true;
			this.buttonFrameMatrixShrink.Click += new System.EventHandler(this.buttonFrameMatrixShrink_Click);
			// 
			// buttonFrameMatrixIdentity
			// 
			this.buttonFrameMatrixIdentity.Location = new System.Drawing.Point(25, 19);
			this.buttonFrameMatrixIdentity.Name = "buttonFrameMatrixIdentity";
			this.buttonFrameMatrixIdentity.Size = new System.Drawing.Size(66, 23);
			this.buttonFrameMatrixIdentity.TabIndex = 62;
			this.buttonFrameMatrixIdentity.Text = "Identity";
			this.buttonFrameMatrixIdentity.UseVisualStyleBackColor = true;
			this.buttonFrameMatrixIdentity.Click += new System.EventHandler(this.buttonFrameMatrixIdentity_Click);
			// 
			// buttonFrameMatrixInverse
			// 
			this.buttonFrameMatrixInverse.Location = new System.Drawing.Point(179, 19);
			this.buttonFrameMatrixInverse.Name = "buttonFrameMatrixInverse";
			this.buttonFrameMatrixInverse.Size = new System.Drawing.Size(66, 23);
			this.buttonFrameMatrixInverse.TabIndex = 66;
			this.buttonFrameMatrixInverse.Text = "Inverse";
			this.buttonFrameMatrixInverse.UseVisualStyleBackColor = true;
			this.buttonFrameMatrixInverse.Click += new System.EventHandler(this.buttonFrameMatrixInverse_Click);
			// 
			// buttonFrameMatrixCombined
			// 
			this.buttonFrameMatrixCombined.Location = new System.Drawing.Point(102, 19);
			this.buttonFrameMatrixCombined.Name = "buttonFrameMatrixCombined";
			this.buttonFrameMatrixCombined.Size = new System.Drawing.Size(66, 23);
			this.buttonFrameMatrixCombined.TabIndex = 64;
			this.buttonFrameMatrixCombined.Text = "Combined";
			this.buttonFrameMatrixCombined.UseVisualStyleBackColor = true;
			this.buttonFrameMatrixCombined.Click += new System.EventHandler(this.buttonFrameMatrixCombined_Click);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(1, 35);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(41, 13);
			this.label15.TabIndex = 157;
			this.label15.Text = "Name2";
			// 
			// textBoxFrameName2
			// 
			this.textBoxFrameName2.Location = new System.Drawing.Point(43, 32);
			this.textBoxFrameName2.Name = "textBoxFrameName2";
			this.textBoxFrameName2.Size = new System.Drawing.Size(207, 20);
			this.textBoxFrameName2.TabIndex = 3;
			// 
			// tabControlFrameMatrix
			// 
			this.tabControlFrameMatrix.Controls.Add(this.tabPageFrameSRT);
			this.tabControlFrameMatrix.Controls.Add(this.tabPageFrameMatrix);
			this.tabControlFrameMatrix.Location = new System.Drawing.Point(0, 135);
			this.tabControlFrameMatrix.Name = "tabControlFrameMatrix";
			this.tabControlFrameMatrix.SelectedIndex = 0;
			this.tabControlFrameMatrix.Size = new System.Drawing.Size(253, 112);
			this.tabControlFrameMatrix.TabIndex = 40;
			// 
			// tabPageFrameSRT
			// 
			this.tabPageFrameSRT.Controls.Add(this.dataGridViewFrameSRT);
			this.tabPageFrameSRT.Location = new System.Drawing.Point(4, 22);
			this.tabPageFrameSRT.Name = "tabPageFrameSRT";
			this.tabPageFrameSRT.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFrameSRT.Size = new System.Drawing.Size(245, 86);
			this.tabPageFrameSRT.TabIndex = 1;
			this.tabPageFrameSRT.Text = "SRT";
			this.tabPageFrameSRT.UseVisualStyleBackColor = true;
			// 
			// dataGridViewFrameSRT
			// 
			this.dataGridViewFrameSRT.AllowUserToAddRows = false;
			this.dataGridViewFrameSRT.AllowUserToDeleteRows = false;
			this.dataGridViewFrameSRT.AllowUserToResizeColumns = false;
			this.dataGridViewFrameSRT.AllowUserToResizeRows = false;
			this.dataGridViewFrameSRT.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewFrameSRT.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridViewFrameSRT.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewFrameSRT.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridViewFrameSRT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewFrameSRT.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewFrameSRT.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewFrameSRT.Name = "dataGridViewFrameSRT";
			this.dataGridViewFrameSRT.RowHeadersVisible = false;
			this.dataGridViewFrameSRT.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridViewFrameSRT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridViewFrameSRT.ShowRowIndex = false;
			this.dataGridViewFrameSRT.Size = new System.Drawing.Size(239, 80);
			this.dataGridViewFrameSRT.TabIndex = 144;
			this.dataGridViewFrameSRT.TabStop = false;
			this.dataGridViewFrameSRT.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSRT_CellValueChanged);
			// 
			// tabPageFrameMatrix
			// 
			this.tabPageFrameMatrix.Controls.Add(this.dataGridViewFrameMatrix);
			this.tabPageFrameMatrix.Location = new System.Drawing.Point(4, 22);
			this.tabPageFrameMatrix.Name = "tabPageFrameMatrix";
			this.tabPageFrameMatrix.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFrameMatrix.Size = new System.Drawing.Size(245, 86);
			this.tabPageFrameMatrix.TabIndex = 0;
			this.tabPageFrameMatrix.Text = "Matrix";
			this.tabPageFrameMatrix.UseVisualStyleBackColor = true;
			// 
			// dataGridViewFrameMatrix
			// 
			this.dataGridViewFrameMatrix.AllowUserToAddRows = false;
			this.dataGridViewFrameMatrix.AllowUserToDeleteRows = false;
			this.dataGridViewFrameMatrix.AllowUserToResizeColumns = false;
			this.dataGridViewFrameMatrix.AllowUserToResizeRows = false;
			this.dataGridViewFrameMatrix.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewFrameMatrix.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridViewFrameMatrix.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewFrameMatrix.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridViewFrameMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewFrameMatrix.ColumnHeadersVisible = false;
			this.dataGridViewFrameMatrix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewFrameMatrix.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewFrameMatrix.Name = "dataGridViewFrameMatrix";
			this.dataGridViewFrameMatrix.RowHeadersVisible = false;
			this.dataGridViewFrameMatrix.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridViewFrameMatrix.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridViewFrameMatrix.ShowRowIndex = false;
			this.dataGridViewFrameMatrix.Size = new System.Drawing.Size(239, 80);
			this.dataGridViewFrameMatrix.TabIndex = 145;
			this.dataGridViewFrameMatrix.TabStop = false;
			this.dataGridViewFrameMatrix.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMatrix_CellValueChanged);
			// 
			// buttonFrameMoveUp
			// 
			this.buttonFrameMoveUp.Location = new System.Drawing.Point(2, 67);
			this.buttonFrameMoveUp.Name = "buttonFrameMoveUp";
			this.buttonFrameMoveUp.Size = new System.Drawing.Size(75, 23);
			this.buttonFrameMoveUp.TabIndex = 10;
			this.buttonFrameMoveUp.Text = "Move Up";
			this.buttonFrameMoveUp.UseVisualStyleBackColor = true;
			this.buttonFrameMoveUp.Click += new System.EventHandler(this.buttonFrameMoveUp_Click);
			// 
			// buttonFrameRemove
			// 
			this.buttonFrameRemove.Location = new System.Drawing.Point(175, 67);
			this.buttonFrameRemove.Name = "buttonFrameRemove";
			this.buttonFrameRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonFrameRemove.TabIndex = 14;
			this.buttonFrameRemove.Text = "Remove";
			this.buttonFrameRemove.UseVisualStyleBackColor = true;
			this.buttonFrameRemove.Click += new System.EventHandler(this.buttonFrameRemove_Click);
			// 
			// buttonFrameMoveDown
			// 
			this.buttonFrameMoveDown.Location = new System.Drawing.Point(2, 100);
			this.buttonFrameMoveDown.Name = "buttonFrameMoveDown";
			this.buttonFrameMoveDown.Size = new System.Drawing.Size(75, 23);
			this.buttonFrameMoveDown.TabIndex = 16;
			this.buttonFrameMoveDown.Text = "Move Down";
			this.buttonFrameMoveDown.UseVisualStyleBackColor = true;
			this.buttonFrameMoveDown.Click += new System.EventHandler(this.buttonFrameMoveDown_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(35, 13);
			this.label4.TabIndex = 85;
			this.label4.Text = "Name";
			// 
			// textBoxFrameName
			// 
			this.textBoxFrameName.Location = new System.Drawing.Point(43, 6);
			this.textBoxFrameName.Name = "textBoxFrameName";
			this.textBoxFrameName.Size = new System.Drawing.Size(207, 20);
			this.textBoxFrameName.TabIndex = 1;
			// 
			// tabPageBoneView
			// 
			this.tabPageBoneView.Controls.Add(this.groupBox4);
			this.tabPageBoneView.Controls.Add(this.tabControlBoneMatrix);
			this.tabPageBoneView.Controls.Add(this.buttonBoneRemove);
			this.tabPageBoneView.Controls.Add(this.buttonBoneCopy);
			this.tabPageBoneView.Controls.Add(this.buttonBoneGotoFrame);
			this.tabPageBoneView.Controls.Add(this.label25);
			this.tabPageBoneView.Controls.Add(this.textBoxBoneName);
			this.tabPageBoneView.Location = new System.Drawing.Point(4, 22);
			this.tabPageBoneView.Name = "tabPageBoneView";
			this.tabPageBoneView.Size = new System.Drawing.Size(253, 492);
			this.tabPageBoneView.TabIndex = 8;
			this.tabPageBoneView.Text = "Bone";
			this.tabPageBoneView.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.buttonBoneMatrixApply);
			this.groupBox4.Controls.Add(this.numericBoneMatrixRatio);
			this.groupBox4.Controls.Add(this.label9);
			this.groupBox4.Controls.Add(this.numericBoneMatrixNumber);
			this.groupBox4.Controls.Add(this.label12);
			this.groupBox4.Controls.Add(this.buttonBoneMatrixPaste);
			this.groupBox4.Controls.Add(this.buttonBoneMatrixCopy);
			this.groupBox4.Controls.Add(this.buttonBoneMatrixGrow);
			this.groupBox4.Controls.Add(this.buttonBoneMatrixShrink);
			this.groupBox4.Controls.Add(this.buttonBoneMatrixIdentity);
			this.groupBox4.Controls.Add(this.buttonBoneMatrixInverse);
			this.groupBox4.Location = new System.Drawing.Point(0, 196);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(253, 149);
			this.groupBox4.TabIndex = 60;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Matrix Operations";
			// 
			// buttonBoneMatrixApply
			// 
			this.buttonBoneMatrixApply.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonBoneMatrixApply.Location = new System.Drawing.Point(67, 114);
			this.buttonBoneMatrixApply.Name = "buttonBoneMatrixApply";
			this.buttonBoneMatrixApply.Size = new System.Drawing.Size(112, 23);
			this.buttonBoneMatrixApply.TabIndex = 78;
			this.buttonBoneMatrixApply.Text = "Apply Changes";
			this.buttonBoneMatrixApply.UseVisualStyleBackColor = true;
			this.buttonBoneMatrixApply.Click += new System.EventHandler(this.buttonBoneMatrixApply_Click);
			// 
			// numericBoneMatrixRatio
			// 
			this.numericBoneMatrixRatio.DecimalPlaces = 2;
			this.numericBoneMatrixRatio.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numericBoneMatrixRatio.Location = new System.Drawing.Point(36, 52);
			this.numericBoneMatrixRatio.Name = "numericBoneMatrixRatio";
			this.numericBoneMatrixRatio.Size = new System.Drawing.Size(56, 20);
			this.numericBoneMatrixRatio.TabIndex = 66;
			this.numericBoneMatrixRatio.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(3, 55);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(32, 13);
			this.label9.TabIndex = 178;
			this.label9.Text = "Ratio";
			// 
			// numericBoneMatrixNumber
			// 
			this.numericBoneMatrixNumber.Location = new System.Drawing.Point(50, 83);
			this.numericBoneMatrixNumber.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericBoneMatrixNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericBoneMatrixNumber.Name = "numericBoneMatrixNumber";
			this.numericBoneMatrixNumber.Size = new System.Drawing.Size(42, 20);
			this.numericBoneMatrixNumber.TabIndex = 72;
			this.numericBoneMatrixNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(3, 87);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(45, 13);
			this.label12.TabIndex = 176;
			this.label12.Text = "Matrix #";
			// 
			// buttonBoneMatrixPaste
			// 
			this.buttonBoneMatrixPaste.Location = new System.Drawing.Point(179, 81);
			this.buttonBoneMatrixPaste.Name = "buttonBoneMatrixPaste";
			this.buttonBoneMatrixPaste.Size = new System.Drawing.Size(66, 23);
			this.buttonBoneMatrixPaste.TabIndex = 76;
			this.buttonBoneMatrixPaste.Text = "Paste";
			this.buttonBoneMatrixPaste.UseVisualStyleBackColor = true;
			this.buttonBoneMatrixPaste.Click += new System.EventHandler(this.buttonBoneMatrixPaste_Click);
			// 
			// buttonBoneMatrixCopy
			// 
			this.buttonBoneMatrixCopy.Location = new System.Drawing.Point(102, 81);
			this.buttonBoneMatrixCopy.Name = "buttonBoneMatrixCopy";
			this.buttonBoneMatrixCopy.Size = new System.Drawing.Size(66, 23);
			this.buttonBoneMatrixCopy.TabIndex = 74;
			this.buttonBoneMatrixCopy.Text = "Copy";
			this.buttonBoneMatrixCopy.UseVisualStyleBackColor = true;
			this.buttonBoneMatrixCopy.Click += new System.EventHandler(this.buttonBoneMatrixCopy_Click);
			// 
			// buttonBoneMatrixGrow
			// 
			this.buttonBoneMatrixGrow.Location = new System.Drawing.Point(102, 51);
			this.buttonBoneMatrixGrow.Name = "buttonBoneMatrixGrow";
			this.buttonBoneMatrixGrow.Size = new System.Drawing.Size(66, 23);
			this.buttonBoneMatrixGrow.TabIndex = 68;
			this.buttonBoneMatrixGrow.Text = "Grow";
			this.buttonBoneMatrixGrow.UseVisualStyleBackColor = true;
			this.buttonBoneMatrixGrow.Click += new System.EventHandler(this.buttonBoneMatrixGrow_Click);
			// 
			// buttonBoneMatrixShrink
			// 
			this.buttonBoneMatrixShrink.Location = new System.Drawing.Point(179, 51);
			this.buttonBoneMatrixShrink.Name = "buttonBoneMatrixShrink";
			this.buttonBoneMatrixShrink.Size = new System.Drawing.Size(66, 23);
			this.buttonBoneMatrixShrink.TabIndex = 70;
			this.buttonBoneMatrixShrink.Text = "Shrink";
			this.buttonBoneMatrixShrink.UseVisualStyleBackColor = true;
			this.buttonBoneMatrixShrink.Click += new System.EventHandler(this.buttonBoneMatrixShrink_Click);
			// 
			// buttonBoneMatrixIdentity
			// 
			this.buttonBoneMatrixIdentity.Location = new System.Drawing.Point(102, 19);
			this.buttonBoneMatrixIdentity.Name = "buttonBoneMatrixIdentity";
			this.buttonBoneMatrixIdentity.Size = new System.Drawing.Size(66, 23);
			this.buttonBoneMatrixIdentity.TabIndex = 62;
			this.buttonBoneMatrixIdentity.Text = "Identity";
			this.buttonBoneMatrixIdentity.UseVisualStyleBackColor = true;
			this.buttonBoneMatrixIdentity.Click += new System.EventHandler(this.buttonBoneMatrixIdentity_Click);
			// 
			// buttonBoneMatrixInverse
			// 
			this.buttonBoneMatrixInverse.Location = new System.Drawing.Point(179, 19);
			this.buttonBoneMatrixInverse.Name = "buttonBoneMatrixInverse";
			this.buttonBoneMatrixInverse.Size = new System.Drawing.Size(66, 23);
			this.buttonBoneMatrixInverse.TabIndex = 64;
			this.buttonBoneMatrixInverse.Text = "Inverse";
			this.buttonBoneMatrixInverse.UseVisualStyleBackColor = true;
			this.buttonBoneMatrixInverse.Click += new System.EventHandler(this.buttonBoneMatrixInverse_Click);
			// 
			// tabControlBoneMatrix
			// 
			this.tabControlBoneMatrix.Controls.Add(this.tabPageBoneSRT);
			this.tabControlBoneMatrix.Controls.Add(this.tabPageBoneMatrix);
			this.tabControlBoneMatrix.Location = new System.Drawing.Point(0, 78);
			this.tabControlBoneMatrix.Name = "tabControlBoneMatrix";
			this.tabControlBoneMatrix.SelectedIndex = 0;
			this.tabControlBoneMatrix.Size = new System.Drawing.Size(253, 112);
			this.tabControlBoneMatrix.TabIndex = 40;
			// 
			// tabPageBoneSRT
			// 
			this.tabPageBoneSRT.Controls.Add(this.dataGridViewBoneSRT);
			this.tabPageBoneSRT.Location = new System.Drawing.Point(4, 22);
			this.tabPageBoneSRT.Name = "tabPageBoneSRT";
			this.tabPageBoneSRT.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageBoneSRT.Size = new System.Drawing.Size(245, 86);
			this.tabPageBoneSRT.TabIndex = 1;
			this.tabPageBoneSRT.Text = "SRT";
			this.tabPageBoneSRT.UseVisualStyleBackColor = true;
			// 
			// dataGridViewBoneSRT
			// 
			this.dataGridViewBoneSRT.AllowUserToAddRows = false;
			this.dataGridViewBoneSRT.AllowUserToDeleteRows = false;
			this.dataGridViewBoneSRT.AllowUserToResizeColumns = false;
			this.dataGridViewBoneSRT.AllowUserToResizeRows = false;
			this.dataGridViewBoneSRT.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewBoneSRT.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridViewBoneSRT.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewBoneSRT.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridViewBoneSRT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewBoneSRT.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewBoneSRT.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewBoneSRT.Name = "dataGridViewBoneSRT";
			this.dataGridViewBoneSRT.RowHeadersVisible = false;
			this.dataGridViewBoneSRT.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridViewBoneSRT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridViewBoneSRT.ShowRowIndex = false;
			this.dataGridViewBoneSRT.Size = new System.Drawing.Size(239, 80);
			this.dataGridViewBoneSRT.TabIndex = 144;
			this.dataGridViewBoneSRT.TabStop = false;
			this.dataGridViewBoneSRT.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSRT_CellValueChanged);
			// 
			// tabPageBoneMatrix
			// 
			this.tabPageBoneMatrix.Controls.Add(this.dataGridViewBoneMatrix);
			this.tabPageBoneMatrix.Location = new System.Drawing.Point(4, 22);
			this.tabPageBoneMatrix.Name = "tabPageBoneMatrix";
			this.tabPageBoneMatrix.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageBoneMatrix.Size = new System.Drawing.Size(245, 86);
			this.tabPageBoneMatrix.TabIndex = 0;
			this.tabPageBoneMatrix.Text = "Matrix";
			this.tabPageBoneMatrix.UseVisualStyleBackColor = true;
			// 
			// dataGridViewBoneMatrix
			// 
			this.dataGridViewBoneMatrix.AllowUserToAddRows = false;
			this.dataGridViewBoneMatrix.AllowUserToDeleteRows = false;
			this.dataGridViewBoneMatrix.AllowUserToResizeColumns = false;
			this.dataGridViewBoneMatrix.AllowUserToResizeRows = false;
			this.dataGridViewBoneMatrix.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewBoneMatrix.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridViewBoneMatrix.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewBoneMatrix.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridViewBoneMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewBoneMatrix.ColumnHeadersVisible = false;
			this.dataGridViewBoneMatrix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewBoneMatrix.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewBoneMatrix.Name = "dataGridViewBoneMatrix";
			this.dataGridViewBoneMatrix.RowHeadersVisible = false;
			this.dataGridViewBoneMatrix.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridViewBoneMatrix.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridViewBoneMatrix.ShowRowIndex = false;
			this.dataGridViewBoneMatrix.Size = new System.Drawing.Size(239, 80);
			this.dataGridViewBoneMatrix.TabIndex = 145;
			this.dataGridViewBoneMatrix.TabStop = false;
			this.dataGridViewBoneMatrix.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMatrix_CellValueChanged);
			// 
			// buttonBoneRemove
			// 
			this.buttonBoneRemove.Location = new System.Drawing.Point(89, 41);
			this.buttonBoneRemove.Name = "buttonBoneRemove";
			this.buttonBoneRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonBoneRemove.TabIndex = 22;
			this.buttonBoneRemove.Text = "Remove";
			this.buttonBoneRemove.UseVisualStyleBackColor = true;
			this.buttonBoneRemove.Click += new System.EventHandler(this.buttonBoneRemove_Click);
			// 
			// buttonBoneCopy
			// 
			this.buttonBoneCopy.Location = new System.Drawing.Point(176, 41);
			this.buttonBoneCopy.Name = "buttonBoneCopy";
			this.buttonBoneCopy.Size = new System.Drawing.Size(75, 23);
			this.buttonBoneCopy.TabIndex = 24;
			this.buttonBoneCopy.Text = "Copy->New";
			this.buttonBoneCopy.UseVisualStyleBackColor = true;
			this.buttonBoneCopy.Click += new System.EventHandler(this.buttonBoneCopy_Click);
			// 
			// buttonBoneGotoFrame
			// 
			this.buttonBoneGotoFrame.Location = new System.Drawing.Point(2, 41);
			this.buttonBoneGotoFrame.Name = "buttonBoneGotoFrame";
			this.buttonBoneGotoFrame.Size = new System.Drawing.Size(75, 23);
			this.buttonBoneGotoFrame.TabIndex = 20;
			this.buttonBoneGotoFrame.Text = "Goto Frame";
			this.buttonBoneGotoFrame.UseVisualStyleBackColor = true;
			this.buttonBoneGotoFrame.Click += new System.EventHandler(this.buttonBoneGotoFrame_Click);
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.Location = new System.Drawing.Point(-2, 9);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(35, 13);
			this.label25.TabIndex = 106;
			this.label25.Text = "Name";
			// 
			// textBoxBoneName
			// 
			this.textBoxBoneName.Location = new System.Drawing.Point(35, 6);
			this.textBoxBoneName.Name = "textBoxBoneName";
			this.textBoxBoneName.Size = new System.Drawing.Size(215, 20);
			this.textBoxBoneName.TabIndex = 1;
			// 
			// tabPageMeshView
			// 
			this.tabPageMeshView.Controls.Add(this.buttonMeshNormals);
			this.tabPageMeshView.Controls.Add(this.buttonMeshEditHex);
			this.tabPageMeshView.Controls.Add(this.buttonMeshMinBones);
			this.tabPageMeshView.Controls.Add(this.MeshGotoFrame);
			this.tabPageMeshView.Controls.Add(this.groupBox2);
			this.tabPageMeshView.Controls.Add(this.checkBoxMeshSkinned);
			this.tabPageMeshView.Controls.Add(this.label8);
			this.tabPageMeshView.Controls.Add(this.label1);
			this.tabPageMeshView.Controls.Add(this.buttonMeshRemove);
			this.tabPageMeshView.Controls.Add(this.groupBox1);
			this.tabPageMeshView.Controls.Add(this.textBoxMeshName);
			this.tabPageMeshView.Location = new System.Drawing.Point(4, 22);
			this.tabPageMeshView.Name = "tabPageMeshView";
			this.tabPageMeshView.Size = new System.Drawing.Size(253, 492);
			this.tabPageMeshView.TabIndex = 0;
			this.tabPageMeshView.Text = "Mesh";
			this.tabPageMeshView.UseVisualStyleBackColor = true;
			// 
			// buttonMeshNormals
			// 
			this.buttonMeshNormals.Location = new System.Drawing.Point(90, 152);
			this.buttonMeshNormals.Name = "buttonMeshNormals";
			this.buttonMeshNormals.Size = new System.Drawing.Size(73, 23);
			this.buttonMeshNormals.TabIndex = 133;
			this.buttonMeshNormals.Text = "Normals...";
			this.buttonMeshNormals.UseVisualStyleBackColor = true;
			this.buttonMeshNormals.Click += new System.EventHandler(this.buttonMeshNormals_Click);
			// 
			// buttonMeshEditHex
			// 
			this.buttonMeshEditHex.Location = new System.Drawing.Point(5, 152);
			this.buttonMeshEditHex.Name = "buttonMeshEditHex";
			this.buttonMeshEditHex.Size = new System.Drawing.Size(73, 23);
			this.buttonMeshEditHex.TabIndex = 132;
			this.buttonMeshEditHex.Text = "Edit Hex";
			this.buttonMeshEditHex.UseVisualStyleBackColor = true;
			this.buttonMeshEditHex.Click += new System.EventHandler(this.buttonMeshEditHex_Click);
			// 
			// buttonMeshMinBones
			// 
			this.buttonMeshMinBones.Location = new System.Drawing.Point(174, 118);
			this.buttonMeshMinBones.Name = "buttonMeshMinBones";
			this.buttonMeshMinBones.Size = new System.Drawing.Size(73, 23);
			this.buttonMeshMinBones.TabIndex = 131;
			this.buttonMeshMinBones.Text = "Min Bones";
			this.buttonMeshMinBones.UseVisualStyleBackColor = true;
			this.buttonMeshMinBones.Click += new System.EventHandler(this.buttonMeshMinBones_Click);
			// 
			// MeshGotoFrame
			// 
			this.MeshGotoFrame.Location = new System.Drawing.Point(5, 118);
			this.MeshGotoFrame.Name = "MeshGotoFrame";
			this.MeshGotoFrame.Size = new System.Drawing.Size(73, 23);
			this.MeshGotoFrame.TabIndex = 110;
			this.MeshGotoFrame.Text = "Goto Frame";
			this.MeshGotoFrame.UseVisualStyleBackColor = true;
			this.MeshGotoFrame.Click += new System.EventHandler(this.buttonMeshGotoFrame_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label28);
			this.groupBox2.Controls.Add(this.comboBoxMeshExportFormat);
			this.groupBox2.Controls.Add(this.buttonMeshExport);
			this.groupBox2.Controls.Add(this.panelMeshExportOptionsDefault);
			this.groupBox2.Controls.Add(this.panelMeshExportOptionsDirectX);
			this.groupBox2.Controls.Add(this.panelMeshExportOptionsCollada);
			this.groupBox2.Controls.Add(this.panelMeshExportOptionsMqo);
			this.groupBox2.Controls.Add(this.panelMeshExportOptionsFbx);
			this.groupBox2.Location = new System.Drawing.Point(0, 42);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(253, 66);
			this.groupBox2.TabIndex = 20;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Export Options";
			// 
			// label28
			// 
			this.label28.AutoSize = true;
			this.label28.Location = new System.Drawing.Point(2, 19);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(39, 13);
			this.label28.TabIndex = 131;
			this.label28.Text = "Format";
			// 
			// comboBoxMeshExportFormat
			// 
			this.comboBoxMeshExportFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMeshExportFormat.Location = new System.Drawing.Point(42, 15);
			this.comboBoxMeshExportFormat.Name = "comboBoxMeshExportFormat";
			this.comboBoxMeshExportFormat.Size = new System.Drawing.Size(126, 21);
			this.comboBoxMeshExportFormat.TabIndex = 22;
			this.comboBoxMeshExportFormat.SelectedIndexChanged += new System.EventHandler(this.comboBoxMeshExportFormat_SelectedIndexChanged);
			// 
			// buttonMeshExport
			// 
			this.buttonMeshExport.Location = new System.Drawing.Point(175, 14);
			this.buttonMeshExport.Name = "buttonMeshExport";
			this.buttonMeshExport.Size = new System.Drawing.Size(73, 23);
			this.buttonMeshExport.TabIndex = 24;
			this.buttonMeshExport.Text = "Export";
			this.buttonMeshExport.UseVisualStyleBackColor = true;
			this.buttonMeshExport.Click += new System.EventHandler(this.buttonMeshExport_Click);
			// 
			// panelMeshExportOptionsFbx
			// 
			this.panelMeshExportOptionsFbx.Controls.Add(this.label13);
			this.panelMeshExportOptionsFbx.Controls.Add(this.textBoxKeyframeRange);
			this.panelMeshExportOptionsFbx.Controls.Add(this.checkBoxMeshExportFbxSkins);
			this.panelMeshExportOptionsFbx.Controls.Add(this.checkBoxMeshExportFbxAllFrames);
			this.panelMeshExportOptionsFbx.Location = new System.Drawing.Point(3, 40);
			this.panelMeshExportOptionsFbx.Name = "panelMeshExportOptionsFbx";
			this.panelMeshExportOptionsFbx.Size = new System.Drawing.Size(246, 22);
			this.panelMeshExportOptionsFbx.TabIndex = 260;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(141, 4);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(56, 13);
			this.label13.TabIndex = 268;
			this.label13.Text = "Keyframes";
			// 
			// textBoxKeyframeRange
			// 
			this.textBoxKeyframeRange.Location = new System.Drawing.Point(200, 1);
			this.textBoxKeyframeRange.MaxLength = 7;
			this.textBoxKeyframeRange.Name = "textBoxKeyframeRange";
			this.textBoxKeyframeRange.Size = new System.Drawing.Size(45, 20);
			this.textBoxKeyframeRange.TabIndex = 268;
			this.textBoxKeyframeRange.Text = "-1-0";
			// 
			// checkBoxMeshExportFbxSkins
			// 
			this.checkBoxMeshExportFbxSkins.AutoSize = true;
			this.checkBoxMeshExportFbxSkins.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
			this.checkBoxMeshExportFbxSkins.Checked = true;
			this.checkBoxMeshExportFbxSkins.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxMeshExportFbxSkins.Location = new System.Drawing.Point(81, 2);
			this.checkBoxMeshExportFbxSkins.Name = "checkBoxMeshExportFbxSkins";
			this.checkBoxMeshExportFbxSkins.Size = new System.Drawing.Size(52, 17);
			this.checkBoxMeshExportFbxSkins.TabIndex = 264;
			this.checkBoxMeshExportFbxSkins.Text = "Skins";
			this.checkBoxMeshExportFbxSkins.UseVisualStyleBackColor = true;
			// 
			// checkBoxMeshExportFbxAllFrames
			// 
			this.checkBoxMeshExportFbxAllFrames.AutoSize = true;
			this.checkBoxMeshExportFbxAllFrames.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
			this.checkBoxMeshExportFbxAllFrames.Location = new System.Drawing.Point(1, 2);
			this.checkBoxMeshExportFbxAllFrames.Name = "checkBoxMeshExportFbxAllFrames";
			this.checkBoxMeshExportFbxAllFrames.Size = new System.Drawing.Size(74, 17);
			this.checkBoxMeshExportFbxAllFrames.TabIndex = 262;
			this.checkBoxMeshExportFbxAllFrames.Text = "All Frames";
			this.checkBoxMeshExportFbxAllFrames.UseVisualStyleBackColor = true;
			// 
			// panelMeshExportOptionsDefault
			// 
			this.panelMeshExportOptionsDefault.Location = new System.Drawing.Point(3, 40);
			this.panelMeshExportOptionsDefault.Name = "panelMeshExportOptionsDefault";
			this.panelMeshExportOptionsDefault.Size = new System.Drawing.Size(246, 22);
			this.panelMeshExportOptionsDefault.TabIndex = 26;
			// 
			// panelMeshExportOptionsDirectX
			// 
			this.panelMeshExportOptionsDirectX.Controls.Add(this.numericMeshExportDirectXTicksPerSecond);
			this.panelMeshExportOptionsDirectX.Controls.Add(this.numericMeshExportDirectXKeyframeLength);
			this.panelMeshExportOptionsDirectX.Controls.Add(this.label35);
			this.panelMeshExportOptionsDirectX.Controls.Add(this.label34);
			this.panelMeshExportOptionsDirectX.Location = new System.Drawing.Point(3, 40);
			this.panelMeshExportOptionsDirectX.Name = "panelMeshExportOptionsDirectX";
			this.panelMeshExportOptionsDirectX.Size = new System.Drawing.Size(246, 22);
			this.panelMeshExportOptionsDirectX.TabIndex = 200;
			// 
			// numericMeshExportDirectXTicksPerSecond
			// 
			this.numericMeshExportDirectXTicksPerSecond.Location = new System.Drawing.Point(60, 1);
			this.numericMeshExportDirectXTicksPerSecond.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericMeshExportDirectXTicksPerSecond.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericMeshExportDirectXTicksPerSecond.Name = "numericMeshExportDirectXTicksPerSecond";
			this.numericMeshExportDirectXTicksPerSecond.Size = new System.Drawing.Size(45, 20);
			this.numericMeshExportDirectXTicksPerSecond.TabIndex = 202;
			this.numericMeshExportDirectXTicksPerSecond.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// numericMeshExportDirectXKeyframeLength
			// 
			this.numericMeshExportDirectXKeyframeLength.Location = new System.Drawing.Point(199, 1);
			this.numericMeshExportDirectXKeyframeLength.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericMeshExportDirectXKeyframeLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericMeshExportDirectXKeyframeLength.Name = "numericMeshExportDirectXKeyframeLength";
			this.numericMeshExportDirectXKeyframeLength.Size = new System.Drawing.Size(45, 20);
			this.numericMeshExportDirectXKeyframeLength.TabIndex = 202;
			this.numericMeshExportDirectXKeyframeLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label35
			// 
			this.label35.AutoSize = true;
			this.label35.Location = new System.Drawing.Point(110, 5);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(87, 13);
			this.label35.TabIndex = 99;
			this.label35.Text = "Keyframe Length";
			// 
			// label34
			// 
			this.label34.AutoSize = true;
			this.label34.Location = new System.Drawing.Point(1, 5);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(57, 13);
			this.label34.TabIndex = 98;
			this.label34.Text = "Ticks/Sec";
			// 
			// panelMeshExportOptionsCollada
			// 
			this.panelMeshExportOptionsCollada.Controls.Add(this.checkBoxMeshExportColladaAllFrames);
			this.panelMeshExportOptionsCollada.Location = new System.Drawing.Point(3, 40);
			this.panelMeshExportOptionsCollada.Name = "panelMeshExportOptionsCollada";
			this.panelMeshExportOptionsCollada.Size = new System.Drawing.Size(246, 22);
			this.panelMeshExportOptionsCollada.TabIndex = 220;
			// 
			// checkBoxMeshExportColladaAllFrames
			// 
			this.checkBoxMeshExportColladaAllFrames.AutoSize = true;
			this.checkBoxMeshExportColladaAllFrames.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
			this.checkBoxMeshExportColladaAllFrames.Location = new System.Drawing.Point(37, 2);
			this.checkBoxMeshExportColladaAllFrames.Name = "checkBoxMeshExportColladaAllFrames";
			this.checkBoxMeshExportColladaAllFrames.Size = new System.Drawing.Size(74, 17);
			this.checkBoxMeshExportColladaAllFrames.TabIndex = 222;
			this.checkBoxMeshExportColladaAllFrames.Text = "All Frames";
			this.checkBoxMeshExportColladaAllFrames.UseVisualStyleBackColor = true;
			// 
			// panelMeshExportOptionsMqo
			// 
			this.panelMeshExportOptionsMqo.Controls.Add(this.checkBoxMeshExportMqoSingleFile);
			this.panelMeshExportOptionsMqo.Controls.Add(this.checkBoxMeshExportMqoWorldCoords);
			this.panelMeshExportOptionsMqo.Location = new System.Drawing.Point(3, 40);
			this.panelMeshExportOptionsMqo.Name = "panelMeshExportOptionsMqo";
			this.panelMeshExportOptionsMqo.Size = new System.Drawing.Size(246, 22);
			this.panelMeshExportOptionsMqo.TabIndex = 240;
			// 
			// checkBoxMeshExportMqoSingleFile
			// 
			this.checkBoxMeshExportMqoSingleFile.AutoSize = true;
			this.checkBoxMeshExportMqoSingleFile.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
			this.checkBoxMeshExportMqoSingleFile.Location = new System.Drawing.Point(37, 2);
			this.checkBoxMeshExportMqoSingleFile.Name = "checkBoxMeshExportMqoSingleFile";
			this.checkBoxMeshExportMqoSingleFile.Size = new System.Drawing.Size(79, 17);
			this.checkBoxMeshExportMqoSingleFile.TabIndex = 242;
			this.checkBoxMeshExportMqoSingleFile.Text = "Single Mqo";
			this.checkBoxMeshExportMqoSingleFile.UseVisualStyleBackColor = true;
			// 
			// checkBoxMeshExportMqoWorldCoords
			// 
			this.checkBoxMeshExportMqoWorldCoords.AutoSize = true;
			this.checkBoxMeshExportMqoWorldCoords.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
			this.checkBoxMeshExportMqoWorldCoords.Location = new System.Drawing.Point(131, 2);
			this.checkBoxMeshExportMqoWorldCoords.Name = "checkBoxMeshExportMqoWorldCoords";
			this.checkBoxMeshExportMqoWorldCoords.Size = new System.Drawing.Size(113, 17);
			this.checkBoxMeshExportMqoWorldCoords.TabIndex = 244;
			this.checkBoxMeshExportMqoWorldCoords.Text = "World Coordinates";
			this.checkBoxMeshExportMqoWorldCoords.UseVisualStyleBackColor = true;
			// 
			// checkBoxMeshSkinned
			// 
			this.checkBoxMeshSkinned.AutoCheck = false;
			this.checkBoxMeshSkinned.AutoSize = true;
			this.checkBoxMeshSkinned.Location = new System.Drawing.Point(221, 23);
			this.checkBoxMeshSkinned.Name = "checkBoxMeshSkinned";
			this.checkBoxMeshSkinned.Size = new System.Drawing.Size(15, 14);
			this.checkBoxMeshSkinned.TabIndex = 10;
			this.checkBoxMeshSkinned.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(204, 4);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(46, 13);
			this.label8.TabIndex = 125;
			this.label8.Text = "Skinned";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(-2, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 123;
			this.label1.Text = "Name";
			// 
			// buttonMeshRemove
			// 
			this.buttonMeshRemove.Location = new System.Drawing.Point(90, 118);
			this.buttonMeshRemove.Name = "buttonMeshRemove";
			this.buttonMeshRemove.Size = new System.Drawing.Size(73, 23);
			this.buttonMeshRemove.TabIndex = 115;
			this.buttonMeshRemove.Text = "Remove";
			this.buttonMeshRemove.UseVisualStyleBackColor = true;
			this.buttonMeshRemove.Click += new System.EventHandler(this.buttonMeshRemove_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.dataGridViewMesh);
			this.groupBox1.Controls.Add(this.buttonSubmeshEdit);
			this.groupBox1.Controls.Add(this.buttonSubmeshRemove);
			this.groupBox1.Location = new System.Drawing.Point(0, 181);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(253, 329);
			this.groupBox1.TabIndex = 139;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Submeshes";
			// 
			// dataGridViewMesh
			// 
			this.dataGridViewMesh.AllowUserToAddRows = false;
			this.dataGridViewMesh.AllowUserToDeleteRows = false;
			this.dataGridViewMesh.AllowUserToResizeRows = false;
			this.dataGridViewMesh.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridViewMesh.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewMesh.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSubmeshVerts,
            this.ColumnSubmeshFaces,
            this.ColumnSubmeshMaterial});
			this.dataGridViewMesh.Location = new System.Drawing.Point(3, 48);
			this.dataGridViewMesh.Name = "dataGridViewMesh";
			this.dataGridViewMesh.RowHeadersVisible = false;
			this.dataGridViewMesh.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewMesh.Size = new System.Drawing.Size(246, 277);
			this.dataGridViewMesh.TabIndex = 152;
			this.dataGridViewMesh.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMesh_CellClick);
			this.dataGridViewMesh.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridViewMesh_EditingControlShowing);
			this.dataGridViewMesh.SelectionChanged += new System.EventHandler(this.dataGridViewMesh_SelectionChanged);
			this.dataGridViewMesh.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewMesh_KeyDown);
			// 
			// ColumnSubmeshVerts
			// 
			this.ColumnSubmeshVerts.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.ColumnSubmeshVerts.HeaderText = "Verts";
			this.ColumnSubmeshVerts.MinimumWidth = 40;
			this.ColumnSubmeshVerts.Name = "ColumnSubmeshVerts";
			this.ColumnSubmeshVerts.ReadOnly = true;
			this.ColumnSubmeshVerts.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ColumnSubmeshVerts.Width = 40;
			// 
			// ColumnSubmeshFaces
			// 
			this.ColumnSubmeshFaces.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.ColumnSubmeshFaces.HeaderText = "Faces";
			this.ColumnSubmeshFaces.MinimumWidth = 40;
			this.ColumnSubmeshFaces.Name = "ColumnSubmeshFaces";
			this.ColumnSubmeshFaces.ReadOnly = true;
			this.ColumnSubmeshFaces.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ColumnSubmeshFaces.Width = 42;
			// 
			// ColumnSubmeshMaterial
			// 
			this.ColumnSubmeshMaterial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ColumnSubmeshMaterial.HeaderText = "Material";
			this.ColumnSubmeshMaterial.Name = "ColumnSubmeshMaterial";
			// 
			// buttonSubmeshEdit
			// 
			this.buttonSubmeshEdit.Location = new System.Drawing.Point(5, 18);
			this.buttonSubmeshEdit.Name = "buttonSubmeshEdit";
			this.buttonSubmeshEdit.Size = new System.Drawing.Size(73, 23);
			this.buttonSubmeshEdit.TabIndex = 140;
			this.buttonSubmeshEdit.Text = "Edit";
			this.buttonSubmeshEdit.UseVisualStyleBackColor = true;
			this.buttonSubmeshEdit.Click += new System.EventHandler(this.buttonSubmeshEdit_Click);
			// 
			// buttonSubmeshRemove
			// 
			this.buttonSubmeshRemove.Location = new System.Drawing.Point(90, 18);
			this.buttonSubmeshRemove.Name = "buttonSubmeshRemove";
			this.buttonSubmeshRemove.Size = new System.Drawing.Size(73, 23);
			this.buttonSubmeshRemove.TabIndex = 142;
			this.buttonSubmeshRemove.Text = "Remove";
			this.buttonSubmeshRemove.UseVisualStyleBackColor = true;
			this.buttonSubmeshRemove.Click += new System.EventHandler(this.buttonSubmeshRemove_Click);
			// 
			// textBoxMeshName
			// 
			this.textBoxMeshName.Location = new System.Drawing.Point(0, 19);
			this.textBoxMeshName.Name = "textBoxMeshName";
			this.textBoxMeshName.ReadOnly = true;
			this.textBoxMeshName.Size = new System.Drawing.Size(200, 20);
			this.textBoxMeshName.TabIndex = 1;
			// 
			// tabPageMaterialView
			// 
			this.tabPageMaterialView.Controls.Add(this.buttonTextureExternal);
			this.tabPageMaterialView.Controls.Add(this.comboBoxMatTex4);
			this.tabPageMaterialView.Controls.Add(this.comboBoxMatTex3);
			this.tabPageMaterialView.Controls.Add(this.comboBoxMatTex2);
			this.tabPageMaterialView.Controls.Add(this.comboBoxMatTex1);
			this.tabPageMaterialView.Controls.Add(this.buttonMaterialRemove);
			this.tabPageMaterialView.Controls.Add(this.buttonMaterialCopy);
			this.tabPageMaterialView.Controls.Add(this.label17);
			this.tabPageMaterialView.Controls.Add(this.label23);
			this.tabPageMaterialView.Controls.Add(this.label11);
			this.tabPageMaterialView.Controls.Add(this.label10);
			this.tabPageMaterialView.Controls.Add(this.label7);
			this.tabPageMaterialView.Controls.Add(this.label2);
			this.tabPageMaterialView.Controls.Add(this.label22);
			this.tabPageMaterialView.Controls.Add(this.label21);
			this.tabPageMaterialView.Controls.Add(this.label20);
			this.tabPageMaterialView.Controls.Add(this.label19);
			this.tabPageMaterialView.Controls.Add(this.label18);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatSpecularPower);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatEmissiveA);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatEmissiveB);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatEmissiveG);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatEmissiveR);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatSpecularA);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatSpecularB);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatSpecularG);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatSpecularR);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatAmbientA);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatAmbientB);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatAmbientG);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatAmbientR);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatDiffuseA);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatDiffuseB);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatDiffuseG);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatDiffuseR);
			this.tabPageMaterialView.Controls.Add(this.textBoxMatName);
			this.tabPageMaterialView.Location = new System.Drawing.Point(4, 22);
			this.tabPageMaterialView.Name = "tabPageMaterialView";
			this.tabPageMaterialView.Size = new System.Drawing.Size(253, 492);
			this.tabPageMaterialView.TabIndex = 1;
			this.tabPageMaterialView.Text = "Material";
			this.tabPageMaterialView.UseVisualStyleBackColor = true;
			// 
			// buttonTextureExternal
			// 
			this.buttonTextureExternal.Location = new System.Drawing.Point(178, 310);
			this.buttonTextureExternal.Name = "buttonTextureExternal";
			this.buttonTextureExternal.Size = new System.Drawing.Size(75, 22);
			this.buttonTextureExternal.TabIndex = 150;
			this.buttonTextureExternal.Text = "External";
			this.buttonTextureExternal.UseVisualStyleBackColor = true;
			this.buttonTextureExternal.Click += new System.EventHandler(this.buttonTextureExternal_Click);
			// 
			// comboBoxMatTex4
			// 
			this.comboBoxMatTex4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMatTex4.Location = new System.Drawing.Point(0, 128);
			this.comboBoxMatTex4.Name = "comboBoxMatTex4";
			this.comboBoxMatTex4.Size = new System.Drawing.Size(250, 21);
			this.comboBoxMatTex4.Sorted = true;
			this.comboBoxMatTex4.TabIndex = 8;
			// 
			// comboBoxMatTex3
			// 
			this.comboBoxMatTex3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMatTex3.Location = new System.Drawing.Point(0, 105);
			this.comboBoxMatTex3.Name = "comboBoxMatTex3";
			this.comboBoxMatTex3.Size = new System.Drawing.Size(250, 21);
			this.comboBoxMatTex3.Sorted = true;
			this.comboBoxMatTex3.TabIndex = 7;
			// 
			// comboBoxMatTex2
			// 
			this.comboBoxMatTex2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMatTex2.Location = new System.Drawing.Point(0, 82);
			this.comboBoxMatTex2.Name = "comboBoxMatTex2";
			this.comboBoxMatTex2.Size = new System.Drawing.Size(250, 21);
			this.comboBoxMatTex2.Sorted = true;
			this.comboBoxMatTex2.TabIndex = 6;
			// 
			// comboBoxMatTex1
			// 
			this.comboBoxMatTex1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMatTex1.Location = new System.Drawing.Point(0, 59);
			this.comboBoxMatTex1.Name = "comboBoxMatTex1";
			this.comboBoxMatTex1.Size = new System.Drawing.Size(250, 21);
			this.comboBoxMatTex1.Sorted = true;
			this.comboBoxMatTex1.TabIndex = 5;
			// 
			// buttonMaterialRemove
			// 
			this.buttonMaterialRemove.Location = new System.Drawing.Point(2, 309);
			this.buttonMaterialRemove.Name = "buttonMaterialRemove";
			this.buttonMaterialRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonMaterialRemove.TabIndex = 145;
			this.buttonMaterialRemove.Text = "Remove";
			this.buttonMaterialRemove.UseVisualStyleBackColor = true;
			this.buttonMaterialRemove.Click += new System.EventHandler(this.buttonMaterialRemove_Click);
			// 
			// buttonMaterialCopy
			// 
			this.buttonMaterialCopy.Location = new System.Drawing.Point(89, 309);
			this.buttonMaterialCopy.Name = "buttonMaterialCopy";
			this.buttonMaterialCopy.Size = new System.Drawing.Size(75, 23);
			this.buttonMaterialCopy.TabIndex = 146;
			this.buttonMaterialCopy.Text = "Copy->New";
			this.buttonMaterialCopy.UseVisualStyleBackColor = true;
			this.buttonMaterialCopy.Click += new System.EventHandler(this.buttonMaterialCopy_Click);
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(-2, 4);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(35, 13);
			this.label17.TabIndex = 8;
			this.label17.Text = "Name";
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(-3, 45);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(76, 13);
			this.label23.TabIndex = 22;
			this.label23.Text = "Textures Used";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(219, 158);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(14, 13);
			this.label11.TabIndex = 69;
			this.label11.Text = "A";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(169, 158);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(14, 13);
			this.label10.TabIndex = 68;
			this.label10.Text = "B";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(119, 158);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(15, 13);
			this.label7.TabIndex = 67;
			this.label7.Text = "G";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(69, 158);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(15, 13);
			this.label2.TabIndex = 66;
			this.label2.Text = "R";
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(2, 247);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(48, 13);
			this.label22.TabIndex = 17;
			this.label22.Text = "Emissive";
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(-2, 271);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(52, 13);
			this.label21.TabIndex = 16;
			this.label21.Text = "Shininess";
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(0, 224);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(49, 13);
			this.label20.TabIndex = 15;
			this.label20.Text = "Specular";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(5, 199);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(45, 13);
			this.label19.TabIndex = 14;
			this.label19.Text = "Ambient";
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(10, 175);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(40, 13);
			this.label18.TabIndex = 13;
			this.label18.Text = "Diffuse";
			// 
			// textBoxMatSpecularPower
			// 
			this.textBoxMatSpecularPower.Location = new System.Drawing.Point(51, 268);
			this.textBoxMatSpecularPower.Name = "textBoxMatSpecularPower";
			this.textBoxMatSpecularPower.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatSpecularPower.TabIndex = 38;
			// 
			// textBoxMatEmissiveA
			// 
			this.textBoxMatEmissiveA.Location = new System.Drawing.Point(201, 244);
			this.textBoxMatEmissiveA.Name = "textBoxMatEmissiveA";
			this.textBoxMatEmissiveA.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatEmissiveA.TabIndex = 37;
			// 
			// textBoxMatEmissiveB
			// 
			this.textBoxMatEmissiveB.Location = new System.Drawing.Point(151, 244);
			this.textBoxMatEmissiveB.Name = "textBoxMatEmissiveB";
			this.textBoxMatEmissiveB.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatEmissiveB.TabIndex = 36;
			// 
			// textBoxMatEmissiveG
			// 
			this.textBoxMatEmissiveG.Location = new System.Drawing.Point(101, 244);
			this.textBoxMatEmissiveG.Name = "textBoxMatEmissiveG";
			this.textBoxMatEmissiveG.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatEmissiveG.TabIndex = 35;
			// 
			// textBoxMatEmissiveR
			// 
			this.textBoxMatEmissiveR.Location = new System.Drawing.Point(51, 244);
			this.textBoxMatEmissiveR.Name = "textBoxMatEmissiveR";
			this.textBoxMatEmissiveR.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatEmissiveR.TabIndex = 34;
			// 
			// textBoxMatSpecularA
			// 
			this.textBoxMatSpecularA.Location = new System.Drawing.Point(201, 220);
			this.textBoxMatSpecularA.Name = "textBoxMatSpecularA";
			this.textBoxMatSpecularA.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatSpecularA.TabIndex = 33;
			// 
			// textBoxMatSpecularB
			// 
			this.textBoxMatSpecularB.Location = new System.Drawing.Point(151, 220);
			this.textBoxMatSpecularB.Name = "textBoxMatSpecularB";
			this.textBoxMatSpecularB.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatSpecularB.TabIndex = 32;
			// 
			// textBoxMatSpecularG
			// 
			this.textBoxMatSpecularG.Location = new System.Drawing.Point(101, 220);
			this.textBoxMatSpecularG.Name = "textBoxMatSpecularG";
			this.textBoxMatSpecularG.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatSpecularG.TabIndex = 31;
			// 
			// textBoxMatSpecularR
			// 
			this.textBoxMatSpecularR.Location = new System.Drawing.Point(51, 220);
			this.textBoxMatSpecularR.Name = "textBoxMatSpecularR";
			this.textBoxMatSpecularR.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatSpecularR.TabIndex = 30;
			// 
			// textBoxMatAmbientA
			// 
			this.textBoxMatAmbientA.Location = new System.Drawing.Point(201, 196);
			this.textBoxMatAmbientA.Name = "textBoxMatAmbientA";
			this.textBoxMatAmbientA.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatAmbientA.TabIndex = 29;
			// 
			// textBoxMatAmbientB
			// 
			this.textBoxMatAmbientB.Location = new System.Drawing.Point(151, 196);
			this.textBoxMatAmbientB.Name = "textBoxMatAmbientB";
			this.textBoxMatAmbientB.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatAmbientB.TabIndex = 28;
			// 
			// textBoxMatAmbientG
			// 
			this.textBoxMatAmbientG.Location = new System.Drawing.Point(101, 196);
			this.textBoxMatAmbientG.Name = "textBoxMatAmbientG";
			this.textBoxMatAmbientG.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatAmbientG.TabIndex = 27;
			// 
			// textBoxMatAmbientR
			// 
			this.textBoxMatAmbientR.Location = new System.Drawing.Point(51, 196);
			this.textBoxMatAmbientR.Name = "textBoxMatAmbientR";
			this.textBoxMatAmbientR.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatAmbientR.TabIndex = 26;
			// 
			// textBoxMatDiffuseA
			// 
			this.textBoxMatDiffuseA.Location = new System.Drawing.Point(201, 172);
			this.textBoxMatDiffuseA.Name = "textBoxMatDiffuseA";
			this.textBoxMatDiffuseA.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatDiffuseA.TabIndex = 25;
			// 
			// textBoxMatDiffuseB
			// 
			this.textBoxMatDiffuseB.Location = new System.Drawing.Point(151, 172);
			this.textBoxMatDiffuseB.Name = "textBoxMatDiffuseB";
			this.textBoxMatDiffuseB.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatDiffuseB.TabIndex = 24;
			// 
			// textBoxMatDiffuseG
			// 
			this.textBoxMatDiffuseG.Location = new System.Drawing.Point(101, 172);
			this.textBoxMatDiffuseG.Name = "textBoxMatDiffuseG";
			this.textBoxMatDiffuseG.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatDiffuseG.TabIndex = 23;
			// 
			// textBoxMatDiffuseR
			// 
			this.textBoxMatDiffuseR.Location = new System.Drawing.Point(51, 172);
			this.textBoxMatDiffuseR.Name = "textBoxMatDiffuseR";
			this.textBoxMatDiffuseR.Size = new System.Drawing.Size(49, 20);
			this.textBoxMatDiffuseR.TabIndex = 18;
			// 
			// textBoxMatName
			// 
			this.textBoxMatName.Location = new System.Drawing.Point(0, 19);
			this.textBoxMatName.Name = "textBoxMatName";
			this.textBoxMatName.Size = new System.Drawing.Size(250, 20);
			this.textBoxMatName.TabIndex = 3;
			// 
			// tabPageTextureView
			// 
			this.tabPageTextureView.Controls.Add(this.buttonTextureAdd);
			this.tabPageTextureView.Controls.Add(this.panelTexturePic);
			this.tabPageTextureView.Controls.Add(this.label3);
			this.tabPageTextureView.Controls.Add(this.buttonTextureExport);
			this.tabPageTextureView.Controls.Add(this.buttonTextureReplace);
			this.tabPageTextureView.Controls.Add(this.buttonTextureRemove);
			this.tabPageTextureView.Controls.Add(this.textBoxTexSize);
			this.tabPageTextureView.Controls.Add(this.textBoxTexName);
			this.tabPageTextureView.Location = new System.Drawing.Point(4, 22);
			this.tabPageTextureView.Name = "tabPageTextureView";
			this.tabPageTextureView.Size = new System.Drawing.Size(253, 492);
			this.tabPageTextureView.TabIndex = 3;
			this.tabPageTextureView.Text = "Texture";
			this.tabPageTextureView.UseVisualStyleBackColor = true;
			// 
			// buttonTextureAdd
			// 
			this.buttonTextureAdd.Location = new System.Drawing.Point(2, 70);
			this.buttonTextureAdd.Name = "buttonTextureAdd";
			this.buttonTextureAdd.Size = new System.Drawing.Size(75, 23);
			this.buttonTextureAdd.TabIndex = 30;
			this.buttonTextureAdd.Text = "Add Image";
			this.buttonTextureAdd.UseVisualStyleBackColor = true;
			this.buttonTextureAdd.Click += new System.EventHandler(this.buttonTextureAdd_Click);
			// 
			// panelTexturePic
			// 
			this.panelTexturePic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelTexturePic.Controls.Add(this.pictureBoxTexture);
			this.panelTexturePic.Location = new System.Drawing.Point(0, 109);
			this.panelTexturePic.Name = "panelTexturePic";
			this.panelTexturePic.Size = new System.Drawing.Size(252, 401);
			this.panelTexturePic.TabIndex = 38;
			// 
			// pictureBoxTexture
			// 
			this.pictureBoxTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBoxTexture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBoxTexture.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxTexture.Name = "pictureBoxTexture";
			this.pictureBoxTexture.Size = new System.Drawing.Size(252, 221);
			this.pictureBoxTexture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxTexture.TabIndex = 1;
			this.pictureBoxTexture.TabStop = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(-3, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Texture";
			// 
			// buttonTextureExport
			// 
			this.buttonTextureExport.Location = new System.Drawing.Point(2, 36);
			this.buttonTextureExport.Name = "buttonTextureExport";
			this.buttonTextureExport.Size = new System.Drawing.Size(75, 22);
			this.buttonTextureExport.TabIndex = 16;
			this.buttonTextureExport.Text = "Export";
			this.buttonTextureExport.UseVisualStyleBackColor = true;
			this.buttonTextureExport.Click += new System.EventHandler(this.buttonTextureExport_Click);
			// 
			// buttonTextureReplace
			// 
			this.buttonTextureReplace.Location = new System.Drawing.Point(89, 70);
			this.buttonTextureReplace.Name = "buttonTextureReplace";
			this.buttonTextureReplace.Size = new System.Drawing.Size(75, 23);
			this.buttonTextureReplace.TabIndex = 36;
			this.buttonTextureReplace.Text = "Replace";
			this.buttonTextureReplace.UseVisualStyleBackColor = true;
			this.buttonTextureReplace.Click += new System.EventHandler(this.buttonTextureReplace_Click);
			// 
			// buttonTextureRemove
			// 
			this.buttonTextureRemove.Location = new System.Drawing.Point(89, 36);
			this.buttonTextureRemove.Name = "buttonTextureRemove";
			this.buttonTextureRemove.Size = new System.Drawing.Size(75, 22);
			this.buttonTextureRemove.TabIndex = 20;
			this.buttonTextureRemove.Text = "Remove";
			this.buttonTextureRemove.UseVisualStyleBackColor = true;
			this.buttonTextureRemove.Click += new System.EventHandler(this.buttonTextureRemove_Click);
			// 
			// textBoxTexSize
			// 
			this.textBoxTexSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxTexSize.Location = new System.Drawing.Point(190, 6);
			this.textBoxTexSize.Name = "textBoxTexSize";
			this.textBoxTexSize.ReadOnly = true;
			this.textBoxTexSize.Size = new System.Drawing.Size(60, 20);
			this.textBoxTexSize.TabIndex = 37;
			this.textBoxTexSize.TabStop = false;
			// 
			// textBoxTexName
			// 
			this.textBoxTexName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxTexName.Location = new System.Drawing.Point(40, 6);
			this.textBoxTexName.Name = "textBoxTexName";
			this.textBoxTexName.Size = new System.Drawing.Size(148, 20);
			this.textBoxTexName.TabIndex = 5;
			// 
			// FormXX
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(524, 536);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormXX";
			this.Text = "FormXX";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.tabControlLists.ResumeLayout(false);
			this.tabPageObject.ResumeLayout(false);
			this.panelObjectTreeBottom.ResumeLayout(false);
			this.tabPageMesh.ResumeLayout(false);
			this.splitContainerMesh.Panel1.ResumeLayout(false);
			this.splitContainerMesh.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMesh)).EndInit();
			this.splitContainerMesh.ResumeLayout(false);
			this.splitContainerMeshCrossRef.Panel1.ResumeLayout(false);
			this.splitContainerMeshCrossRef.Panel1.PerformLayout();
			this.splitContainerMeshCrossRef.Panel2.ResumeLayout(false);
			this.splitContainerMeshCrossRef.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMeshCrossRef)).EndInit();
			this.splitContainerMeshCrossRef.ResumeLayout(false);
			this.tabPageMaterial.ResumeLayout(false);
			this.splitContainerMaterial.Panel1.ResumeLayout(false);
			this.splitContainerMaterial.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMaterial)).EndInit();
			this.splitContainerMaterial.ResumeLayout(false);
			this.splitContainerMaterialCrossRef.Panel1.ResumeLayout(false);
			this.splitContainerMaterialCrossRef.Panel1.PerformLayout();
			this.splitContainerMaterialCrossRef.Panel2.ResumeLayout(false);
			this.splitContainerMaterialCrossRef.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMaterialCrossRef)).EndInit();
			this.splitContainerMaterialCrossRef.ResumeLayout(false);
			this.tabPageTexture.ResumeLayout(false);
			this.splitContainerTexture.Panel1.ResumeLayout(false);
			this.splitContainerTexture.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerTexture)).EndInit();
			this.splitContainerTexture.ResumeLayout(false);
			this.splitContainerTextureCrossRef.Panel1.ResumeLayout(false);
			this.splitContainerTextureCrossRef.Panel1.PerformLayout();
			this.splitContainerTextureCrossRef.Panel2.ResumeLayout(false);
			this.splitContainerTextureCrossRef.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerTextureCrossRef)).EndInit();
			this.splitContainerTextureCrossRef.ResumeLayout(false);
			this.tabControlViews.ResumeLayout(false);
			this.tabPageFrameView.ResumeLayout(false);
			this.tabPageFrameView.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericFrameMatrixRatio)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericFrameMatrixNumber)).EndInit();
			this.tabControlFrameMatrix.ResumeLayout(false);
			this.tabPageFrameSRT.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewFrameSRT)).EndInit();
			this.tabPageFrameMatrix.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewFrameMatrix)).EndInit();
			this.tabPageBoneView.ResumeLayout(false);
			this.tabPageBoneView.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericBoneMatrixRatio)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericBoneMatrixNumber)).EndInit();
			this.tabControlBoneMatrix.ResumeLayout(false);
			this.tabPageBoneSRT.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewBoneSRT)).EndInit();
			this.tabPageBoneMatrix.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewBoneMatrix)).EndInit();
			this.tabPageMeshView.ResumeLayout(false);
			this.tabPageMeshView.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.panelMeshExportOptionsFbx.ResumeLayout(false);
			this.panelMeshExportOptionsFbx.PerformLayout();
			this.panelMeshExportOptionsDirectX.ResumeLayout(false);
			this.panelMeshExportOptionsDirectX.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericMeshExportDirectXTicksPerSecond)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericMeshExportDirectXKeyframeLength)).EndInit();
			this.panelMeshExportOptionsCollada.ResumeLayout(false);
			this.panelMeshExportOptionsCollada.PerformLayout();
			this.panelMeshExportOptionsMqo.ResumeLayout(false);
			this.panelMeshExportOptionsMqo.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewMesh)).EndInit();
			this.tabPageMaterialView.ResumeLayout(false);
			this.tabPageMaterialView.PerformLayout();
			this.tabPageTextureView.ResumeLayout(false);
			this.tabPageTextureView.PerformLayout();
			this.panelTexturePic.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTexture)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.TabControl tabControlLists;
		private System.Windows.Forms.TabPage tabPageObject;
		public System.Windows.Forms.TreeView treeViewObjectTree;
		private System.Windows.Forms.Panel panelObjectTreeBottom;
		private System.Windows.Forms.Button buttonObjectTreeCollapse;
		private System.Windows.Forms.Button buttonObjectTreeExpand;
		private System.Windows.Forms.TabPage tabPageMesh;
		private System.Windows.Forms.SplitContainer splitContainerMesh;
		public System.Windows.Forms.ListView listViewMesh;
		private System.Windows.Forms.ColumnHeader meshlistHeader;
		private System.Windows.Forms.SplitContainer splitContainerMeshCrossRef;
		private System.Windows.Forms.ListView listViewMeshMaterial;
		private System.Windows.Forms.ColumnHeader listViewMeshMaterialHeader;
		private System.Windows.Forms.Label label68;
		private System.Windows.Forms.ListView listViewMeshTexture;
		private System.Windows.Forms.ColumnHeader listViewMeshTextureHeader;
		private System.Windows.Forms.Label label70;
		private System.Windows.Forms.TabPage tabPageMaterial;
		private System.Windows.Forms.SplitContainer splitContainerMaterial;
		private System.Windows.Forms.ListView listViewMaterial;
		private System.Windows.Forms.ColumnHeader materiallistHeader;
		private System.Windows.Forms.SplitContainer splitContainerMaterialCrossRef;
		private System.Windows.Forms.ListView listViewMaterialMesh;
		private System.Windows.Forms.ColumnHeader listViewMaterialMeshHeader;
		private System.Windows.Forms.Label label71;
		private System.Windows.Forms.ListView listViewMaterialTexture;
		private System.Windows.Forms.ColumnHeader listViewMaterialTextureHeader;
		private System.Windows.Forms.Label label72;
		private System.Windows.Forms.TabPage tabPageTexture;
		private System.Windows.Forms.SplitContainer splitContainerTexture;
		private System.Windows.Forms.ListView listViewTexture;
		private System.Windows.Forms.ColumnHeader texturelistHeader;
		private System.Windows.Forms.SplitContainer splitContainerTextureCrossRef;
		private System.Windows.Forms.ListView listViewTextureMesh;
		private System.Windows.Forms.ColumnHeader listViewTextureMeshHeader;
		private System.Windows.Forms.Label label73;
		private System.Windows.Forms.ListView listViewTextureMaterial;
		private System.Windows.Forms.ColumnHeader listViewTextureMaterialHeader;
		private System.Windows.Forms.Label label74;
		private System.Windows.Forms.Label label1;
		private EditTextBox textBoxMeshName;
		private System.Windows.Forms.Button buttonSubmeshRemove;
		private System.Windows.Forms.Button buttonMeshRemove;
		private System.Windows.Forms.ComboBox comboBoxMeshExportFormat;
		private System.Windows.Forms.Button buttonSubmeshEdit;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel panelMeshExportOptionsCollada;
		private System.Windows.Forms.CheckBox checkBoxMeshExportColladaAllFrames;
		private System.Windows.Forms.Panel panelMeshExportOptionsMqo;
		private System.Windows.Forms.CheckBox checkBoxMeshExportMqoSingleFile;
		private System.Windows.Forms.CheckBox checkBoxMeshExportMqoWorldCoords;
		private System.Windows.Forms.TabPage tabPageMaterialView;
		private System.Windows.Forms.ComboBox comboBoxMatTex4;
		private System.Windows.Forms.ComboBox comboBoxMatTex3;
		private System.Windows.Forms.ComboBox comboBoxMatTex2;
		private System.Windows.Forms.ComboBox comboBoxMatTex1;
		private System.Windows.Forms.Button buttonMaterialRemove;
		private System.Windows.Forms.Button buttonMaterialCopy;
		private EditTextBox textBoxMatSpecularPower;
		private EditTextBox textBoxMatEmissiveA;
		private EditTextBox textBoxMatEmissiveB;
		private EditTextBox textBoxMatEmissiveG;
		private EditTextBox textBoxMatEmissiveR;
		private EditTextBox textBoxMatSpecularA;
		private EditTextBox textBoxMatSpecularB;
		private EditTextBox textBoxMatSpecularG;
		private EditTextBox textBoxMatSpecularR;
		private EditTextBox textBoxMatAmbientA;
		private EditTextBox textBoxMatAmbientB;
		private EditTextBox textBoxMatAmbientG;
		private EditTextBox textBoxMatAmbientR;
		private EditTextBox textBoxMatDiffuseA;
		private EditTextBox textBoxMatDiffuseB;
		private EditTextBox textBoxMatDiffuseG;
		private EditTextBox textBoxMatDiffuseR;
		private EditTextBox textBoxMatName;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Panel panelMeshExportOptionsFbx;
		private System.Windows.Forms.CheckBox checkBoxMeshExportFbxSkins;
		private System.Windows.Forms.CheckBox checkBoxMeshExportFbxAllFrames;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox checkBoxMeshSkinned;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.NumericUpDown numericMeshExportDirectXKeyframeLength;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Button buttonMeshExport;
		private System.Windows.Forms.Button buttonBoneRemove;
		private System.Windows.Forms.Button buttonBoneCopy;
		private System.Windows.Forms.Button buttonBoneGotoFrame;
		private System.Windows.Forms.Label label3;
		private EditTextBox textBoxTexName;
		private System.Windows.Forms.Button buttonTextureRemove;
		private System.Windows.Forms.Button buttonTextureExport;
		private EditTextBox textBoxTexSize;
		private System.Windows.Forms.SplitContainer splitContainer1;
		public System.Windows.Forms.TabControl tabControlViews;
		private System.Windows.Forms.TabPage tabPageFrameView;
		private System.Windows.Forms.Button buttonFrameMoveUp;
		private System.Windows.Forms.Button buttonFrameRemove;
		private System.Windows.Forms.Button buttonFrameMoveDown;
		private System.Windows.Forms.Button buttonFrameMatrixIdentity;
		private System.Windows.Forms.Button buttonFrameMatrixInverse;
		private System.Windows.Forms.Button buttonFrameMatrixCombined;
		private System.Windows.Forms.Label label4;
		private EditTextBox textBoxFrameName;
		private System.Windows.Forms.TabPage tabPageBoneView;
		private EditTextBox textBoxBoneName;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.TabPage tabPageMeshView;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Panel panelMeshExportOptionsDefault;
		private System.Windows.Forms.Panel panelMeshExportOptionsDirectX;
		private System.Windows.Forms.NumericUpDown numericMeshExportDirectXTicksPerSecond;
		private System.Windows.Forms.TabPage tabPageTextureView;
		private System.Windows.Forms.TabControl tabControlFrameMatrix;
		private System.Windows.Forms.TabPage tabPageFrameSRT;
		private System.Windows.Forms.TabPage tabPageFrameMatrix;
		private System.Windows.Forms.Label label15;
		private EditTextBox textBoxFrameName2;
		private EditTextBox textBoxFormat;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Button buttonConvert;
		private System.Windows.Forms.Button buttonEditHex;
		private System.Windows.Forms.DataGridView dataGridViewMesh;
		private System.Windows.Forms.Panel panelTexturePic;
		private System.Windows.Forms.Button buttonTextureReplace;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button buttonFrameMatrixGrow;
		private System.Windows.Forms.Button buttonFrameMatrixShrink;
		private System.Windows.Forms.PictureBox pictureBoxTexture;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSubmeshVerts;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSubmeshFaces;
		private System.Windows.Forms.DataGridViewComboBoxColumn ColumnSubmeshMaterial;
		private System.Windows.Forms.Button MeshGotoFrame;
		private System.Windows.Forms.Button buttonTextureAdd;
		private System.Windows.Forms.Button buttonMeshMinBones;
		private System.Windows.Forms.NumericUpDown numericFrameMatrixRatio;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericFrameMatrixNumber;
		private System.Windows.Forms.Button buttonFrameMatrixPaste;
		private System.Windows.Forms.Button buttonFrameMatrixCopy;
		private DataGridViewEditor dataGridViewFrameSRT;
		private DataGridViewEditor dataGridViewFrameMatrix;
		private System.Windows.Forms.Button buttonFrameMatrixApply;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button buttonBoneMatrixApply;
		private System.Windows.Forms.NumericUpDown numericBoneMatrixRatio;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown numericBoneMatrixNumber;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Button buttonBoneMatrixPaste;
		private System.Windows.Forms.Button buttonBoneMatrixCopy;
		private System.Windows.Forms.Button buttonBoneMatrixGrow;
		private System.Windows.Forms.Button buttonBoneMatrixShrink;
		private System.Windows.Forms.Button buttonBoneMatrixIdentity;
		private System.Windows.Forms.Button buttonBoneMatrixInverse;
		private System.Windows.Forms.TabControl tabControlBoneMatrix;
		private System.Windows.Forms.TabPage tabPageBoneSRT;
		private DataGridViewEditor dataGridViewBoneSRT;
		private System.Windows.Forms.TabPage tabPageBoneMatrix;
		private DataGridViewEditor dataGridViewBoneMatrix;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Button buttonFrameEditHex;
		private System.Windows.Forms.Button buttonMeshEditHex;
		private System.Windows.Forms.Button buttonMeshNormals;
		private System.Windows.Forms.Button buttonTextureExternal;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reopenToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem savexxToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem savexxAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem keepBackupToolStripMenuItem;
		private System.Windows.Forms.Label label13;
		public EditTextBox textBoxKeyframeRange;
	}
}