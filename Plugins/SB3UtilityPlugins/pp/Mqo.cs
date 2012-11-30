using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		[PluginOpensFile(".mqo")]
		public static void WorkspaceMqo(string path, string variable)
		{
			if (path.ToLower().EndsWith(".morph.mqo"))
				throw new Exception("Note! \"" + path + "\" is not opened as normal mqo.");

			string importVar = Gui.Scripting.GetNextVariable("importMqo");
			var importer = (Mqo.Importer)Gui.Scripting.RunScript(importVar + " = ImportMqo(\"" + path + "\")");

			string editorVar = Gui.Scripting.GetNextVariable("importedEditor");
			var editor = (ImportedEditor)Gui.Scripting.RunScript(editorVar + " = ImportedEditor(" + importVar + ")");

			new FormWorkspace(path, importer, editorVar, editor);
		}

		[Plugin]
		[PluginOpensFile(".morph.mqo")]
		public static void WorkspaceMorphMqo(string path, string variable)
		{
			string importVar = Gui.Scripting.GetNextVariable("importMorphMqo");
			var importer = (Mqo.ImporterMorph)Gui.Scripting.RunScript(importVar + " = ImportMorphMqo(\"" + path + "\")");

			string editorVar = Gui.Scripting.GetNextVariable("importedEditor");
			var editor = (ImportedEditor)Gui.Scripting.RunScript(editorVar + " = ImportedEditor(" + importVar + ")");

			new FormWorkspace(path, importer, editorVar, editor);
		}

		/// <summary>
		/// Exports the specified meshes to Metasequoia format.
		/// </summary>
		/// <param name="parser"><b>[DefaultVar]</b> The xxParser.</param>
		/// <param name="meshNames"><b>(string[])</b> The names of the meshes to export.</param>
		/// <param name="dirPath">The destination directory.</param>
		/// <param name="singleMqo"><b>True</b> will export all meshes in a single file. <b>False</b> will export a file per mesh.</param>
		/// <param name="worldCoords"><b>True</b> will transform vertices into world coordinates by multiplying them by their parent frames. <b>False</b> will keep their local coordinates.</param>
		[Plugin]
		public static void ExportMqo([DefaultVar]xxParser parser, object[] meshNames, string dirPath, bool singleMqo, bool worldCoords)
		{
			List<xxFrame> meshParents = xx.FindMeshFrames(parser.Frame, new List<string>(Utility.Convert<string>(meshNames)));
			Mqo.Exporter.Export(dirPath, parser, meshParents, singleMqo, worldCoords);
		}

		[Plugin]
		public static void ExportMorphMqo([DefaultVar]string dirPath, xxParser xxparser, xxFrame meshFrame, xaParser xaparser, xaMorphClip clip)
		{
			Mqo.ExporterMorph.Export(dirPath, xxparser, meshFrame, xaparser, clip);
		}

		[Plugin]
		public static Mqo.Importer ImportMqo([DefaultVar]string path)
		{
			return new Mqo.Importer(path);
		}

		[Plugin]
		public static Mqo.ImporterMorph ImportMorphMqo([DefaultVar]string path)
		{
			return new Mqo.ImporterMorph(path);
		}
	}

	public class Mqo
	{
		public class Importer : IImported
		{
			public List<ImportedFrame> FrameList { get; protected set; }
			public List<ImportedMesh> MeshList { get; protected set; }
			public List<ImportedMaterial> MaterialList { get; protected set; }
			public List<ImportedTexture> TextureList { get; protected set; }
			public List<ImportedAnimation> AnimationList { get; protected set; }
			public List<ImportedMorph> MorphList { get; protected set; }

			public Importer(string path)
			{
				try
				{
					List<string> mqoMaterials = new List<string>();
					List<MqoObject> mqoObjects = new List<MqoObject>();
					using (StreamReader reader = new StreamReader(path, Utility.EncodingShiftJIS))
					{
						string line;
						while ((line = reader.ReadLine()) != null)
						{
							if (line.Contains("Object"))
							{
								MqoObject mqoObject = ParseObject(line, reader);
								if (mqoObject != null)
								{
									mqoObjects.Add(mqoObject);
								}
							}
							else if (line.Contains("Material"))
							{
								string[] sArray = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
								int numMaterials = Int32.Parse(sArray[1]);
								while ((numMaterials > 0) && (line = reader.ReadLine()).Contains("\""))
								{
									int matNameStart = line.IndexOf('\"') + 1;
									int matNameEnd = line.IndexOf('\"', matNameStart);
									mqoMaterials.Add(line.Substring(matNameStart, matNameEnd - matNameStart));
									numMaterials--;
								}
							}
						}
					}

					List<List<MqoObject>> groupedMeshes = new List<List<MqoObject>>();
					for (int i = 0; i < mqoObjects.Count; i++)
					{
						bool found = false;
						for (int j = 0; j < groupedMeshes.Count; j++)
						{
							if (mqoObjects[i].name == groupedMeshes[j][0].name)
							{
								groupedMeshes[j].Add(mqoObjects[i]);
								found = true;
								break;
							}
						}
						if (!found)
						{
							List<MqoObject> group = new List<MqoObject>();
							group.Add(mqoObjects[i]);
							groupedMeshes.Add(group);
						}
					}

					MeshList = new List<ImportedMesh>(groupedMeshes.Count);
					for (int i = 0; i < groupedMeshes.Count; i++)
					{
						ImportedMesh meshList = ImportMeshList(groupedMeshes[i], mqoMaterials);
						MeshList.Add(meshList);
					}
				}
				catch (Exception e)
				{
					Report.ReportLog("Error importing .mqo: " + e.Message);
				}
			}

			private class MqoObject
			{
				public MqoVertex[] vertices = null;
				public MqoFace[] faces = null;
				public int baseIdx = -1;
				public bool worldCoords = false;
				public string name = null;
				public string fullname = null;
			}

			private class MqoVertex
			{
				public Vector3 coords;
			}

			private class MqoFace
			{
				public int materialIndex = -1;
				public int[] vertexIndices = new int[3];
				public float[][] UVs = new float[3][] { new float[2], new float[2], new float[2] };
			}

			private class VertexMap : IComparable<VertexMap>
			{
				public int mqoIdx = -1;
				public int wsMeshIdx = -1;
				public ImportedVertex vert = null;
				public Dictionary<float[], VertexMap> uvDic = new Dictionary<float[], VertexMap>(new FloatArrayComparer());

				public int CompareTo(VertexMap other)
				{
					return this.mqoIdx - other.mqoIdx;
				}
			}

			private class FloatArrayComparer : IEqualityComparer<float[]>
			{
				public bool Equals(float[] x, float[] y)
				{
					if (x.Length != y.Length)
					{
						return false;
					}

					for (int i = 0; i < x.Length; i++)
					{
						if (x[i] != y[i])
						{
							return false;
						}
					}

					return true;
				}

				public int GetHashCode(float[] obj)
				{
					int hash = 0;
					for (int i = 0; i < obj.Length; i++)
					{
						hash += obj[i].GetHashCode();
					}
					return hash;
				}
			}

			private static ImportedMesh ImportMeshList(List<MqoObject> mqoObjects, List<string> mqoMaterials)
			{
				ImportedMesh meshList = new ImportedMesh();
				meshList.Name = mqoObjects[0].name;
				meshList.BoneList = new List<ImportedBone>(0);
				meshList.SubmeshList = new List<ImportedSubmesh>(mqoObjects.Count);

				int vertIdx = 0;
				foreach (MqoObject mqoObject in mqoObjects)
				{
					List<VertexMap>[] vertexMapList = new List<VertexMap>[mqoMaterials.Count + 1];
					Dictionary<int, VertexMap>[] vertexMapDic = new Dictionary<int, VertexMap>[mqoMaterials.Count + 1];
					List<VertexMap[]>[] faceMap = new List<VertexMap[]>[mqoMaterials.Count + 1];
					foreach (MqoFace mqoFace in mqoObject.faces)
					{
						int mqoFaceMatIdxOffset = mqoFace.materialIndex + 1;
						if (vertexMapList[mqoFaceMatIdxOffset] == null)
						{
							vertexMapList[mqoFaceMatIdxOffset] = new List<VertexMap>(mqoObject.vertices.Length);
							vertexMapDic[mqoFaceMatIdxOffset] = new Dictionary<int, VertexMap>();
							faceMap[mqoFaceMatIdxOffset] = new List<VertexMap[]>(mqoObject.faces.Length);
						}

						VertexMap[] faceMapArray = new VertexMap[mqoFace.vertexIndices.Length];
						faceMap[mqoFaceMatIdxOffset].Add(faceMapArray);
						for (int i = 0; i < mqoFace.vertexIndices.Length; i++)
						{
							VertexMap vertMap;
							if (!vertexMapDic[mqoFaceMatIdxOffset].TryGetValue(mqoFace.vertexIndices[i], out vertMap))
							{
								ImportedVertex vert = new ImportedVertex();
								vert.BoneIndices = new byte[4];
								vert.Weights = new float[4];
								vert.Normal = new Vector3();
								vert.UV = mqoFace.UVs[i];
								vert.Position = mqoObject.vertices[mqoFace.vertexIndices[i]].coords;

								vertMap = new VertexMap { mqoIdx = mqoFace.vertexIndices[i], vert = vert };
								vertexMapDic[mqoFaceMatIdxOffset].Add(mqoFace.vertexIndices[i], vertMap);
								vertMap.uvDic.Add(mqoFace.UVs[i], vertMap);
								vertexMapList[mqoFaceMatIdxOffset].Add(vertMap);
							}

							VertexMap uvVertMap;
							if (!vertMap.uvDic.TryGetValue(mqoFace.UVs[i], out uvVertMap))
							{
								ImportedVertex vert = new ImportedVertex();
								vert.BoneIndices = new byte[4];
								vert.Weights = new float[4];
								vert.Normal = new Vector3();
								vert.UV = mqoFace.UVs[i];
								vert.Position = mqoObject.vertices[mqoFace.vertexIndices[i]].coords;

								uvVertMap = new VertexMap { mqoIdx = Int32.MaxValue, vert = vert };
								vertMap.uvDic.Add(mqoFace.UVs[i], uvVertMap);
								vertexMapList[mqoFaceMatIdxOffset].Add(uvVertMap);
							}

							faceMapArray[i] = uvVertMap;
						}
					}

					for (int i = 0; i < vertexMapList.Length; i++)
					{
						if (vertexMapList[i] != null)
						{
							ImportedSubmesh mesh = new ImportedSubmesh();
							mesh.VertexList = new List<ImportedVertex>(vertexMapList[i].Count);
							mesh.FaceList = new List<ImportedFace>(faceMap[i].Count);
							mesh.Index = mqoObject.baseIdx;
							mesh.WorldCoords = mqoObject.worldCoords;
							int matIdx = i - 1;
							if ((matIdx >= 0) && (matIdx < mqoMaterials.Count))
							{
								mesh.Material = mqoMaterials[matIdx];
							}
							meshList.SubmeshList.Add(mesh);

							vertexMapList[i].Sort();
							for (int j = 0; j < vertexMapList[i].Count; j++)
							{
								vertexMapList[i][j].wsMeshIdx = j;
								mesh.VertexList.Add(vertexMapList[i][j].vert);
								vertIdx++;
							}

							for (int j = 0; j < faceMap[i].Count; j++)
							{
								ImportedFace face = new ImportedFace();
								face.VertexIndices = new int[] { faceMap[i][j][0].wsMeshIdx, faceMap[i][j][2].wsMeshIdx, faceMap[i][j][1].wsMeshIdx };
								mesh.FaceList.Add(face);
							}
						}
					}
				}

				return meshList;
			}

			private static void ParseVertices(StreamReader reader, MqoObject mqoObject)
			{
				MqoVertex[] vertices = null;
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					int countStart = line.IndexOf("vertex");
					if (countStart >= 0)
					{
						countStart += 7;
						int countEnd = line.IndexOf(' ', countStart);
						int vertexCount = Int32.Parse(line.Substring(countStart, countEnd - countStart));
						vertices = new MqoVertex[vertexCount];

						for (int i = 0; i < vertexCount; i++)
						{
							MqoVertex vertex = new MqoVertex();
							line = reader.ReadLine();
							string[] sArray = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
							float[] coords = new float[] { Utility.ParseFloat(sArray[0]), Utility.ParseFloat(sArray[1]), Utility.ParseFloat(sArray[2]) };

							for (int j = 0; j < 3; j++)
							{
								coords[j] /= 10f;
								if (coords[j].Equals(Single.NaN))
								{
									throw new Exception("vertex " + i + " has invalid coordinates in mesh object " + mqoObject.fullname);
								}
							}
							vertex.coords = new Vector3(coords[0], coords[1], coords[2]);
							vertices[i] = vertex;
						}
						break;
					}
				}
				mqoObject.vertices = vertices;
			}

			private static void ParseFaces(StreamReader reader, MqoObject mqoObject)
			{
				List<MqoFace> faceList = null;
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					int countStart = line.IndexOf("face");
					if (countStart >= 0)
					{
						countStart += 5;
						int countEnd = line.IndexOf(' ', countStart);
						int faceCount = Int32.Parse(line.Substring(countStart, countEnd - countStart));
						faceList = new List<MqoFace>(faceCount);

						for (int i = 0; i < faceCount; i++)
						{
							// get vertex indices & uv
							line = reader.ReadLine();
							string[] sArray = line.Split(new char[] { '\t', ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
							int numVertices = Int32.Parse(sArray[0]);
							if (numVertices > 3)
							{
								throw new Exception("Face " + i + " in mesh object " + mqoObject.fullname + " has more than 3 vertices. Triangulate the meshes");
							}
							else if (numVertices < 3)
							{
								Report.ReportLog("Warning: Skipping face " + i + " in mesh object " + mqoObject.fullname + " because it has a less than 3 vertices");
							}
							else
							{
								MqoFace face = new MqoFace();
								faceList.Add(face);

								for (int j = 1; j < sArray.Length; j++)
								{
									if (sArray[j].ToUpper() == "V")
									{
										for (int k = 0; k < face.vertexIndices.Length; k++)
										{
											face.vertexIndices[k] = Int32.Parse(sArray[++j]);
										}
									}
									else if (sArray[j].ToUpper() == "M")
									{
										face.materialIndex = Int32.Parse(sArray[++j]);
									}
									else if (sArray[j].ToUpper() == "UV")
									{
										for (int k = 0; k < face.UVs.Length; k++)
										{
											face.UVs[k] = new float[2] { Utility.ParseFloat(sArray[++j]), Utility.ParseFloat(sArray[++j]) };
										}
									}
								}
							}
						}
						break;
					}
				}
				mqoObject.faces = faceList.ToArray();
			}

			private static MqoObject ParseObject(string line, StreamReader reader)
			{
				MqoObject mqoObject = new MqoObject();
				try
				{
					int nameStart = line.IndexOf('\"') + 1;
					int nameEnd = line.IndexOf('\"', nameStart);
					string name = line.Substring(nameStart, nameEnd - nameStart);
					mqoObject.fullname = name;

					if (name.Contains("[W]") || name.Contains("[w]"))
					{
						mqoObject.worldCoords = true;
						name = name.Replace("[W]", String.Empty);
						name = name.Replace("[w]", String.Empty);
					}

					int posStart;
					if ((posStart = name.LastIndexOf('[')) >= 0)
					{
						posStart++;
						int posEnd = name.LastIndexOf(']');
						int baseIdx;
						if ((posEnd > posStart) && Int32.TryParse(name.Substring(posStart, posEnd - posStart), out baseIdx))
						{
							mqoObject.baseIdx = baseIdx;
							name = name.Substring(0, posStart - 1);
						}
					}
					if ((mqoObject.baseIdx < 0) && ((posStart = name.LastIndexOf('-')) >= 0))
					{
						posStart++;
						int baseIdx;
						if (Int32.TryParse(name.Substring(posStart, name.Length - posStart), out baseIdx))
						{
							mqoObject.baseIdx = baseIdx;
							name = name.Substring(0, posStart - 1);
						}
					}
					mqoObject.name = name;

					ParseVertices(reader, mqoObject);
					ParseFaces(reader, mqoObject);
				}
				catch (Exception ex)
				{
					Report.ReportLog("Error parsing object " + mqoObject.fullname + ": " + ex.Message);
					mqoObject = null;
				}
				return mqoObject;
			}
		}

		public class ImporterMorph : IImported
		{
			public List<ImportedFrame> FrameList { get; protected set; }
			public List<ImportedMesh> MeshList { get; protected set; }
			public List<ImportedMaterial> MaterialList { get; protected set; }
			public List<ImportedTexture> TextureList { get; protected set; }
			public List<ImportedAnimation> AnimationList { get; protected set; }
			public List<ImportedMorph> MorphList { get; protected set; }

			public ImporterMorph(string path)
			{
				try
				{
					Importer importer = new Importer(path);
					MorphList = new List<ImportedMorph>();

					ImportedMorph morphList = new ImportedMorph();
					MorphList.Add(morphList);
					morphList.KeyframeList = new List<ImportedMorphKeyframe>(importer.MeshList.Count);
					foreach (ImportedMesh meshList in importer.MeshList)
					{
						foreach (ImportedSubmesh submesh in meshList.SubmeshList)
						{
							ImportedMorphKeyframe morph = new ImportedMorphKeyframe();
							morph.Name = meshList.Name;
							morph.VertexList = submesh.VertexList;
							morphList.KeyframeList.Add(morph);
						}
					}

					int startIdx = path.IndexOf('-') + 1;
					int endIdx = path.LastIndexOf('-');
					if (startIdx > endIdx)
					{
						int extIdx = path.ToLower().LastIndexOf(".morph.mqo");
						for (int i = extIdx - 1; i >= 0; i--)
						{
							if (!Char.IsDigit(path[i]))
							{
								endIdx = i + 1;
								break;
							}
						}
					}
					if ((startIdx > 0) && (endIdx > 0) && (startIdx < endIdx))
					{
						morphList.Name = path.Substring(startIdx, endIdx - startIdx);
					}
					if (morphList.Name == String.Empty)
					{
						morphList.Name = "(no name)";
					}
				}
				catch (Exception ex)
				{
					Report.ReportLog("Error importing .morphs.mqo: " + ex.Message);
				}
			}
		}

		public static class ExporterCommon
		{
			public static void WriteMeshObject(StreamWriter writer, List<ImportedVertex> vertexList, List<ImportedFace> faceList, int mqoMatIdx, bool[] colorVertex)
			{
				writer.WriteLine("\tvertex " + vertexList.Count + " {");
				for (int i = 0; i < vertexList.Count; i++)
				{
					ImportedVertex vertex = vertexList[i];
					Vector3 pos = vertex.Position * 10f;
					writer.WriteLine("\t\t" + pos.X.ToFloatString() + " " + pos.Y.ToFloatString() + " " + pos.Z.ToFloatString());
				}
				writer.WriteLine("\t}");

				writer.WriteLine("\tface " + faceList.Count + " {");
				for (int i = 0; i < faceList.Count; i++)
				{
					ImportedFace face = faceList[i];
					int[] vertIndices = new int[] { face.VertexIndices[0], face.VertexIndices[2], face.VertexIndices[1] };
					float[] uv1 = vertexList[vertIndices[0]].UV;
					float[] uv2 = vertexList[vertIndices[1]].UV;
					float[] uv3 = vertexList[vertIndices[2]].UV;

					writer.Write("\t\t3 V(" + vertIndices[0] + " " + vertIndices[1] + " " + vertIndices[2] + ")");
					if (mqoMatIdx >= 0)
					{
						writer.Write(" M(" + mqoMatIdx + ")");
					}
					writer.Write(" UV("
						+ uv1[0].ToFloatString() + " " + uv1[1].ToFloatString() + " "
						+ uv2[0].ToFloatString() + " " + uv2[1].ToFloatString() + " "
						+ uv3[0].ToFloatString() + " " + uv3[1].ToFloatString() + ")");
					if ((colorVertex != null) && (colorVertex[vertIndices[0]] || colorVertex[vertIndices[1]] || colorVertex[vertIndices[2]]))
					{
						string s = " COL(";
						for (int j = 0; j < vertIndices.Length; j++)
						{
							if (colorVertex[vertIndices[j]])
							{
								s += 0xFFFF0000 + " ";
							}
							else
							{
								s += 0xFF000000 + " ";
							}
						}
						s = s.Substring(0, s.Length - 1) + ")";
						writer.Write(s);
					}
					writer.WriteLine();
				}
				writer.WriteLine("\t}");
			}
		}

		public class Exporter
		{
			public static void Export(string dirPath, xxParser parser, List<xxFrame> meshParents, bool singleMqo, bool worldCoords)
			{
				DirectoryInfo dir = new DirectoryInfo(dirPath);
				List<xxTexture> usedTextures = new List<xxTexture>(parser.TextureList.Count);
				if (singleMqo)
				{
					try
					{
						string dest = Utility.GetDestFile(dir, "meshes", ".mqo");
						List<xxTexture> texList = Export(dest, parser, meshParents, worldCoords);
						foreach (xxTexture tex in texList)
						{
							if (!usedTextures.Contains(tex))
							{
								usedTextures.Add(tex);
							}
						}
						Report.ReportLog("Finished exporting meshes to " + dest);
					}
					catch (Exception ex)
					{
						Report.ReportLog("Error exporting meshes: " + ex.Message);
					}
				}
				else
				{
					for (int i = 0; i < meshParents.Count; i++)
					{
						try
						{
							string frameName = meshParents[i].Name;
							string dest = dir.FullName + @"\" + frameName + ".mqo";
							List<xxTexture> texList = Export(dest, parser, new List<xxFrame> { meshParents[i] }, worldCoords);
							foreach (xxTexture tex in texList)
							{
								if (!usedTextures.Contains(tex))
								{
									usedTextures.Add(tex);
								}
							}
							Report.ReportLog("Finished exporting mesh to " + dest);
						}
						catch (Exception ex)
						{
							Report.ReportLog("Error exporting mesh: " + ex.Message);
						}
					}
				}

				foreach (xxTexture tex in usedTextures)
				{
					xx.ExportTexture(tex, dir.FullName + @"\" + Path.GetFileName(tex.Name));
				}
			}

			private static List<xxTexture> Export(string dest, xxParser parser, List<xxFrame> meshParents, bool worldCoords)
			{
				List<xxTexture> usedTextures = new List<xxTexture>(parser.TextureList.Count);
				DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(dest));
				if (!dir.Exists)
				{
					dir.Create();
				}

				List<int> materialList = new List<int>(parser.MaterialList.Count);
				using (StreamWriter writer = new StreamWriter(dest, false))
				{
					for (int i = 0; i < meshParents.Count; i++)
					{
						xxMesh meshListSome = meshParents[i].Mesh;
						for (int j = 0; j < meshListSome.SubmeshList.Count; j++)
						{
							xxSubmesh meshObj = meshListSome.SubmeshList[j];
							int meshObjMatIdx = meshObj.MaterialIndex;
							if ((meshObjMatIdx >= 0) && (meshObjMatIdx < parser.MaterialList.Count))
							{
								if (!materialList.Contains(meshObjMatIdx))
								{
									materialList.Add(meshObjMatIdx);
								}
							}
							else
							{
								Report.ReportLog("Warning: Mesh " + meshParents[i].Name + " Object " + j + " has an invalid material");
							}
						}
					}

					writer.WriteLine("Metasequoia Document");
					writer.WriteLine("Format Text Ver 1.0");
					writer.WriteLine();
					writer.WriteLine("Material " + materialList.Count + " {");
					foreach (int matIdx in materialList)
					{
						xxMaterial mat = parser.MaterialList[matIdx];
						string s = "\t\"" + mat.Name + "\" col(0.800 0.800 0.800 1.000) dif(0.500) amb(0.100) emi(0.500) spc(0.100) power(30.00)";
						string matTexName = mat.Textures[0].Name;
						if (matTexName != String.Empty)
						{
							s += " tex(\"" + Path.GetFileName(matTexName) + "\")";
						}
						writer.WriteLine(s);
					}
					writer.WriteLine("}");

					Random rand = new Random();
					for (int i = 0; i < meshParents.Count; i++)
					{
						Matrix transform = Matrix.Identity;
						if (worldCoords)
						{
							xxFrame parent = meshParents[i];
							while (parent != null)
							{
								transform = parent.Matrix * transform;
								parent = (xxFrame)parent.Parent;
							}
						}

						string meshName = meshParents[i].Name;
						xxMesh meshListSome = meshParents[i].Mesh;
						for (int j = 0; j < meshListSome.SubmeshList.Count; j++)
						{
							xxSubmesh meshObj = meshListSome.SubmeshList[j];
							int meshObjMatIdx = meshObj.MaterialIndex;
							int mqoMatIdx = -1;
							if ((meshObjMatIdx >= 0) && (meshObjMatIdx < parser.MaterialList.Count))
							{
								mqoMatIdx = materialList.IndexOf(meshObjMatIdx);
							}
							float[] color = new float[3];
							for (int k = 0; k < color.Length; k++)
							{
								color[k] = (float)((rand.NextDouble() / 2) + 0.5);
							}

							string mqoName = meshName + "[" + j + "]";
							if (worldCoords)
							{
								mqoName += "[W]";
							}
							writer.WriteLine("Object \"" + mqoName + "\" {");
							writer.WriteLine("\tshading 1");
							writer.WriteLine("\tcolor " + color[0].ToFloatString() + " " + color[1].ToFloatString() + " " + color[2].ToFloatString());
							writer.WriteLine("\tcolor_type 1");

							List<ImportedVertex> vertList = xx.ImportedVertexList(meshObj.VertexList, xx.IsSkinned(meshListSome));
							List<ImportedFace> faceList = xx.ImportedFaceList(meshObj.FaceList);
							if (worldCoords)
							{
								for (int k = 0; k < vertList.Count; k++)
								{
									vertList[k].Position = Vector3.TransformCoordinate(vertList[k].Position, transform);
								}
							}

							ExporterCommon.WriteMeshObject(writer, vertList, faceList, mqoMatIdx, null);
							writer.WriteLine("}");
						}
					}
					writer.WriteLine("Eof");
				}

				foreach (int matIdx in materialList)
				{
					xxMaterial mat = parser.MaterialList[matIdx];
					xxMaterialTexture matTex = mat.Textures[0];
					string matTexName = matTex.Name;
					if (matTexName != String.Empty)
					{
						for (int i = 0; i < parser.TextureList.Count; i++)
						{
							xxTexture tex = parser.TextureList[i];
							string texName = tex.Name;
							if ((texName == matTexName) && !usedTextures.Contains(tex))
							{
								usedTextures.Add(tex);
								break;
							}
						}
					}
				}
				return usedTextures;
			}
		}

		public class ExporterMorph
		{
			private xxParser xxParser = null;
			private xaParser xaParser = null;
			private xaMorphClip clip = null;

			private bool[] colorVertex = null;
			private List<string> morphNames = null;

			List<List<ImportedVertex>> vertLists;
			List<ImportedFace> faceList;
			List<xxTexture> usedTextures;

			public static void Export(string dirPath, xxParser xxParser, xxFrame meshFrame, xaParser xaParser, xaMorphClip clip)
			{
				DirectoryInfo dir = new DirectoryInfo(dirPath);
				ExporterMorph exporter = new ExporterMorph(dir, xxParser, xaParser, clip);
				exporter.Export(dir, meshFrame);
			}

			private ExporterMorph(DirectoryInfo dir, xxParser xxParser, xaParser xaParser, xaMorphClip clip)
			{
				this.xxParser = xxParser;
				this.xaParser = xaParser;
				this.clip = clip;
			}

			private void Export(DirectoryInfo dir, xxFrame meshFrame)
			{
				try
				{
					xaMorphSection morphSection = xaParser.MorphSection;
					xaMorphIndexSet indexSet = xa.FindMorphIndexSet(clip.Name, morphSection);
					ushort[] meshIndices = indexSet.MeshIndices;
					ushort[] morphIndices = indexSet.MorphIndices;

					xxMesh meshList = meshFrame.Mesh;
					int meshObjIdx = xa.MorphMeshObjIdx(meshIndices, meshList);
					if (meshObjIdx < 0)
					{
						throw new Exception("no valid mesh object was found for the morph");
					}

					xxSubmesh meshObjBase = meshList.SubmeshList[meshObjIdx];
					colorVertex = new bool[meshObjBase.VertexList.Count];
					for (int i = 0; i < meshIndices.Length; i++)
					{
						colorVertex[meshIndices[i]] = true;
					}

					string dest = Utility.GetDestFile(dir, meshFrame.Name + "-" + clip.Name + "-", ".morph.mqo");

					List<xaMorphKeyframeRef> refList = clip.KeyframeRefList;
					morphNames = new List<string>(refList.Count);
					vertLists = new List<List<ImportedVertex>>(refList.Count);
					for (int i = 0; i < refList.Count; i++)
					{
						if (!morphNames.Contains(refList[i].Name))
						{
							List<ImportedVertex> vertList = xx.ImportedVertexList(meshObjBase.VertexList, xx.IsSkinned(meshList));
							vertLists.Add(vertList);

							xaMorphKeyframe keyframe = xa.FindMorphKeyFrame(refList[i].Name, morphSection);
							for (int j = 0; j < meshIndices.Length; j++)
							{
								ImportedVertex vert = vertList[meshIndices[j]];
								vert.Position = keyframe.PositionList[morphIndices[j]];
							}
							morphNames.Add(keyframe.Name);
						}
					}

					faceList = xx.ImportedFaceList(meshObjBase.FaceList);
					Export(dest, meshObjBase.MaterialIndex);
					foreach (xxTexture tex in usedTextures)
					{
						xx.ExportTexture(tex, dir.FullName + @"\" + Path.GetFileName(tex.Name));
					}
					Report.ReportLog("Finished exporting morph to " + dest);
				}
				catch (Exception ex)
				{
					Report.ReportLog("Error exporting morph: " + ex.Message);
				}
			}

			private void Export(string dest, int matIdx)
			{
				DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(dest));
				if (!dir.Exists)
				{
					dir.Create();
				}

				usedTextures = new List<xxTexture>(xxParser.TextureList.Count);
				using (StreamWriter writer = new StreamWriter(dest, false))
				{
					writer.WriteLine("Metasequoia Document");
					writer.WriteLine("Format Text Ver 1.0");
					writer.WriteLine();

					if ((matIdx >= 0) && (matIdx < xxParser.MaterialList.Count))
					{
						writer.WriteLine("Material 1 {");
						xxMaterial mat = xxParser.MaterialList[matIdx];
						string s = "\t\"" + mat.Name + "\" vcol(1) col(0.800 0.800 0.800 1.000) dif(0.500) amb(0.100) emi(0.500) spc(0.100) power(30.00)";
						string matTexName = mat.Textures[0].Name;
						if (matTexName != String.Empty)
						{
							s += " tex(\"" + Path.GetFileName(matTexName) + "\")";

							for (int i = 0; i < xxParser.TextureList.Count; i++)
							{
								xxTexture tex = xxParser.TextureList[i];
								if (tex.Name == matTexName)
								{
									usedTextures.Add(tex);
									break;
								}
							}
						}
						writer.WriteLine(s);
						writer.WriteLine("}");

						matIdx = 0;
					}
					else
					{
						matIdx = -1;
					}

					Random rand = new Random();
					for (int i = 0; i < vertLists.Count; i++)
					{
						float[] color = new float[3];
						for (int k = 0; k < color.Length; k++)
						{
							color[k] = (float)((rand.NextDouble() / 2) + 0.5);
						}

						writer.WriteLine("Object \"" + morphNames[i] + "\" {");
						writer.WriteLine("\tvisible 0");
						writer.WriteLine("\tshading 1");
						writer.WriteLine("\tcolor " + color[0].ToFloatString() + " " + color[1].ToFloatString() + " " + color[2].ToFloatString());
						writer.WriteLine("\tcolor_type 1");
						ExporterCommon.WriteMeshObject(writer, vertLists[i], faceList, matIdx, colorVertex);
						writer.WriteLine("}");
					}

					writer.WriteLine("Eof");
				}
			}
		}
	}
}
