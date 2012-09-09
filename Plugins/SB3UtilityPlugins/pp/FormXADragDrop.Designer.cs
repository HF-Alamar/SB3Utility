namespace SB3Utility
{
	partial class FormXADragDrop
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panelUnused = new System.Windows.Forms.Panel();
			this.panelMorphList = new System.Windows.Forms.Panel();
			this.textBoxNewName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownMinimumDistanceSquared = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.radioButtonReplaceNormalsNo = new System.Windows.Forms.RadioButton();
			this.radioButtonReplaceNormalsYes = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.panelMorphList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinimumDistanceSquared)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(87, 152);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 9;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(225, 152);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 10;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// panelUnused
			// 
			this.panelUnused.Location = new System.Drawing.Point(12, 12);
			this.panelUnused.Name = "panelUnused";
			this.panelUnused.Size = new System.Drawing.Size(367, 121);
			this.panelUnused.TabIndex = 15;
			// 
			// panelMorphList
			// 
			this.panelMorphList.Controls.Add(this.textBoxNewName);
			this.panelMorphList.Controls.Add(this.label3);
			this.panelMorphList.Controls.Add(this.numericUpDownMinimumDistanceSquared);
			this.panelMorphList.Controls.Add(this.label5);
			this.panelMorphList.Controls.Add(this.textBoxName);
			this.panelMorphList.Controls.Add(this.panel1);
			this.panelMorphList.Controls.Add(this.label6);
			this.panelMorphList.Controls.Add(this.label7);
			this.panelMorphList.Location = new System.Drawing.Point(12, 12);
			this.panelMorphList.Name = "panelMorphList";
			this.panelMorphList.Size = new System.Drawing.Size(367, 121);
			this.panelMorphList.TabIndex = 16;
			// 
			// textBoxNewName
			// 
			this.textBoxNewName.Location = new System.Drawing.Point(139, 36);
			this.textBoxNewName.Name = "textBoxNewName";
			this.textBoxNewName.Size = new System.Drawing.Size(112, 20);
			this.textBoxNewName.TabIndex = 20;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 39);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 13);
			this.label3.TabIndex = 19;
			this.label3.Text = "Rename to";
			// 
			// numericUpDownMinimumDistanceSquared
			// 
			this.numericUpDownMinimumDistanceSquared.DecimalPlaces = 6;
			this.numericUpDownMinimumDistanceSquared.Increment = new decimal(new int[] {
            1,
            0,
            0,
            393216});
			this.numericUpDownMinimumDistanceSquared.Location = new System.Drawing.Point(138, 87);
			this.numericUpDownMinimumDistanceSquared.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.numericUpDownMinimumDistanceSquared.Name = "numericUpDownMinimumDistanceSquared";
			this.numericUpDownMinimumDistanceSquared.Size = new System.Drawing.Size(71, 20);
			this.numericUpDownMinimumDistanceSquared.TabIndex = 18;
			this.numericUpDownMinimumDistanceSquared.Value = new decimal(new int[] {
            1,
            0,
            0,
            327680});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(9, 89);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 13);
			this.label5.TabIndex = 17;
			this.label5.Text = "Minimum Distance²";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(139, 10);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(112, 20);
			this.textBoxName.TabIndex = 14;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioButtonReplaceNormalsNo);
			this.panel1.Controls.Add(this.radioButtonReplaceNormalsYes);
			this.panel1.Location = new System.Drawing.Point(132, 62);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(108, 21);
			this.panel1.TabIndex = 16;
			// 
			// radioButtonReplaceNormalsNo
			// 
			this.radioButtonReplaceNormalsNo.AutoSize = true;
			this.radioButtonReplaceNormalsNo.Location = new System.Drawing.Point(53, 2);
			this.radioButtonReplaceNormalsNo.Name = "radioButtonReplaceNormalsNo";
			this.radioButtonReplaceNormalsNo.Size = new System.Drawing.Size(39, 17);
			this.radioButtonReplaceNormalsNo.TabIndex = 1;
			this.radioButtonReplaceNormalsNo.Text = "No";
			this.radioButtonReplaceNormalsNo.UseVisualStyleBackColor = true;
			// 
			// radioButtonReplaceNormalsYes
			// 
			this.radioButtonReplaceNormalsYes.AutoSize = true;
			this.radioButtonReplaceNormalsYes.Checked = true;
			this.radioButtonReplaceNormalsYes.Location = new System.Drawing.Point(6, 2);
			this.radioButtonReplaceNormalsYes.Name = "radioButtonReplaceNormalsYes";
			this.radioButtonReplaceNormalsYes.Size = new System.Drawing.Size(43, 17);
			this.radioButtonReplaceNormalsYes.TabIndex = 0;
			this.radioButtonReplaceNormalsYes.TabStop = true;
			this.radioButtonReplaceNormalsYes.Text = "Yes";
			this.radioButtonReplaceNormalsYes.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(9, 66);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(94, 13);
			this.label6.TabIndex = 15;
			this.label6.Text = "Replace Normals?";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(9, 13);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(127, 13);
			this.label7.TabIndex = 13;
			this.label7.Text = "Target Morph Clip/Object";
			// 
			// FormXADragDrop
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(390, 192);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.panelMorphList);
			this.Controls.Add(this.panelUnused);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormXADragDrop";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options";
			this.panelMorphList.ResumeLayout(false);
			this.panelMorphList.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinimumDistanceSquared)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panelUnused;
		private System.Windows.Forms.Panel panelMorphList;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.NumericUpDown numericUpDownMinimumDistanceSquared;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		public System.Windows.Forms.TextBox textBoxNewName;
		public System.Windows.Forms.TextBox textBoxName;
		public System.Windows.Forms.RadioButton radioButtonReplaceNormalsNo;
		public System.Windows.Forms.RadioButton radioButtonReplaceNormalsYes;
	}
}