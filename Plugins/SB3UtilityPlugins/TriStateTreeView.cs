using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;

namespace SB3Utility
{
	public class TriStateTreeView : TreeView
	{
		public TriStateTreeView()
			: base()
		{
			Bitmap bmp = new Bitmap(16, 16);
			Graphics g = Graphics.FromImage(bmp);

			this.StateImageList = new ImageList();
			InitStateImage(CheckBoxState.UncheckedNormal);
			InitStateImage(CheckBoxState.CheckedNormal);
			InitStateImage(CheckBoxState.MixedNormal);
		}

		void InitStateImage(CheckBoxState state)
		{
			Bitmap bmp = new Bitmap(16, 16);
			Graphics g = Graphics.FromImage(bmp);
			CheckBoxRenderer.DrawCheckBox(g, new Point(0, 2), state);
			this.StateImageList.Images.Add(bmp);
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			CheckBoxes = false;
		}

		public void RemoveChild(TreeNode node)
		{
			if (node.TreeView != this)
			{
				throw new Exception("The node doesn't belong to this TreeView");
			}

			TreeNode parent = node.Parent;
			node.Remove();
			UpdateParent(parent);
		}

		public void AddChild(TreeNode node)
		{
			Nodes.Add(node);
			InitNode(node);
		}

		public void AddChild(TreeNode parent, TreeNode node)
		{
			if (parent.TreeView != this)
			{
				throw new Exception("The parent node doesn't belong to this TreeView");
			}

			parent.Nodes.Add(node);
			InitNode(node);
		}

		void InitNode(TreeNode node)
		{
			node.StateImageIndex = (node.Checked) ? (int)CheckState.Checked : (int)CheckState.Unchecked;
			if (node.Checked)
			{
				UpdateChildren(node, node.Checked, node.StateImageIndex);
			}
			else
			{
				if ((node.NextNode == null) && (node.Nodes.Count <= 0))
				{
					UpdateParent(node);
				}

				foreach (TreeNode child in node.Nodes)
				{
					InitNode(child);
				}
			}
		}

		void UpdateNode(TreeNode node)
		{
			node.StateImageIndex = (node.Checked) ? (int)CheckState.Checked : (int)CheckState.Unchecked;
			UpdateChildren(node, node.Checked, node.StateImageIndex);
			UpdateParent(node.Parent);
		}

		void UpdateParent(TreeNode node)
		{
			if (node == null)
			{
				return;
			}

			bool isChecked = false;
			bool isUnchecked = false;
			foreach (TreeNode child in node.Nodes)
			{
				if (child.Checked)
				{
					isChecked = true;
				}
				else
				{
					isUnchecked = true;
				}

				if (isChecked && isUnchecked)
				{
					node.Checked = false;
					node.StateImageIndex = (int)CheckState.Indeterminate;
					UpdateParentMixed(node.Parent);
					return;
				}
			}

			if (isChecked && !isUnchecked)
			{
				node.Checked = true;
				node.StateImageIndex = (int)CheckState.Checked;
			}
			else if (!isChecked && isUnchecked)
			{
				node.Checked = false;
				node.StateImageIndex = (int)CheckState.Unchecked;
			}

			UpdateParent(node.Parent);
		}

		void UpdateParentMixed(TreeNode node)
		{
			if (node == null)
			{
				return;
			}

			node.Checked = false;
			node.StateImageIndex = (int)CheckState.Indeterminate;
			UpdateParentMixed(node.Parent);
		}

		void UpdateChildren(TreeNode parent, bool isChecked, int stateIndex)
		{
			foreach (TreeNode node in parent.Nodes)
			{
				node.Checked = isChecked;
				node.StateImageIndex = stateIndex;
				UpdateChildren(node, isChecked, stateIndex);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if ((e.KeyCode == Keys.Space) && (SelectedNode != null))
			{
				SelectedNode.Checked = !SelectedNode.Checked;
				UpdateNode(SelectedNode);
			}
		}

		protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
		{
			base.OnNodeMouseClick(e);

			TreeViewHitTestInfo info = HitTest(e.X, e.Y);
			if ((info == null) || (info.Location != TreeViewHitTestLocations.StateImage))
			{
				return;
			}

			e.Node.Checked = !e.Node.Checked;
			UpdateNode(e.Node);
		}
	}
}
