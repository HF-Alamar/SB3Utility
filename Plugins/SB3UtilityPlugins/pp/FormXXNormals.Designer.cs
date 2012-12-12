namespace SB3Utility
{
	partial class FormXXNormals
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormXXNormals));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.numericThreshold = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.checkBoxSelectedItemsOnly = new System.Windows.Forms.CheckBox();
			this.checkBoxCalculateNormalsInXAs = new System.Windows.Forms.CheckBox();
			this.checkBoxNewNormalsForMesh = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numericThreshold)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(16, 118);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(138, 118);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// numericThreshold
			// 
			this.numericThreshold.DecimalPlaces = 6;
			this.numericThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numericThreshold.Location = new System.Drawing.Point(73, 16);
			this.numericThreshold.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericThreshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.numericThreshold.Name = "numericThreshold";
			this.numericThreshold.Size = new System.Drawing.Size(140, 20);
			this.numericThreshold.TabIndex = 2;
			this.toolTip1.SetToolTip(this.numericThreshold, resources.GetString("numericThreshold.ToolTip"));
			this.numericThreshold.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Threshold";
			// 
			// checkBoxSelectedItemsOnly
			// 
			this.checkBoxSelectedItemsOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxSelectedItemsOnly.AutoSize = true;
			this.checkBoxSelectedItemsOnly.Location = new System.Drawing.Point(42, 90);
			this.checkBoxSelectedItemsOnly.Name = "checkBoxSelectedItemsOnly";
			this.checkBoxSelectedItemsOnly.Size = new System.Drawing.Size(171, 17);
			this.checkBoxSelectedItemsOnly.TabIndex = 5;
			this.checkBoxSelectedItemsOnly.Text = "Selected Clips/Keyframes Only";
			this.toolTip1.SetToolTip(this.checkBoxSelectedItemsOnly, "Only the selection in the Morph Clips is taken.");
			this.checkBoxSelectedItemsOnly.UseVisualStyleBackColor = true;
			// 
			// checkBoxCalculateNormalsInXAs
			// 
			this.checkBoxCalculateNormalsInXAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxCalculateNormalsInXAs.AutoSize = true;
			this.checkBoxCalculateNormalsInXAs.Location = new System.Drawing.Point(16, 67);
			this.checkBoxCalculateNormalsInXAs.Name = "checkBoxCalculateNormalsInXAs";
			this.checkBoxCalculateNormalsInXAs.Size = new System.Drawing.Size(197, 17);
			this.checkBoxCalculateNormalsInXAs.TabIndex = 4;
			this.checkBoxCalculateNormalsInXAs.Text = "Calculate Normals Of Morphs In XAs";
			this.checkBoxCalculateNormalsInXAs.UseVisualStyleBackColor = true;
			// 
			// checkBoxNewNormalsForMesh
			// 
			this.checkBoxNewNormalsForMesh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxNewNormalsForMesh.AutoSize = true;
			this.checkBoxNewNormalsForMesh.Checked = true;
			this.checkBoxNewNormalsForMesh.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxNewNormalsForMesh.Location = new System.Drawing.Point(16, 42);
			this.checkBoxNewNormalsForMesh.Name = "checkBoxNewNormalsForMesh";
			this.checkBoxNewNormalsForMesh.Size = new System.Drawing.Size(183, 17);
			this.checkBoxNewNormalsForMesh.TabIndex = 6;
			this.checkBoxNewNormalsForMesh.Text = "Calculate New Normals For Mesh";
			this.checkBoxNewNormalsForMesh.UseVisualStyleBackColor = true;
			// 
			// FormXXNormals
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(234, 155);
			this.Controls.Add(this.checkBoxNewNormalsForMesh);
			this.Controls.Add(this.checkBoxSelectedItemsOnly);
			this.Controls.Add(this.checkBoxCalculateNormalsInXAs);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericThreshold);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormXXNormals";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Calculate Normals";
			((System.ComponentModel.ISupportInitialize)(this.numericThreshold)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.NumericUpDown numericThreshold;
		private System.Windows.Forms.ToolTip toolTip1;
		public System.Windows.Forms.CheckBox checkBoxSelectedItemsOnly;
		public System.Windows.Forms.CheckBox checkBoxCalculateNormalsInXAs;
		public System.Windows.Forms.CheckBox checkBoxNewNormalsForMesh;
	}
}