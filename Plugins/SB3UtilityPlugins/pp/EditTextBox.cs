using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SB3Utility
{
	public class EditTextBox : TextBox
	{
		private bool hasTextChanged = false;
		private string beforeEditText;
		private Font beforeEditFont;

		public event EventHandler AfterEditTextChanged;

		public EditTextBox()
			: base()
		{
			beforeEditText = this.Text;
			beforeEditFont = this.Font;
			this.TextChanged += new EventHandler(EditTextBox_TextChanged);
			this.GotFocus += new EventHandler(EditTextBox_GotFocus);
		}

/*		public EditTextBox(TextBox textBox)
		{
			base = textBox;
			beforeEditText = this.Text;
			beforeEditFont = this.Font;
			this.TextChanged += new EventHandler(EditTextBox_TextChanged);
			this.GotFocus += new EventHandler(EditTextBox_GotFocus);
		}*/

		private void EditTextBox_GotFocus(object sender, EventArgs e)
		{
			beforeEditText = this.Text;
		}

		protected virtual void OnAfterEditTextChanged(EventArgs e)
		{
			beforeEditText = this.Text;
			ResetState();

			EventHandler handler = AfterEditTextChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if (hasTextChanged)
			{
				OnAfterEditTextChanged(e);
			}

			base.OnLostFocus(e);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				if (hasTextChanged)
				{
					OnAfterEditTextChanged(e);
				}
			}
			else if (e.KeyChar == (char)Keys.Escape)
			{
				this.Text = beforeEditText;
				ResetState();
				this.Parent.Focus();
			}

			base.OnKeyPress(e);
		}

		private void EditTextBox_TextChanged(object sender, EventArgs e)
		{
			if (this.Focused)
			{
				hasTextChanged = true;
				this.Font = new Font(this.Font, FontStyle.Bold);
			}
		}

		private void ResetState()
		{
			hasTextChanged = false;
			this.Font = beforeEditFont;
		}
	}

	public class ToolStripEditTextBox : ToolStripTextBox
	{
		private bool hasTextChanged = false;
		private string beforeEditText;
		private Font beforeEditFont;

		public event EventHandler AfterEditTextChanged;

		public ToolStripEditTextBox()
			: base()
		{
			beforeEditText = this.Text;
			beforeEditFont = this.Font;
			this.TextChanged += new EventHandler(EditTextBox_TextChanged);
			this.GotFocus += new EventHandler(EditTextBox_GotFocus);
		}

		private void EditTextBox_GotFocus(object sender, EventArgs e)
		{
			beforeEditText = this.Text;
		}

		protected virtual void OnAfterEditTextChanged(EventArgs e)
		{
			beforeEditText = this.Text;
			ResetState();

			EventHandler handler = AfterEditTextChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if (hasTextChanged)
			{
				OnAfterEditTextChanged(e);
			}

			base.OnLostFocus(e);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				if (hasTextChanged)
				{
					OnAfterEditTextChanged(e);
				}
			}
			else if (e.KeyChar == (char)Keys.Escape)
			{
				this.Text = beforeEditText;
				ResetState();
				this.Parent.Focus();
			}

			base.OnKeyPress(e);
		}

		private void EditTextBox_TextChanged(object sender, EventArgs e)
		{
			if (this.Focused)
			{
				hasTextChanged = true;
				this.Font = new Font(this.Font, FontStyle.Bold);
			}
		}

		private void ResetState()
		{
			hasTextChanged = false;
			this.Font = beforeEditFont;
		}
	}
}
