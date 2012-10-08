using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;

namespace SB3Utility
{
	public partial class MDIParent : Form, IDocking
	{
		FormRenderer dockRenderer;
		FormImage dockImage;
		FormLog dockLog;
		FormScript dockScript;

		Tuple<DockContent, ToolStripMenuItem>[] defaultDocks;
		bool viewToolStripMenuItemCheckedChangedSent = false;

		public string MainVar = "GUI";

		public event EventHandler<DockContentEventArgs> DockContentAdded;
		public event EventHandler<DockContentEventArgs> DockContentRemoved;

		public DockContent DockFiles { get; protected set; }
		public DockContent DockEditors { get; protected set; }
		public DockContent DockImage { get { return dockImage; } }
		public DockContent DockRenderer { get { return dockRenderer; } }
		public DockContent DockLog { get { return dockLog; } }
		public DockContent DockScript { get { return dockScript; } }
		public Dictionary<Type, List<DockContent>> DockContents { get; protected set; }

		public MDIParent()
		{
			try
			{
				InitializeComponent();
				this.Text += Gui.Version;

				openFileDialog1.Filter = "All Files (*.*)|*.*";

				DockFiles = new DockContent();
				DockEditors = new DockContent();
				DockContents = new Dictionary<Type, List<DockContent>>();

				dockLog = new FormLog();
				Report.Log += new Action<string>(dockLog.Logger);
				Report.ReportLog("Settings are saved at " + Assembly.GetExecutingAssembly().Location + ".config");

				dockScript = new FormScript();
				dockImage = new FormImage();
				dockRenderer = new FormRenderer();

				Gui.Scripting = dockScript;
				Gui.Docking = this;
				Gui.ImageControl = dockImage;
				Gui.Renderer = dockRenderer.Renderer;

				Report.Status += new Action<string>(MDIParent_Status);
				this.FormClosing += new FormClosingEventHandler(MDIParent_FormClosing);

				Gui.Scripting.Variables.Add(MainVar, this);
				PluginManager.RegisterFunctions(Assembly.GetExecutingAssembly());

				Gui.Config = Properties.Settings.Default;
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void MDIParent_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				string pluginsDoNotLoad = String.Empty;
				foreach (var plugin in PluginManager.DoNotLoad)
				{
					pluginsDoNotLoad += plugin + ";";
				}
				Properties.Settings.Default["PluginsDoNotLoad"] = pluginsDoNotLoad;

				Properties.Settings.Default.Save();

				Application.Exit();
			}
			catch
			{
			}
		}

