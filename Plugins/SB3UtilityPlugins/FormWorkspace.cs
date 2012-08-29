using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SB3Utility
{
	[Plugin]
	[PluginTool("Workspace")]
	public partial class FormWorkspace : DockContent
	{
		public TriStateTreeView TreeView { get { return treeView; } }

		public FormWorkspace()
		{
			try
			{
				InitializeComponent();

				Gui.Docking.ShowDockContent(this, Gui.Docking.DockFiles);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			try
			{
				if (e.Item is TreeNode)
				{
					treeView.DoDragDrop(e.Item, DragDropEffects.Copy);
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void treeView_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				UpdateDragStatus(sender, e);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void treeView_DragOver(object sender, DragEventArgs e)
		{
			try
			{
				UpdateDragStatus(sender, e);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void UpdateDragStatus(object sender, DragEventArgs e)
		{
			Point p = treeView.PointToClient(new Point(e.X, e.Y));
			TreeNode target = treeView.GetNodeAt(p);
			if ((target != null) && ((p.X < target.Bounds.Left) || (p.X > target.Bounds.Right) || (p.Y < target.Bounds.Top) || (p.Y > target.Bounds.Bottom)))
			{
				target = null;
			}
			treeView.SelectedNode = target;

			TreeNode node = (TreeNode)e.Data.GetData(typeof(TreeNode));
			if (node == null)
			{
				Gui.Docking.DockDragEnter(sender, e);
			}
			else
			{
				e.Effect = e.AllowedEffect & DragDropEffects.Copy;
			}
		}

		private void treeView_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				TreeNode node = (TreeNode)e.Data.GetData(typeof(TreeNode));
				if (node == null)
				{
					Gui.Docking.DockDragDrop(sender, e);
				}
				else
				{
					if (node.TreeView != treeView)
					{
						DragSource? source = node.Tag as DragSource?;
						if (source != null)
						{
							TreeNode clone = (TreeNode)node.Clone();
							clone.Checked = true;

							TreeNode type = null;
							foreach (TreeNode root in treeView.Nodes)
							{
								if (root.Text == source.Value.Type.Name)
								{
									type = root;
									break;
								}
							}

							if (type == null)
							{
								type = new TreeNode(source.Value.Type.Name);
								type.Checked = true;
								treeView.AddChild(type);
							}

							treeView.AddChild(type, clone);
						}

						foreach (TreeNode root in treeView.Nodes)
						{
							root.Expand();
						}
						if (treeView.Nodes.Count > 0)
						{
							treeView.Nodes[0].EnsureVisible();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (treeView.SelectedNode != null)
			{
				TreeNode parent = treeView.SelectedNode.Parent;
				if (parent == null)
				{
					treeView.RemoveChild(treeView.SelectedNode);
				}
				else if (parent.Parent == null)
				{
					if (parent.Nodes.Count <= 1)
					{
						treeView.RemoveChild(parent);
					}
					else
					{
						treeView.RemoveChild(treeView.SelectedNode);
					}
				}
			}
		}

		private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeView.BeginUpdate();
			treeView.ExpandAll();
			treeView.EndUpdate();
		}

		private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeView.BeginUpdate();
			treeView.CollapseAll();
			treeView.EndUpdate();
		}
	}
}
