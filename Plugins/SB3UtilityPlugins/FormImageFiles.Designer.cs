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
			this.listViewImages.Location = new System.Drawing.Point(0, 0);
			this.listViewImages.MultiSelect = false;
			this.listViewImages.Name = "listViewImages";
			this.listViewImages.ShowGroups = false;
			this.listViewImages.ShowItemToolTips = true;
			this.listViewImages.Size = new System.Drawing.Size(292, 273);
			this.listViewImages.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewImages.TabIndex = 5;
			this.listViewImages.TabStop = false;
			this.listViewImages.UseCompatibleStateImageBehavior = false;
			this.listViewImages.View = System.Windows.Forms.View.Details;
			this.listViewImages.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewImages_ItemSelectionChanged);
			this.listViewImages.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewImages_KeyDown);
			// 
			// FormImageFiles
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.listViewImages);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "FormImageFiles";
			this.Text = "Images";
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.ListView listViewImages;
		private System.Windows.Forms.ColumnHeader imageFilesHeader;
	}
}