		void MDIParent_Shown(object sender, EventArgs e)
		{
			try
			{
				SetDockDefault(DockFiles, "Files");
				DockFiles.Show(dockPanel, DockState.Document);

				SetDockDefault(DockEditors, "Editors");
				DockEditors.Show(DockFiles.Pane, DockAlignment.Right, 0.7);

				SetDockDefault(DockRenderer, "Renderer");
				DockRenderer.Show(dockPanel, DockState.DockRight);

				SetDockDefault(DockImage, "Image");
				DockImage.Show(dockPanel, DockState.DockRight);
				DockRenderer.Activate();

				SetDockDefault(DockLog, "Log");
				DockLog.Show(dockPanel, DockState.DockBottom);

				SetDockDefault(DockScript, "Script");
				DockScript.Show(DockLog.Pane, DockAlignment.Right, 0.5);

				defaultDocks = new Tuple<DockContent, ToolStripMenuItem>[] {
					new Tuple<DockContent, ToolStripMenuItem>(DockFiles, viewFilesToolStripMenuItem),
					new Tuple<DockContent, ToolStripMenuItem>(DockEditors, viewEditorsToolStripMenuItem),
					new Tuple<DockContent, ToolStripMenuItem>(DockImage, viewImageToolStripMenuItem),
					new Tuple<DockContent, ToolStripMenuItem>(DockRenderer, viewRendererToolStripMenuItem),
					new Tuple<DockContent, ToolStripMenuItem>(DockLog, viewLogToolStripMenuItem),
					new Tuple<DockContent, ToolStripMenuItem>(DockScript, viewScriptToolStripMenuItem) };

				viewFilesToolStripMenuItem.Checked = true;
				viewEditorsToolStripMenuItem.Checked = true;
				viewRendererToolStripMenuItem.Checked = true;
				viewImageToolStripMenuItem.Checked = true;
				viewLogToolStripMenuItem.Checked = true;
				viewScriptToolStripMenuItem.Checked = true;

				viewFilesToolStripMenuItem.CheckedChanged += new EventHandler(viewFilesToolStripMenuItem_CheckedChanged);
				viewEditorsToolStripMenuItem.CheckedChanged += new EventHandler(viewEditorsToolStripMenuItem_CheckedChanged);
				viewRendererToolStripMenuItem.CheckedChanged += new EventHandler(viewRendererToolStripMenuItem_CheckedChanged);
				viewImageToolStripMenuItem.CheckedChanged += new EventHandler(viewImageToolStripMenuItem_CheckedChanged);
				viewLogToolStripMenuItem.CheckedChanged += new EventHandler(viewLogToolStripMenuItem_CheckedChanged);
				viewScriptToolStripMenuItem.CheckedChanged += new EventHandler(viewScriptToolStripMenuItem_CheckedChanged);

				KeysConverter conv = new KeysConverter();
				foreach (var tool in PluginManager.Tools)
				{
					ToolStripMenuItem item = new ToolStripMenuItem(tool[1], null, new EventHandler(OpenTool));
					item.Tag = tool[0];
					item.ShortcutKeys = (Keys)conv.ConvertFromString(tool[2]);
					toolsToolStripMenuItem.DropDownItems.Add(item);
				}
#if DEBUG
				Test();
#endif
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void Test()
		{
			//OpenFile(@"C:\Program Files\illusion\SexyBeach3\data\sb3_0510\bo02_01_00_00_00\meshes0.fbx");

			/*var formPPVar = Gui.Scripting.GetNextVariable("opensPP");
			var formPP = (FormPP)Gui.Scripting.RunScript(formPPVar + " = FormPP(\"" + @"C:\Program Files\illusion\SexyBeach3\data\sb3_0510.pp" + "\", \"" + formPPVar + "\")", false);
			var formXX = (FormXX)Gui.Scripting.RunScript(formPPVar + ".OpenXXSubfile(name=\"bo02_01_00_00_00.xx\")", false);
			formXX.listViewMesh.Items[90].Selected = true;
			dockRenderer.Renderer.CenterView();*/
			//Gui.Scripting.RunScript(formPPVar + ".OpenXASubfile(name=\"dt02_01_00_00_00.xa\")", false);
			//Gui.Scripting.RunScript(formPPVar + ".OpenXASubfile(name=\"dt02_02_00_00_00.xa\")", false);

			/*var formPPVar = Gui.Scripting.GetNextVariable("opensPP");
			var formPP = (FormPP)Gui.Scripting.RunScript(formPPVar + " = FormPP(\"" + @"C:\illusion\AG3\data\js3_00_02_00.pp" + "\", \"" + formPPVar + "\")", false);
			var formXX = (FormXX)Gui.Scripting.RunScript(formPPVar + ".OpenXXSubfile(name=\"a15_01.xx\")", false);
			for (int i = 0; i < formXX.listViewMesh.Items.Count; i++)
			{
				formXX.listViewMesh.Items[i].Selected = true;
			}
			var formXA = (FormXA)Gui.Scripting.RunScript(formPPVar + ".OpenXASubfile(name=\"a15_01.xa\")", false);
			formXA.AnimationSetClip(1);*/
		}

		void SetDockDefault(DockContent defaultDock, string text)
		{
			defaultDock.Text = text;
			defaultDock.CloseButtonVisible = false;
			defaultDock.FormClosing += new FormClosingEventHandler(defaultDock_FormClosing);
		}

		void defaultDock_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (e.CloseReason == CloseReason.UserClosing)
				{
					e.Cancel = true;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void OpenTool(object sender, EventArgs e)
		{
			var item = (ToolStripMenuItem)sender;
			Gui.Scripting.RunScript((string)item.Tag + "()", false);
		}

		void MDIParent_Status(string s)
		{
			try
			{
				statusStrip.Text = s;
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		[Plugin]
		public List<object> OpenFile(string path)
		{
			List<object> results = new List<object>();
			try
			{
				OpenFile(path, results);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
			return results;
		}

		void OpenFile(string path, List<object> results)
		{
			for (int extIdx = 0; (extIdx = path.IndexOf('.', extIdx)) > 0; extIdx++)
			{
				string ext = path.Substring(extIdx).ToLowerInvariant();
				List<string> functions;
				if (PluginManager.OpensFile.TryGetValue(ext, out functions))
				{
					for (int i = 0; i < functions.Count; i++)
					{
						string opensFileVar = Gui.Scripting.GetNextVariable("opens" + ext.Replace(".", String.Empty).ToUpperInvariant());
						object result = Gui.Scripting.RunScript(opensFileVar + " = " + functions[i] + "(\"" + path + "\", \"" + opensFileVar + "\")", false);
						results.Add(result);
					}
				}
			}
		}

		[Plugin]
		public List<object> OpenDirectory(string path)
		{
			List<object> results = new List<object>();
			try
			{
				OpenDirectory(path, results);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
			return results;
		}

		void OpenDirectory(string path, List<object> results)
		{
			foreach (string filename in Directory.GetFiles(path))
			{
				OpenFile(filename, results);
			}

			foreach (string dir in Directory.GetDirectories(path))
			{
				OpenDirectory(dir, results);
			}
		}

		public void ShowDockContent(DockContent content, DockContent defaultDock)
		{
			try
			{
				content.FormClosing += new FormClosingEventHandler(content_FormClosing);

				List<DockContent> typeList;
				Type type = content.GetType();
				if (!DockContents.TryGetValue(type, out typeList))
				{
					typeList = new List<DockContent>();
					DockContents.Add(type, typeList);
				}
				typeList.Add(content);

				var handler = DockContentAdded;
				if (handler != null)
				{
					handler(this, new DockContentEventArgs(content));
				}

				if (defaultDock == null)
				{
					content.Show(this.dockPanel, DockState.Float);
				}
				else
				{
					content.Show(defaultDock.Pane, null);

					if (((defaultDock == DockFiles) || (defaultDock == DockEditors)) && !defaultDock.IsHidden)
					{
						defaultDock.Hide();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void content_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				DockContent dock = (DockContent)sender;
				dock.FormClosing -= new FormClosingEventHandler(content_FormClosing);

				List<DockContent> typeList = DockContents[dock.GetType()];
				typeList.Remove(dock);

				var handler = DockContentRemoved;
				if (handler != null)
				{
					handler(this, new DockContentEventArgs(dock));
				}

				if (dock.Pane.Contents.Count == 2)
				{
					if (viewFilesToolStripMenuItem.Checked && (dock.Pane == DockFiles.Pane))
					{
						DockFiles.Show();
					}
					else if (viewEditorsToolStripMenuItem.Checked && (dock.Pane == DockEditors.Pane))
					{
						DockEditors.Show();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void OpenFile(object sender, EventArgs e)
		{
			try
			{
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					foreach (var file in openFileDialog1.FileNames)
					{
						OpenFile(file);
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				this.Close();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		public void DockDragDrop(object sender, DragEventArgs e)
		{
			try
			{
				string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
				if (files != null)
				{
					foreach (string path in files)
					{
						if (File.Exists(path))
						{
							OpenFile(path);
						}
						else if (Directory.Exists(path))
						{
							OpenDirectory(path);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		public void DockDragEnter(object sender, DragEventArgs e)
		{
			try
			{
				if (e.Data.GetDataPresent(DataFormats.FileDrop))
				{
					e.Effect = e.AllowedEffect & DragDropEffects.Link;
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void settingsPluginsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				var formPlugins = new FormPlugins();
				formPlugins.ShowDialog();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				var about = new AboutBox();
				about.ShowDialog();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\SB3Utility.chm";
				if (File.Exists(path))
				{
					Help.ShowHelp(this, path);
				}
				else
				{
					Report.ReportLog("Couldn't find help file: " + path);
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void viewFilesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				SetDockPaneVisible(DockFiles, viewFilesToolStripMenuItem);

				if (viewFilesToolStripMenuItem.Checked && (DockFiles.Pane.Contents.Count > 1))
				{
					DockFiles.Hide();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void viewEditorsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				SetDockPaneVisible(DockEditors, viewEditorsToolStripMenuItem);

				if (viewEditorsToolStripMenuItem.Checked && (DockEditors.Pane.Contents.Count > 1))
				{
					DockEditors.Hide();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void viewImageToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				SetDockPaneVisible(DockImage, viewImageToolStripMenuItem);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void viewRendererToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				SetDockPaneVisible(DockRenderer, viewRendererToolStripMenuItem);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void viewLogToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				SetDockPaneVisible(DockLog, viewLogToolStripMenuItem);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void viewScriptToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				SetDockPaneVisible(DockScript, viewScriptToolStripMenuItem);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void SetDockPaneVisible(DockContent defaultDock, ToolStripMenuItem menuItem)
		{
			if (!viewToolStripMenuItemCheckedChangedSent)
			{
				viewToolStripMenuItemCheckedChangedSent = true;

				foreach (var tuple in defaultDocks)
				{
					if ((tuple.Item1.Pane == defaultDock.Pane) && (tuple.Item2.Checked != menuItem.Checked))
					{
						tuple.Item2.Checked = menuItem.Checked;
					}
				}

				if (menuItem.Checked)
				{
					foreach (DockContent content in defaultDock.Pane.Contents)
					{
						content.Show();
					}
				}
				else
				{
					foreach (DockContent content in defaultDock.Pane.Contents)
					{
						content.Hide();
					}
				}

				viewToolStripMenuItemCheckedChangedSent = false;
			}
		}
	}
}
