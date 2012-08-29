using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SB3Utility
{
	public partial class FormLog : DockContent
	{
		public FormLog()
		{
			InitializeComponent();

			richTextBox1.AllowDrop = true;
			richTextBox1.DragEnter += new DragEventHandler(richTextBox1_DragEnter);
			richTextBox1.DragDrop += new DragEventHandler(richTextBox1_DragDrop);
		}

		public void Logger(string s)
		{
			richTextBox1.SuspendLayout();
			richTextBox1.AppendText(s + Environment.NewLine);
			richTextBox1.SelectionStart = richTextBox1.Text.Length;
			richTextBox1.ScrollToCaret();
			richTextBox1.ResumeLayout();
		}

		void richTextBox1_DragEnter(object sender, DragEventArgs e)
		{
			Gui.Docking.DockDragEnter(sender, e);
		}

		void richTextBox1_DragDrop(object sender, DragEventArgs e)
		{
			Gui.Docking.DockDragDrop(sender, e);
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			richTextBox1.Text = String.Empty;
		}
	}
}
