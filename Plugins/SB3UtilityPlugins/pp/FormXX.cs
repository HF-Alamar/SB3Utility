using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Windows;
using WeifenLuo.WinFormsUI.Docking;

namespace SB3Utility
{
	[Plugin]
	[PluginOpensFile(".xx")]
	public partial class FormXX : DockContent
	{
		private enum MeshExportFormat
		{
			[Description("Metasequoia")]
			Mqo,
			[Description("DirectX (SDK)")]
			DirectXSDK,
			[Description("Collada")]
			Collada,
			[Description("Collada (FBX 2012.2)")]
			ColladaFbx,
			[Description("FBX 2012.2")]
			Fbx,
			[Description("AutoCAD DXF")]
			Dxf,
			[Description("3D Studio 3DS")]
			_3ds,
			[Description("Alias OBJ")]
			Obj
		}

		private class KeyList<T>
		{
			public List<T> List { get; protected set; }
			public int Index { get; protected set; }

			public KeyList(List<T> list, int index)
			{
				List = list;
				Index = index;
			}
		}

		public xxEditor Editor { get; protected set; }
		public string EditorVar { get; protected set; }
		public string ParserVar { get; protected set; }

		string exportDir;
		EditTextBox[][] matMatrixText = new EditTextBox[5][];
		ComboBox[] matTexNameCombo;
		bool SetComboboxEvent = false;

		int loadedFrame = -1;
		int[] loadedBone = null;
		int[] highlightedBone = null;
		int loadedMesh = -1;
		int loadedMaterial = -1;
		int loadedTexture = -1;

		Matrix[] copyMatrices = new Matrix[10];

		Dictionary<int, List<KeyList<xxMaterial>>> crossRefMeshMaterials = new Dictionary<int, List<KeyList<xxMaterial>>>();
		Dictionary<int, List<KeyList<xxTexture>>> crossRefMeshTextures = new Dictionary<int, List<KeyList<xxTexture>>>();
		Dictionary<int, List<KeyList<xxFrame>>> crossRefMaterialMeshes = new Dictionary<int, List<KeyList<xxFrame>>>();
		Dictionary<int, List<KeyList<xxTexture>>> crossRefMaterialTextures = new Dictionary<int, List<KeyList<xxTexture>>>();
		Dictionary<int, List<KeyList<xxFrame>>> crossRefTextureMeshes = new Dictionary<int, List<KeyList<xxFrame>>>();
		Dictionary<int, List<KeyList<xxMaterial>>> crossRefTextureMaterials = new Dictionary<int, List<KeyList<xxMaterial>>>();
		Dictionary<int, int> crossRefMeshMaterialsCount = new Dictionary<int, int>();
		Dictionary<int, int> crossRefMeshTexturesCount = new Dictionary<int, int>();
		Dictionary<int, int> crossRefMaterialMeshesCount = new Dictionary<int, int>();
		Dictionary<int, int> crossRefMaterialTexturesCount = new Dictionary<int, int>();
		Dictionary<int, int> crossRefTextureMeshesCount = new Dictionary<int, int>();
		Dictionary<int, int> crossRefTextureMaterialsCount = new Dictionary<int, int>();

		List<RenderObjectXX> renderObjectMeshes;
		List<int> renderObjectIds;

		private bool listViewItemSyncSelectedSent = false;

