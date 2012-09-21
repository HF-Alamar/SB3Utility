using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SB3Utility
{
	[Plugin]
	[PluginOpensFile(".pp")]
	public partial class FormPP : DockContent
	{
		public string FormVariable { get; protected set; }
		public ppEditor Editor { get; protected set; }
		public string EditorVar { get; protected set; }
		public string ParserVar { get; protected set; }

		List<ListView> subfileListViews = new List<ListView>();

		Dictionary<string, string> ChildParserVars = new Dictionary<string, string>();
		Dictionary<string, DockContent> ChildForms = new Dictionary<string, DockContent>();

		public FormPP(string path, string variable)
		{
			try
			{
				InitializeComponent();

				FormVariable = variable;

				ParserVar = Gui.Scripting.GetNextVariable("ppParser");
				ppParser ppParser = (ppParser)Gui.Scripting.RunScript(ParserVar + " = OpenPP(path=\"" + path + "\")");

				EditorVar = Gui.Scripting.GetNextVariable("ppEditor");
				Editor = (ppEditor)Gui.Scripting.RunScript(EditorVar + " = ppEditor(parser=" + ParserVar + ")");

				Text = Path.GetFileName(ppParser.FilePath);
				ToolTipText = ppParser.FilePath;
				ShowHint = DockState.Document;

				saveFileDialog1.Filter = ".pp Files (*.pp)|*.pp|All Files (*.*)|*.*";

				subfileListViews.Add(xxSubfilesList);
				subfileListViews.Add(xaSubfilesList);
				subfileListViews.Add(imageSubfilesList);
				subfileListViews.Add(otherSubfilesList);

				InitSubfileLists();

				comboBoxFormat.Items.AddRange(ppFormat.Array);
				comboBoxFormat.SelectedIndex = (int)ppParser.Format.ppFormatIdx;
				comboBoxFormat.SelectedIndexChanged += new EventHandler(comboBoxFormat_SelectedIndexChanged);

				Gui.Docking.ShowDockContent(this, Gui.Docking.DockFiles);
				this.FormClosing += new FormClosingEventHandler(FormPP_FormClosing);

				List<DockContent> formPPList;
				if (Gui.Docking.DockContents.TryGetValue(typeof(FormPP), out formPPList))
				{
					var listCopy = new List<FormPP>(formPPList.Count);
					for (int i = 0; i < formPPList.Count; i++)
					{
						listCopy.Add((FormPP)formPPList[i]);
					}

					foreach (var form in listCopy)
					{
						if (form != this)
						{
							var formParser = (ppParser)Gui.Scripting.Variables[form.ParserVar];
							if (formParser.FilePath == path)
							{
								form.Close();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void FormPP_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				foreach (var pair in ChildForms)
				{
					if (pair.Value.IsHidden)
					{
						pair.Value.Show();
					}

					pair.Value.FormClosing -= new FormClosingEventHandler(ChildForms_FormClosing);
					pair.Value.Close();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void InitSubfileLists()
		{
			xxSubfilesList.Items.Clear();
			xaSubfilesList.Items.Clear();
			imageSubfilesList.Items.Clear();
			otherSubfilesList.Items.Clear();

			adjustSubfileListsEnabled(false);
			List<ListViewItem> xxFiles = new List<ListViewItem>(Editor.Parser.Subfiles.Count);
			List<ListViewItem> xaFiles = new List<ListViewItem>(Editor.Parser.Subfiles.Count);
			List<ListViewItem> imageFiles = new List<ListViewItem>(Editor.Parser.Subfiles.Count);
			List<ListViewItem> otherFiles = new List<ListViewItem>(Editor.Parser.Subfiles.Count);
			for (int i = 0; i < Editor.Parser.Subfiles.Count; i++)
			{
				IWriteFile subfile = Editor.Parser.Subfiles[i];
				ListViewItem item = new ListViewItem(subfile.Name);
				item.Tag = subfile;

				string ext = Path.GetExtension(subfile.Name).ToLower();
				if (ext.Equals(".xx"))
				{
					xxFiles.Add(item);
				}
				else if (ext.Equals(".xa"))
				{
					xaFiles.Add(item);
				}
				else if (ext.Equals(".ema") || Utility.ImageSupported(ext))
				{
					imageFiles.Add(item);
				}
				else
				{
					otherFiles.Add(item);
				}
			}
			xxSubfilesList.Items.AddRange(xxFiles.ToArray());
			xaSubfilesList.Items.AddRange(xaFiles.ToArray());
			imageSubfilesList.Items.AddRange(imageFiles.ToArray());
			otherSubfilesList.Items.AddRange(otherFiles.ToArray());
			adjustSubfileLists();
			adjustSubfileListsEnabled(true);
		}

		private void adjustSubfileListsEnabled(bool enabled)
		{
			if (enabled)
			{
				for (int i = 0; i < subfileListViews.Count; i++)
				{
					subfileListViews[i].EndUpdate();
				}
			}
			else
			{
				for (int i = 0; i < subfileListViews.Count; i++)
				{
					subfileListViews[i].BeginUpdate();
				}
			}
		}

		private void adjustSubfileLists()
		{
			for (int i = 0; i < subfileListViews.Count; i++)
			{
				subfileListViews[i].BeginUpdate();
				subfileListViews[i].Sort();
				subfileListViews[i].AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				subfileListViews[i].EndUpdate();

				TabPage tabPage = (TabPage)subfileListViews[i].Parent;
				int countIdx = tabPage.Text.IndexOf('[');
				if (countIdx > 0)
				{
					tabPage.Text = tabPage.Text.Substring(0, countIdx) + "[" + subfileListViews[i].Items.Count + "]";
				}
				else
				{
					tabPage.Text += " [" + subfileListViews[i].Items.Count + "]";
				}
			}
		}

		private void xxSubfilesList_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				OpenXXSubfilesList();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void xaSubfilesList_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				OpenXASubfilesList();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void xxSubfilesList_KeyPress(object sender, KeyPressEventArgs e)
		{
			try
			{
				if (e.KeyChar == '\r')
				{
					OpenXXSubfilesList();
					e.Handled = true;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void xaSubfilesList_KeyPress(object sender, KeyPressEventArgs e)
		{
			try
			{
				if (e.KeyChar == '\r')
				{
					OpenXASubfilesList();
					e.Handled = true;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		public List<FormXX> OpenXXSubfilesList()
		{
			List<FormXX> list = new List<FormXX>(xxSubfilesList.SelectedItems.Count);
			foreach (ListViewItem item in xxSubfilesList.SelectedItems)
			{
				IWriteFile writeFile = (IWriteFile)item.Tag;
				FormXX formXX = (FormXX)Gui.Scripting.RunScript(FormVariable + ".OpenXXSubfile(name=\"" + writeFile.Name + "\")", false);
				formXX.Activate();
				list.Add(formXX);
			}
			return list;
		}

		[Plugin]
		public FormXX OpenXXSubfile(string name)
		{
			DockContent child;
			if (!ChildForms.TryGetValue(name, out child))
			{
				string childParserVar;
				if (!ChildParserVars.TryGetValue(name, out childParserVar))
				{
					childParserVar = Gui.Scripting.GetNextVariable("xxParser");
					Gui.Scripting.RunScript(childParserVar + " = OpenXX(parser=" + ParserVar + ", name=\"" + name + "\")");
					Gui.Scripting.RunScript(EditorVar + ".ReplaceSubfile(file=" + childParserVar + ")");
					ChildParserVars.Add(name, childParserVar);

					foreach (ListViewItem item in xxSubfilesList.Items)
					{
						if (((IWriteFile)item.Tag).Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
						{
							item.Font = new Font(item.Font, FontStyle.Bold);
							xxSubfilesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
							break;
						}
					}
				}

				child = new FormXX(Editor.Parser, childParserVar);
				child.FormClosing += new FormClosingEventHandler(ChildForms_FormClosing);
				child.Tag = name;
				ChildForms.Add(name, child);
			}

			return child as FormXX;
		}

		public List<FormXA> OpenXASubfilesList()
		{
			List<FormXA> list = new List<FormXA>(xaSubfilesList.SelectedItems.Count);
			foreach (ListViewItem item in xaSubfilesList.SelectedItems)
			{
				IWriteFile writeFile = (IWriteFile)item.Tag;
				FormXA formXA = (FormXA)Gui.Scripting.RunScript(FormVariable + ".OpenXASubfile(name=\"" + writeFile.Name + "\")", false);
				formXA.Activate();
				list.Add(formXA);

				item.Font = new Font(item.Font, FontStyle.Bold);
				xaSubfilesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}
			return list;
		}

		[Plugin]
		public FormXA OpenXASubfile(string name)
		{
			DockContent child;
			if (!ChildForms.TryGetValue(name, out child))
			{
				string childParserVar;
				if (!ChildParserVars.TryGetValue(name, out childParserVar))
				{
					childParserVar = Gui.Scripting.GetNextVariable("xaParser");
					Gui.Scripting.RunScript(childParserVar + " = OpenXA(parser=" + ParserVar + ", name=\"" + name + "\")");
					Gui.Scripting.RunScript(EditorVar + ".ReplaceSubfile(file=" + childParserVar + ")");
					ChildParserVars.Add(name, childParserVar);

					foreach (ListViewItem item in xaSubfilesList.Items)
					{
						if (((IWriteFile)item.Tag).Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
						{
							item.Font = new Font(item.Font, FontStyle.Bold);
							xaSubfilesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
							break;
						}
					}
				}

				child = new FormXA(Editor.Parser, childParserVar);
				child.FormClosing += new FormClosingEventHandler(ChildForms_FormClosing);
				child.Tag = name;
				ChildForms.Add(name, child);
			}

			return child as FormXA;
		}

		private void ChildForms_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				DockContent form = (DockContent)sender;
				form.FormClosing -= new FormClosingEventHandler(ChildForms_FormClosing);
				ChildForms.Remove((string)form.Tag);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void comboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				Gui.Scripting.RunScript(EditorVar + ".SetFormat(" + (int)((ppFormat)comboBoxFormat.SelectedItem).ppFormatIdx + ")");
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void imageSubfilesList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			try
			{
				if (e.IsSelected)
				{
					IReadFile subfile = (IReadFile)e.Item.Tag;
					ImportedTexture image;
					string stream = EditorVar + ".ReadSubfile(name=\"" + subfile.Name + "\")";

					if (Path.GetExtension(subfile.Name).ToLowerInvariant() == ".ema")
					{
						image = (ImportedTexture)Gui.Scripting.RunScript(Gui.ImageControl.ImageScriptVariable + " = ImportEmaTexture(stream=" + stream + ", name=\"" + subfile.Name + "\")");
					}
					else
					{
						image = (ImportedTexture)Gui.Scripting.RunScript(Gui.ImageControl.ImageScriptVariable + " = ImportTexture(stream=" + stream + ", name=\"" + subfile.Name + "\")");
					}

					Gui.ImageControl.Image = image;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void saveppToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				BackgroundWorker worker = (BackgroundWorker)Gui.Scripting.RunScript(EditorVar + ".SavePP(keepBackup=" + keepBackupToolStripMenuItem.Checked + ", background=True)");
				ShowBlockingDialog(Editor.Parser.FilePath, worker);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void saveppAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					BackgroundWorker worker = (BackgroundWorker)Gui.Scripting.RunScript(EditorVar + ".SavePP(path=\"" + saveFileDialog1.FileName + "\", keepBackup=" + keepBackupToolStripMenuItem.Checked + ", background=True)");
					ShowBlockingDialog(saveFileDialog1.FileName, worker);
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void ShowBlockingDialog(string path, BackgroundWorker worker)
		{
			using (FormPPSave blockingForm = new FormPPSave(worker))
			{
				blockingForm.Text = "Saving " + Path.GetFileName(path) + "...";
				if (blockingForm.ShowDialog() == DialogResult.OK)
				{
					Report.ReportLog("Finished saving to " + saveFileDialog1.FileName);
				}
			}
		}

		private void reopenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				string opensFileVar = Gui.Scripting.GetNextVariable("opensPP");
				Gui.Scripting.RunScript(opensFileVar + " = FormPP(path=\"" + Editor.Parser.FilePath + "\", variable=\"" + opensFileVar + "\")", false);

				List<DockContent> formPPList;
				if (Gui.Docking.DockContents.TryGetValue(typeof(FormPP), out formPPList))
				{
					var listCopy = new List<FormPP>(formPPList.Count);
					for (int i = 0; i < formPPList.Count; i++)
					{
						listCopy.Add((FormPP)formPPList[i]);
					}

					foreach (var form in listCopy)
					{
						if (form.FormVariable == FormVariable)
						{
							form.Close();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					foreach (string path in openFileDialog1.FileNames)
					{
						Gui.Scripting.RunScript(EditorVar + ".AddSubfile(path=\"" + path + "\", replace=True)");
					}

					InitSubfileLists();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				bool removed = false;

				if (tabControlSubfiles.SelectedTab == tabPageXXSubfiles)
				{
					foreach (ListViewItem item in xxSubfilesList.SelectedItems)
					{
						IWriteFile writeFile = (IWriteFile)item.Tag;

						if (ChildParserVars.ContainsKey(writeFile.Name))
						{
							ChildParserVars.Remove(writeFile.Name);
						}

						if (ChildForms.ContainsKey(writeFile.Name))
						{
							ChildForms[writeFile.Name].Close();
						}

						Gui.Scripting.RunScript(EditorVar + ".RemoveSubfile(name=\"" + writeFile.Name + "\")");
						removed = true;
					}
				}
				else if (tabControlSubfiles.SelectedTab == tabPageXASubfiles)
				{
					foreach (ListViewItem item in xaSubfilesList.SelectedItems)
					{
						IWriteFile writeFile = (IWriteFile)item.Tag;

						if (ChildParserVars.ContainsKey(writeFile.Name))
						{
							ChildParserVars.Remove(writeFile.Name);
						}

						if (ChildForms.ContainsKey(writeFile.Name))
						{
							ChildForms[writeFile.Name].Close();
						}

						Gui.Scripting.RunScript(EditorVar + ".RemoveSubfile(name=\"" + writeFile.Name + "\")");
						removed = true;
					}
				}
				else if (tabControlSubfiles.SelectedTab == tabPageImageSubfiles)
				{
					foreach (ListViewItem item in imageSubfilesList.SelectedItems)
					{
						IWriteFile writeFile = (IWriteFile)item.Tag;
						Gui.Scripting.RunScript(EditorVar + ".RemoveSubfile(name=\"" + writeFile.Name + "\")");
						removed = true;
					}
				}
				else if (tabControlSubfiles.SelectedTab == tabPageOtherSubfiles)
				{
					foreach (ListViewItem item in otherSubfilesList.SelectedItems)
					{
						IWriteFile writeFile = (IWriteFile)item.Tag;
						Gui.Scripting.RunScript(EditorVar + ".RemoveSubfile(name=\"" + writeFile.Name + "\")");
						removed = true;
					}
				}

				if (removed)
				{
					InitSubfileLists();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void renameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				ListViewItem item = null;
				if (tabControlSubfiles.SelectedTab == tabPageXXSubfiles)
				{
					if (xxSubfilesList.SelectedItems.Count > 0)
					{
						item = xxSubfilesList.SelectedItems[0];
					}
				}
				else if (tabControlSubfiles.SelectedTab == tabPageXASubfiles)
				{
					if (xaSubfilesList.SelectedItems.Count > 0)
					{
						item = xaSubfilesList.SelectedItems[0];
					}
				}
				else if (tabControlSubfiles.SelectedTab == tabPageImageSubfiles)
				{
					if (imageSubfilesList.SelectedItems.Count > 0)
					{
						item = imageSubfilesList.SelectedItems[0];
					}
				}
				else if (tabControlSubfiles.SelectedTab == tabPageOtherSubfiles)
				{
					if (otherSubfilesList.SelectedItems.Count > 0)
					{
						item = otherSubfilesList.SelectedItems[0];
					}
				}

				if (item != null)
				{
					using (FormPPRename renameForm = new FormPPRename(item))
					{
						if (renameForm.ShowDialog() == DialogResult.OK)
						{
							IWriteFile subfile = (IWriteFile)item.Tag;
							string newName = (string)Gui.Scripting.RunScript(EditorVar + ".RenameSubfile(subfile=\"" + subfile.Name + "\", newName=\"" + renameForm.NewName + "\")");

							item.Text = newName;
							item.ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

							if (tabControlSubfiles.SelectedTab == tabPageXXSubfiles)
							{
								if (ChildParserVars.ContainsKey(subfile.Name))
								{
									string value = ChildParserVars[subfile.Name];
									ChildParserVars.Remove(subfile.Name);
									ChildParserVars.Add(newName, value);
								}

								if (ChildForms.ContainsKey(subfile.Name))
								{
									DockContent value = ChildForms[subfile.Name];
									ChildForms.Remove(subfile.Name);
									ChildForms.Add(newName, value);
									value.Text = newName;
									value.ToolTipText = Editor.Parser.FilePath + @"\" + newName;
								}
							}
							else if (tabControlSubfiles.SelectedTab == tabPageXASubfiles)
							{
								if (ChildParserVars.ContainsKey(subfile.Name))
								{
									string value = ChildParserVars[subfile.Name];
									ChildParserVars.Remove(subfile.Name);
									ChildParserVars.Add(newName, value);
								}

								if (ChildForms.ContainsKey(subfile.Name))
								{
									DockContent value = ChildForms[subfile.Name];
									ChildForms.Remove(subfile.Name);
									ChildForms.Add(newName, value);
									value.Text = newName;
									value.ToolTipText = Editor.Parser.FilePath + @"\" + newName;
								}
							}

							InitSubfileLists();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void exportPPToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(this.Editor.Parser.FilePath);
				folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
				if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				{
					Gui.Scripting.RunScript("ExportPP(parser=" + ParserVar + ", path=\"" + folderBrowserDialog1.SelectedPath + "\")");
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void exportSubfilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				{
					if (tabControlSubfiles.SelectedTab == tabPageXXSubfiles)
					{
						foreach (ListViewItem item in xxSubfilesList.SelectedItems)
						{
							IWriteFile subfile = (IWriteFile)item.Tag;
							Gui.Scripting.RunScript("ExportSubfile(parser=" + ParserVar + ", name=\"" + subfile.Name + "\", path=\"" + folderBrowserDialog1.SelectedPath + @"\" + subfile.Name + "\")");
						}
					}
					else if (tabControlSubfiles.SelectedTab == tabPageXASubfiles)
					{
						foreach (ListViewItem item in xaSubfilesList.SelectedItems)
						{
							IWriteFile subfile = (IWriteFile)item.Tag;
							Gui.Scripting.RunScript("ExportSubfile(parser=" + ParserVar + ", name=\"" + subfile.Name + "\", path=\"" + folderBrowserDialog1.SelectedPath + @"\" + subfile.Name + "\")");
						}
					}
					else if (tabControlSubfiles.SelectedTab == tabPageImageSubfiles)
					{
						foreach (ListViewItem item in imageSubfilesList.SelectedItems)
						{
							IWriteFile subfile = (IWriteFile)item.Tag;
							Gui.Scripting.RunScript("ExportSubfile(parser=" + ParserVar + ", name=\"" + subfile.Name + "\", path=\"" + folderBrowserDialog1.SelectedPath + @"\" + subfile.Name + "\")");
						}
					}
					else if (tabControlSubfiles.SelectedTab == tabPageOtherSubfiles)
					{
						foreach (ListViewItem item in otherSubfilesList.SelectedItems)
						{
							IWriteFile subfile = (IWriteFile)item.Tag;
							Gui.Scripting.RunScript("ExportSubfile(parser=" + ParserVar + ", name=\"" + subfile.Name + "\", path=\"" + folderBrowserDialog1.SelectedPath + @"\" + subfile.Name + "\")");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Close();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}
	}
}
