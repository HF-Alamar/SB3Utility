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
			this.panelAnimation = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.numericResample = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.numericPosition = new System.Windows.Forms.NumericUpDown();
			this.comboBoxMethod = new System.Windows.Forms.ComboBox();
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
			this.panelAnimation.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericResample)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericPosition)).BeginInit();
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
			// panelAnimation
			// 
			this.panelAnimation.Controls.Add(this.groupBox1);
			this.panelAnimation.Location = new System.Drawing.Point(12, 12);
			this.panelAnimation.Name = "panelAnimation";
			this.panelAnimation.Size = new System.Drawing.Size(367, 121);
			this.panelAnimation.TabIndex = 15;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.numericResample);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.numericPosition);
			this.groupBox1.Controls.Add(this.comboBoxMethod);
			this.groupBox1.Location = new System.Drawing.Point(38, 17);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(291, 87);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Replacing Options";
			// 
			// numericResample
			// 
			this.numericResample.Location = new System.Drawing.Point(179, 16);
			this.numericResample.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericResample.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericResample.Name = "numericResample";
			this.numericResample.Size = new System.Drawing.Size(104, 20);
			this.numericResample.TabIndex = 5;
			this.numericResample.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(170, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Resample Number of Keyframes to";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(168, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Merge/Insert At Keyframe Position";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 42);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(189, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Importing Method for Animation Tracks";
			// 
			// numericPosition
			// 
			this.numericPosition.Location = new System.Drawing.Point(179, 61);
			this.numericPosition.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericPosition.Name = "numericPosition";
			this.numericPosition.Size = new System.Drawing.Size(104, 20);
			this.numericPosition.TabIndex = 1;
			// 
			// comboBoxMethod
			// 
			this.comboBoxMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMethod.FormattingEnabled = true;
			this.comboBoxMethod.Location = new System.Drawing.Point(199, 38);
			this.comboBoxMethod.Name = "comboBoxMethod";
			this.comboBoxMethod.Size = new System.Drawing.Size(84, 21);
			this.comboBoxMethod.TabIndex = 0;
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
			this.Controls.Add(this.panelAnimation);
			this.Controls.Add(this.panelMorphList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormXADragDrop";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options";
			this.panelAnimation.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericResample)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericPosition)).EndInit();
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
		private System.Windows.Forms.Panel panelAnimation;
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
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.NumericUpDown numericResample;
		public System.Windows.Forms.ComboBox comboBoxMethod;
		public System.Windows.Forms.NumericUpDown numericPosition;
	}
}