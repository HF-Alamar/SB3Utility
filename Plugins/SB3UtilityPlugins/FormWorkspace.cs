using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;

namespace SB3Utility
{
	[Plugin]
	[PluginTool("Workspace")]
	public partial class FormWorkspace : DockContent
	{
		public FormWorkspace(string path, Fbx.Importer importer, string editorVar, ImportedEditor editor)
		{
			try
			{
				InitializeComponent();
				toolStripTextBoxTargetPosition.AfterEditTextChanged += toolStripTextBoxTargetPosition_AfterEditTextChanged;
				toolStripTextBoxMaterialName.AfterEditTextChanged += toolStripTextBoxMaterialName_AfterEditTextChanged;
				InitWorkspace(path, importer, editorVar, editor);

				Gui.Docking.ShowDockContent(this, Gui.Docking.DockFiles);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void InitWorkspace(string path, Fbx.Importer importer, string editorVar, ImportedEditor editor)
		{
			this.Text = Path.GetFileName(path);
			this.ToolTipText = path;

			if (editor.Frames != null)
			{
				TreeNode root = new TreeNode(typeof(ImportedFrame).Name);
				root.Checked = true;
				this.treeView.AddChild(root);

				for (int i = 0; i < importer.FrameList.Count; i++)
				{
					var frame = importer.FrameList[i];
					TreeNode node = new TreeNode(frame.Name);
					node.Checked = true;
					node.Tag = new DragSource(editorVar, typeof(ImportedFrame), editor.Frames.IndexOf(frame));
					this.treeView.AddChild(root, node);

					foreach (var child in frame)
					{
						BuildTree(editorVar, child, node, editor);
					}
				}
			}

			AddList(editor.Meshes, typeof(ImportedMesh).Name, editorVar);
			AddList(importer.MaterialList, typeof(ImportedMaterial).Name, editorVar);
			AddList(importer.TextureList, typeof(ImportedTexture).Name, editorVar);
			AddList(importer.MorphList, typeof(ImportedMorph).Name, editorVar);

			if ((importer.AnimationList != null) && (importer.AnimationList.Count > 0))
			{
				TreeNode root = new TreeNode(typeof(ImportedAnimation).Name);
				root.Checked = true;
				this.treeView.AddChild(root);

				for (int i = 0; i < importer.AnimationList.Count; i++)
				{
					TreeNode node = new TreeNode("Animation" + i);
					node.Checked = true;
					node.Tag = new DragSource(editorVar, typeof(ImportedAnimation), i);
					this.treeView.AddChild(root, node);
				}
			}

			foreach (TreeNode root in this.treeView.Nodes)
			{
				root.Expand();
			}
			if (this.treeView.Nodes.Count > 0)
			{
				this.treeView.Nodes[0].EnsureVisible();
			}

			this.treeView.AfterCheck += treeView_AfterCheck;
		}

		private void AddList<T>(List<T> list, string rootName, string editorVar)
		{
			if ((list != null) && (list.Count > 0))
			{
				TreeNode root = new TreeNode(rootName);
				root.Checked = true;
				this.treeView.AddChild(root);

				for (int i = 0; i < list.Count; i++)
				{
					dynamic item = list[i];
					TreeNode node = new TreeNode(item.Name);
					node.Checked = true;
					node.Tag = new DragSource(editorVar, typeof(T), i);
					this.treeView.AddChild(root, node);
					if (item is WorkspaceMesh)
					{
						WorkspaceMesh mesh = item;
						for (int j = 0; j < mesh.SubmeshList.Count; j++)
						{
							ImportedSubmesh submesh = mesh.SubmeshList[j];
							TreeNode submeshNode = new TreeNode();
							submeshNode.Checked = mesh.isSubmeshEnabled(submesh);
							submeshNode.Tag = submesh;
							submeshNode.ContextMenuStrip = this.contextMenuStripSubmesh;
							this.treeView.AddChild(node, submeshNode);
							UpdateSubmeshNode(submeshNode);
						}
					}
				}
			}
		}

		private void UpdateSubmeshNode(TreeNode node)
		{
			ImportedSubmesh submesh = (ImportedSubmesh)node.Tag;
			TreeNode meshNode = node.Parent;
			DragSource dragSrc = (DragSource)meshNode.Tag;
			var srcEditor = (ImportedEditor)Gui.Scripting.Variables[dragSrc.Variable];
			bool replaceSubmesh = srcEditor.Meshes[(int)dragSrc.Id].isSubmeshReplacingOriginals(submesh);
			node.Text = "Sub: V " + submesh.VertexList.Count + ", F " + submesh.FaceList.Count + ", Base: " + submesh.Index + ", Replace: " + replaceSubmesh + ", Mat: " + submesh.Material + ", World:" + submesh.WorldCoords;
		}

		private void BuildTree(string editorVar, ImportedFrame frame, TreeNode parent, ImportedEditor editor)
		{
			TreeNode node = new TreeNode(frame.Name);
			node.Checked = true;
			node.Tag = new DragSource(editorVar, typeof(ImportedFrame), editor.Frames.IndexOf(frame));
			this.treeView.AddChild(parent, node);

			foreach (var child in frame)
			{
				BuildTree(editorVar, child, node, editor);
			}
		}

		private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is ImportedSubmesh)
			{
				TreeNode submeshNode = e.Node;
				ImportedSubmesh submesh = (ImportedSubmesh)submeshNode.Tag;
				TreeNode meshNode = submeshNode.Parent;
				DragSource dragSrc = (DragSource)meshNode.Tag;
				var srcEditor = (ImportedEditor)Gui.Scripting.Variables[dragSrc.Variable];
				srcEditor.Meshes[(int)dragSrc.Id].setSubmeshEnabled(submesh, submeshNode.Checked);
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

		private void contextMenuStripSubmesh_Opening(object sender, CancelEventArgs e)
		{
			Point contextLoc = new Point(contextMenuStripSubmesh.Left, contextMenuStripSubmesh.Top);
			Point relativeLoc = treeView.PointToClient(contextLoc);
			TreeNode submeshNode = treeView.GetNodeAt(relativeLoc);
			ImportedSubmesh submesh = (ImportedSubmesh)submeshNode.Tag;
			toolStripTextBoxTargetPosition.Text = submesh.Index.ToString();
			TreeNode meshNode = submeshNode.Parent;
			DragSource dragSrc = (DragSource)meshNode.Tag;
			var srcEditor = (ImportedEditor)Gui.Scripting.Variables[dragSrc.Variable];
			bool replaceSubmesh = srcEditor.Meshes[(int)dragSrc.Id].isSubmeshReplacingOriginals(submesh);
			replaceToolStripMenuItem.Checked = replaceSubmesh;
			toolStripTextBoxMaterialName.Text = submesh.Material;
			worldCoordinatesToolStripMenuItem.Checked = submesh.WorldCoords;
		}

		private void toolStripTextBoxTargetPosition_AfterEditTextChanged(object sender, EventArgs e)
		{
			Point contextLoc = new Point(contextMenuStripSubmesh.Left, contextMenuStripSubmesh.Top);
			Point relativeLoc = treeView.PointToClient(contextLoc);
			TreeNode submeshNode = treeView.GetNodeAt(relativeLoc);
			ImportedSubmesh submesh = (ImportedSubmesh)submeshNode.Tag;
			int newIndex;
			if (Int32.TryParse(toolStripTextBoxTargetPosition.Text, out newIndex))
			{
				submesh.Index = newIndex;
				UpdateSubmeshNode(submeshNode);
			}
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Point contextLoc = new Point(contextMenuStripSubmesh.Left, contextMenuStripSubmesh.Top);
			Point relativeLoc = treeView.PointToClient(contextLoc);
			TreeNode submeshNode = treeView.GetNodeAt(relativeLoc);
			ImportedSubmesh submesh = (ImportedSubmesh)submeshNode.Tag;
			TreeNode meshNode = submeshNode.Parent;
			DragSource dragSrc = (DragSource)meshNode.Tag;
			var srcEditor = (ImportedEditor)Gui.Scripting.Variables[dragSrc.Variable];
			bool replaceSubmesh = srcEditor.Meshes[(int)dragSrc.Id].isSubmeshReplacingOriginals(submesh);
			replaceSubmesh ^= true;
			srcEditor.Meshes[(int)dragSrc.Id].setSubmeshReplacingOriginals(submesh, replaceSubmesh);
			replaceToolStripMenuItem.Checked = replaceSubmesh;
			UpdateSubmeshNode(submeshNode);
		}

		private void toolStripTextBoxMaterialName_AfterEditTextChanged(object sender, EventArgs e)
		{
			Point contextLoc = new Point(contextMenuStripSubmesh.Left, contextMenuStripSubmesh.Top);
			Point relativeLoc = treeView.PointToClient(contextLoc);
			TreeNode submeshNode = treeView.GetNodeAt(relativeLoc);
			ImportedSubmesh submesh = (ImportedSubmesh)submeshNode.Tag;
			submesh.Material = toolStripTextBoxMaterialName.Text;
			UpdateSubmeshNode(submeshNode);
		}

		private void worldCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Point contextLoc = new Point(contextMenuStripSubmesh.Left, contextMenuStripSubmesh.Top);
			Point relativeLoc = treeView.PointToClient(contextLoc);
			TreeNode submeshNode = treeView.GetNodeAt(relativeLoc);
			ImportedSubmesh submesh = (ImportedSubmesh)submeshNode.Tag;
			submesh.WorldCoords ^= true;
			worldCoordinatesToolStripMenuItem.Checked = submesh.WorldCoords;
			UpdateSubmeshNode(submeshNode);
		}
	}
}
