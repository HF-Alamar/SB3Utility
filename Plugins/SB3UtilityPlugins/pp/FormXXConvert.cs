using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SB3Utility
{
	public partial class FormXXConvert : Form
	{
		public int Format { get; protected set; }

		public FormXXConvert(int format)
		{
			InitializeComponent();

			Format = format;
			numericUpDown1.Value = format;
			numericUpDown1.ValueChanged += new EventHandler(numericUpDown1_ValueChanged);
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			Format = Decimal.ToInt32(numericUpDown1.Value);
		}
	}
}
