using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SlimDX;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		[PluginOpensFile(".fbx")]
		public static void WorkspaceFbx(string path, string variable)
		{
			string importVar = Gui.Scripting.GetNextVariable("importFbx");
			var importer = (Fbx.Importer)Gui.Scripting.RunScript(importVar + " = ImportFbx(\"" + path + "\")");

			string editorVar = Gui.Scripting.GetNextVariable("importedEditor");
			var editor = (ImportedEditor)Gui.Scripting.RunScript(editorVar + " = ImportedEditor(" + importVar + ")");

			var workspace = new FormWorkspace();
			workspace.Text = Path.GetFileName(path);
			workspace.ToolTipText = path;

			if (editor.Frames != null)
			{
				TreeNode root = new TreeNode(typeof(ImportedFrame).Name);
				root.Checked = true;
				workspace.TreeView.AddChild(root);

				for (int i = 0; i < importer.FrameList.Count; i++)
				{
					var frame = importer.FrameList[i];
					TreeNode node = new TreeNode(frame.Name);
					node.Checked = true;
					node.Tag = new DragSource(editorVar, typeof(ImportedFrame), editor.Frames.IndexOf(frame));
					workspace.TreeView.AddChild(root, node);

					foreach (var child in frame)
					{
						BuildTree(editorVar, child, node, editor, workspace);
					}
				}
			}

			AddList(importer.MeshList, typeof(ImportedMesh).Name, workspace, editorVar);
			AddList(importer.MaterialList, typeof(ImportedMaterial).Name, workspace, editorVar);
			AddList(importer.TextureList, typeof(ImportedTexture).Name, workspace, editorVar);
			AddList(importer.MorphList, typeof(ImportedMorph).Name, workspace, editorVar);

			if ((importer.AnimationList != null) && (importer.AnimationList.Count > 0))
			{
				TreeNode root = new TreeNode(typeof(ImportedAnimation).Name);
				root.Checked = true;
				workspace.TreeView.AddChild(root);

				for (int i = 0; i < importer.AnimationList.Count; i++)
				{
					TreeNode node = new TreeNode("Animation" + i);
					node.Checked = true;
					node.Tag = new DragSource(editorVar, typeof(ImportedAnimation), i);
					workspace.TreeView.AddChild(root, node);
				}
			}

			foreach (TreeNode root in workspace.TreeView.Nodes)
			{
				root.Expand();
			}
			if (workspace.TreeView.Nodes.Count > 0)
			{
				workspace.TreeView.Nodes[0].EnsureVisible();
			}
		}

		static void AddList<T>(List<T> list, string rootName, FormWorkspace workspace, string editorVar)
		{
			if ((list != null) && (list.Count > 0))
			{
				TreeNode root = new TreeNode(rootName);
				root.Checked = true;
				workspace.TreeView.AddChild(root);

				for (int i = 0; i < list.Count; i++)
				{
					dynamic item = list[i];
					TreeNode node = new TreeNode(item.Name);
					node.Checked = true;
					node.Tag = new DragSource(editorVar, typeof(T), i);
					workspace.TreeView.AddChild(root, node);
				}
			}
		}

		static void BuildTree(string editorVar, ImportedFrame frame, TreeNode parent, ImportedEditor editor, FormWorkspace workspace)
		{
			TreeNode node = new TreeNode(frame.Name);
			node.Checked = true;
			node.Tag = editor.Frames.IndexOf(frame);
			node.Tag = new DragSource(editorVar, typeof(ImportedFrame), editor.Frames.IndexOf(frame));
			workspace.TreeView.AddChild(parent, node);

			foreach (var child in frame)
			{
				BuildTree(editorVar, child, node, editor, workspace);
			}
		}

		[Plugin]
		public static void ExportFbx([DefaultVar]xxParser xxParser, object[] meshNames, object[] xaParsers, string path, string exportFormat, bool allFrames, bool skins)
		{
			List<xaParser> xaParserList = null;
			if (xaParsers != null)
			{
				xaParserList = new List<xaParser>(Utility.Convert<xaParser>(xaParsers));
			}

			List<xxFrame> meshFrames = xx.FindMeshFrames(xxParser.Frame, new List<string>(Utility.Convert<string>(meshNames)));
			Fbx.Exporter.Export(path, xxParser, meshFrames, xaParserList, exportFormat, allFrames, skins);
		}

		[Plugin]
		public static void ExportMorphFbx([DefaultVar]xxParser xxparser, string path, xxFrame meshFrame, xaParser xaparser, xaMorphClip morphClip, string exportFormat)
		{
			Fbx.Exporter.ExportMorph(path, xxparser, meshFrame, morphClip, xaparser, exportFormat);
		}

		[Plugin]
		public static Fbx.Importer ImportFbx([DefaultVar]string path)
		{
			return new Fbx.Importer(path);
		}
	}

	public static class FbxUtility
	{
		public static Vector3 QuaternionToEuler(Quaternion q)
		{
			return Fbx.QuaternionToEuler(q);
		}

		public static Quaternion EulerToQuaternion(Vector3 v)
		{
			return Fbx.EulerToQuaternion(v);
		}

		public static Matrix SRTToMatrix(Vector3 scale, Vector3 euler, Vector3 translate)
		{
			return Matrix.Scaling(scale) * Matrix.RotationQuaternion(EulerToQuaternion(euler)) * Matrix.Translation(translate);
		}

		public static Vector3[] MatrixToSRT(Matrix m)
		{
			Quaternion q;
			Vector3[] srt = new Vector3[3];
			m.Decompose(out srt[0], out q, out srt[2]);
			srt[1] = QuaternionToEuler(q);
			return srt;
		}
	}
}
