namespace SB3Utility
{
	partial class FormRenderer
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonCenterView = new System.Windows.Forms.Button();
			this.buttonResetPose = new System.Windows.Forms.Button();
			this.numericSensitivity = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.rendererToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.wireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.normalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cullingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.diffuseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ambientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.specularToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.numericSensitivity)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Location = new System.Drawing.Point(12, 39);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(320, 198);
			this.panel1.TabIndex = 0;
			// 
			// buttonCenterView
			// 
			this.buttonCenterView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCenterView.Location = new System.Drawing.Point(109, 251);
			this.buttonCenterView.Name = "buttonCenterView";
			this.buttonCenterView.Size = new System.Drawing.Size(75, 23);
			this.buttonCenterView.TabIndex = 1;
			this.buttonCenterView.TabStop = false;
			this.buttonCenterView.Text = "Center View";
			this.buttonCenterView.UseVisualStyleBackColor = true;
			this.buttonCenterView.Click += new System.EventHandler(this.buttonCenterView_Click);
			// 
			// buttonResetPose
			// 
			this.buttonResetPose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonResetPose.Location = new System.Drawing.Point(12, 251);
			this.buttonResetPose.Name = "buttonResetPose";
			this.buttonResetPose.Size = new System.Drawing.Size(75, 23);
			this.buttonResetPose.TabIndex = 2;
			this.buttonResetPose.TabStop = false;
			this.buttonResetPose.Text = "Reset Pose";
			this.buttonResetPose.UseVisualStyleBackColor = true;
			this.buttonResetPose.Click += new System.EventHandler(this.buttonResetPose_Click);
			// 
			// numericSensitivity
			// 
			this.numericSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericSensitivity.Location = new System.Drawing.Point(281, 253);
			this.numericSensitivity.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericSensitivity.Name = "numericSensitivity";
			this.numericSensitivity.Size = new System.Drawing.Size(51, 20);
			this.numericSensitivity.TabIndex = 3;
			this.numericSensitivity.TabStop = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(221, 256);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Sensitivity";
			// 
			// colorDialog1
			// 
			this.colorDialog1.AnyColor = true;
			this.colorDialog1.FullOpen = true;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rendererToolStripMenuItem,
            this.colorToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(344, 24);
			this.menuStrip1.TabIndex = 13;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// rendererToolStripMenuItem
			// 
			this.rendererToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wireframeToolStripMenuItem,
            this.normalsToolStripMenuItem,
            this.bonesToolStripMenuItem,
            this.cullingToolStripMenuItem});
			this.rendererToolStripMenuItem.Name = "rendererToolStripMenuItem";
			this.rendererToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.rendererToolStripMenuItem.Text = "Render";
			// 
			// wireframeToolStripMenuItem
			// 
			this.wireframeToolStripMenuItem.CheckOnClick = true;
			this.wireframeToolStripMenuItem.Name = "wireframeToolStripMenuItem";
			this.wireframeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.wireframeToolStripMenuItem.Text = "Wireframe";
			// 
			// normalsToolStripMenuItem
			// 
			this.normalsToolStripMenuItem.CheckOnClick = true;
			this.normalsToolStripMenuItem.Name = "normalsToolStripMenuItem";
			this.normalsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.normalsToolStripMenuItem.Text = "Normals";
			// 
			// bonesToolStripMenuItem
			// 
			this.bonesToolStripMenuItem.CheckOnClick = true;
			this.bonesToolStripMenuItem.Name = "bonesToolStripMenuItem";
			this.bonesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.bonesToolStripMenuItem.Text = "Bones";
			// 
			// cullingToolStripMenuItem
			// 
			this.cullingToolStripMenuItem.CheckOnClick = true;
			this.cullingToolStripMenuItem.Name = "cullingToolStripMenuItem";
			this.cullingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.cullingToolStripMenuItem.Text = "Culling";
			// 
			// colorToolStripMenuItem
			// 
			this.colorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.diffuseToolStripMenuItem,
            this.ambientToolStripMenuItem,
            this.specularToolStripMenuItem,
            this.backgroundToolStripMenuItem});
			this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
			this.colorToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.colorToolStripMenuItem.Text = "Color";
			// 
			// diffuseToolStripMenuItem
			// 
			this.diffuseToolStripMenuItem.Name = "diffuseToolStripMenuItem";
			this.diffuseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.diffuseToolStripMenuItem.Text = "Diffuse...";
			this.diffuseToolStripMenuItem.Click += new System.EventHandler(this.diffuseToolStripMenuItem_Click);
			// 
			// ambientToolStripMenuItem
			// 
			this.ambientToolStripMenuItem.Name = "ambientToolStripMenuItem";
			this.ambientToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.ambientToolStripMenuItem.Text = "Ambient...";
			this.ambientToolStripMenuItem.Click += new System.EventHandler(this.ambientToolStripMenuItem_Click);
			// 
			// specularToolStripMenuItem
			// 
			this.specularToolStripMenuItem.Name = "specularToolStripMenuItem";
			this.specularToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.specularToolStripMenuItem.Text = "Specular...";
			this.specularToolStripMenuItem.Click += new System.EventHandler(this.specularToolStripMenuItem_Click);
			// 
			// backgroundToolStripMenuItem
			// 
			this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
			this.backgroundToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.backgroundToolStripMenuItem.Text = "Background...";
			this.backgroundToolStripMenuItem.Click += new System.EventHandler(this.backgroundToolStripMenuItem_Click);
			// 
			// FormRenderer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 285);
			this.Controls.Add(this.numericSensitivity);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonResetPose);
			this.Controls.Add(this.buttonCenterView);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormRenderer";
			this.Text = "FormRenderer";
			((System.ComponentModel.ISupportInitialize)(this.numericSensitivity)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonCenterView;
		private System.Windows.Forms.Button buttonResetPose;
		private System.Windows.Forms.NumericUpDown numericSensitivity;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem rendererToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem wireframeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem normalsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bonesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cullingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem diffuseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ambientToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem specularToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem backgroundToolStripMenuItem;
	}
}