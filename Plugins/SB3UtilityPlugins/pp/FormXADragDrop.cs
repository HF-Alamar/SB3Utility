﻿using System;
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
		private xaEditor editor;

		public FormXADragDrop(xaEditor destEditor)
		{
			InitializeComponent();
			editor = destEditor;
		}
	}
}
