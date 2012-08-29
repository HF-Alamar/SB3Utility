using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace SB3Utility
{
	public partial class FormPlugins : Form
	{
		public FormPlugins()
		{
			InitializeComponent();

			var plugins = new Dictionary<string, Tuple<string, List<FunctionBase>>>(PluginManager.Plugins);

			listViewFiles.BeginUpdate();
			DirectoryInfo pluginDir = new DirectoryInfo(Gui.Scripting.PluginDirectory);
			foreach (var file in pluginDir.GetFiles("*.dll"))
			{
				ListViewItem fileItem = new ListViewItem(file.Name);
				listViewFiles.Items.Add(fileItem);

				string fileNameLower = file.Name.ToLowerInvariant();
				if (plugins.ContainsKey(fileNameLower))
				{
					fileItem.Checked = !PluginManager.DoNotLoad.Contains(fileNameLower);

					List<FunctionBase> functions = plugins[fileNameLower].Item2;
					fileItem.Tag = CreateFunctionItems(functions);

					plugins.Remove(fileNameLower);
				}
				else
				{
					fileItem.Font = new Font(fileItem.Font, FontStyle.Italic);
				}
			}

			foreach (var pair in plugins)
			{
				string fileName = pair.Value.Item1;
				ListViewItem fileItem = new ListViewItem(fileName);
				fileItem.Checked = !PluginManager.DoNotLoad.Contains(fileName.ToLowerInvariant());
				fileItem.Tag = CreateFunctionItems(pair.Value.Item2);
				listViewFiles.Items.Add(fileItem);
			}

			listViewFiles.AutoResizeColumns();
			listViewFiles.EndUpdate();
			listViewFiles.ItemChecked += new ItemCheckedEventHandler(listViewFiles_ItemChecked);

			listViewFunctions.ListViewItemSorter = new FunctionSorter();
		}

		ListViewItem[] CreateFunctionItems(List<FunctionBase> functions)
		{
			ListViewItem[] functionItems = new ListViewItem[functions.Count];
			for (int i = 0; i < functionItems.Length; i++)
			{
				var function = functions[i];

				string type;
				if (function is FunctionClass)
				{
					type = "constructor";
				}
				else
				{
					type = (function.Method.IsStatic) ? "static" : "[" + function.Type.GenericName() + "]";
				}

				ListViewItem functionItem = new ListViewItem(new string[] { type, function.Return, function.Name, function.ParameterString });
				functionItems[i] = functionItem;

				if (function.Comments != null)
				{
					var comments = function.Comments;
					string s = function.Name + ": " + comments.Description + Environment.NewLine + Environment.NewLine;
					if (function is FunctionMethod)
					{
						if (!String.IsNullOrEmpty(function.Return))
						{
							s += "Returns " + function.Return;
							if (comments.Returns != null)
							{
								s += ": " + comments.Returns;
							}
							s += Environment.NewLine + Environment.NewLine;
						}
					}

					for (int j = 0; j < function.Parameters.Length; j++)
					{
						ParameterInfo parameter = function.Parameters[j];
						s += parameter.Name + Environment.NewLine + "Type: " + parameter.ParameterType.Name + Environment.NewLine;
						string parameterComment;
						if (comments.Parameters.TryGetValue(parameter.Name, out parameterComment))
						{
							s += parameterComment;
						}
						s += Environment.NewLine + Environment.NewLine;
					}

					functionItem.Tag = s.TrimEnd();
				}
			}
			return functionItems;
		}

		private void listViewFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			var functionItems = e.Item.Tag as ListViewItem[];

			listViewFunctions.BeginUpdate();
			if (e.IsSelected && (functionItems != null) && (functionItems.Length > 0))
			{
				listViewFunctions.Items.AddRange(functionItems);
				listViewFunctions.Sort();
			}
			else
			{
				listViewFunctions.Items.Clear();
			}
			listViewFunctions.AutoResizeColumns();
			listViewFunctions.EndUpdate();
		}

		private void listViewFunctions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (e.IsSelected)
			{
				if (e.Item.Tag == null)
				{
					richTextBox1.Text = String.Empty;
				}
				else
				{
					richTextBox1.Text = (string)e.Item.Tag;
				}
			}
			else
			{
				richTextBox1.Text = String.Empty;
			}
		}

		private void listViewFiles_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			string lower = e.Item.Text.ToLowerInvariant();
			if (e.Item.Checked)
			{
				while (PluginManager.DoNotLoad.Contains(lower))
				{
					PluginManager.DoNotLoad.Remove(lower);
				}
			}
			else
			{
				if (!PluginManager.DoNotLoad.Contains(lower))
				{
					PluginManager.DoNotLoad.Add(lower);
				}
			}
		}

		class FunctionSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				var strX1 = ((ListViewItem)x).SubItems[0].Text;
				var strY1 = ((ListViewItem)y).SubItems[0].Text;
				var strX2 = ((ListViewItem)x).SubItems[2].Text;
				var strY2 = ((ListViewItem)y).SubItems[2].Text;

				int compare1 = strX1.CompareTo(strY1);
				if (compare1 == 0)
				{
					return strX2.CompareTo(strY2);
				}
				else
				{
					return compare1;
				}
			}
		}
	}
}
