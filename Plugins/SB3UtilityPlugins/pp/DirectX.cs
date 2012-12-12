using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SlimDX;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		[PluginOpensFile(".x")]
		public static void WorkspaceDirectX(string path, string variable)
		{
			string importVar = Gui.Scripting.GetNextVariable("importDirectX");
			var importer = (DirectX.Importer)Gui.Scripting.RunScript(importVar + " = ImportDirectX(\"" + path + "\")");

			string editorVar = Gui.Scripting.GetNextVariable("importedEditor");
			var editor = (ImportedEditor)Gui.Scripting.RunScript(editorVar + " = ImportedEditor(" + importVar + ")");

			new FormWorkspace(path, importer, editorVar, editor);
		}

		[Plugin]
		public static void ExportDirectX(string path, xxParser xxParser, object[] meshNames, object[] xaParsers, int ticksPerSecond, int keyframeLength)
		{
			List<xaParser> xaParserList = null;
			if (xaParsers != null)
			{
				xaParserList = new List<xaParser>(Utility.Convert<xaParser>(xaParsers));
			}

			List<xxFrame> meshParents = xx.FindMeshFrames(xxParser.Frame, new List<string>(Utility.Convert<string>(meshNames)));
			DirectX.Exporter.Export(path, xxParser, meshParents.ToArray(), xaParserList, ticksPerSecond, keyframeLength);
		}

		[Plugin]
		public static DirectX.Importer ImportDirectX([DefaultVar]string path)
		{
			return new DirectX.Importer(path);
		}
	}

	public class DirectX
	{
		private const ushort TOKEN_NAME = 1;
		private const ushort TOKEN_STRING = 2;
		private const ushort TOKEN_INTEGER = 3;
		private const ushort TOKEN_GUID = 5;
		private const ushort TOKEN_INTEGER_LIST = 6;
		private const ushort TOKEN_REALNUM_LIST = 7;

		private const ushort TOKEN_OBRACE = 10;
		private const ushort TOKEN_CBRACE = 11;
		private const ushort TOKEN_OPAREN = 12;
		private const ushort TOKEN_CPAREN = 13;
		private const ushort TOKEN_OBRACKET = 14;
		private const ushort TOKEN_CBRACKET = 15;
		private const ushort TOKEN_OANGLE = 16;
		private const ushort TOKEN_CANGLE = 17;
		private const ushort TOKEN_DOT = 18;
		private const ushort TOKEN_COMMA = 19;
		private const ushort TOKEN_SEMICOLON = 20;
		private const ushort TOKEN_TEMPLATE = 31;
		private const ushort TOKEN_WORD = 40;
		private const ushort TOKEN_DWORD = 41;
		private const ushort TOKEN_FLOAT = 42;
		private const ushort TOKEN_DOUBLE = 43;
		private const ushort TOKEN_CHAR = 44;
		private const ushort TOKEN_UCHAR = 45;
		private const ushort TOKEN_SWORD = 46;
		private const ushort TOKEN_SDWORD = 47;
		private const ushort TOKEN_VOID = 48;
		private const ushort TOKEN_LPSTR = 49;
		private const ushort TOKEN_UNICODE = 50;
		private const ushort TOKEN_CSTRING = 51;
		private const ushort TOKEN_ARRAY = 52;

		private static Matrix RHToLHMatrix(Matrix matrix)
		{
			matrix[0, 2] = -matrix[0, 2];
			matrix[1, 2] = -matrix[1, 2];
			matrix[2, 0] = -matrix[2, 0];
			matrix[2, 1] = -matrix[2, 1];
			matrix[3, 2] = -matrix[3, 2];
			return matrix;
		}

		private struct VertexBoneAssignment
		{
			public int vertexIndex;
			public float weight;
		}

		public class Exporter
		{
			private StreamWriter writer;
			private xxParser xxParser;
			private Dictionary<string, xxTexture> usedTextures = new Dictionary<string, xxTexture>();
			private HashSet<string> frameNames = new HashSet<string>();

			public static void Export(string dest, xxParser parser, xxFrame[] meshParents, List<xaParser> xaSubfiles, int ticksPerSecond, int keyframeLength)
			{
				HashSet<string> meshNames = new HashSet<string>();
				foreach (xxFrame frame in meshParents)
				{
					meshNames.Add(frame.Name);
				}
				Exporter exporter = new Exporter(dest, parser, meshNames, xaSubfiles, ticksPerSecond, keyframeLength);
			}

			public Exporter(string dest, xxParser parser, HashSet<string> meshNames, List<xaParser> xaParsers, int ticksPerSecond, int keyframeLength)
			{
				try
				{
					DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(dest));
					if (!dir.Exists)
					{
						dir.Create();
					}

					writer = new StreamWriter(dest, false);
					if (parser != null)
					{
						WriteXHeader(writer);
						this.xxParser = parser;
						ExportFrame(parser.Frame, meshNames, 0);
						foreach (var pair in usedTextures)
						{
							xxTexture tex = pair.Value;
							xx.ExportTexture(tex, dir.FullName + @"\" + Path.GetFileName(tex.Name));
						}
					}
					if (xaParsers != null && xaParsers.Count > 0)
					{
						writer.WriteLine("AnimTicksPerSecond {");
						writer.WriteLine(" " + ticksPerSecond + ";");
						writer.WriteLine("}");
						writer.WriteLine();

						SortedDictionary<string, int> animationNameDic = new SortedDictionary<string, int>();
						foreach (xaParser xaParser in xaParsers)
						{
							List<xaAnimationTrack> animationList = xaParser.AnimationSection.TrackList;
							string animationName = xaParser.Name.Replace('.', '_');
							int animationNameCount;
							if (animationNameDic.TryGetValue(xaParser.Name, out animationNameCount))
							{
								animationNameCount++;
								animationNameDic[xaParser.Name] = animationNameCount;
								ExportAnimation(animationList, ticksPerSecond, keyframeLength, animationName + animationNameCount);
							}
							else
							{
								animationNameDic.Add(xaParser.Name, 1);
								ExportAnimation(animationList, ticksPerSecond, keyframeLength, animationName);
							}
						}
					}
					writer.Close();

					Report.ReportLog("Finished exporting to " + dest);
				}
				catch (Exception ex)
				{
					Utility.ReportException(ex);
				}
			}

			private void ExportFrame(xxFrame frame, HashSet<string> meshNames, int indent)
			{
				frameNames.Add(frame.Name);
				Matrix matrix = RHToLHMatrix(frame.Matrix);
				string spaces = GetStringSpaces(indent);
				writer.WriteLine(spaces + "Frame " + frame.Name + " {");
				writer.WriteLine(spaces + " FrameTransformMatrix {");
				writer.WriteLine(spaces + "  " + GetStringMatrix(matrix) + ";;");
				writer.WriteLine(spaces + " }");
				writer.WriteLine();

				if (meshNames.Contains(frame.Name))
				{
					ExportMesh(frame, indent + 1);
				}

				for (int i = 0; i < frame.Count; i++)
				{
					ExportFrame(frame[i], meshNames, indent + 1);
				}

				writer.WriteLine(GetStringSpaces(indent) + "}");
				writer.WriteLine();
			}

			private void ExportMesh(xxFrame meshParent, int indent)
			{
				string meshSpaces = GetStringSpaces(indent);
				xxMesh mesh = meshParent.Mesh;
				List<xxBone> boneList = mesh.BoneList;
				bool skinned = (boneList.Count > 0);
				for (int i = 0; i < mesh.SubmeshList.Count; i++)
				{
					List<VertexBoneAssignment>[] boneAssignments = null;
					if (skinned)
					{
						boneAssignments = new List<VertexBoneAssignment>[256];
						for (int j = 0; j < boneAssignments.Length; j++)
						{
							boneAssignments[j] = new List<VertexBoneAssignment>();
						}
					}

					writer.WriteLine(meshSpaces + "Mesh " + meshParent.Name + i + " {");
					xxSubmesh submesh = mesh.SubmeshList[i];
					List<xxFace> faceList = submesh.FaceList;
					List<xxVertex> vertexList = submesh.VertexList;
					writer.WriteLine(meshSpaces + " " + vertexList.Count + ";");
					for (int j = 0; j < vertexList.Count; j++)
					{
						string s = meshSpaces + " ";
						xxVertex vertex = vertexList[j];
						Vector3 coords = vertex.Position;
						coords[2] = -coords[2];

						for (int k = 0; k < 3; k++)
						{
							s += coords[k].ToFloat6String() + ";";
						}
						if (j < (vertexList.Count - 1))
						{
							s += ",";
						}
						else
						{
							s += ";";
						}
						writer.WriteLine(s);

						if (skinned)
						{
							float[] weights4 = vertex.Weights4(skinned);
							byte[] boneIndices = vertex.BoneIndices;
							for (int k = 0; k < boneIndices.Length; k++)
							{
								if ((boneIndices[k] != 0xFF) && (weights4[k] > 0))
								{
									VertexBoneAssignment vba = new VertexBoneAssignment();
									vba.vertexIndex = j;
									vba.weight = weights4[k];
									boneAssignments[boneIndices[k]].Add(vba);
								}
							}
						}
					}
					writer.WriteLine(meshSpaces + " " + faceList.Count + ";");
					for (int j = 0; j < faceList.Count; j++)
					{
						string s = meshSpaces + " 3;";
						xxFace face = faceList[j];
						s += face.VertexIndices[0].ToString() + ",";
						s += face.VertexIndices[2].ToString() + ",";
						s += face.VertexIndices[1].ToString() + ";";
						if (j < (faceList.Count - 1))
						{
							s += ",";
						}
						else
						{
							s += ";";
						}
						writer.WriteLine(s);
					}
					writer.WriteLine();

					writer.WriteLine(meshSpaces + " MeshNormals {");
					writer.WriteLine(meshSpaces + "  " + vertexList.Count + ";");
					for (int j = 0; j < vertexList.Count; j++)
					{
						xxVertex vertex = vertexList[j];
						Vector3 normal = vertex.Normal;
						writer.Write(meshSpaces + "  " + normal[0].ToFloat6String() + ";" + normal[1].ToFloat6String() + ";" + (-normal[2]).ToFloat6String() + ";");
						if (j < (vertexList.Count - 1))
						{
							writer.WriteLine(",");
						}
						else
						{
							writer.WriteLine(";");
						}
					}
					writer.WriteLine(meshSpaces + "  " + faceList.Count + ";");
					for (int j = 0; j < faceList.Count; j++)
					{
						xxFace face = faceList[j];
						writer.Write(meshSpaces + "  3;" + face.VertexIndices[0] + "," + face.VertexIndices[2] + "," + face.VertexIndices[1] + ";");
						if (j < (faceList.Count - 1))
						{
							writer.WriteLine(",");
						}
						else
						{
							writer.WriteLine(";");
						}
					}
					writer.WriteLine(meshSpaces + " }");
					writer.WriteLine();

					writer.WriteLine(meshSpaces + " MeshTextureCoords {");
					writer.WriteLine(meshSpaces + "  " + vertexList.Count + ";");
					for (int j = 0; j < vertexList.Count; j++)
					{
						xxVertex vertex = vertexList[j];
						float[] uv = vertex.UV;
						writer.Write(meshSpaces + "  " + uv[0].ToFloat6String() + ";" + uv[1].ToFloat6String() + ";");
						if (j < (vertexList.Count - 1))
						{
							writer.WriteLine(",");
						}
						else
						{
							writer.WriteLine(";");
						}
					}
					writer.WriteLine(meshSpaces + " }");
					writer.WriteLine();

					int materialIdx = submesh.MaterialIndex;
					if ((materialIdx >= 0) && (materialIdx < xxParser.MaterialList.Count))
					{
						writer.WriteLine(meshSpaces + " MeshMaterialList {");
						writer.WriteLine(meshSpaces + "  1;");
						writer.WriteLine(meshSpaces + "  " + faceList.Count + ";");
						for (int j = 0; j < faceList.Count; j++)
						{
							writer.Write(meshSpaces + "  0");
							if (j < (faceList.Count - 1))
							{
								writer.WriteLine(",");
							}
							else
							{
								writer.WriteLine(";");
							}
						}

						xxMaterial mat = xxParser.MaterialList[materialIdx];
						Color4 ambient = mat.Ambient;
						Color4 specular = mat.Specular;
						Color4 emissive = mat.Emissive;
						writer.WriteLine();
						writer.WriteLine(meshSpaces + "  Material " + mat.Name + " {");
						writer.WriteLine(meshSpaces + "   " + ambient.Red.ToFloat6String() + ";" + ambient.Green.ToFloat6String() + ";" + ambient.Blue.ToFloat6String() + ";" + ambient.Alpha.ToFloat6String() + ";;");
						writer.WriteLine(meshSpaces + "   " + mat.Power.ToFloat6String() + ";");
						writer.WriteLine(meshSpaces + "   " + specular.Red.ToFloat6String() + ";" + specular.Green.ToFloat6String() + ";" + specular.Blue.ToFloat6String() + ";;");
						writer.WriteLine(meshSpaces + "   " + emissive.Red.ToFloat6String() + ";" + emissive.Green.ToFloat6String() + ";" + emissive.Blue.ToFloat6String() + ";;");
						xxMaterialTexture matTex = mat.Textures[0];
						if (matTex.Name != String.Empty)
						{
							writer.WriteLine();
							writer.WriteLine(meshSpaces + "   TextureFilename {");
							writer.WriteLine(meshSpaces + "    \"" + matTex.Name + "\";");
							writer.WriteLine(meshSpaces + "   }");

							for (int j = 0; j < xxParser.TextureList.Count; j++)
							{
								xxTexture tex = xxParser.TextureList[j];
								if (matTex.Name == tex.Name)
								{
									if (!usedTextures.ContainsKey(tex.Name))
									{
										usedTextures.Add(tex.Name, tex);
									}
									break;
								}
							}
						}
						writer.WriteLine(meshSpaces + "  }");
						writer.WriteLine(meshSpaces + " }" + Environment.NewLine);
					}
					else
					{
						Report.ReportLog("Warning: mesh " + meshParent.Name + " object " + i + " uses non-existant material index " + materialIdx);
					}

					if (skinned)
					{
						int numUsedBones = 0;
						for (int j = 0; j < boneList.Count; j++)
						{
							xxBone bone = boneList[j];
							if (boneAssignments[bone.Index].Count > 0)
							{
								numUsedBones++;
							}
						}

						writer.WriteLine(meshSpaces + " XSkinMeshHeader {");
						writer.WriteLine(meshSpaces + "  4;");
						writer.WriteLine(meshSpaces + "  12;");
						writer.WriteLine(meshSpaces + "  " + numUsedBones + ";");
						writer.WriteLine(meshSpaces + " }");
						writer.WriteLine();

						for (int j = 0; j < boneList.Count; j++)
						{
							xxBone bone = boneList[j];
							List<VertexBoneAssignment> boneAssignmentList = boneAssignments[bone.Index];
							if (boneAssignmentList.Count <= 0)
							{
								continue;
							}

							writer.WriteLine(meshSpaces + " SkinWeights {");
							writer.WriteLine(meshSpaces + "  \"" + bone.Name + "\";");
							writer.WriteLine(meshSpaces + "  " + boneAssignmentList.Count + ";");

							string vertexString = String.Empty;
							string weightString = String.Empty;
							for (int k = 0; k < boneAssignmentList.Count; k++)
							{
								vertexString += meshSpaces + "  " + boneAssignmentList[k].vertexIndex;
								weightString += meshSpaces + "  " + boneAssignmentList[k].weight.ToFloat6String();
								if (k < (boneAssignmentList.Count - 1))
								{
									vertexString += "," + Environment.NewLine;
									weightString += "," + Environment.NewLine;
								}
								else
								{
									vertexString += ";" + Environment.NewLine;
									weightString += ";" + Environment.NewLine;
								}
							}

							Matrix matrix = RHToLHMatrix(bone.Matrix);
							writer.Write(vertexString);
							writer.Write(weightString);
							writer.WriteLine(meshSpaces + "  " + GetStringMatrix(matrix) + ";;");
							writer.WriteLine(meshSpaces + " }");
							writer.WriteLine();
						}
					}

					writer.WriteLine(meshSpaces + "}");
					writer.WriteLine();
				}
			}

			private void ExportAnimation(List<xaAnimationTrack> animationNodeList, int ticksPerSecond, int keyframeLength, string baseName)
			{
				if (animationNodeList.Count > 0)
				{
					writer.WriteLine("AnimationSet " + baseName + " {");

					for (int i = 0; i < animationNodeList.Count; i++)
					{
						xaAnimationTrack animationNode = animationNodeList[i];
						List<xaAnimationKeyframe> keyframes = animationNode.KeyframeList;
						writer.WriteLine(" Animation " + baseName + "_" + i + " {");
						writer.WriteLine("  { " + animationNode.Name + " }");

						writer.WriteLine("  AnimationKey {");
						writer.WriteLine("   0;");
						writer.WriteLine("   " + keyframes.Count + ";");
						for (int j = 0; j < keyframes.Count; j++)
						{
							xaAnimationKeyframe frame = keyframes[j];
							writer.Write("   " + (frame.Index * keyframeLength) + ";4;" + frame.Rotation.W.ToFloat6String() + "," + frame.Rotation.X.ToFloat6String() + "," + frame.Rotation.Y.ToFloat6String() + "," + (-frame.Rotation.Z).ToFloat6String() + ";;");
							if (j < (keyframes.Count - 1))
							{
								writer.Write("," + Environment.NewLine);
							}
							else
							{
								writer.Write(";" + Environment.NewLine);
							}
						}
						writer.WriteLine("  }");
						writer.WriteLine();

						writer.WriteLine("  AnimationKey {");
						writer.WriteLine("   1;");
						writer.WriteLine("   " + keyframes.Count + ";");
						for (int j = 0; j < keyframes.Count; j++)
						{
							xaAnimationKeyframe frame = keyframes[j];
							writer.Write("   " + (frame.Index * keyframeLength) + ";3;" + frame.Scaling.X.ToFloat6String() + "," + frame.Scaling.Y.ToFloat6String() + "," + frame.Scaling.Z.ToFloat6String() + ";;");
							if (j < (keyframes.Count - 1))
							{
								writer.Write("," + Environment.NewLine);
							}
							else
							{
								writer.Write(";" + Environment.NewLine);
							}
						}
						writer.WriteLine("  }");
						writer.WriteLine();

						writer.WriteLine("  AnimationKey {");
						writer.WriteLine("   2;");
						writer.WriteLine("   " + keyframes.Count + ";");
						for (int j = 0; j < keyframes.Count; j++)
						{
							xaAnimationKeyframe frame = keyframes[j];
							writer.Write("   " + (frame.Index * keyframeLength) + ";3;" + frame.Translation.X.ToFloat6String() + "," + frame.Translation.Y.ToFloat6String() + "," + (-frame.Translation.Z).ToFloat6String() + ";;");
							if (j < (keyframes.Count - 1))
							{
								writer.Write("," + Environment.NewLine);
							}
							else
							{
								writer.Write(";" + Environment.NewLine);
							}
						}
						writer.WriteLine("  }");
						writer.WriteLine(" }");
						writer.WriteLine();
					}
					writer.WriteLine("}");
					writer.WriteLine();
				}
			}

			private static string GetStringMatrix(Matrix matrix)
			{
				string s = String.Empty;
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						s += matrix[i, j].ToFloat6String() + ",";
					}
				}
				return s.Substring(0, s.Length - 1);
			}

			private static string GetStringSpaces(int indent)
			{
				string s = String.Empty;
				for (int i = 0; i < indent; i++)
				{
					s += " ";
				}
				return s;
			}

			private void WriteXHeader(StreamWriter writer)
			{
				writer.WriteLine("xof 0303txt 0032");
				writer.WriteLine("template AnimTicksPerSecond {");
				writer.WriteLine(" <9e415a43-7ba6-4a73-8743-b73d47e88476>");
				writer.WriteLine(" DWORD AnimTicksPerSecond;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template AnimationOptions {");
				writer.WriteLine(" <e2bf56c0-840f-11cf-8f52-0040333594a3>");
				writer.WriteLine(" DWORD openclosed;");
				writer.WriteLine(" DWORD positionquality;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template FloatKeys {");
				writer.WriteLine(" <10dd46a9-775b-11cf-8f52-0040333594a3>");
				writer.WriteLine(" DWORD nValues;");
				writer.WriteLine(" array FLOAT values[nValues];");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template TimedFloatKeys {");
				writer.WriteLine(" <f406b180-7b3b-11cf-8f52-0040333594a3>");
				writer.WriteLine(" DWORD time;");
				writer.WriteLine(" FloatKeys tfkeys;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template AnimationKey {");
				writer.WriteLine(" <10dd46a8-775b-11cf-8f52-0040333594a3>");
				writer.WriteLine(" DWORD keyType;");
				writer.WriteLine(" DWORD nKeys;");
				writer.WriteLine(" array TimedFloatKeys keys[nKeys];");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template Animation {");
				writer.WriteLine(" <3d82ab4f-62da-11cf-ab39-0020af71e433>");
				writer.WriteLine(" [...]");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template AnimationSet {");
				writer.WriteLine(" <3d82ab50-62da-11cf-ab39-0020af71e433>");
				writer.WriteLine(" [Animation]");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template TextureFilename {");
				writer.WriteLine(" <a42790e1-7810-11cf-8f52-0040333594a3>");
				writer.WriteLine(" STRING filename;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template ColorRGBA {");
				writer.WriteLine(" <35ff44e0-6c7c-11cf-8f52-0040333594a3>");
				writer.WriteLine(" FLOAT red;");
				writer.WriteLine(" FLOAT green;");
				writer.WriteLine(" FLOAT blue;");
				writer.WriteLine(" FLOAT alpha;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template ColorRGB {");
				writer.WriteLine(" <d3e16e81-7835-11cf-8f52-0040333594a3>");
				writer.WriteLine(" FLOAT red;");
				writer.WriteLine(" FLOAT green;");
				writer.WriteLine(" FLOAT blue;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template Material {");
				writer.WriteLine(" <3d82ab4d-62da-11cf-ab39-0020af71e433>");
				writer.WriteLine(" ColorRGBA faceColor;");
				writer.WriteLine(" FLOAT power;");
				writer.WriteLine(" ColorRGB specularColor;");
				writer.WriteLine(" ColorRGB emissiveColor;");
				writer.WriteLine(" [...]");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template MeshMaterialList {");
				writer.WriteLine(" <f6f23f42-7686-11cf-8f52-0040333594a3>");
				writer.WriteLine(" DWORD nMaterials;");
				writer.WriteLine(" DWORD nFaceIndexes;");
				writer.WriteLine(" array DWORD faceIndexes[nFaceIndexes];");
				writer.WriteLine(" [Material]");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template Matrix4x4 {");
				writer.WriteLine(" <f6f23f45-7686-11cf-8f52-0040333594a3>");
				writer.WriteLine(" array FLOAT matrix[16];");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template SkinWeights {");
				writer.WriteLine(" <6f0d123b-bad2-4167-a0d0-80224f25fabb>");
				writer.WriteLine(" STRING transformNodeName;");
				writer.WriteLine(" DWORD nWeights;");
				writer.WriteLine(" array DWORD vertexIndices[nWeights];");
				writer.WriteLine(" array FLOAT weights[nWeights];");
				writer.WriteLine(" Matrix4x4 matrixOffset;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template XSkinMeshHeader {");
				writer.WriteLine(" <3cf169ce-ff7c-44ab-93c0-f78f62d172e2>");
				writer.WriteLine(" WORD nMaxSkinWeightsPerVertex;");
				writer.WriteLine(" WORD nMaxSkinWeightsPerFace;");
				writer.WriteLine(" WORD nBones;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template Coords2d {");
				writer.WriteLine(" <f6f23f44-7686-11cf-8f52-0040333594a3>");
				writer.WriteLine(" FLOAT u;");
				writer.WriteLine(" FLOAT v;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template MeshTextureCoords {");
				writer.WriteLine(" <f6f23f40-7686-11cf-8f52-0040333594a3>");
				writer.WriteLine(" DWORD nTextureCoords;");
				writer.WriteLine(" array Coords2d textureCoords[nTextureCoords];");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template MeshFace {");
				writer.WriteLine(" <3d82ab5f-62da-11cf-ab39-0020af71e433>");
				writer.WriteLine(" DWORD nFaceVertexIndices;");
				writer.WriteLine(" array DWORD faceVertexIndices[nFaceVertexIndices];");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template Vector {");
				writer.WriteLine(" <3d82ab5e-62da-11cf-ab39-0020af71e433>");
				writer.WriteLine(" FLOAT x;");
				writer.WriteLine(" FLOAT y;");
				writer.WriteLine(" FLOAT z;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template MeshNormals {");
				writer.WriteLine(" <f6f23f43-7686-11cf-8f52-0040333594a3>");
				writer.WriteLine(" DWORD nNormals;");
				writer.WriteLine(" array Vector normals[nNormals];");
				writer.WriteLine(" DWORD nFaceNormals;");
				writer.WriteLine(" array MeshFace faceNormals[nFaceNormals];");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template Mesh {");
				writer.WriteLine(" <3d82ab44-62da-11cf-ab39-0020af71e433>");
				writer.WriteLine(" DWORD nVertices;");
				writer.WriteLine(" array Vector vertices[nVertices];");
				writer.WriteLine(" DWORD nFaces;");
				writer.WriteLine(" array MeshFace faces[nFaces];");
				writer.WriteLine(" [...]");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template FrameTransformMatrix {");
				writer.WriteLine(" <f6f23f41-7686-11cf-8f52-0040333594a3>");
				writer.WriteLine(" Matrix4x4 frameMatrix;");
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("template Frame {");
				writer.WriteLine(" <3d82ab46-62da-11cf-ab39-0020af71e433>");
				writer.WriteLine(" [...]");
				writer.WriteLine("}");
				writer.WriteLine();
			}
		}

		public class Importer : IImported
		{
			public List<ImportedFrame> FrameList { get; protected set; }
			public List<ImportedMesh> MeshList { get; protected set; }
			public List<ImportedMaterial> MaterialList { get; protected set; }
			public List<ImportedTexture> TextureList { get; protected set; }
			public List<ImportedAnimation> AnimationList { get; protected set; }
			public List<ImportedMorph> MorphList { get; protected set; }

			private string path;
			private int noNameCount = 0;
			private HashSet<string> matList = new HashSet<string>();
			private HashSet<string> texList = new HashSet<string>();

			public Importer(string path)
			{
				try
				{
					FrameList = new List<ImportedFrame>();
					MeshList = new List<ImportedMesh>();
					MaterialList = new List<ImportedMaterial>();
					TextureList = new List<ImportedTexture>();
					AnimationList = new List<ImportedAnimation>();
					MorphList = new List<ImportedMorph>();

					this.path = path;

					List<Section> sectionList = new List<Section>();
					using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
					{
						string header = Encoding.ASCII.GetString(reader.ReadBytes(16));
						if (!header.StartsWith("xof "))
						{
							throw new Exception("invalid .x format");
						}
						if (!header.EndsWith("0032"))
						{
							throw new Exception("only 32-bit .x format is supported");
						}

						string type = header.Substring(8, 4).Trim();
						if (type == "txt")
						{
							ParserTxt parser = new ParserTxt(path);
							Section newSection;
							while ((newSection = parser.ParseSection(null)) != null)
							{
								sectionList.Add(newSection);
							}
						}
						else if (type == "bin")
						{
							ParserBin parser = new ParserBin(path);
							Section newSection;
							while ((newSection = parser.ParseSection(0, null)) != null)
							{
								sectionList.Add(newSection);
							}
						}
						else
						{
							throw new Exception("unexpected .x format");
						}
					}

					List<bool> hasBonesList = new List<bool>();
					SortedDictionary<string, byte> boneDic = new SortedDictionary<string, byte>();
					ImportedMesh mesh = new ImportedMesh();
					mesh.Name = "no_name_meshes";
					mesh.BoneList = new List<ImportedBone>(0);
					mesh.SubmeshList = new List<ImportedSubmesh>();

					foreach (Section section in sectionList)
					{
						if (section.type == "Frame")
						{
							ImportedFrame frame = ImportFrame(section);
							if (frame != null)
							{
								FrameList.Add(frame);
							}
						}
						else if (section.type == "Mesh")
						{
							ImportMesh(section, mesh, boneDic, hasBonesList);
						}
						else if (section.type == "AnimationSet")
						{
							ImportAnimation(section);
						}
						else if (section.type == "template")
						{
						}
						else if (section.type == "AnimTicksPerSecond")
						{
						}
						else if (section.type == "AnimationOptions")
						{
						}
						else
						{
							Report.ReportLog("Warning: unexpected section " + section.type);
						}
					}
					SetBones(mesh, hasBonesList);
					if (mesh.SubmeshList.Count > 0)
					{
						MeshList.Add(mesh);
					}
				}
				catch (Exception ex)
				{
					Report.ReportLog("Error importing .x: " + ex.Message);
				}
			}

			private ushort ConvertUInt16(object obj)
			{
				if (obj is string)
				{
					return UInt16.Parse((string)obj);
				}
				else
				{
					return BitConverter.ToUInt16((byte[])obj, 0);
				}
			}

			private int ConvertInt32(object obj)
			{
				if (obj is string)
				{
					return Int32.Parse((string)obj);
				}
				else
				{
					return BitConverter.ToInt32((byte[])obj, 0);
				}
			}

			private float ConvertFloat(object obj)
			{
				if (obj is string)
				{
					return Utility.ParseFloat((string)obj);
				}
				else
				{
					return BitConverter.ToSingle((byte[])obj, 0);
				}
			}

			private string ConvertString(object obj)
			{
				if (obj is string)
				{
					return (string)obj;
				}
				else
				{
					return Utility.EncodingShiftJIS.GetString((byte[])obj);
				}
			}

			private void SetBones(ImportedMesh mesh, List<bool> hasBonesList)
			{
				for (int i = 0; i < mesh.SubmeshList.Count; i++)
				{
					if (!hasBonesList[i])
					{
						if (mesh.BoneList.Count > 0)
						{
							foreach (ImportedVertex vert in mesh.SubmeshList[i].VertexList)
							{
								vert.BoneIndices = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
								vert.Weights = new float[] { 1, 0, 0, 0 };
							}
						}
						else
						{
							foreach (ImportedVertex vert in mesh.SubmeshList[i].VertexList)
							{
								vert.BoneIndices = new byte[4];
								vert.Weights = new float[4];
							}
						}
					}
				}
			}

			private ImportedFrame ImportFrame(Section section)
			{
				ImportedFrame frame = new ImportedFrame();
				frame.InitChildren(0);

				if (section.name == null)
				{
					frame.Name = "no_name" + noNameCount;
					noNameCount++;
				}
				else
				{
					frame.Name = section.name;
				}

				List<bool> hasBonesList = new List<bool>();
				SortedDictionary<string, byte> boneDic = new SortedDictionary<string, byte>();
				ImportedMesh meshList = new ImportedMesh();
				meshList.Name = frame.Name;
				meshList.BoneList = new List<ImportedBone>();
				meshList.SubmeshList = new List<ImportedSubmesh>();

				Matrix matrix = new Matrix();
				foreach (Section child in section.children)
				{
					if (child.type == "FrameTransformMatrix")
					{
						LinkedListNode<object> node = child.data.First;
						for (int i = 0; i < 4; i++)
						{
							for (int j = 0; j < 4; j++)
							{
								matrix[i, j] = ConvertFloat(node.Value);
								node = node.Next;
							}
						}
						frame.Matrix = RHToLHMatrix(matrix);
					}
					else if (child.type == "Mesh")
					{
						ImportMesh(child, meshList, boneDic, hasBonesList);
					}
					else if (child.type == "Frame")
					{
						ImportedFrame childFrame = ImportFrame(child);
						if (childFrame != null)
						{
							frame.AddChild(childFrame);
						}
					}
					else
					{
						Report.ReportLog("Warning: unexpected section " + child.type);
					}
				}
				SetBones(meshList, hasBonesList);
				if (meshList.SubmeshList.Count > 0)
				{
					MeshList.Add(meshList);
				}

				if (matrix == null)
				{
					frame.Matrix = Matrix.Identity;
				}
				return frame;
			}

			private void ImportMesh(Section section, ImportedMesh meshList, SortedDictionary<string, byte> boneDic, List<bool> hasBonesList)
			{
				int vertIdxOffset = 0;
				foreach (ImportedSubmesh submesh in meshList.SubmeshList)
				{
					vertIdxOffset += submesh.VertexList.Count;
				}

				LinkedListNode<object> node = section.data.First;
				int numVertices = ConvertInt32(node.Value);
				node = node.Next;
				DXVertex[] vertices = new DXVertex[numVertices];
				for (int i = 0; i < numVertices; i++)
				{
					vertices[i] = new DXVertex();
					float[] pos = new float[3];
					for (int j = 0; j < pos.Length; j++)
					{
						pos[j] = ConvertFloat(node.Value);
						node = node.Next;
					}
					pos[2] = -pos[2];
					vertices[i].position = pos;
				}

				int numFaces = ConvertInt32(node.Value);
				node = node.Next;
				DXFace[] faces = new DXFace[numFaces];
				for (int i = 0; i < numFaces; i++)
				{
					int numFaceVerts = ConvertInt32(node.Value);
					node = node.Next;
					if (numFaceVerts != 3)
					{
						throw new Exception("Meshes must be triangulated");
					}

					faces[i] = new DXFace();
					faces[i].vertexIndices[0] = ConvertUInt16(node.Value);
					node = node.Next;
					faces[i].vertexIndices[2] = ConvertUInt16(node.Value);
					node = node.Next;
					faces[i].vertexIndices[1] = ConvertUInt16(node.Value);
					node = node.Next;
				}

				string[] materials = new string[] { String.Empty };
				bool hasNormals = false;
				bool hasBones = false;
				bool hasUVs = false;
				List<KeyValuePair<byte, float>>[] boneAssignments = new List<KeyValuePair<byte, float>>[numVertices];
				for (int i = 0; i < boneAssignments.Length; i++)
				{
					boneAssignments[i] = new List<KeyValuePair<byte, float>>();
				}
				foreach (Section child in section.children)
				{
					if (child.type == "VertexDuplicationIndices")
					{
					}
					else if (child.type == "MeshNormals")
					{
						hasNormals = true;
						LinkedListNode<object> childNode = child.data.First;
						int numNormals = ConvertInt32(childNode.Value);
						childNode = childNode.Next;
						if (numNormals != numVertices)
						{
							throw new Exception("Number of normals doesn't match the number of vertices");
						}
						foreach (DXVertex vert in vertices)
						{
							float[] norm = new float[3];
							for (int i = 0; i < norm.Length; i++)
							{
								norm[i] = ConvertFloat(childNode.Value);
								childNode = childNode.Next;
							}
							norm[2] = -norm[2];
							vert.normal = norm;
						}
					}
					else if (child.type == "MeshTextureCoords")
					{
						hasUVs = true;
						LinkedListNode<object> childNode = child.data.First;
						int numTexCoords = ConvertInt32(childNode.Value);
						childNode = childNode.Next;
						if (numTexCoords != numVertices)
						{
							throw new Exception("Number of texture coordinates doesn't match the number of vertices");
						}
						foreach (DXVertex vert in vertices)
						{
							float[] uv = new float[2];
							for (int i = 0; i < uv.Length; i++)
							{
								uv[i] = ConvertFloat(childNode.Value);
								childNode = childNode.Next;
							}
							vert.uv = uv;
						}
					}
					else if (child.type == "MeshMaterialList")
					{
						materials = ImportMaterials(child, faces);
					}
					else if (child.type == "XSkinMeshHeader")
					{
						hasBones = true;
					}
					else if (child.type == "SkinWeights")
					{
						LinkedListNode<object> childNode = child.data.First;
						string boneName = ConvertString(childNode.Value);
						childNode = childNode.Next;
						int numWeights = ConvertInt32(childNode.Value);
						childNode = childNode.Next;
						int[] vertIndices = new int[numWeights];
						for (int i = 0; i < numWeights; i++)
						{
							vertIndices[i] = ConvertInt32(childNode.Value);
							childNode = childNode.Next;
						}
						float[] weights = new float[numWeights];
						for (int i = 0; i < numWeights; i++)
						{
							weights[i] = ConvertFloat(childNode.Value);
							childNode = childNode.Next;
						}

						byte boneIdx;
						if (!boneDic.TryGetValue(boneName, out boneIdx))
						{
							boneIdx = (byte)boneDic.Count;
							boneDic.Add(boneName, boneIdx);

							ImportedBone boneInfo = new ImportedBone();
							meshList.BoneList.Add(boneInfo);
							boneInfo.Name = boneName;

							Matrix matrix = new Matrix();
							for (int i = 0; i < 4; i++)
							{
								for (int j = 0; j < 4; j++)
								{
									matrix[i, j] = ConvertFloat(childNode.Value);
									childNode = childNode.Next;
								}
							}
							boneInfo.Matrix = RHToLHMatrix(matrix);
						}

						for (int i = 0; i < numWeights; i++)
						{
							boneAssignments[vertIndices[i]].Add(new KeyValuePair<byte, float>(boneIdx, weights[i]));
						}
					}
					else
					{
						Report.ReportLog("Warning: unexpected section " + child.type);
					}
				}

				if (hasBones)
				{
					for (int i = 0; i < boneAssignments.Length; i++)
					{
						byte[] boneIndices = new byte[4];
						float[] weights4 = new float[4];
						for (int j = 0; (j < 4) && (j < boneAssignments[i].Count); j++)
						{
							boneIndices[j] = boneAssignments[i][j].Key;
							weights4[j] = boneAssignments[i][j].Value;
						}
						for (int j = boneAssignments[i].Count; j < 4; j++)
						{
							boneIndices[j] = 0xFF;
							weights4[j] = 0;
						}

						vertices[i].boneIndices = boneIndices;
						vertices[i].weights = new float[] { weights4[0], weights4[1], weights4[2], weights4[3] };
					}
				}

				SortedDictionary<ushort, ushort>[] vertexMaps = new SortedDictionary<ushort, ushort>[materials.Length];
				ImportedSubmesh[] submeshes = new ImportedSubmesh[materials.Length];
				for (int i = 0; i < materials.Length; i++)
				{
					submeshes[i] = new ImportedSubmesh();
					submeshes[i].Material = materials[i];
					submeshes[i].VertexList = new List<ImportedVertex>(vertices.Length);
					submeshes[i].FaceList = new List<ImportedFace>(faces.Length);
					vertexMaps[i] = new SortedDictionary<ushort, ushort>();
				}

				foreach (DXFace dxFace in faces)
				{
					ImportedSubmesh submesh = submeshes[dxFace.materialIndex];
					ImportedFace face = new ImportedFace();
					submesh.FaceList.Add(face);

					ushort[] foundVertexIndices = new ushort[3];
					for (int i = 0; i < dxFace.vertexIndices.Length; i++)
					{
						ushort dxVertIdx = dxFace.vertexIndices[i];
						SortedDictionary<ushort, ushort> vertexMap = vertexMaps[dxFace.materialIndex];
						if (!vertexMap.TryGetValue(dxVertIdx, out foundVertexIndices[i]))
						{
							DXVertex dxVert = vertices[dxVertIdx];
							ImportedVertex vert = new ImportedVertex();
							submesh.VertexList.Add(vert);
							if (hasNormals)
							{
								vert.Normal = new Vector3(dxVert.normal[0], dxVert.normal[1], dxVert.normal[2]);
							}
							if (hasUVs)
							{
								vert.UV = (float[])dxVert.uv.Clone();
							}
							if (hasBones)
							{
								vert.BoneIndices = (byte[])dxVert.boneIndices.Clone();
								vert.Weights = (float[])dxVert.weights.Clone();
							}
							vert.Position = new Vector3(dxVert.position[0], dxVert.position[1], dxVert.position[2]);
							vertIdxOffset++;

							foundVertexIndices[i] = (ushort)vertexMap.Count;
							vertexMap.Add(dxVertIdx, foundVertexIndices[i]);
						}
					}

					face.VertexIndices = new int[] { foundVertexIndices[0], foundVertexIndices[1], foundVertexIndices[2] };
				}

				foreach (ImportedSubmesh submesh in submeshes)
				{
					if (submesh.VertexList.Count > 0)
					{
						submesh.VertexList.TrimExcess();
						submesh.FaceList.TrimExcess();
						submesh.Index = meshList.SubmeshList.Count;
						meshList.SubmeshList.Add(submesh);
						hasBonesList.Add(hasBones);

						if (!hasNormals)
						{
							for (int i = 0; i < submesh.VertexList.Count; i++)
							{
								submesh.VertexList[i].Normal = new Vector3();
							}
						}
					}
				}
			}

			private string[] ImportMaterials(Section section, DXFace[] faces)
			{
				LinkedListNode<object> node = section.data.First;

				int numMaterials = ConvertInt32(node.Value);
				node = node.Next;
				if (numMaterials != section.children.Count)
				{
					throw new Exception("number of materials doesn't match number of children");
				}

				int numFaces = ConvertInt32(node.Value);
				node = node.Next;
				if (numFaces != faces.Length)
				{
					throw new Exception("number of faces doesn't match with material");
				}
				for (int i = 0; i < numFaces; i++)
				{
					faces[i].materialIndex = ConvertInt32(node.Value);
					node = node.Next;
				}

				string[] materialNames = new string[numMaterials];
				for (int i = 0; i < numMaterials; i++)
				{
					Section matSection = section.children[i];
					if (matSection.type == "Material")
					{
						string texName = String.Empty;
						foreach (Section texSection in matSection.children)
						{
							if (texSection.type == "TextureFilename")
							{
								texName = ImportTexture(texSection);
								break;
							}
							else
							{
								Report.ReportLog("Warning: unexpected section " + matSection.type);
							}
						}

						if (matSection.name == null)
						{
							if (texName == String.Empty)
							{
								materialNames[i] = "no_name_" + noNameCount;
							}
							else
							{
								materialNames[i] = Path.GetFileNameWithoutExtension(texName) + "_" + noNameCount;
							}
							noNameCount++;
						}
						else
						{
							materialNames[i] = matSection.name;
						}

						if (matList.Add(materialNames[i]))
						{
							LinkedListNode<object> dataNode = section.children[i].data.First;
							ImportedMaterial matInfo = new ImportedMaterial();
							matInfo.Name = materialNames[i];
							matInfo.Diffuse = new Color4(1, 1, 1, 1);

							float[] ambient = new float[4];
							for (int j = 0; j < ambient.Length; j++)
							{
								ambient[j] = ConvertFloat(dataNode.Value);
								dataNode = dataNode.Next;
							}
							matInfo.Ambient = new Color4(ambient[3], ambient[0], ambient[1], ambient[2]);

							matInfo.Power = ConvertFloat(dataNode.Value);
							dataNode = dataNode.Next;

							float[] specular = new float[4];
							for (int j = 0; j < 3; j++)
							{
								specular[j] = ConvertFloat(dataNode.Value);
								dataNode = dataNode.Next;
							}
							specular[3] = 1;
							matInfo.Specular = new Color4(specular[3], specular[0], specular[1], specular[2]);

							float[] emissive = new float[4];
							for (int j = 0; j < 3; j++)
							{
								emissive[j] = ConvertFloat(dataNode.Value);
								dataNode = dataNode.Next;
							}
							emissive[3] = 1;
							matInfo.Emissive = new Color4(emissive[3], emissive[0], emissive[1], emissive[2]);

							if (texName != String.Empty)
							{
								matInfo.Textures = new string[] { texName };
							}

							MaterialList.Add(matInfo);
						}
					}
					else if (matSection.type == "ref")
					{
						if (matSection.name != null)
						{
							materialNames[i] = matSection.name;
						}
					}
					else
					{
						Report.ReportLog("Warning: unexpected section " + matSection.type);
					}
				}

				return materialNames;
			}

			private string ImportTexture(Section section)
			{
				string texName = String.Empty;
				if (section.data.First != null)
				{
					texName = Path.GetFileName(ConvertString(section.data.First.Value));
					if (texList.Add(texName))
					{
						string texPath = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + texName;
						ImportedTexture tex = new ImportedTexture(texPath);
						TextureList.Add(tex);
					}
				}
				return texName;
			}

			private void ImportAnimation(Section section)
			{
				ImportedAnimation workspaceAnimation = new ImportedAnimation();
				workspaceAnimation.TrackList = new List<ImportedAnimationTrack>(section.children.Count);

				foreach (Section animSection in section.children)
				{
					if (animSection.type == "Animation")
					{
						string trackName = null;
						float[][][] keyDataArray = new float[5][][];
						foreach (Section keySection in animSection.children)
						{
							if (keySection.type == "AnimationKey")
							{
								LinkedListNode<object> keyNode = keySection.data.First;
								int keyType = ConvertInt32(keyNode.Value);
								keyNode = keyNode.Next;
								int numKeys = ConvertInt32(keyNode.Value);
								keyNode = keyNode.Next;
								float[][] keyData = new float[numKeys][];
								for (int i = 0; i < numKeys; i++)
								{
									int keyIdx = ConvertInt32(keyNode.Value);
									keyNode = keyNode.Next;
									int numFloats = ConvertInt32(keyNode.Value);
									keyNode = keyNode.Next;
									keyData[i] = new float[numFloats];
									for (int j = 0; j < numFloats; j++)
									{
										keyData[i][j] = ConvertFloat(keyNode.Value);
										keyNode = keyNode.Next;
									}
								}
								keyDataArray[keyType] = keyData;
							}
							else if (keySection.type == "ref")
							{
								trackName = keySection.name;
							}
							else
							{
								Report.ReportLog("Warning: unexpected section " + animSection.type);
							}
						}

						if (trackName == null)
						{
							throw new Exception("animation doesn't have a track name");
						}
						if ((keyDataArray[0] == null) || (keyDataArray[2] == null))
						{
							throw new Exception("animation " + trackName + " doesn't have the correct key types");
						}
						if (keyDataArray[1] == null)
						{
							keyDataArray[1] = new float[keyDataArray[0].Length][];
							for (int i = 0; i < keyDataArray[1].Length; i++)
							{
								keyDataArray[1][i] = new float[] { 1, 1, 1 };
							}
						}
						if ((keyDataArray[0].Length != keyDataArray[1].Length) || (keyDataArray[0].Length != keyDataArray[2].Length))
						{
							throw new Exception("animation " + trackName + " doesn't have the same number of keys for each type");
						}

						ImportedAnimationKeyframe[] keyframes = new ImportedAnimationKeyframe[keyDataArray[0].Length];
						for (int i = 0; i < keyframes.Length; i++)
						{
							float[] rotation = keyDataArray[0][i];
							float[] scaling = keyDataArray[1][i];
							float[] translation = keyDataArray[2][i];
							keyframes[i] = new ImportedAnimationKeyframe();
							keyframes[i].Rotation = new Quaternion(rotation[0], rotation[1], rotation[2], -rotation[3]);
							keyframes[i].Scaling = new Vector3(scaling[0], scaling[1], scaling[2]);
							keyframes[i].Translation = new Vector3(translation[0], translation[1], -translation[2]);
						}
						if (keyframes.Length > 0)
						{
							ImportedAnimationTrack track = new ImportedAnimationTrack();
							track.Name = trackName;
							track.Keyframes = keyframes;
							workspaceAnimation.TrackList.Add(track);
						}
					}
					else
					{
						Report.ReportLog("Warning: unexpected section " + animSection.type);
					}
				}

				if (workspaceAnimation.TrackList.Count > 0)
				{
					AnimationList.Add(workspaceAnimation);
				}
			}

			private class Section
			{
				public string type = null;
				public string name = null;
				public LinkedList<object> data = new LinkedList<object>();
				public List<Section> children = new List<Section>();
			}

			private class ParserTxt
			{
				private string buf;
				private int bufIdx = 0;
				private static char[] dataDelimiters = new char[] { '\r', '\n', '\t', ' ', ',', ';', '\"', '{', '}' };
				private static char[] sectionDelimiters = new char[] { '{', '}' };
				private static char[] newlineDelimiters = new char[] { '\r', '\n' };

				public ParserTxt(string path)
				{
					using (StreamReader reader = new StreamReader(path, Utility.EncodingShiftJIS))
					{
						buf = reader.ReadToEnd();
						bufIdx = 16;
					}
				}

				public Section ParseSection(Section parent)
				{
					int headerIdx = buf.IndexOf('{', bufIdx);
					if (headerIdx < 0)
					{
						return null;
					}

					string[] headerArray = buf.Substring(bufIdx, headerIdx - bufIdx).Split(dataDelimiters, StringSplitOptions.RemoveEmptyEntries);
					bufIdx = headerIdx + 1;
					Section section = new Section();
					if (parent != null)
					{
						parent.children.Add(section);
					}
					section.type = headerArray[0];
					if (headerArray.Length > 1)
					{
						section.name = headerArray[1];
					}

					int dataIdx;
					do
					{
						dataIdx = buf.IndexOfAny(sectionDelimiters, bufIdx);
						if (buf[dataIdx] == '{')
						{
							string dataStr = buf.Substring(bufIdx, dataIdx - bufIdx);
							int dataEndIdx = dataStr.LastIndexOfAny(newlineDelimiters);
							bufIdx = bufIdx + dataEndIdx + 1;

							string[] dataArrayInner = dataStr.Substring(0, dataEndIdx).Split(dataDelimiters, StringSplitOptions.RemoveEmptyEntries);
							foreach (string s in dataArrayInner)
							{
								section.data.AddLast(s);
							}

							string[] nextSectionArray = dataStr.Substring(dataEndIdx).Split(dataDelimiters, StringSplitOptions.RemoveEmptyEntries);
							if (nextSectionArray.Length > 0)
							{
								ParseSection(section);
							}
							else
							{
								int sectionRefIdx = buf.IndexOf('}', bufIdx);
								string[] sectionArray = buf.Substring(bufIdx, sectionRefIdx - bufIdx).Split(dataDelimiters, StringSplitOptions.RemoveEmptyEntries);
								Section sectionRef = new Section();
								sectionRef.type = "ref";
								sectionRef.name = sectionArray[0];
								section.children.Add(sectionRef);
								bufIdx = sectionRefIdx + 1;
							}
						}
					}
					while (buf[dataIdx] == '{');

					string[] dataArrayOuter = buf.Substring(bufIdx, dataIdx - bufIdx).Split(dataDelimiters, StringSplitOptions.RemoveEmptyEntries);
					foreach (string s in dataArrayOuter)
					{
						section.data.AddLast(s);
					}
					bufIdx = dataIdx + 1;

					return section;
				}
			}

			private class ParserBin
			{
				BinaryReader reader = null;

				public ParserBin(string path)
				{
					using (BinaryReader fileReader = new BinaryReader(File.OpenRead(path)))
					{
						reader = new BinaryReader(new MemoryStream(fileReader.ReadBytes((int)fileReader.BaseStream.Length)));
						reader.BaseStream.Seek(16, SeekOrigin.Begin);
					}
				}

				public Section ParseSection(ushort type, Section parent)
				{
					if (reader.BaseStream.Position == reader.BaseStream.Length)
					{
						return null;
					}

					Section section = new Section();
					if (parent != null)
					{
						parent.children.Add(section);
					}

					if (type == 0)
					{
						type = reader.ReadUInt16();
					}

					if (type == TOKEN_TEMPLATE)
					{
						section.type = "template";
						int tempNameToken = reader.ReadUInt16();
						section.name = ParseName();
						int oBrace = reader.ReadUInt16();

						byte[] data;
						do
						{
							data = ParseTemplateData();
							if (data != null)
							{
								section.data.AddLast(data);
							}
						}
						while (data != null);
					}
					else if (type == TOKEN_NAME)
					{
						int typeLen = reader.ReadInt32();
						section.type = Encoding.ASCII.GetString(reader.ReadBytes(typeLen));
						ushort secToken = reader.ReadUInt16();
						switch (secToken)
						{
							case TOKEN_NAME:
								section.name = ParseName();
								ushort secOBrace = reader.ReadUInt16();
								break;
							case TOKEN_OBRACE:
								break;
							default:
								throw new Exception("unexpected section token: " + secToken);
						}

						byte[][] dataArray;
						do
						{
							dataArray = ParseData(section);
							if (dataArray != null)
							{
								foreach (byte[] data in dataArray)
								{
									section.data.AddLast(data);
								}
							}
						}
						while (dataArray != null);
					}
					else
					{
						throw new Exception("unexpected section type: " + type);
					}

					return section;
				}

				private string ParseName()
				{
					int nameLen = reader.ReadInt32();
					return Utility.EncodingShiftJIS.GetString(reader.ReadBytes(nameLen));
				}

				private byte[] ParseTemplateData()
				{
					byte[] data = null;
					ushort token = reader.ReadUInt16();
					switch (token)
					{
						case TOKEN_GUID:
							data = reader.ReadBytes(16);
							break;
						case TOKEN_WORD:
						case TOKEN_DWORD:
						case TOKEN_FLOAT:
						case TOKEN_DOUBLE:
						case TOKEN_CHAR:
						case TOKEN_UCHAR:
						case TOKEN_STRING:
						case TOKEN_ARRAY:
						case TOKEN_OBRACKET:
						case TOKEN_CBRACKET:
						case TOKEN_DOT:
						case TOKEN_SEMICOLON:
							data = BitConverter.GetBytes(token);
							break;
						case TOKEN_NAME:
							int nameLen = reader.ReadInt32();
							data = reader.ReadBytes(nameLen);
							break;
						case TOKEN_INTEGER:
							data = reader.ReadBytes(4);
							break;
						case TOKEN_CBRACE:
							break;
						default:
							throw new Exception("unexpected data token: " + token);
					}
					return data;
				}

				private byte[][] ParseData(Section section)
				{
					byte[][] dataArray = null;
					ushort token = reader.ReadUInt16();
					switch (token)
					{
						case TOKEN_GUID:
							dataArray = new byte[][] { reader.ReadBytes(16) };
							break;
						case TOKEN_WORD:
							dataArray = new byte[][] { reader.ReadBytes(2) };
							break;
						case TOKEN_DWORD:
						case TOKEN_FLOAT:
							dataArray = new byte[][] { reader.ReadBytes(4) };
							break;
						case TOKEN_DOUBLE:
							dataArray = new byte[][] { reader.ReadBytes(8) };
							break;
						case TOKEN_CHAR:
						case TOKEN_UCHAR:
							dataArray = new byte[][] { reader.ReadBytes(1) };
							break;
						case TOKEN_INTEGER_LIST:
						case TOKEN_REALNUM_LIST:
							int listLen = reader.ReadInt32();
							dataArray = new byte[listLen][];
							for (int i = 0; i < listLen; i++)
							{
								dataArray[i] = reader.ReadBytes(4);
							}
							break;
						case TOKEN_STRING:
							int strLen = reader.ReadInt32();
							dataArray = new byte[][] { reader.ReadBytes(strLen) };
							ushort strEnd = reader.ReadUInt16();
							break;
						case TOKEN_NAME:
							ParseSection(token, section);
							dataArray = ParseData(section);
							break;
						case TOKEN_OBRACE:
							ushort sectionRefNameToken = reader.ReadUInt16();
							Section sectionRef = new Section();
							sectionRef.type = "ref";
							sectionRef.name = ParseName();
							section.children.Add(sectionRef);
							ushort sectionRefCBrace = reader.ReadUInt16();
							dataArray = ParseData(sectionRef);
							break;
						case TOKEN_CBRACE:
							break;
						default:
							throw new Exception("unexpected data token: " + token);
					}
					return dataArray;
				}
			}

			private class DXVertex
			{
				public float[] position = null;
				public float[] normal = null;
				public float[] uv = null;
				public byte[] boneIndices = null;
				public float[] weights = null;
			}

			private class DXFace
			{
				public int materialIndex = 0;
				public ushort[] vertexIndices = new ushort[3];
			}
		}
	}
}
