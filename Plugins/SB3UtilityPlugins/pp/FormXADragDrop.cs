using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SB3Utility
{
	public partial class FormXADragDrop : Form
	{
		public ReplaceAnimationMethod ReplaceMethod { get; protected set; }

		private xaEditor editor;

		public FormXADragDrop(xaEditor destEditor, bool morphOrAnimation)
		{
			InitializeComponent();
			editor = destEditor;

			if (morphOrAnimation)
				panelMorphList.BringToFront();
			else
			{
				panelAnimation.BringToFront();
				comboBoxMethod.Items.AddRange(Enum.GetNames(typeof(ReplaceAnimationMethod)));
			}
		}
	}
}
