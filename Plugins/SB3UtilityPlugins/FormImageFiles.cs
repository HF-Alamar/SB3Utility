using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SB3Utility
{
	public partial class FormImageFiles : DockContent
	{
		public static FormImageFiles Singleton = null;

		private struct PathVariable
		{
			public string Path;
			public string Variable;
		}

		Dictionary<string, ListViewItem> files = new Dictionary<string, ListViewItem>();
		List<string> freeVariables = new List<string>();

		private FormImageFiles()
		{
			try
			{
				InitializeComponent();

				this.FormClosing += new FormClosingEventHandler(FormImageFiles_FormClosing);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void FormImageFiles_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				Singleton = null;
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void AddImageFile(string path)
		{
			string variable;
			ListViewItem item;
			if (files.TryGetValue(path, out item))
			{
				PathVariable tag = (PathVariable)item.Tag;
				variable = tag.Variable;
			}
			else
			{
				item = new ListViewItem(Path.GetFileName(path));
				listViewImages.Items.Add(item);
				listViewImages.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

				files.Add(path, item);

				if (freeVariables.Count > 0)
				{
					variable = freeVariables[0];
					freeVariables.RemoveAt(0);
				}
				else
				{
					variable = Gui.Scripting.GetNextVariable("image");
				}
			}

			Gui.Scripting.RunScript(variable + " = ImportTexture(path=\"" + path + "\")");
			item.Tag = new PathVariable() { Path = path, Variable = variable };
		}

		private void listViewImages_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			try
			{
				if (e.IsSelected)
				{
					PathVariable tag = (PathVariable)e.Item.Tag;
					string variable = tag.Variable;
					Gui.ImageControl.Image = (ImportedTexture)Gui.Scripting.Variables[variable];
					Gui.Scripting.RunScript(Gui.ImageControl.ImageScriptVariable + " = " + variable);
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		[Plugin]
		[PluginOpensFile(".bmp")]
		[PluginOpensFile(".jpg")]
		[PluginOpensFile(".tga")]
		[PluginOpensFile(".png")]
		[PluginOpensFile(".dds")]
		[PluginOpensFile(".ppm")]
		[PluginOpensFile(".dib")]
		[PluginOpensFile(".hdr")]
		[PluginOpensFile(".pfm")]
		public static void OpenImageFile(string path, string variable)
		{
			try
			{
				if (Singleton == null)
				{
					Singleton = new FormImageFiles();
					Gui.Docking.ShowDockContent(Singleton, Gui.Docking.DockFiles);
				}

				Singleton.AddImageFile(path);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void reopenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ListViewItem lastSelected = null;
			foreach (ListViewItem item in listViewImages.SelectedItems)
			{
				PathVariable tag = (PathVariable)item.Tag;
				AddImageFile(tag.Path);
				lastSelected = item;
			}
			if (lastSelected != null)
			{
				lastSelected.Selected = false;
				lastSelected.Selected = true;
			}
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				while (listViewImages.SelectedItems.Count > 0)
				{
					ListViewItem item = listViewImages.SelectedItems[0];
					PathVariable tag = (PathVariable)item.Tag;
					files.Remove(tag.Path);
					freeVariables.Add(tag.Variable);
					item.Remove();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}
	}
}
