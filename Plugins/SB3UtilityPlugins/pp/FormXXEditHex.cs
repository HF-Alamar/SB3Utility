using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SB3Utility
{
	public partial class FormXXEditHex : Form
	{
		private DataTable xxTable = new DataTable();
		private DataTable frameTable = new DataTable();
		private DataTable meshTable = new DataTable();
		private DataTable submeshTable = new DataTable();
		private DataTable materialTable = new DataTable();
		private DataTable textureTable = new DataTable();

		private FormXX formXX = null;
		private xxEditor editor;
		private xxParser parser;
		private List<int[]> gotoCells;

		public FormXXEditHex(FormXX formXX, List<int[]> gotoCells)
		{
			InitializeComponent();
			this.formXX = formXX;
			this.editor = formXX.Editor;
			this.parser = editor.Parser;
			this.gotoCells = gotoCells;

			foreach (TabPage page in tabControl1.TabPages)
			{
				foreach (Control control in page.Controls)
				{
					if (control is DataGridViewEditor)
					{
						page.Tag = control;
						break;
					}
				}
			}

			xxTable.BeginLoadData();
			frameTable.BeginLoadData();
			meshTable.BeginLoadData();
			submeshTable.BeginLoadData();
			materialTable.BeginLoadData();
			textureTable.BeginLoadData();

			InitColumns();
			InitTables();

			xxTable.EndLoadData();
			xxTable.AcceptChanges();
			frameTable.EndLoadData();
			frameTable.AcceptChanges();
			meshTable.EndLoadData();
			meshTable.AcceptChanges();
			submeshTable.EndLoadData();
			submeshTable.AcceptChanges();
			materialTable.EndLoadData();
			materialTable.AcceptChanges();
			textureTable.EndLoadData();
			textureTable.AcceptChanges();

			this.KeyDown += new KeyEventHandler(FormXXEditHex_KeyDown);
			this.Load += new EventHandler(FormXXEditHex_Load);
		}

		void FormXXEditHex_Load(object sender, EventArgs e)
		{
			dataGridViewEditorXX.Initialize(xxTable, new DataGridViewEditor.ValidateCellDelegate(ValidateCellXX), 0);
			dataGridViewEditorFrame.Initialize(frameTable, new DataGridViewEditor.ValidateCellDelegate(ValidateCellFrame), 0);
			dataGridViewEditorMesh.Initialize(meshTable, new DataGridViewEditor.ValidateCellDelegate(ValidateCellMesh), 0);
			dataGridViewEditorSubmesh.Initialize(submeshTable, new DataGridViewEditor.ValidateCellDelegate(ValidateCellSubmesh), 0);
			dataGridViewEditorMaterial.Initialize(materialTable, new DataGridViewEditor.ValidateCellDelegate(ValidateCellMaterial), 0);
			dataGridViewEditorTexture.Initialize(textureTable, new DataGridViewEditor.ValidateCellDelegate(ValidateCellTexture), 0);

			DataGridView[] gridViews = new DataGridView[] { dataGridViewEditorXX, dataGridViewEditorFrame, dataGridViewEditorMesh, dataGridViewEditorSubmesh, dataGridViewEditorMaterial, dataGridViewEditorTexture };
			for (int i = 0; i < gridViews.Length; i++)
			{
				var gridView = gridViews[i];
				gridView.Columns[0].Visible = false;
				gridView.Columns[1].DefaultCellStyle = gridView.ColumnHeadersDefaultCellStyle;
			}

			if (gotoCells != null)
			{
				for (int i = 0; i < gotoCells.Count; i++)
				{
					int[] cellPath = gotoCells[i];
					int tabId = cellPath[0];
					int meshId = cellPath[1];
					try
					{
						tabControl1.SelectedIndex = tabId;
						var gridView = gridViews[tabId];
						gridView.CurrentCell = gridView.Rows[meshId].Cells[1];

						if (gridView == dataGridViewEditorMesh)
						{
							int submeshId = 0;
							for (int j = 0; j < meshId; j++)
							{
								submeshId += editor.Meshes[j].Mesh.SubmeshList.Count;
							}

							dataGridViewEditorSubmesh.CurrentCell = dataGridViewEditorSubmesh.Rows[submeshId].Cells[1];
						}
					}
					catch (Exception ex)
					{
						Utility.ReportException(ex);
					}
				}
			}
		}

		private void InitColumns()
		{
			xxTable.Columns.Add("id", typeof(int)); // 0
			xxTable.Columns.Add("Name", typeof(string)); // 1
			xxTable.Columns.Add("Unknown", typeof(string)); // 2

			frameTable.Columns.Add("id", typeof(int)); // 0
			frameTable.Columns.Add("Name", typeof(string)); // 1
			frameTable.Columns.Add("Frame Flags", typeof(string)); // 2 (Unknown1)
			frameTable.Columns.Add("Mesh Flags", typeof(string)); // 3 (Unknown2)

			meshTable.Columns.Add("id", typeof(int)); // 0
			meshTable.Columns.Add("Name", typeof(string)); // 1
			meshTable.Columns.Add("NumVector2PerVertex", typeof(string)); // 2
			meshTable.Columns.Add("VertexListDuplicate", typeof(string)); // 3

			submeshTable.Columns.Add("id", typeof(object)); // 0
			submeshTable.Columns.Add("Name", typeof(string)); // 1
			submeshTable.Columns.Add("Submesh Flags", typeof(string)); // 2 (Unknown1)
			submeshTable.Columns.Add("Unknown2", typeof(string)); // 3
			if (parser.Format < 7) { submeshTable.Columns[3].ReadOnly = true; }
			submeshTable.Columns.Add("Unknown3", typeof(string)); // 4
			if (parser.Format < 2) { submeshTable.Columns[4].ReadOnly = true; }
			submeshTable.Columns.Add("Unknown4", typeof(string)); // 5
			submeshTable.Columns.Add("Unknown5", typeof(string)); // 6
			submeshTable.Columns.Add("Unknown6", typeof(string)); // 7
			if (parser.Format != 6) { submeshTable.Columns[7].ReadOnly = true; }

			materialTable.Columns.Add("id", typeof(int)); // 0
			materialTable.Columns.Add("Name", typeof(string)); // 1
			materialTable.Columns.Add("Unknown", typeof(string)); // 2
			materialTable.Columns.Add("Texture1", typeof(string)); // 3
			materialTable.Columns.Add("Texture2", typeof(string)); // 4
			materialTable.Columns.Add("Texture3", typeof(string)); // 5
			materialTable.Columns.Add("Texture4", typeof(string)); // 6

			textureTable.Columns.Add("id", typeof(int)); // 0
			textureTable.Columns.Add("Name", typeof(string)); // 1
			textureTable.Columns.Add("Unknown", typeof(string)); // 2

			DataTable[] tables = new DataTable[] { xxTable, frameTable, meshTable, submeshTable, materialTable, textureTable };
			for (int i = 0; i < tables.Length; i++)
			{
				var table = tables[i];
				table.PrimaryKey = new DataColumn[] { table.Columns[0] };
				table.Columns[1].ReadOnly = true;
			}
		}

		private void InitTables()
		{
			xxTable.Rows.Add(new object[] { 0, "Header", Utility.BytesToString(parser.Header) });
			xxTable.Rows.Add(new object[] { 1, "MaterialSection", Utility.BytesToString(parser.MaterialSectionUnknown) });
			if (parser.Format >= 2)
			{
				xxTable.Rows.Add(new object[] { 2, "Footer", Utility.BytesToString(parser.Footer) }); // 3
			}

			for (int i = 0; i < editor.Frames.Count; i++)
			{
				xxFrame frame = editor.Frames[i];
				frameTable.Rows.Add(new object[] { i, frame.Name, Utility.BytesToString(frame.Unknown1), Utility.BytesToString(frame.Unknown2) });
			}

			for (int i = 0; i < editor.Meshes.Count; i++)
			{
				xxFrame frame = editor.Meshes[i];
				meshTable.Rows.Add(new object[] { i, frame.Name, frame.Mesh.NumVector2PerVertex.ToString("X2"), Utility.BytesToString(frame.Mesh.VertexListDuplicateUnknown) });

				for (int j = 0; j < frame.Mesh.SubmeshList.Count; j++)
				{
					xxSubmesh submesh = frame.Mesh.SubmeshList[j];
					submeshTable.Rows.Add(new object[] { new int[] { i, j }, frame.Name + "[" + j + "]",
						Utility.BytesToString(submesh.Unknown1),
						Utility.BytesToString(submesh.Unknown2),
						Utility.BytesToString(submesh.Unknown3),
						Utility.BytesToString(submesh.Unknown4),
						Utility.BytesToString(submesh.Unknown5),
						Utility.BytesToString(submesh.Unknown6) });
				}
			}

			for (int i = 0; i < parser.MaterialList.Count; i++)
			{
				xxMaterial mat = parser.MaterialList[i];
				materialTable.Rows.Add(new object[] { i, mat.Name,
					Utility.BytesToString(mat.Unknown1),
					Utility.BytesToString(mat.Textures[0].Unknown1),
					Utility.BytesToString(mat.Textures[1].Unknown1),
					Utility.BytesToString(mat.Textures[2].Unknown1),
					Utility.BytesToString(mat.Textures[3].Unknown1) });
			}

			for (int i = 0; i < parser.TextureList.Count; i++)
			{
				xxTexture tex = parser.TextureList[i];
				textureTable.Rows.Add(new object[] { i, tex.Name,
					Utility.BytesToString(tex.Unknown1) });
			}
		}

		private bool ValidateCell(string data, int expectedLen)
		{
			byte[] bytes = Utility.StringToBytes(data);
			int len = bytes.Length;
			if (len != expectedLen)
			{
				if (expectedLen == 1)
				{
					MessageBox.Show("There must be " + expectedLen + " byte.", "Error");
				}
				else
				{
					MessageBox.Show("There must be " + expectedLen + " bytes.", "Error");
				}
				return false;
			}
			return true;
		}

		private bool ValidateCellXX(string data, int rowIdx, int columnIdx)
		{
			bool valid = true;

			try
			{
				if (columnIdx == 2)
				{
					if (rowIdx == 0)
					{
						if (parser.Format >= 1)
						{
							valid = ValidateCell(data, 26);
						}
						else
						{
							valid = ValidateCell(data, 21);
						}
					}
					else if (rowIdx == 1)
					{
						valid = ValidateCell(data, 4);
					}
					else if (rowIdx == 2)
					{
						if (parser.Format >= 2)
						{
							valid = ValidateCell(data, 10);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error");
				valid = false;
			}

			return valid;
		}

		private bool ValidateCellFrame(string data, int rowIdx, int columnIdx)
		{
			bool valid = true;

			try
			{
				if (columnIdx == 2)
				{
					if (parser.Format >= 7)
					{
						valid = ValidateCell(data, 32);
					}
					else
					{
						valid = ValidateCell(data, 16);
					}
				}
				else if (columnIdx == 3)
				{
					byte[] bytes = Utility.StringToBytes(data);
					int len = bytes.Length;

					if (parser.Format >= 7)
					{
						valid = ValidateCell(data, 64);
					}
					else
					{
						valid = ValidateCell(data, 16);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error");
				valid = false;
			}

			return valid;
		}

		private bool ValidateCellMesh(string data, int rowIdx, int columnIdx)
		{
			bool valid = true;

			try
			{
				if (columnIdx == 2)
				{
					valid = ValidateCell(data, 1);
				}
				else if (columnIdx == 3)
				{
					valid = ValidateCell(data, 8);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error");
				valid = false;
			}

			return valid;
		}

		private bool ValidateCellSubmesh(string data, int rowIdx, int columnIdx)
		{
			bool valid = true;

			try
			{
				if (columnIdx == 2)
				{
					if (parser.Format >= 7)
					{
						valid = ValidateCell(data, 64);
					}
					else
					{
						valid = ValidateCell(data, 16);
					}
				}
				else if (columnIdx == 3)
				{
					if (parser.Format >= 7)
					{
						valid = ValidateCell(data, 20);
					}
				}
				else if (columnIdx == 4)
				{
					if (parser.Format >= 7)
					{
						valid = ValidateCell(data, 100);
					}
				}
				else if (columnIdx == 5)
				{
					if (parser.Format >= 7)
					{
						valid = ValidateCell(data, 284);
					}
					else if (parser.Format >= 3)
					{
						valid = ValidateCell(data, 64);
					}
				}
				else if (columnIdx == 6)
				{
					if (parser.Format >= 8)
					{
						byte[] bytes = Utility.StringToBytes(data);
						int nameLength = BitConverter.ToInt32(bytes, 1);
						valid = ValidateCell(data, 1 + 4 + nameLength + 12 + 4);
					}
					else if (parser.Format >= 5)
					{
						valid = ValidateCell(data, 20);
					}
				}
				else if (columnIdx == 7)
				{
					if (parser.Format == 6)
					{
						valid = ValidateCell(data, 28);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error");
				valid = false;
			}

			return valid;
		}

		private bool ValidateCellMaterial(string data, int rowIdx, int columnIdx)
		{
			bool valid = true;

			try
			{
				if (columnIdx == 2)
				{
					if (parser.Format >= 0)
					{
						valid = ValidateCell(data, 88);
					}
					else
					{
						valid = ValidateCell(data, 4);
					}
				}
				else if ((columnIdx == 3) || (columnIdx == 4) || (columnIdx == 5) || (columnIdx == 6))
				{
					valid = ValidateCell(data, 16);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error");
				valid = false;
			}

			return valid;
		}

		private bool ValidateCellTexture(string data, int rowIdx, int columnIdx)
		{
			bool valid = true;

			try
			{
				if (columnIdx == 2)
				{
					valid = ValidateCell(data, 4);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error");
				valid = false;
			}

			return valid;
		}

		private void FormXXEditHex_KeyDown(object sender, KeyEventArgs e)
		{
			DataGridViewEditor editor = (DataGridViewEditor)tabControl1.SelectedTab.Tag;
			editor.DataGridViewEditor_KeyDown(sender, e);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			var xxTableChanges = xxTable.GetChanges();
			if (xxTableChanges != null)
			{
				foreach (DataRow row in xxTableChanges.Rows)
				{
					int id = (int)row[0];
					if (id == 0)
					{
						ScriptHelper.SetProperty(formXX.ParserVar, "Header", row[2]);
					}
					else if (id == 1)
					{
						ScriptHelper.SetProperty(formXX.ParserVar, "MaterialSectionUnknown", row[2]);
					}
					else if (id == 2)
					{
						ScriptHelper.SetProperty(formXX.ParserVar, "Footer", row[2]);
					}
				}
			}

			var frameTableChanges = frameTable.GetChanges();
			if (frameTableChanges != null)
			{
				foreach (DataRow row in frameTableChanges.Rows)
				{
					Gui.Scripting.RunScript(formXX.EditorVar + ".SetFrameUnknowns(" +
						ScriptHelper.Parameters(new string[] {
							"id=" + row[0],
							ScriptHelper.Bytes("unknown1", row[2]),
							ScriptHelper.Bytes("unknown2", row[3]) }) + ")");
				}
			}

			var meshTableChanges = meshTable.GetChanges();
			if (meshTableChanges != null)
			{
				foreach (DataRow row in meshTableChanges.Rows)
				{
					Gui.Scripting.RunScript(formXX.EditorVar + ".SetMeshUnknowns(" +
						ScriptHelper.Parameters(new string[] {
							"id=" + row[0],
							ScriptHelper.Bytes("numVector2", row[2]),
							ScriptHelper.Bytes("vertListDup", row[3]) }) + ")");
				}
			}

			var submeshTableChanges = submeshTable.GetChanges();
			if (submeshTableChanges != null)
			{
				foreach (DataRow row in submeshTableChanges.Rows)
				{
					var id = (int[])row[0];
					Gui.Scripting.RunScript(formXX.EditorVar + ".SetSubmeshUnknowns(" + 
						ScriptHelper.Parameters(new string[] {
							"meshId=" + id[0],
							"submeshId=" + id[1],
							ScriptHelper.Bytes("unknown1", row[2]),
							ScriptHelper.Bytes("unknown2", row[3]),
							ScriptHelper.Bytes("unknown3", row[4]),
							ScriptHelper.Bytes("unknown4", row[5]),
							ScriptHelper.Bytes("unknown5", row[6]),
							ScriptHelper.Bytes("unknown6", row[7]) }) + ")");
				}
			}

			var materialTableChanges = materialTable.GetChanges();
			if (materialTableChanges != null)
			{
				foreach (DataRow row in materialTableChanges.Rows)
				{
					Gui.Scripting.RunScript(formXX.EditorVar + ".SetMaterialUnknowns(" +
						ScriptHelper.Parameters(new string[] {
							"id=" + row[0],
							ScriptHelper.Bytes("unknown1", row[2]),
							ScriptHelper.Bytes("tex1", row[3]),
							ScriptHelper.Bytes("tex2", row[4]),
							ScriptHelper.Bytes("tex3", row[5]),
							ScriptHelper.Bytes("tex4", row[6]) }) + ")");
				}
			}

			var textureTableChanges = textureTable.GetChanges();
			if (textureTableChanges != null)
			{
				foreach (DataRow row in textureTableChanges.Rows)
				{
					Gui.Scripting.RunScript(formXX.EditorVar + ".SetTextureUnknowns(" +
						ScriptHelper.Parameters(new string[] {
							"id=" + row[0],
							ScriptHelper.Bytes("unknown1", row[2]) }) + ")");
				}
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OnKeyDown(new KeyEventArgs(Keys.Control | Keys.C));
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OnKeyDown(new KeyEventArgs(Keys.Control | Keys.V));
		}
	}
}