		public FormXX(string path, string variable)
		{
			try
			{
				InitializeComponent();

				this.ShowHint = DockState.Document;
				this.Text = Path.GetFileName(path);
				this.ToolTipText = path;
				this.exportDir = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);

				ParserVar = Gui.Scripting.GetNextVariable("xxParser");
				string parserCommand = ParserVar + " = OpenXX(path=\"" + path + "\")";
				xxParser parser = (xxParser)Gui.Scripting.RunScript(parserCommand);

				EditorVar = Gui.Scripting.GetNextVariable("xxEditor");
				string editorCommand = EditorVar + " = xxEditor(parser=" + ParserVar + ")";
				Editor = (xxEditor)Gui.Scripting.RunScript(editorCommand);

				Init();
				LoadXX();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		public FormXX(ppParser ppParser, string xxParserVar)
		{
			try
			{
				InitializeComponent();

				xxParser parser = (xxParser)Gui.Scripting.Variables[xxParserVar];

				this.ShowHint = DockState.Document;
				this.Text = parser.Name;
				this.ToolTipText = ppParser.FilePath + @"\" + parser.Name;
				this.exportDir = Path.GetDirectoryName(ppParser.FilePath) + @"\" + Path.GetFileNameWithoutExtension(ppParser.FilePath) + @"\" + Path.GetFileNameWithoutExtension(parser.Name);

				ParserVar = xxParserVar;

				EditorVar = Gui.Scripting.GetNextVariable("xxEditor");
				Editor = (xxEditor)Gui.Scripting.RunScript(EditorVar + " = xxEditor(parser=" + ParserVar + ")");

				Init();
				LoadXX();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void CustomDispose()
		{
			try
			{
				DisposeRenderObjects();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void DisposeRenderObjects()
		{
			foreach (ListViewItem item in listViewMesh.SelectedItems)
			{
				Gui.Renderer.RemoveRenderObject(renderObjectIds[(int)item.Tag]);
			}

			for (int i = 0; i < renderObjectMeshes.Count; i++)
			{
				if (renderObjectMeshes[i] != null)
				{
					renderObjectMeshes[i].Dispose();
					renderObjectMeshes[i] = null;
				}
			}
		}

		void Init()
		{
			panelTexturePic.Resize += new EventHandler(panelTexturePic_Resize);
			splitContainer1.Panel2MinSize = tabControlViews.Width;

			matTexNameCombo = new ComboBox[4] { comboBoxMatTex1, comboBoxMatTex2, comboBoxMatTex3, comboBoxMatTex4 };

			matMatrixText[0] = new EditTextBox[4] { textBoxMatDiffuseR, textBoxMatDiffuseG, textBoxMatDiffuseB, textBoxMatDiffuseA };
			matMatrixText[1] = new EditTextBox[4] { textBoxMatAmbientR, textBoxMatAmbientG, textBoxMatAmbientB, textBoxMatAmbientA };
			matMatrixText[2] = new EditTextBox[4] { textBoxMatSpecularR, textBoxMatSpecularG, textBoxMatSpecularB, textBoxMatSpecularA };
			matMatrixText[3] = new EditTextBox[4] { textBoxMatEmissiveR, textBoxMatEmissiveG, textBoxMatEmissiveB, textBoxMatEmissiveA };
			matMatrixText[4] = new EditTextBox[1] { textBoxMatSpecularPower };

			InitDataGridViewSRT(dataGridViewFrameSRT);
			InitDataGridViewMatrix(dataGridViewFrameMatrix);
			InitDataGridViewSRT(dataGridViewBoneSRT);
			InitDataGridViewMatrix(dataGridViewBoneMatrix);

			textBoxFrameName.AfterEditTextChanged += new EventHandler(textBoxFrameName_AfterEditTextChanged);
			textBoxFrameName2.AfterEditTextChanged += new EventHandler(textBoxFrameName2_AfterEditTextChanged);
			textBoxBoneName.AfterEditTextChanged += new EventHandler(textBoxBoneName_AfterEditTextChanged);
			textBoxMatName.AfterEditTextChanged += new EventHandler(textBoxMatName_AfterEditTextChanged);
			textBoxTexName.AfterEditTextChanged += new EventHandler(textBoxTexName_AfterEditTextChanged);

			ColumnSubmeshMaterial.DisplayMember = "Item1";
			ColumnSubmeshMaterial.ValueMember = "Item2";
			ColumnSubmeshMaterial.DefaultCellStyle.NullValue = "(invalid)";

			for (int i = 0; i < matMatrixText.Length; i++)
			{
				for (int j = 0; j < matMatrixText[i].Length; j++)
				{
					matMatrixText[i][j].AfterEditTextChanged += new EventHandler(matMatrixText_AfterEditTextChanged);
				}
			}

			for (int i = 0; i < matTexNameCombo.Length; i++)
			{
				matTexNameCombo[i].Tag = i;
				matTexNameCombo[i].SelectedIndexChanged += new EventHandler(matTexNameCombo_SelectedIndexChanged);
			}

			MeshExportFormat[] values = Enum.GetValues(typeof(MeshExportFormat)) as MeshExportFormat[];
			string[] descriptions = new string[values.Length];
			for (int i = 0; i < descriptions.Length; i++)
			{
				descriptions[i] = values[i].GetDescription();
			}
			comboBoxMeshExportFormat.Items.AddRange(descriptions);
			comboBoxMeshExportFormat.SelectedIndex = 4;

			Gui.Docking.ShowDockContent(this, Gui.Docking.DockEditors);
		}

		// http://connect.microsoft.com/VisualStudio/feedback/details/151567/datagridviewcomboboxcell-needs-selectedindexchanged-event
		private void dataGridViewMesh_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			try
			{
				if (!SetComboboxEvent)
				{
					if (e.Control.GetType() == typeof(DataGridViewComboBoxEditingControl))
					{
						ComboBox comboBoxCell = (ComboBox)e.Control;
						if (comboBoxCell != null)
						{
							//Remove an existing event-handler, if present, to avoid
							//adding multiple handlers when the editing control is reused.
							comboBoxCell.SelectionChangeCommitted -= new EventHandler(comboBoxCell_SelectionChangeCommitted);

							//Add the event handler.
							comboBoxCell.SelectionChangeCommitted += new EventHandler(comboBoxCell_SelectionChangeCommitted);
							SetComboboxEvent = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void comboBoxCell_SelectionChangeCommitted(object sender, EventArgs e)
		{
			try
			{
				ComboBox combo = (ComboBox)sender;
				if (combo.SelectedValue == null)
				{
					return;
				}

				int comboValue = (int)combo.SelectedValue;
				if (comboValue != (int)dataGridViewMesh.CurrentCell.Value)
				{
					dataGridViewMesh.CommitEdit(DataGridViewDataErrorContexts.Commit);

					int rowIdx = dataGridViewMesh.CurrentCell.RowIndex;
					xxSubmesh submesh = Editor.Meshes[loadedMesh].Mesh.SubmeshList[rowIdx];
					object val = comboValue;
					int matIdx = (val == null) ? -1 : (int)val;

					if (submesh.MaterialIndex != matIdx)
					{
						Gui.Scripting.RunScript(EditorVar + ".SetSubmeshMaterial(meshId=" + loadedMesh + ", submeshId=" + rowIdx + ", material=" + matIdx + ")");

						RecreateRenderObjects();
						RecreateCrossRefs();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void textBoxTexName_AfterEditTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (loadedTexture < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".SetTextureName(id=" + loadedTexture + ", name=\"" + textBoxTexName.Text + "\")");

				xxTexture tex = Editor.Parser.TextureList[loadedTexture];
				RenameListViewItems(Editor.Parser.TextureList, listViewTexture, tex, tex.Name);
				RenameListViewItems(Editor.Parser.TextureList, listViewMeshTexture, tex, tex.Name);
				RenameListViewItems(Editor.Parser.TextureList, listViewMaterialTexture, tex, tex.Name);

				InitTextures();
				LoadMaterial(loadedMaterial);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void matTexNameCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (loadedMaterial < 0)
				{
					return;
				}

				ComboBox combo = (ComboBox)sender;
				int matTexIdx = (int)combo.Tag;
				string name = (combo.SelectedIndex == 0) ? String.Empty : (string)combo.Items[combo.SelectedIndex];

				Gui.Scripting.RunScript(EditorVar + ".SetMaterialTexture(id=" + loadedMaterial + ", index=" + matTexIdx + ", name=\"" + name + "\")");

				RecreateRenderObjects();
				RecreateCrossRefs();
				LoadMaterial(loadedMaterial);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void matMatrixText_AfterEditTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (loadedMaterial < 0)
				{
					return;
				}

				xxMaterial mat = Editor.Parser.MaterialList[loadedMaterial];
				Gui.Scripting.RunScript(EditorVar + ".SetMaterialPhong(id=" + loadedMaterial +
					", diffuse=" + MatMatrixColorScript(matMatrixText[0]) +
					", ambient=" + MatMatrixColorScript(matMatrixText[1]) +
					", specular=" + MatMatrixColorScript(matMatrixText[2]) +
					", emissive=" + MatMatrixColorScript(matMatrixText[3]) +
					", shininess=" + Single.Parse(matMatrixText[4][0].Text).ToFloatString() + ")");

				RecreateRenderObjects();
				RecreateCrossRefs();
				LoadMaterial(loadedMaterial);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		string MatMatrixColorScript(EditTextBox[] textBoxes)
		{
			return "{ " +
				Single.Parse(textBoxes[0].Text).ToFloatString() + ", " +
				Single.Parse(textBoxes[1].Text).ToFloatString() + ", " +
				Single.Parse(textBoxes[2].Text).ToFloatString() + ", " +
				Single.Parse(textBoxes[3].Text).ToFloatString() + " }";
		}

		void textBoxMatName_AfterEditTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (loadedMaterial < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".SetMaterialName(id=" + loadedMaterial + ", name=\"" + textBoxMatName.Text + "\")");

				xxMaterial mat = Editor.Parser.MaterialList[loadedMaterial];
				RenameListViewItems(Editor.Parser.MaterialList, listViewMaterial, mat, mat.Name);
				RenameListViewItems(Editor.Parser.MaterialList, listViewMeshMaterial, mat, mat.Name);
				RenameListViewItems(Editor.Parser.MaterialList, listViewTextureMaterial, mat, mat.Name);

				InitMaterials();
				LoadMaterial(loadedMaterial);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void InitDataGridViewSRT(DataGridViewEditor view)
		{
			DataTable tableSRT = new DataTable();
			tableSRT.Columns.Add(" ", typeof(string));
			tableSRT.Columns[0].ReadOnly = true;
			tableSRT.Columns.Add("X", typeof(float));
			tableSRT.Columns.Add("Y", typeof(float));
			tableSRT.Columns.Add("Z", typeof(float));
			tableSRT.Rows.Add(new object[] { "Translate", 0f, 0f, 0f });
			tableSRT.Rows.Add(new object[] { "Rotate", 0f, 0f, 0f });
			tableSRT.Rows.Add(new object[] { "Scale", 1f, 1f, 1f });
			view.Initialize(tableSRT, new DataGridViewEditor.ValidateCellDelegate(ValidateCellSRT), 3);
			view.Scroll += new ScrollEventHandler(dataGridViewEditor_Scroll);

			view.Columns[0].DefaultCellStyle = view.ColumnHeadersDefaultCellStyle;
			for (int i = 0; i < view.Columns.Count; i++)
			{
				view.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
			}
		}

		void InitDataGridViewMatrix(DataGridViewEditor view)
		{
			DataTable tableMatrix = new DataTable();
			tableMatrix.Columns.Add("1", typeof(float));
			tableMatrix.Columns.Add("2", typeof(float));
			tableMatrix.Columns.Add("3", typeof(float));
			tableMatrix.Columns.Add("4", typeof(float));
			tableMatrix.Rows.Add(new object[] { 1f, 0f, 0f, 0f });
			tableMatrix.Rows.Add(new object[] { 0f, 1f, 0f, 0f });
			tableMatrix.Rows.Add(new object[] { 0f, 0f, 1f, 0f });
			tableMatrix.Rows.Add(new object[] { 0f, 0f, 0f, 1f });
			view.Initialize(tableMatrix, new DataGridViewEditor.ValidateCellDelegate(ValidateCellSingle), 4);
			view.Scroll += new ScrollEventHandler(dataGridViewEditor_Scroll);

			for (int i = 0; i < view.Columns.Count; i++)
			{
				view.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
			}
		}

		void dataGridViewEditor_Scroll(object sender, ScrollEventArgs e)
		{
			try
			{
				e.NewValue = e.OldValue;
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		bool ValidateCellSRT(string s, int row, int col)
		{
			if (col == 0)
			{
				return true;
			}
			else
			{
				return ValidateCellSingle(s, row, col);
			}
		}

		bool ValidateCellSingle(string s, int row, int col)
		{
			float f;
			if (Single.TryParse(s, out f))
			{
				return true;
			}
			return false;
		}

		void RecreateRenderObjects()
		{
			DisposeRenderObjects();

			renderObjectMeshes = new List<RenderObjectXX>(new RenderObjectXX[Editor.Meshes.Count]);
			renderObjectIds = new List<int>(new int[Editor.Meshes.Count]);

			foreach (ListViewItem item in listViewMesh.SelectedItems)
			{
				int id = (int)item.Tag;
				xxFrame meshFrame = Editor.Meshes[id];
				HashSet<string> meshNames = new HashSet<string>() { meshFrame.Name };
				renderObjectMeshes[id] = new RenderObjectXX(Editor.Parser, meshNames);

				RenderObjectXX renderObj = renderObjectMeshes[id];
				renderObjectIds[id] = Gui.Renderer.AddRenderObject(renderObj);
			}

			HighlightSubmeshes();
			if (highlightedBone != null)
				HighlightBone(highlightedBone, true);
		}

		void textBoxFrameName_AfterEditTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (loadedFrame < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".SetFrameName(id=" + loadedFrame + ", name=\"" + textBoxFrameName.Text + "\")");

				RecreateRenderObjects();

				xxFrame frame = Editor.Frames[loadedFrame];
				TreeNode node = FindFrameNode(frame, treeViewObjectTree.Nodes);
				node.Text = frame.Name;

				RenameListViewItems(Editor.Meshes, listViewMesh, frame, frame.Name);
				RenameListViewItems(Editor.Meshes, listViewMaterialMesh, frame, frame.Name);
				RenameListViewItems(Editor.Meshes, listViewTextureMesh, frame, frame.Name);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void RenameListViewItems<T>(List<T> list, ListView listView, T obj, string name)
		{
			foreach (ListViewItem item in listView.Items)
			{
				if (list[(int)item.Tag].Equals(obj))
				{
					item.Text = name;
					break;
				}
			}
		}

		void textBoxFrameName2_AfterEditTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (loadedFrame < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".SetFrameName2(id=" + loadedFrame + ", name=\"" + textBoxFrameName2.Text + "\")");
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void LoadXX()
		{
			renderObjectMeshes = new List<RenderObjectXX>(new RenderObjectXX[Editor.Meshes.Count]);
			renderObjectIds = new List<int>(new int[Editor.Meshes.Count]);

			InitFormat();

			InitFrames();
			InitMeshes();
			InitMaterials();
			InitTextures();

			RecreateCrossRefs();
		}

		void InitFormat()
		{
			textBoxFormat.Text = Editor.Parser.Format.ToString();

			if (Editor.Parser.Format >= 6)
			{
				textBoxFrameName2.ReadOnly = false;
				textBoxFrameName2.BackColor = SystemColors.Window;
			}
			else
			{
				textBoxFrameName2.ReadOnly = true;
				textBoxFrameName2.BackColor = SystemColors.Control;
			}
		}

		void InitFrames()
		{
			TreeNode objRootNode = CreateFrameTree(Editor.Parser.Frame, null);

			if (treeViewObjectTree.Nodes.Count > 0)
			{
				treeViewObjectTree.Nodes.RemoveAt(0);
			}
			treeViewObjectTree.Nodes.Insert(0, objRootNode);
		}

		private TreeNode CreateFrameTree(xxFrame frame, TreeNode parentNode)
		{
			TreeNode newNode = new TreeNode(frame.Name);
			newNode.Tag = new DragSource(EditorVar, typeof(xxFrame), Editor.Frames.IndexOf(frame));

			if (frame.Mesh != null)
			{
				int meshId = Editor.Meshes.IndexOf(frame);
				TreeNode meshNode = new TreeNode("Mesh");
				meshNode.Tag = new DragSource(EditorVar, typeof(xxMesh), meshId);
				newNode.Nodes.Add(meshNode);

				if (frame.Mesh.BoneList.Count > 0)
				{
					TreeNode boneListNode = new TreeNode("Bones");
					meshNode.Nodes.Add(boneListNode);
					for (int i = 0; i < frame.Mesh.BoneList.Count; i++)
					{
						xxBone bone = frame.Mesh.BoneList[i];
						TreeNode boneNode = new TreeNode(bone.Name);
						boneNode.Tag = new DragSource(EditorVar, typeof(xxBone), new int[] { meshId, i });
						boneListNode.Nodes.Add(boneNode);
					}
				}
			}

			if (parentNode != null)
			{
				parentNode.Nodes.Add(newNode);
			}
			for (int i = 0; i < frame.Count; i++)
			{
				CreateFrameTree(frame[i], newNode);
			}

			return newNode;
		}

		void InitMeshes()
		{
			ListViewItem[] meshItems = new ListViewItem[Editor.Meshes.Count];
			for (int i = 0; i < Editor.Meshes.Count; i++)
			{
				xxFrame frame = Editor.Meshes[i];
				meshItems[i] = new ListViewItem(frame.Name);
				meshItems[i].Tag = i;
			}
			listViewMesh.Items.Clear();
			listViewMesh.Items.AddRange(meshItems);
			meshlistHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		void InitMaterials()
		{
			List<Tuple<string, int>> columnMaterials = new List<Tuple<string, int>>(Editor.Parser.MaterialList.Count);
			ListViewItem[] materialItems = new ListViewItem[Editor.Parser.MaterialList.Count];
			for (int i = 0; i < Editor.Parser.MaterialList.Count; i++)
			{
				xxMaterial mat = Editor.Parser.MaterialList[i];
				materialItems[i] = new ListViewItem(mat.Name);
				materialItems[i].Tag = i;

				columnMaterials.Add(new Tuple<string, int>(mat.Name, i));
			}
			listViewMaterial.Items.Clear();
			listViewMaterial.Items.AddRange(materialItems);
			materiallistHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

			ColumnSubmeshMaterial.DataSource = columnMaterials;
			SetComboboxEvent = false;

			TreeNode materialsNode = new TreeNode("Materials");
			for (int i = 0; i < Editor.Parser.MaterialList.Count; i++)
			{
				TreeNode matNode = new TreeNode(Editor.Parser.MaterialList[i].Name);
				matNode.Tag = new DragSource(EditorVar, typeof(xxMaterial), i);
				materialsNode.Nodes.Add(matNode);
			}

			if (treeViewObjectTree.Nodes.Count > 1)
			{
				treeViewObjectTree.Nodes.RemoveAt(1);
			}
			treeViewObjectTree.Nodes.Insert(1, materialsNode);
		}

		void InitTextures()
		{
			for (int i = 0; i < matTexNameCombo.Length; i++)
			{
				matTexNameCombo[i].Items.Clear();
				matTexNameCombo[i].Items.Add("(none)");
			}

			ListViewItem[] textureItems = new ListViewItem[Editor.Parser.TextureList.Count];
			for (int i = 0; i < Editor.Parser.TextureList.Count; i++)
			{
				xxTexture tex = Editor.Parser.TextureList[i];
				textureItems[i] = new ListViewItem(tex.Name);
				textureItems[i].Tag = i;
				for (int j = 0; j < matTexNameCombo.Length; j++)
				{
					matTexNameCombo[j].Items.Add(tex.Name);
				}
			}
			listViewTexture.Items.Clear();
			listViewTexture.Items.AddRange(textureItems);
			texturelistHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

			TreeNode texturesNode = new TreeNode("Textures");
			for (int i = 0; i < Editor.Parser.TextureList.Count; i++)
			{
				TreeNode texNode = new TreeNode(Editor.Parser.TextureList[i].Name);
				texNode.Tag = new DragSource(EditorVar, typeof(xxTexture), i);
				texturesNode.Nodes.Add(texNode);
			}

			if (treeViewObjectTree.Nodes.Count > 2)
			{
				treeViewObjectTree.Nodes.RemoveAt(2);
			}
			treeViewObjectTree.Nodes.Insert(2, texturesNode);
		}

		void LoadFrame(int id)
		{
			if (id < 0)
			{
				textBoxFrameName.Text = String.Empty;
				textBoxFrameName2.Text = String.Empty;
				LoadMatrix(Matrix.Identity, dataGridViewFrameSRT, dataGridViewFrameMatrix);
			}
			else
			{
				xxFrame frame = Editor.Frames[id];
				textBoxFrameName.Text = frame.Name;

				if (Editor.Parser.Format >= 6)
				{
					textBoxFrameName2.Text = frame.Name2;
				}

				LoadMatrix(frame.Matrix, dataGridViewFrameSRT, dataGridViewFrameMatrix);
			}
			loadedFrame = id;
		}

		void LoadMatrix(Matrix matrix, DataGridView viewSRT, DataGridView viewMatrix)
		{
			Vector3[] srt = FbxUtility.MatrixToSRT(matrix);
			DataTable tableSRT = (DataTable)viewSRT.DataSource;
			for (int i = 0; i < 3; i++)
			{
				tableSRT.Rows[0][i + 1] = srt[2][i];
				tableSRT.Rows[1][i + 1] = srt[1][i];
				tableSRT.Rows[2][i + 1] = srt[0][i];
			}

			DataTable tableMatrix = (DataTable)viewMatrix.DataSource;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					tableMatrix.Rows[i][j] = matrix[i, j];
				}
			}
		}

		void LoadBone(int[] id)
		{
			if (id == null)
			{
				textBoxBoneName.Text = String.Empty;
				LoadMatrix(Matrix.Identity, dataGridViewBoneSRT, dataGridViewBoneMatrix);
			}
			else
			{
				xxBone bone = Editor.Meshes[id[0]].Mesh.BoneList[id[1]];
				textBoxBoneName.Text = bone.Name;
				LoadMatrix(bone.Matrix, dataGridViewBoneSRT, dataGridViewBoneMatrix);
			}
			loadedBone = id;
		}

		void LoadMesh(int id)
		{
			dataGridViewMesh.Rows.Clear();
			if (id < 0)
			{
				textBoxMeshName.Text = String.Empty;
				checkBoxMeshSkinned.Checked = false;
			}
			else
			{
				xxFrame frame = Editor.Meshes[id];
				for (int i = 0; i < frame.Mesh.SubmeshList.Count; i++)
				{
					xxSubmesh submesh = frame.Mesh.SubmeshList[i];
					int matIdx = submesh.MaterialIndex;
					if ((matIdx >= 0) && (matIdx < Editor.Parser.MaterialList.Count))
					{
						dataGridViewMesh.Rows.Add(new object[] { submesh.VertexList.Count, submesh.FaceList.Count, matIdx });
					}
					else
					{
						dataGridViewMesh.Rows.Add(new object[] { submesh.VertexList.Count, submesh.FaceList.Count, null });
					}
				}
				dataGridViewMesh.ClearSelection();

				textBoxMeshName.Text = frame.Name;
				checkBoxMeshSkinned.Checked = xx.IsSkinned(frame.Mesh);
			}
			loadedMesh = id;
		}

		void LoadMaterial(int id)
		{
			if (loadedMaterial >= 0)
			{
				loadedMaterial = -1;
			}

			if (id < 0)
			{
				textBoxMatName.Text = String.Empty;
				for (int i = 0; i < 4; i++)
				{
					matTexNameCombo[i].SelectedIndex = -1;
					for (int j = 0; j < 4; j++)
					{
						matMatrixText[i][j].Text = String.Empty;
					}
				}
				matMatrixText[4][0].Text = String.Empty;
			}
			else
			{
				xxMaterial mat = Editor.Parser.MaterialList[id];
				textBoxMatName.Text = mat.Name;
				for (int i = 0; i < mat.Textures.Length; i++)
				{
					xxMaterialTexture matTex = mat.Textures[i];
					string matTexName = matTex.Name;
					if (matTexName == String.Empty)
					{
						matTexNameCombo[i].SelectedIndex = 0;
					}
					else
					{
						matTexNameCombo[i].SelectedIndex = matTexNameCombo[i].FindStringExact(matTexName);
					}
				}

				Color4[] colors = new Color4[] { mat.Diffuse, mat.Ambient, mat.Specular, mat.Emissive };
				for (int i = 0; i < colors.Length; i++)
				{
					matMatrixText[i][0].Text = colors[i].Red.ToFloatString();
					matMatrixText[i][1].Text = colors[i].Green.ToFloatString();
					matMatrixText[i][2].Text = colors[i].Blue.ToFloatString();
					matMatrixText[i][3].Text = colors[i].Alpha.ToFloatString();
				}
				matMatrixText[4][0].Text = mat.Power.ToFloatString();
			}
			loadedMaterial = id;
		}

		void LoadTexture(int id)
		{
			if (id < 0)
			{
				textBoxTexName.Text = String.Empty;
				textBoxTexSize.Text = String.Empty;
				pictureBoxTexture.Image = null;
			}
			else
			{
				xxTexture tex = Editor.Parser.TextureList[id];
				textBoxTexName.Text = tex.Name;
				textBoxTexSize.Text = tex.Width + "x" + tex.Height;

				ImportedTexture importedTex = xx.ImportedTexture(tex);
				Texture renderTexture = Texture.FromMemory(Gui.Renderer.Device, importedTex.Data);
				Bitmap bitmap = new Bitmap(Texture.ToStream(renderTexture, ImageFileFormat.Bmp));
				renderTexture.Dispose();
				pictureBoxTexture.Image = bitmap;

				ResizeImage();
			}
			loadedTexture = id;
		}

		void panelTexturePic_Resize(object sender, EventArgs e)
		{
			try
			{
				ResizeImage();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void ResizeImage()
		{
			if (pictureBoxTexture.Image != null)
			{
				Decimal x = (Decimal)panelTexturePic.Width / pictureBoxTexture.Image.Width;
				Decimal y = (Decimal)panelTexturePic.Height / pictureBoxTexture.Image.Height;
				if (x > y)
				{
					pictureBoxTexture.Width = Decimal.ToInt32(pictureBoxTexture.Image.Width * y);
					pictureBoxTexture.Height = Decimal.ToInt32(pictureBoxTexture.Image.Height * y);
				}
				else
				{
					pictureBoxTexture.Width = Decimal.ToInt32(pictureBoxTexture.Image.Width * x);
					pictureBoxTexture.Height = Decimal.ToInt32(pictureBoxTexture.Image.Height * x);
				}
			}
		}

		private void RecreateCrossRefs()
		{
			CrossRefsClear();

			crossRefMeshMaterials.Clear();
			crossRefMeshTextures.Clear();
			crossRefMaterialMeshes.Clear();
			crossRefMaterialTextures.Clear();
			crossRefTextureMeshes.Clear();
			crossRefTextureMaterials.Clear();
			crossRefMeshMaterialsCount.Clear();
			crossRefMeshTexturesCount.Clear();
			crossRefMaterialMeshesCount.Clear();
			crossRefMaterialTexturesCount.Clear();
			crossRefTextureMeshesCount.Clear();
			crossRefTextureMaterialsCount.Clear();

			var meshes = Editor.Meshes;
			var materials = Editor.Parser.MaterialList;
			var textures = Editor.Parser.TextureList;

			for (int i = 0; i < meshes.Count; i++)
			{
				crossRefMeshMaterials.Add(i, new List<KeyList<xxMaterial>>(materials.Count));
				crossRefMeshTextures.Add(i, new List<KeyList<xxTexture>>(textures.Count));
				crossRefMaterialMeshesCount.Add(i, 0);
				crossRefTextureMeshesCount.Add(i, 0);
			}

			for (int i = 0; i < materials.Count; i++)
			{
				crossRefMaterialMeshes.Add(i, new List<KeyList<xxFrame>>(meshes.Count));
				crossRefMaterialTextures.Add(i, new List<KeyList<xxTexture>>(textures.Count));
				crossRefMeshMaterialsCount.Add(i, 0);
				crossRefTextureMaterialsCount.Add(i, 0);
			}

			for (int i = 0; i < textures.Count; i++)
			{
				crossRefTextureMeshes.Add(i, new List<KeyList<xxFrame>>(meshes.Count));
				crossRefTextureMaterials.Add(i, new List<KeyList<xxMaterial>>(materials.Count));
				crossRefMeshTexturesCount.Add(i, 0);
				crossRefMaterialTexturesCount.Add(i, 0);
			}

			for (int i = 0; i < materials.Count; i++)
			{
				xxMaterial mat = materials[i];
				for (int j = 0; j < mat.Textures.Length; j++)
				{
					xxMaterialTexture matTex = mat.Textures[j];
					string matTexName = matTex.Name;
					if (matTex.Name != String.Empty)
					{
						bool foundMatTex = false;
						for (int m = 0; m < textures.Count; m++)
						{
							xxTexture tex = textures[m];
							if (matTexName == tex.Name)
							{
								crossRefMaterialTextures[i].Add(new KeyList<xxTexture>(textures, m));
								crossRefTextureMaterials[m].Add(new KeyList<xxMaterial>(materials, i));
								foundMatTex = true;
								break;
							}
						}
						if (!foundMatTex)
						{
							matTex.Name = String.Empty;
							Report.ReportLog("Warning: Couldn't find texture " + matTexName + " for material " + mat.Name + ". Setting it to (none)");
						}
					}
				}
			}

			for (int i = 0; i < meshes.Count; i++)
			{
				xxFrame meshParent = meshes[i];
				for (int j = 0; j < meshParent.Mesh.SubmeshList.Count; j++)
				{
					xxSubmesh submesh = meshParent.Mesh.SubmeshList[j];
					int matIdx = submesh.MaterialIndex;
					if ((matIdx >= 0) && (matIdx < materials.Count))
					{
						xxMaterial mat = materials[matIdx];
						crossRefMeshMaterials[i].Add(new KeyList<xxMaterial>(materials, matIdx));
						crossRefMaterialMeshes[matIdx].Add(new KeyList<xxFrame>(meshes, i));
						for (int k = 0; k < mat.Textures.Length; k++)
						{
							xxMaterialTexture matTex = mat.Textures[k];
							string matTexName = matTex.Name;
							if (matTex.Name != String.Empty)
							{
								bool foundMatTex = false;
								for (int m = 0; m < textures.Count; m++)
								{
									xxTexture tex = textures[m];
									if (matTexName == tex.Name)
									{
										crossRefMeshTextures[i].Add(new KeyList<xxTexture>(textures, m));
										crossRefTextureMeshes[m].Add(new KeyList<xxFrame>(meshes, i));
										foundMatTex = true;
										break;
									}
								}
								if (!foundMatTex)
								{
									matTex.Name = String.Empty;
									Report.ReportLog("Warning: Couldn't find texture " + matTexName + " for material " + mat.Name + ". Setting it to (none)");
								}
							}
						}
					}
					else
					{
						submesh.MaterialIndex = -1;
						Report.ReportLog("Warning: Mesh " + meshParent.Name + " Object " + j + " has an invalid material index");
					}
				}
			}

			CrossRefsSet();
		}

		private void CrossRefsSet()
		{
			listViewItemSyncSelectedSent = true;

			listViewMeshMaterial.BeginUpdate();
			listViewMeshTexture.BeginUpdate();
			for (int i = 0; i < listViewMesh.SelectedItems.Count; i++)
			{
				int meshParent = (int)listViewMesh.SelectedItems[i].Tag;
				CrossRefAddItem(crossRefMeshMaterials[meshParent], crossRefMeshMaterialsCount, listViewMeshMaterial, listViewMaterial);
				CrossRefAddItem(crossRefMeshTextures[meshParent], crossRefMeshTexturesCount, listViewMeshTexture, listViewTexture);
			}
			listViewMeshMaterial.EndUpdate();
			listViewMeshTexture.EndUpdate();

			listViewMaterialMesh.BeginUpdate();
			listViewMaterialTexture.BeginUpdate();
			for (int i = 0; i < listViewMaterial.SelectedItems.Count; i++)
			{
				int mat = (int)listViewMaterial.SelectedItems[i].Tag;
				CrossRefAddItem(crossRefMaterialMeshes[mat], crossRefMaterialMeshesCount, listViewMaterialMesh, listViewMesh);
				CrossRefAddItem(crossRefMaterialTextures[mat], crossRefMaterialTexturesCount, listViewMaterialTexture, listViewTexture);
			}
			listViewMaterialMesh.EndUpdate();
			listViewMaterialTexture.EndUpdate();

			listViewTextureMesh.BeginUpdate();
			listViewTextureMaterial.BeginUpdate();
			for (int i = 0; i < listViewTexture.SelectedItems.Count; i++)
			{
				int tex = (int)listViewTexture.SelectedItems[i].Tag;
				CrossRefAddItem(crossRefTextureMeshes[tex], crossRefTextureMeshesCount, listViewTextureMesh, listViewMesh);
				CrossRefAddItem(crossRefTextureMaterials[tex], crossRefTextureMaterialsCount, listViewTextureMaterial, listViewMaterial);
			}
			listViewTextureMesh.EndUpdate();
			listViewTextureMaterial.EndUpdate();

			listViewItemSyncSelectedSent = false;
		}

		private void CrossRefsClear()
		{
			listViewItemSyncSelectedSent = true;

			listViewMeshMaterial.BeginUpdate();
			listViewMeshTexture.BeginUpdate();
			foreach (var pair in crossRefMeshMaterials)
			{
				int mesh = pair.Key;
				CrossRefRemoveItem(pair.Value, crossRefMeshMaterialsCount, listViewMeshMaterial);
				CrossRefRemoveItem(crossRefMeshTextures[mesh], crossRefMeshTexturesCount, listViewMeshTexture);
			}
			listViewMeshMaterial.EndUpdate();
			listViewMeshTexture.EndUpdate();

			listViewMaterialMesh.BeginUpdate();
			listViewMaterialTexture.BeginUpdate();
			foreach (var pair in crossRefMaterialMeshes)
			{
				int mat = pair.Key;
				CrossRefRemoveItem(pair.Value, crossRefMaterialMeshesCount, listViewMaterialMesh);
				CrossRefRemoveItem(crossRefMaterialTextures[mat], crossRefMaterialTexturesCount, listViewMaterialTexture);
			}
			listViewMaterialMesh.EndUpdate();
			listViewMaterialTexture.EndUpdate();

			listViewTextureMesh.BeginUpdate();
			listViewTextureMaterial.BeginUpdate();
			foreach (var pair in crossRefTextureMeshes)
			{
				int tex = pair.Key;
				CrossRefRemoveItem(pair.Value, crossRefTextureMeshesCount, listViewTextureMesh);
				CrossRefRemoveItem(crossRefTextureMaterials[tex], crossRefTextureMaterialsCount, listViewTextureMaterial);
			}
			listViewTextureMesh.EndUpdate();
			listViewTextureMaterial.EndUpdate();

			listViewItemSyncSelectedSent = false;
		}

		private void CrossRefAddItem<T>(List<KeyList<T>> list, Dictionary<int, int> dic, ListView listView, ListView mainView)
		{
			bool added = false;
			for (int i = 0; i < list.Count; i++)
			{
				int count = dic[list[i].Index] + 1;
				dic[list[i].Index] = count;
				if (count == 1)
				{
					var keylist = list[i];
					ListViewItem item = new ListViewItem(keylist.List[keylist.Index].ToString());
					item.Tag = keylist.Index;

					foreach (ListViewItem mainItem in mainView.Items)
					{
						if ((int)mainItem.Tag == keylist.Index)
						{
							item.Selected = mainItem.Selected;
							break;
						}
					}

					listView.Items.Add(item);
					added = true;
				}
			}

			if (added)
			{
				listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}

		private void CrossRefRemoveItem<T>(List<KeyList<T>> list, Dictionary<int, int> dic, ListView listView)
		{
			bool removed = false;
			for (int i = 0; i < list.Count; i++)
			{
				int count = dic[list[i].Index] - 1;
				dic[list[i].Index] = count;
				if (count == 0)
				{
					var tuple = list[i];
					for (int j = 0; j < listView.Items.Count; j++)
					{
						if ((int)listView.Items[j].Tag == tuple.Index)
						{
							listView.Items.RemoveAt(j);
							removed = true;
							break;
						}
					}
				}
			}

			if (removed)
			{
				listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}

		private void CrossRefSetSelected(bool selected, ListView view, int tag)
		{
			foreach (ListViewItem item in view.Items)
			{
				if ((int)item.Tag == tag)
				{
					item.Selected = selected;
					break;
				}
			}
		}

		private void treeViewObjectTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			try
			{
				if (e.Node.Tag is DragSource)
				{
					var tag = (DragSource)e.Node.Tag;
					if (tag.Type == typeof(xxFrame))
					{
						tabControlViews.SelectedTab = tabPageFrameView;
						LoadFrame((int)tag.Id);
					}
					else if (tag.Type == typeof(xxMesh))
					{
						SetListViewAfterNodeSelect(listViewMesh, tag);
					}
					else if (tag.Type == typeof(xxBone))
					{
						tabControlViews.SelectedTab = tabPageBoneView;
						int[] ids = (int[])tag.Id;
						LoadBone(ids);

						if (highlightedBone != null)
							HighlightBone(highlightedBone, false);
						HighlightBone(ids, true);
						highlightedBone = ids;
					}
					else if (tag.Type == typeof(xxMaterial))
					{
						SetListViewAfterNodeSelect(listViewMaterial, tag);
					}
					else if (tag.Type == typeof(xxTexture))
					{
						SetListViewAfterNodeSelect(listViewTexture, tag);
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void treeViewObjectTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Tag is DragSource && ((DragSource)e.Node.Tag).Type == typeof(xxBone) && e.Node.IsSelected)
			{
				if (highlightedBone != null)
				{
					HighlightBone(highlightedBone, false);
					highlightedBone = null;
				}
				else
				{
					highlightedBone = (int[])((DragSource)e.Node.Tag).Id;
					HighlightBone(highlightedBone, true);
				}
			}
		}

		private void HighlightBone(int[] boneIds, bool show)
		{
			RenderObjectXX renderObj = renderObjectMeshes[boneIds[0]];
			if (renderObj != null)
			{
				xxMesh mesh = Editor.Meshes[boneIds[0]].Mesh;
				renderObj.HighlightBone(mesh, boneIds[1], show);
				Gui.Renderer.Render();
			}
		}

		private void SetListViewAfterNodeSelect(ListView listView, DragSource tag)
		{
			while (listView.SelectedItems.Count > 0)
			{
				listView.SelectedItems[0].Selected = false;
			}

			for (int i = 0; i < listView.Items.Count; i++)
			{
				var item = listView.Items[i];
				if ((int)item.Tag == (int)tag.Id)
				{
					item.Selected = true;
					break;
				}
			}
		}

		private void listViewMesh_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			try
			{
				if (listViewItemSyncSelectedSent == false)
				{
					listViewItemSyncSelectedSent = true;
					listViewMeshMaterial.BeginUpdate();
					listViewMeshTexture.BeginUpdate();

					int id = (int)e.Item.Tag;
					if (e.IsSelected)
					{
						if (!Gui.Docking.DockRenderer.IsHidden)
						{
							Gui.Docking.DockRenderer.Activate();
						}
						tabControlViews.SelectedTab = tabPageMeshView;
						LoadMesh(id);
						CrossRefAddItem(crossRefMeshMaterials[id], crossRefMeshMaterialsCount, listViewMeshMaterial, listViewMaterial);
						CrossRefAddItem(crossRefMeshTextures[id], crossRefMeshTexturesCount, listViewMeshTexture, listViewTexture);

						if (renderObjectMeshes[id] == null)
						{
							xxFrame frame = Editor.Meshes[id];
							HashSet<string> meshNames = new HashSet<string>() { frame.Name };
							renderObjectMeshes[id] = new RenderObjectXX(Editor.Parser, meshNames);
						}
						RenderObjectXX renderObj = renderObjectMeshes[id];
						renderObjectIds[id] = Gui.Renderer.AddRenderObject(renderObj);
					}
					else
					{
						if (id == loadedMesh)
						{
							LoadMesh(-1);
						}
						CrossRefRemoveItem(crossRefMeshMaterials[id], crossRefMeshMaterialsCount, listViewMeshMaterial);
						CrossRefRemoveItem(crossRefMeshTextures[id], crossRefMeshTexturesCount, listViewMeshTexture);

						Gui.Renderer.RemoveRenderObject(renderObjectIds[id]);
					}

					CrossRefSetSelected(e.IsSelected, listViewMesh, id);
					CrossRefSetSelected(e.IsSelected, listViewMaterialMesh, id);
					CrossRefSetSelected(e.IsSelected, listViewTextureMesh, id);

					listViewMeshMaterial.EndUpdate();
					listViewMeshTexture.EndUpdate();
					listViewItemSyncSelectedSent = false;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void listViewMaterial_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			try
			{
				if (listViewItemSyncSelectedSent == false)
				{
					listViewItemSyncSelectedSent = true;
					listViewMaterialMesh.BeginUpdate();
					listViewMaterialTexture.BeginUpdate();

					int id = (int)e.Item.Tag;
					if (e.IsSelected)
					{
						tabControlViews.SelectedTab = tabPageMaterialView;
						LoadMaterial(id);
						CrossRefAddItem(crossRefMaterialMeshes[id], crossRefMaterialMeshesCount, listViewMaterialMesh, listViewMesh);
						CrossRefAddItem(crossRefMaterialTextures[id], crossRefMaterialTexturesCount, listViewMaterialTexture, listViewTexture);
					}
					else
					{
						if (id == loadedMaterial)
						{
							LoadMaterial(-1);
						}
						CrossRefRemoveItem(crossRefMaterialMeshes[id], crossRefMaterialMeshesCount, listViewMaterialMesh);
						CrossRefRemoveItem(crossRefMaterialTextures[id], crossRefMaterialTexturesCount, listViewMaterialTexture);
					}

					CrossRefSetSelected(e.IsSelected, listViewMaterial, id);
					CrossRefSetSelected(e.IsSelected, listViewMeshMaterial, id);
					CrossRefSetSelected(e.IsSelected, listViewTextureMaterial, id);

					listViewMaterialMesh.EndUpdate();
					listViewMaterialTexture.EndUpdate();
					listViewItemSyncSelectedSent = false;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void listViewTexture_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			try
			{
				if (listViewItemSyncSelectedSent == false)
				{
					listViewItemSyncSelectedSent = true;
					listViewTextureMesh.BeginUpdate();
					listViewTextureMaterial.BeginUpdate();

					int id = (int)e.Item.Tag;
					if (e.IsSelected)
					{
						tabControlViews.SelectedTab = tabPageTextureView;
						LoadTexture(id);
						CrossRefAddItem(crossRefTextureMeshes[id], crossRefTextureMeshesCount, listViewTextureMesh, listViewMesh);
						CrossRefAddItem(crossRefTextureMaterials[id], crossRefTextureMaterialsCount, listViewTextureMaterial, listViewMaterial);
					}
					else
					{
						if (id == loadedTexture)
						{
							LoadTexture(-1);
						}
						CrossRefRemoveItem(crossRefTextureMeshes[id], crossRefTextureMeshesCount, listViewTextureMesh);
						CrossRefRemoveItem(crossRefTextureMaterials[id], crossRefTextureMaterialsCount, listViewTextureMaterial);
					}

					CrossRefSetSelected(e.IsSelected, listViewTexture, id);
					CrossRefSetSelected(e.IsSelected, listViewMeshTexture, id);
					CrossRefSetSelected(e.IsSelected, listViewMaterialTexture, id);

					listViewTextureMesh.EndUpdate();
					listViewTextureMaterial.EndUpdate();
					listViewItemSyncSelectedSent = false;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void listViewMeshMaterial_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			listViewMaterial_ItemSelectionChanged(sender, e);
		}

		private void listViewMeshTexture_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			listViewTexture_ItemSelectionChanged(sender, e);
		}

		private void listViewMaterialMesh_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			listViewMesh_ItemSelectionChanged(sender, e);
		}

		private void listViewMaterialTexture_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			listViewTexture_ItemSelectionChanged(sender, e);
		}

		private void listViewTextureMesh_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			listViewMesh_ItemSelectionChanged(sender, e);
		}

		private void listViewTextureMaterial_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			listViewMaterial_ItemSelectionChanged(sender, e);
		}

		TreeNode FindFrameNode(string name, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				var source = node.Tag as DragSource?;
				if ((source == null) || (source.Value.Type != typeof(xxFrame)))
				{
					return null;
				}

				if (Editor.Frames[(int)source.Value.Id].Name == name)
				{
					return node;
				}

				TreeNode found = FindFrameNode(name, node.Nodes);
				if (found != null)
				{
					return found;
				}
			}

			return null;
		}

		TreeNode FindFrameNode(xxFrame frame, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				var source = node.Tag as DragSource?;
				if ((source == null) || (source.Value.Type != typeof(xxFrame)))
				{
					return null;
				}

				if (Editor.Frames[(int)source.Value.Id].Equals(frame))
				{
					return node;
				}

				TreeNode found = FindFrameNode(frame, node.Nodes);
				if (found != null)
				{
					return found;
				}
			}

			return null;
		}

		TreeNode FindBoneNode(xxBone bone, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				var source = node.Tag as DragSource?;
				
				var tuple = node.Tag as Tuple<xxBone, int[]>;
				if ((source != null) && (source.Value.Type == typeof(xxBone)))
				{
					var id = (int[])source.Value.Id;
					if (Editor.Meshes[id[0]].Mesh.BoneList[id[1]].Equals(bone))
					{
						return node;
					}
				}

				TreeNode found = FindBoneNode(bone, node.Nodes);
				if (found != null)
				{
					return found;
				}
			}

			return null;
		}

		private void buttonBoneGotoFrame_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedBone != null)
				{
					xxBone bone = Editor.Meshes[loadedBone[0]].Mesh.BoneList[loadedBone[1]];
					TreeNode node = FindFrameNode(bone.Name, treeViewObjectTree.Nodes);
					if (node != null)
					{
						tabControlLists.SelectedTab = tabPageObject;
						treeViewObjectTree.SelectedNode = node;
						node.Expand();
						node.EnsureVisible();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonMeshGotoFrame_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedMesh >= 0)
				{
					TreeNode node = FindFrameNode(Editor.Meshes[loadedMesh], treeViewObjectTree.Nodes);
					if (node != null)
					{
						tabControlLists.SelectedTab = tabPageObject;
						treeViewObjectTree.SelectedNode = node;
						node.Expand();
						node.EnsureVisible();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void treeViewObjectTree_ItemDrag(object sender, ItemDragEventArgs e)
		{
			try
			{
				if (e.Item is TreeNode)
				{
					treeViewObjectTree.DoDragDrop(e.Item, DragDropEffects.Copy);
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void treeViewObjectTree_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				UpdateDragDrop(sender, e);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void treeViewObjectTree_DragOver(object sender, DragEventArgs e)
		{
			try
			{
				UpdateDragDrop(sender, e);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void treeViewObjectTree_DragDrop(object sender, DragEventArgs e)
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
					ProcessDragDropSources(node);
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void ProcessDragDropSources(TreeNode node)
		{
			if (node.Tag is DragSource)
			{
				if ((node.Parent != null) && !node.Checked)
				{
					return;
				}

				DragSource? dest = null;
				if (treeViewObjectTree.SelectedNode != null)
				{
					dest = treeViewObjectTree.SelectedNode.Tag as DragSource?;
				}

				DragSource source = (DragSource)node.Tag;
				if (source.Type == typeof(xxFrame))
				{
					using (var dragOptions = new FormXXDragDrop(Editor, true))
					{
						var srcEditor = (xxEditor)Gui.Scripting.Variables[source.Variable];
						var srcFrameName = srcEditor.Frames[(int)source.Id].Name;
						dragOptions.numericFrameId.Value = GetDestParentId(srcFrameName, dest);
						if (dragOptions.ShowDialog() == DialogResult.OK)
						{
							Gui.Scripting.RunScript(EditorVar + "." + dragOptions.FrameMethod.GetName() + "(srcFrame=" + source.Variable + ".Frames[" + (int)source.Id + "], srcFormat=" + source.Variable + ".Parser.Format, destParentId=" + dragOptions.numericFrameId.Value + ", meshMatOffset=" + dragOptions.numericFrameMeshMatOffset.Value + ")");
							RecreateFrames();
						}
					}
				}
				else if (source.Type == typeof(xxMaterial))
				{
					Gui.Scripting.RunScript(EditorVar + ".MergeMaterial(mat=" + source.Variable + ".Parser.MaterialList[" + (int)source.Id + "], srcFormat=" + source.Variable + ".Parser.Format)");
					RecreateMaterials();
				}
				else if (source.Type == typeof(xxTexture))
				{
					Gui.Scripting.RunScript(EditorVar + ".MergeTexture(tex=" + source.Variable + ".Parser.TextureList[" + (int)source.Id + "])");
					RecreateTextures();
				}
				else if (source.Type == typeof(ImportedFrame))
				{
					using (var dragOptions = new FormXXDragDrop(Editor, true))
					{
						var srcEditor = (ImportedEditor)Gui.Scripting.Variables[source.Variable];
						var srcFrameName = srcEditor.Frames[(int)source.Id].Name;
						dragOptions.numericFrameId.Value = GetDestParentId(srcFrameName, dest);
						if (dragOptions.ShowDialog() == DialogResult.OK)
						{
							Gui.Scripting.RunScript(EditorVar + "." + dragOptions.FrameMethod.GetName() + "(srcFrame=" + source.Variable + ".Frames[" + (int)source.Id + "], destParentId=" + dragOptions.numericFrameId.Value + ", meshMatOffset=" + dragOptions.numericFrameMeshMatOffset.Value + ")");
							RecreateFrames();
						}
					}
				}
				else if (source.Type == typeof(ImportedMesh))
				{
					using (var dragOptions = new FormXXDragDrop(Editor, false))
					{
						var srcEditor = (ImportedEditor)Gui.Scripting.Variables[source.Variable];

						var destFrameId = Editor.GetFrameId(srcEditor.Imported.MeshList[(int)source.Id].Name);
						if (destFrameId < 0)
						{
							destFrameId = 0;
						}
						dragOptions.numericMeshId.Value = destFrameId;

						if (dragOptions.ShowDialog() == DialogResult.OK)
						{
							Gui.Scripting.RunScript(EditorVar + ".ReplaceMesh(mesh=" + source.Variable + ".Imported.MeshList[" + (int)source.Id + "], frameId=" + dragOptions.numericMeshId.Value + ", merge=" + dragOptions.radioButtonMeshMerge.Checked + ", normals=\"" + dragOptions.NormalsMethod.GetName() + "\", bones=\"" + dragOptions.BonesMethod.GetName() + "\")");
							RecreateMeshes();
						}
					}
				}
				else if (source.Type == typeof(ImportedMaterial))
				{
					Gui.Scripting.RunScript(EditorVar + ".MergeMaterial(mat=" + source.Variable + ".Imported.MaterialList[" + (int)source.Id + "])");
					RecreateMaterials();
				}
				else if (source.Type == typeof(ImportedTexture))
				{
					Gui.Scripting.RunScript(EditorVar + ".MergeTexture(tex=" + source.Variable + ".Imported.TextureList[" + (int)source.Id + "])");
					RecreateTextures();
				}
			}
			else
			{
				foreach (TreeNode child in node.Nodes)
				{
					ProcessDragDropSources(child);
				}
			}
		}

		private void RecreateFrames()
		{
			CrossRefsClear();
			DisposeRenderObjects();
			LoadFrame(-1);
			LoadMesh(-1);
			InitFrames();
			InitMeshes();
			RecreateRenderObjects();
			RecreateCrossRefs();
		}

		private void RecreateMeshes()
		{
			CrossRefsClear();
			DisposeRenderObjects();
			LoadMesh(-1);
			InitFrames();
			InitMeshes();
			RecreateRenderObjects();
			RecreateCrossRefs();
		}

		private void RecreateMaterials()
		{
			CrossRefsClear();
			DisposeRenderObjects();
			LoadMaterial(-1);
			InitMaterials();
			RecreateRenderObjects();
			RecreateCrossRefs();
			LoadMesh(loadedMesh);
		}

		private void RecreateTextures()
		{
			CrossRefsClear();
			DisposeRenderObjects();
			LoadTexture(-1);
			InitTextures();
			RecreateRenderObjects();
			RecreateCrossRefs();
			LoadMaterial(loadedMaterial);
		}

		private int GetDestParentId(string srcFrameName, DragSource? dest)
		{
			int destParentId = -1;
			if (dest == null)
			{
				var destFrameId = Editor.GetFrameId(srcFrameName);
				if (destFrameId >= 0)
				{
					var destFrameParent = Editor.Frames[destFrameId].Parent;
					if (destFrameParent != null)
					{
						for (int i = 0; i < Editor.Frames.Count; i++)
						{
							if (Editor.Frames[i] == destFrameParent)
							{
								destParentId = i;
								break;
							}
						}
					}
				}
			}
			else if (dest.Value.Type == typeof(xxFrame))
			{
				destParentId = (int)dest.Value.Id;
			}

			return destParentId;
		}

		private void UpdateDragDrop(object sender, DragEventArgs e)
		{
			Point p = treeViewObjectTree.PointToClient(new Point(e.X, e.Y));
			TreeNode target = treeViewObjectTree.GetNodeAt(p);
			if ((target != null) && ((p.X < target.Bounds.Left) || (p.X > target.Bounds.Right) || (p.Y < target.Bounds.Top) || (p.Y > target.Bounds.Bottom)))
			{
				target = null;
			}
			treeViewObjectTree.SelectedNode = target;

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

		private void buttonObjectTreeExpand_Click(object sender, EventArgs e)
		{
			try
			{
				treeViewObjectTree.BeginUpdate();
				treeViewObjectTree.ExpandAll();
				treeViewObjectTree.EndUpdate();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonObjectTreeCollapse_Click(object sender, EventArgs e)
		{
			try
			{
				treeViewObjectTree.BeginUpdate();
				treeViewObjectTree.CollapseAll();
				treeViewObjectTree.EndUpdate();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonConvert_Click(object sender, EventArgs e)
		{
			try
			{
				using (FormXXConvert convert = new FormXXConvert(Editor.Parser.Format))
				{
					if (convert.ShowDialog() == DialogResult.OK)
					{
						Gui.Scripting.RunScript("ConvertXX(parser=" + ParserVar + ", format=" + convert.Format + ")");

						InitFormat();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonEditHex_Click(object sender, EventArgs e)
		{
			try
			{
				using (var editHex = new FormXXEditHex(this, null))
				{
					editHex.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMoveUp_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedFrame < 0)
				{
					return;
				}

				var frame = Editor.Frames[loadedFrame];
				var parent = (xxFrame)frame.Parent;
				if (parent == null)
				{
					return;
				}

				int idx = parent.IndexOf(frame);
				if ((idx > 0) && (idx < parent.Count))
				{
					TreeNode node = FindFrameNode(frame, treeViewObjectTree.Nodes);
					TreeNode parentNode = node.Parent;
					bool selected = node.Equals(node.TreeView.SelectedNode);
					int nodeIdx = node.Index;
					node.TreeView.BeginUpdate();
					parentNode.Nodes.RemoveAt(nodeIdx);
					parentNode.Nodes.Insert(nodeIdx - 1, node);
					if (selected)
					{
						node.TreeView.SelectedNode = node;
					}
					node.TreeView.EndUpdate();

					var source = (DragSource)parentNode.Tag;
					Gui.Scripting.RunScript(EditorVar + ".MoveFrame(id=" + loadedFrame + ", parent=" + (int)source.Id + ", index=" + (idx - 1) + ")");
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMoveDown_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedFrame < 0)
				{
					return;
				}

				var frame = Editor.Frames[loadedFrame];
				var parent = (xxFrame)frame.Parent;
				if (parent == null)
				{
					return;
				}

				int idx = parent.IndexOf(frame);
				if ((idx >= 0) && (idx < (parent.Count - 1)))
				{
					TreeNode node = FindFrameNode(frame, treeViewObjectTree.Nodes);
					TreeNode parentNode = node.Parent;
					bool selected = node.Equals(node.TreeView.SelectedNode);
					int nodeIdx = node.Index;
					node.TreeView.BeginUpdate();
					parentNode.Nodes.RemoveAt(nodeIdx);
					parentNode.Nodes.Insert(nodeIdx + 1, node);
					if (selected)
					{
						node.TreeView.SelectedNode = node;
					}
					node.TreeView.EndUpdate();

					var source = (DragSource)parentNode.Tag;
					Gui.Scripting.RunScript(EditorVar + ".MoveFrame(id=" + loadedFrame + ", parent=" + (int)source.Id + ", index=" + (idx + 1) + ")");
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameRemove_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedFrame < 0)
				{
					return;
				}
				if (Editor.Frames[loadedFrame].Parent == null)
				{
					Report.ReportLog("Can't remove the root frame");
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".RemoveFrame(id=" + loadedFrame + ")");

				RecreateFrames();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMatrixIdentity_Click(object sender, EventArgs e)
		{
			try
			{
				LoadMatrix(Matrix.Identity, dataGridViewFrameSRT, dataGridViewFrameMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMatrixCombined_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedFrame < 0)
				{
					return;
				}

				xxFrame frame = Editor.Frames[loadedFrame];
				Matrix m = frame.Matrix;
				xxFrame parent = (xxFrame)frame.Parent;
				while (parent != null)
				{
					m = parent.Matrix * m;
					parent = parent.Parent;
				}
				LoadMatrix(m, dataGridViewFrameSRT, dataGridViewFrameMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMatrixInverse_Click(object sender, EventArgs e)
		{
			try
			{
				Matrix m = GetMatrix(dataGridViewFrameMatrix);
				LoadMatrix(Matrix.Invert(m), dataGridViewFrameSRT, dataGridViewFrameMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMatrixGrow_Click(object sender, EventArgs e)
		{
			try
			{
				float ratio = Decimal.ToSingle(numericFrameMatrixRatio.Value);
				Vector3[] srt = GetSRT(dataGridViewFrameSRT);
				srt[0] = srt[0] * ratio;
				srt[2] = srt[2] * ratio;
				LoadMatrix(FbxUtility.SRTToMatrix(srt[0], srt[1], srt[2]), dataGridViewFrameSRT, dataGridViewFrameMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMatrixShrink_Click(object sender, EventArgs e)
		{
			try
			{
				float ratio = Decimal.ToSingle(numericFrameMatrixRatio.Value);
				Vector3[] srt = GetSRT(dataGridViewFrameSRT);
				srt[0] = srt[0] / ratio;
				srt[2] = srt[2] / ratio;
				LoadMatrix(FbxUtility.SRTToMatrix(srt[0], srt[1], srt[2]), dataGridViewFrameSRT, dataGridViewFrameMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMatrixCopy_Click(object sender, EventArgs e)
		{
			try
			{
				copyMatrices[Decimal.ToInt32(numericFrameMatrixNumber.Value) - 1] = GetMatrix(dataGridViewFrameMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMatrixPaste_Click(object sender, EventArgs e)
		{
			try
			{
				LoadMatrix(copyMatrices[Decimal.ToInt32(numericFrameMatrixNumber.Value) - 1], dataGridViewFrameSRT, dataGridViewFrameMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameMatrixApply_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedFrame < 0)
				{
					return;
				}

				Matrix m = GetMatrix(dataGridViewFrameMatrix);
				string command = EditorVar + ".SetFrameMatrix(id=" + loadedFrame;
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						command += ", m" + (i + 1) + (j + 1) + "=" + m[i, j].ToFloatString();
					}
				}
				command += ")";

				Gui.Scripting.RunScript(command);
				RecreateRenderObjects();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		Matrix GetMatrix(DataGridView viewMatrix)
		{
			Matrix m = new Matrix();
			DataTable table = (DataTable)viewMatrix.DataSource;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					m[i, j] = (float)table.Rows[i][j];
				}
			}
			return m;
		}

		Vector3[] GetSRT(DataGridView viewSRT)
		{
			DataTable table = (DataTable)viewSRT.DataSource;
			Vector3[] srt = new Vector3[3];
			for (int i = 0; i < 3; i++)
			{
				srt[0][i] = (float)table.Rows[2][i + 1];
				srt[1][i] = (float)table.Rows[1][i + 1];
				srt[2][i] = (float)table.Rows[0][i + 1];
			}
			return srt;
		}

		private void buttonBoneMatrixIdentity_Click(object sender, EventArgs e)
		{
			try
			{
				LoadMatrix(Matrix.Identity, dataGridViewBoneSRT, dataGridViewBoneMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonBoneMatrixInverse_Click(object sender, EventArgs e)
		{
			try
			{
				Matrix m = GetMatrix(dataGridViewBoneMatrix);
				LoadMatrix(Matrix.Invert(m), dataGridViewBoneSRT, dataGridViewBoneMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonBoneMatrixGrow_Click(object sender, EventArgs e)
		{
			try
			{
				float ratio = Decimal.ToSingle(numericBoneMatrixRatio.Value);
				Vector3[] srt = GetSRT(dataGridViewBoneSRT);
				srt[0] = srt[0] * ratio;
				srt[2] = srt[2] * ratio;
				LoadMatrix(FbxUtility.SRTToMatrix(srt[0], srt[1], srt[2]), dataGridViewBoneSRT, dataGridViewBoneMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonBoneMatrixShrink_Click(object sender, EventArgs e)
		{
			try
			{
				float ratio = Decimal.ToSingle(numericBoneMatrixRatio.Value);
				Vector3[] srt = GetSRT(dataGridViewBoneSRT);
				srt[0] = srt[0] / ratio;
				srt[2] = srt[2] / ratio;
				LoadMatrix(FbxUtility.SRTToMatrix(srt[0], srt[1], srt[2]), dataGridViewBoneSRT, dataGridViewBoneMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonBoneMatrixCopy_Click(object sender, EventArgs e)
		{
			try
			{
				copyMatrices[Decimal.ToInt32(numericBoneMatrixNumber.Value) - 1] = GetMatrix(dataGridViewBoneMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonBoneMatrixPaste_Click(object sender, EventArgs e)
		{
			try
			{
				LoadMatrix(copyMatrices[Decimal.ToInt32(numericBoneMatrixNumber.Value) - 1], dataGridViewBoneSRT, dataGridViewBoneMatrix);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonBoneMatrixApply_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedBone == null)
				{
					return;
				}

				Matrix m = GetMatrix(dataGridViewBoneMatrix);
				string command = EditorVar + ".SetBoneMatrix(meshId=" + loadedBone[0] + ", boneId=" + loadedBone[1];
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						command += ", m" + (i + 1) + (j + 1) + "=" + m[i, j].ToFloatString();
					}
				}
				command += ")";

				Gui.Scripting.RunScript(command);
				RecreateRenderObjects();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonBoneRemove_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedBone == null)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".RemoveBone(meshId=" + loadedBone[0] + ", boneId=" + loadedBone[1] + ")");

				LoadBone(null);
				RecreateRenderObjects();
				InitFrames();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonBoneCopy_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedBone == null)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".CopyBone(meshId=" + loadedBone[0] + ", boneId=" + loadedBone[1] + ")");

				InitFrames();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void textBoxBoneName_AfterEditTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (loadedBone == null)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".SetBoneName(meshId=" + loadedBone[0] + ", boneId=" + loadedBone[1] + ", name=\"" + textBoxBoneName.Text + "\")");
				RecreateRenderObjects();

				var bone = Editor.Meshes[loadedBone[0]].Mesh.BoneList[loadedBone[1]];
				TreeNode node = FindBoneNode(bone, treeViewObjectTree.Nodes);
				node.Text = bone.Name;
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonMeshRemove_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedMesh < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".RemoveMesh(id=" + loadedMesh + ")");

				RecreateMeshes();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonMeshMinBones_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedMesh < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".MinBones(id=" + loadedMesh + ")");

				InitFrames();
				RecreateRenderObjects();
				LoadBone(null);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonMeshNormals_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedMesh < 0)
				{
					return;
				}

				using (var normals = new FormXXNormals())
				{
					if (normals.ShowDialog() == DialogResult.OK)
					{
						Gui.Scripting.RunScript(EditorVar + ".CalculateNormals(id=" + loadedMesh + ", threshold=" + normals.numericThreshold.Value + ")");

						RecreateRenderObjects();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonSubmeshEdit_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedMesh < 0)
				{
					return;
				}

				Report.ReportLog("not implemented");
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonSubmeshRemove_Click(object sender, EventArgs e)
		{
			try
			{
				if ((loadedMesh < 0) || (dataGridViewMesh.SelectedRows.Count <= 0))
				{
					return;
				}

				dataGridViewMesh.SelectionChanged -= new EventHandler(dataGridViewMesh_SelectionChanged);

				int lastSelectedRow = -1;
				List<int> indices = new List<int>();
				foreach (DataGridViewRow row in dataGridViewMesh.SelectedRows)
				{
					indices.Add(row.Index);
					lastSelectedRow = row.Index;
				}
				indices.Sort();

				bool meshRemoved = (indices.Count == Editor.Meshes[loadedMesh].Mesh.SubmeshList.Count);

				for (int i = 0; i < indices.Count; i++)
				{
					int index = indices[i] - i;
					Gui.Scripting.RunScript(EditorVar + ".RemoveSubmesh(meshId=" + loadedMesh + ", submeshId=" + index + ")");
				}

				dataGridViewMesh.SelectionChanged += new EventHandler(dataGridViewMesh_SelectionChanged);

				if (meshRemoved)
				{
					RecreateMeshes();
				}
				else
				{
					LoadMesh(loadedMesh);
					if (lastSelectedRow == dataGridViewMesh.Rows.Count)
						lastSelectedRow--;
					dataGridViewMesh.Rows[lastSelectedRow].Selected = true;
					dataGridViewMesh.FirstDisplayedScrollingRowIndex = lastSelectedRow;
					RecreateRenderObjects();
					RecreateCrossRefs();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonMaterialRemove_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedMaterial < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".RemoveMaterial(id=" + loadedMaterial + ")");

				RecreateRenderObjects();
				InitMaterials();
				RecreateCrossRefs();
				LoadMesh(loadedMesh);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonMaterialCopy_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedMaterial < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".CopyMaterial(id=" + loadedMaterial + ")");

				InitMaterials();
				RecreateCrossRefs();
				LoadMesh(loadedMesh);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonTextureExport_Click(object sender, EventArgs e)
		{
			try
			{
				foreach (ListViewItem item in listViewTexture.SelectedItems)
				{
					int id = (int)item.Tag;
					xxTexture tex = Editor.Parser.TextureList[id];
					ImportedTexture importedTex = xx.ImportedTexture(tex);
					string path = exportDir + importedTex.Name;
					Gui.Scripting.RunScript(EditorVar + ".ExportTexture(id=" + id + ", path=\"" + path + "\")");
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonTextureRemove_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedTexture < 0)
				{
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".RemoveTexture(id=" + loadedTexture + ")");

				RecreateRenderObjects();
				InitTextures();
				RecreateCrossRefs();
				LoadMaterial(loadedMaterial);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonTextureAdd_Click(object sender, EventArgs e)
		{
			try
			{
				if (Gui.ImageControl.Image == null)
				{
					Report.ReportLog("An image hasn't been loaded");
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".AddTexture(image=" + Gui.ImageControl.ImageScriptVariable + ")");

				RecreateRenderObjects();
				InitTextures();
				RecreateCrossRefs();
				LoadMaterial(loadedMaterial);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonTextureReplace_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedTexture < 0)
				{
					return;
				}
				if (Gui.ImageControl.Image == null)
				{
					Report.ReportLog("An image hasn't been loaded");
					return;
				}

				Gui.Scripting.RunScript(EditorVar + ".ReplaceTexture(id=" + loadedTexture + ", image=" + Gui.ImageControl.ImageScriptVariable + ")");

				RecreateRenderObjects();
				InitTextures();
				RecreateCrossRefs();
				LoadMaterial(loadedMaterial);
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void dataGridViewMesh_SelectionChanged(object sender, EventArgs e)
		{
			try
			{
				HighlightSubmeshes();
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		void HighlightSubmeshes()
		{
			if (loadedMesh < 0)
			{
				return;
			}

			RenderObjectXX renderObj = renderObjectMeshes[loadedMesh];
			if (renderObj != null)
			{
				renderObj.HighlightSubmesh.Clear();
				foreach (DataGridViewRow row in dataGridViewMesh.SelectedRows)
				{
					renderObj.HighlightSubmesh.Add(row.Index);
				}
				Gui.Renderer.Render();
			}
		}

		private void dataGridViewMesh_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyData == Keys.Escape)
				{
					while (dataGridViewMesh.SelectedRows.Count > 0)
					{
						dataGridViewMesh.SelectedRows[0].Selected = false;
					}
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void dataGridViewMesh_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if ((dataGridViewMesh.CurrentRow != null) && (dataGridViewMesh.CurrentCell.ColumnIndex == 2))
				{
					dataGridViewMesh.BeginEdit(true);
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonMeshExport_Click(object sender, EventArgs e)
		{
			try
			{
				DirectoryInfo dir = new DirectoryInfo(exportDir);

				string meshNames = String.Empty;
				if (listViewMesh.SelectedItems.Count > 0)
				{
					for (int i = 0; i < listViewMesh.SelectedItems.Count; i++)
					{
						meshNames += "\"" + Editor.Meshes[(int)listViewMesh.SelectedItems[i].Tag].Name + "\", ";
					}
				}
				else
				{
					if (listViewMesh.Items.Count <= 0)
					{
						Report.ReportLog("There are no meshes for exporting");
						return;
					}

					for (int i = 0; i < listViewMesh.Items.Count; i++)
					{
						meshNames += "\"" + Editor.Meshes[(int)listViewMesh.Items[i].Tag].Name + "\", ";
					}
				}
				meshNames = "{ " + meshNames.Substring(0, meshNames.Length - 2) + " }";

				Report.ReportLog("Started exporting to " + comboBoxMeshExportFormat.SelectedItem + " format...");
				Application.DoEvents();

				string xaVars = String.Empty;
				List<DockContent> formXAList;
				if (Gui.Docking.DockContents.TryGetValue(typeof(FormXA), out formXAList))
				{
					foreach (FormXA form in formXAList)
					{
						var xaParser = (xaParser)Gui.Scripting.Variables[form.ParserVar];
						if (xaParser.AnimationSection != null)
						{
							xaVars += form.ParserVar + ", ";
						}
					}
				}

				if (xaVars.Length > 0)
				{
					xaVars = "{ " + xaVars.Substring(0, xaVars.Length - 2) + " }";
				}
				else
				{
					xaVars = "null";
				}

				switch ((MeshExportFormat)comboBoxMeshExportFormat.SelectedIndex)
				{
					case MeshExportFormat.Mqo:
						Gui.Scripting.RunScript("ExportMqo(parser=" + ParserVar + ", meshNames=" + meshNames + ", dirPath=\"" + dir.FullName + "\", singleMqo=" + checkBoxMeshExportMqoSingleFile.Checked + ", worldCoords=" + checkBoxMeshExportMqoWorldCoords.Checked + ")");
						break;
					case MeshExportFormat.DirectXSDK:
						Report.ReportLog("not implemented");
						//DirectX.Exporter.Export(Utility.GetDestFile(dir, "meshes", ".x"), parser, meshParents, xaSubfileList, 10, 1);
						break;
					case MeshExportFormat.Collada:
						Report.ReportLog("not implemented");
						//Collada.Exporter.Export(Utility.GetDestFile(dir, "meshes", ".dae"), parser, meshParents, xaSubfileList, checkBoxMeshExportColladaAllFrames.Checked);
						break;
					case MeshExportFormat.ColladaFbx:
						Report.ReportLog("not implemented");
						//Fbx.Exporter.Export(Utility.GetDestFile(dir, "meshes", ".dae"), parser, meshParents, xaSubfileList, checkBoxMeshExportFbxAllFrames.Checked, checkBoxMeshExportFbxSkins.Checked, ".dae");
						break;
					case MeshExportFormat.Fbx:
						Gui.Scripting.RunScript("ExportFbx(xxParser=" + ParserVar + ", meshNames=" + meshNames + ", xaParsers=" + xaVars + ", path=\"" + Utility.GetDestFile(dir, "meshes", ".fbx") + "\", exportFormat=\".fbx\", allFrames=" + checkBoxMeshExportFbxAllFrames.Checked + ", skins=" + checkBoxMeshExportFbxSkins.Checked + ")");
						break;
					case MeshExportFormat.Dxf:
						Gui.Scripting.RunScript("ExportFbx(xxParser=" + ParserVar + ", meshNames=" + meshNames + ", xaParsers=" + xaVars + ", path=\"" + Utility.GetDestFile(dir, "meshes", ".dxf") + "\", exportFormat=\".dxf\", allFrames=" + checkBoxMeshExportFbxAllFrames.Checked + ", skins=" + checkBoxMeshExportFbxSkins.Checked + ")");
						break;
					case MeshExportFormat._3ds:
						Gui.Scripting.RunScript("ExportFbx(xxParser=" + ParserVar + ", meshNames=" + meshNames + ", xaParsers=" + xaVars + ", path=\"" + Utility.GetDestFile(dir, "meshes", ".3ds") + "\", exportFormat=\".3ds\", allFrames=" + checkBoxMeshExportFbxAllFrames.Checked + ", skins=" + checkBoxMeshExportFbxSkins.Checked + ")");
						break;
					case MeshExportFormat.Obj:
						Gui.Scripting.RunScript("ExportFbx(xxParser=" + ParserVar + ", meshNames=" + meshNames + ", xaParsers=" + xaVars + ", path=\"" + Utility.GetDestFile(dir, "meshes", ".obj") + "\", exportFormat=\".obj\", allFrames=" + checkBoxMeshExportFbxAllFrames.Checked + ", skins=" + checkBoxMeshExportFbxSkins.Checked + ")");
						break;
					default:
						throw new Exception("Unexpected ExportFormat");
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonFrameEditHex_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedFrame < 0)
				{
					return;
				}

				List<int[]> gotoCells = new List<int[]>();
				var frame = Editor.Frames[loadedFrame];
				if (frame.Mesh != null)
				{
					for (int i = 0; i < Editor.Meshes.Count; i++)
					{
						if (Editor.Meshes[i] == frame)
						{
							gotoCells.Add(new int[] { 2, i });
							break;
						}
					}
				}
				gotoCells.Add(new int[] { 1, loadedFrame });

				using (var editHex = new FormXXEditHex(this, gotoCells))
				{
					editHex.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void buttonMeshEditHex_Click(object sender, EventArgs e)
		{
			try
			{
				if (loadedMesh < 0)
				{
					return;
				}

				int frameId = -1;
				for (int i = 0; i < Editor.Frames.Count; i++)
				{
					if (Editor.Frames[i] == Editor.Meshes[loadedMesh])
					{
						frameId = i;
						break;
					}
				}

				List<int[]> gotoCells = new List<int[]>();
				gotoCells.Add(new int[] { 1, frameId });
				gotoCells.Add(new int[] { 2, loadedMesh });

				using (var editHex = new FormXXEditHex(this, gotoCells))
				{
					editHex.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}

		private void comboBoxMeshExportFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				switch ((MeshExportFormat)comboBoxMeshExportFormat.SelectedIndex)
				{
					case MeshExportFormat.Mqo:
						panelMeshExportOptionsMqo.BringToFront();
						break;
					case MeshExportFormat.DirectXSDK:
						panelMeshExportOptionsDirectX.BringToFront();
						break;
					case MeshExportFormat.Collada:
						panelMeshExportOptionsCollada.BringToFront();
						break;
					case MeshExportFormat.Fbx:
					case MeshExportFormat.ColladaFbx:
					case MeshExportFormat.Dxf:
					case MeshExportFormat._3ds:
					case MeshExportFormat.Obj:
						panelMeshExportOptionsFbx.BringToFront();
						break;
					default:
						panelMeshExportOptionsDefault.BringToFront();
						break;
				}
			}
			catch (Exception ex)
			{
				Utility.ReportException(ex);
			}
		}
	}
}
