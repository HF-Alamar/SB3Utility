using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;

namespace SB3Utility
{
	public partial class FormScript : DockContent, IScripting
	{
		public string PluginDirectory { get; protected set; }
		public Dictionary<string, object> Variables { get { return ScriptMain.ScriptEnvironment.Variables; } }

		public ScriptMain ScriptMain = new ScriptMain();
		Dictionary<string, int> varDic = new Dictionary<string, int>();

		StreamWriter autosaveScriptWriter = null;

		public FormScript()
		{
			InitializeComponent();

			string fileDialogFilter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			openFileDialog1.Filter = fileDialogFilter;
			saveFileDialog1.Filter = fileDialogFilter;

			richTextBoxScript.AllowDrop = true;
			richTextBoxScript.DragEnter += new DragEventHandler(richTextBoxScript_DragEnter);
			richTextBoxScript.DragDrop += new DragEventHandler(richTextBoxScript_DragDrop);

			string[] pluginsDoNotLoad = Properties.Settings.Default.PluginsDoNotLoad.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < pluginsDoNotLoad.Length; i++)
			{
				string plugin = pluginsDoNotLoad[i].ToLowerInvariant();
				if (!PluginManager.DoNotLoad.Contains(plugin))
				{
					PluginManager.DoNotLoad.Add(plugin);
				}
			}

			PluginDirectory = (string)ScriptMain.ScriptEnvironment.Variables[ScriptExecutor.PluginDirectoryName];
			DirectoryInfo pluginDir = new DirectoryInfo(PluginDirectory);
			foreach (var plugin in pluginDir.GetFiles("*.dll"))
			{
				ScriptMain.LoadPlugin(plugin.FullName);
			}

			if (autosaveToolStripMenuItem.Checked)
			{
				InitAutosave();
			}

			this.FormClosing += new FormClosingEventHandler(FormScript_FormClosing);
		}

		void FormScript_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				CloseAutosave();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		public object RunScript(string command, bool show = true)
		{
			var mem = new MemoryStream(Encoding.UTF8.GetBytes(command));

			if (captureCommandsToolStripMenuItem.Checked)
			{
				richTextBoxScript.SuspendLayout();
				Color color = (show) ? Color.Empty : SystemColors.GrayText;
				AppendText(command + (show ? "" : " // GUI only") + Environment.NewLine, color);
				richTextBoxScript.SelectionStart = richTextBoxScript.Text.Length;
				richTextBoxScript.ScrollToCaret();
				richTextBoxScript.ResumeLayout();

				if (autosaveToolStripMenuItem.Checked)
				{
					autosaveScriptWriter.WriteLine(command);
				}
			}

			return ScriptMain.RunScript(mem, "Script");
		}

		void AppendText(string s, Color color)
		{
			int start = richTextBoxScript.TextLength;
			richTextBoxScript.AppendText(s);
			int end = richTextBoxScript.TextLength;

			richTextBoxScript.Select(start, end - start);
			richTextBoxScript.SelectionColor = color;
			richTextBoxScript.SelectionLength = 0;
		}

		public string GetNextVariable(string prefix)
		{
			string lower = prefix.ToLowerInvariant();

			int count;
			if (!varDic.TryGetValue(lower, out count))
			{
				count = 0;
				varDic.Add(lower, count);
			}

			while (Variables.ContainsKey(lower + count))
			{
				count++;
			}

			varDic[lower] = count + 1;
			return prefix + count;
		}

		void richTextBoxScript_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				Gui.Docking.DockDragEnter(sender, e);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void richTextBoxScript_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				Gui.Docking.DockDragDrop(sender, e);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void runToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				RunScript(richTextBoxScript.Text);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void runSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				RunScript(richTextBoxScript.SelectedText);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
				{
					string FileName = openFileDialog1.FileName;
					using (StreamReader reader = new StreamReader(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
					{
						richTextBoxScript.Rtf = null;
						richTextBoxScript.Text = reader.ReadToEnd();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
				{
					string FileName = saveFileDialog1.FileName;
					using (StreamWriter writer = File.CreateText(FileName))
					{
						writer.Write(richTextBoxScript.Text);
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				richTextBoxScript.Text = String.Empty;
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void autosaveToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (autosaveToolStripMenuItem.Checked)
				{
					InitAutosave();
				}
				else
				{
					CloseAutosave();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void InitAutosave()
		{
			string path = Assembly.GetExecutingAssembly().Location;
			path = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + ".autosavescript.txt";

			autosaveScriptWriter = new StreamWriter(File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
			autosaveScriptWriter.AutoFlush = true;
			autosaveScriptWriter.WriteLine("; start session");

			Report.ReportLog("Autosaving script to " + path);
		}

		void CloseAutosave()
		{
			if (autosaveScriptWriter != null)
			{
				autosaveScriptWriter.WriteLine("; end session");
				autosaveScriptWriter.WriteLine();
				autosaveScriptWriter.Close();
				autosaveScriptWriter = null;
			}
		}

		private void quickSaveSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			quickSavedToolStripMenuItem.ToolTipText = richTextBoxScript.SelectedText;
			quickSavedToolStripMenuItem.Visible = quickSavedToolStripMenuItem.ToolTipText.Length > 0;
		}

		private void runQuickSavedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (quickSavedToolStripMenuItem.ToolTipText != null && quickSavedToolStripMenuItem.ToolTipText.Length > 0)
					RunScript(quickSavedToolStripMenuItem.ToolTipText);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}
	}
}
