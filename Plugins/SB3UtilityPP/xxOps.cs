using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SlimDX;

namespace SB3Utility
{
	public static partial class xx
	{
		private class VertexRef
		{
			public xxVertex vert;
			public Vector3 norm;
		}

		private class VertexRefComparerX : IComparer<VertexRef>
		{
			public int Compare(VertexRef x, VertexRef y)
			{
				return System.Math.Sign(x.vert.Position[0] - y.vert.Position[0]);
			}
		}

		private class VertexRefComparerY : IComparer<VertexRef>
		{
			public int Compare(VertexRef x, VertexRef y)
			{
				return System.Math.Sign(x.vert.Position[1] - y.vert.Position[1]);
			}
		}

		private class VertexRefComparerZ : IComparer<VertexRef>
		{
			public int Compare(VertexRef x, VertexRef y)
			{
				return System.Math.Sign(x.vert.Position[2] - y.vert.Position[2]);
			}
		}

		public static bool IsSkinned(xxMesh mesh)
		{
			return (mesh.BoneList.Count > 0);
		}

		public static void ExportTexture(xxTexture tex, string path)
		{
			FileInfo file = new FileInfo(path);
			DirectoryInfo dir = file.Directory;
			if (!dir.Exists)
			{
				dir.Create();
			}
			using (FileStream stream = file.Create())
			{
				ExportTexture(tex, stream);
			}
		}

		public static void ExportTexture(xxTexture tex, Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			if (Path.GetExtension(tex.Name).ToLowerInvariant() == ".bmp")
			{
				writer.Write((byte)'B');
				writer.Write((byte)'M');
				writer.Write(tex.ImageData, 2, tex.ImageData.Length - 2);
			}
			else
			{
				writer.Write(tex.ImageData);
			}
		}

		public static ImportedTexture ImportedTexture(xxTexture texture)
		{
			ImportedTexture importedTex = new ImportedTexture();
			importedTex.Name = texture.Name;
			importedTex.Data = (byte[])texture.ImageData.Clone();
			if (Path.GetExtension(texture.Name).ToLowerInvariant() == ".bmp")
			{
				importedTex.Data[0] = (byte)'B';
				importedTex.Data[1] = (byte)'M';
			}
			return importedTex;
		}

		public static List<ImportedVertex> ImportedVertexList(List<xxVertex> vertList, bool skinned)
		{
			List<ImportedVertex> importedList = new List<ImportedVertex>(vertList.Count);
			for (int i = 0; i < vertList.Count; i++)
			{
				ImportedVertex importedVert = new ImportedVertex();
				importedList.Add(importedVert);
				importedVert.Position = vertList[i].Position;
				importedVert.Normal = vertList[i].Normal;
				importedVert.UV = vertList[i].UV;
				importedVert.BoneIndices = vertList[i].BoneIndices;
				importedVert.Weights = vertList[i].Weights4(skinned);
			}
			return importedList;
		}

		public static List<ImportedFace> ImportedFaceList(List<xxFace> faceList)
		{
			List<ImportedFace> importedList = new List<ImportedFace>(faceList.Count);
			for (int i = 0; i < faceList.Count; i++)
			{
				ImportedFace importedFace = new ImportedFace();
				importedList.Add(importedFace);
				importedFace.VertexIndices = new int[3];
				for (int j = 0; j < 3; j++)
				{
					importedFace.VertexIndices[j] = faceList[i].VertexIndices[j];
				}
			}
			return importedList;
		}

		private class VertexCompare : IComparable<VertexCompare>
		{
			public int index;
			public float[] position;
			public float[] normal;
			public byte[] boneIndices;
			public float[] weights;

			public VertexCompare(int index, Vector3 position, Vector3 normal, byte[] boneIndices, float[] weights)
			{
				this.index = index;
				this.position = new float[] { position.X, position.Y, position.Z };
				this.normal = new float[] { normal.X, normal.Y, normal.Z };
				this.boneIndices = boneIndices;
				this.weights = weights;
			}

			public int CompareTo(VertexCompare other)
			{
				int diff = Diff(this.position, other.position);
				if (diff != 0)
				{
					return diff;
				}

				diff = Diff(this.normal, other.normal);
				if (diff != 0)
				{
					return diff;
				}

				diff = Diff(this.boneIndices, other.boneIndices);
				if (diff != 0)
				{
					return diff;
				}

				diff = Diff(this.weights, other.weights);
				if (diff != 0)
				{
					return diff;
				}
				return 0;
			}

			private static int Diff(float[] a, float[] b)
			{
				for (int i = 0; i < a.Length; i++)
				{
					int diff = System.Math.Sign(a[i] - b[i]);
					if (diff != 0)
					{
						return diff;
					}
				}
				return 0;
			}

			private static int Diff(byte[] a, byte[] b)
			{
				for (int i = 0; i < a.Length; i++)
				{
					int diff = a[i] - b[i];
					if (diff != 0)
					{
						return diff;
					}
				}
				return 0;
			}
		}

		public static HashSet<string> SearchHierarchy(xxFrame frame, HashSet<string> meshNames)
		{
			HashSet<string> exportFrames = new HashSet<string>();
			SearchHierarchy(frame, frame, meshNames, exportFrames);
			return exportFrames;
		}

		static void SearchHierarchy(xxFrame root, xxFrame frame, HashSet<string> meshNames, HashSet<string> exportFrames)
		{
			if (frame.Mesh != null)
			{
				if (meshNames.Contains(frame.Name))
				{
					xxFrame parent = frame;
					while (parent != null)
					{
						exportFrames.Add(parent.Name);
						parent = (xxFrame)parent.Parent;
					}

					xxMesh meshListSome = frame.Mesh;
					List<xxBone> boneList = meshListSome.BoneList;
					for (int i = 0; i < boneList.Count; i++)
					{
						if (!exportFrames.Contains(boneList[i].Name))
						{
							xxFrame boneParent = FindFrame(boneList[i].Name, root);
							while (boneParent != null)
							{
								exportFrames.Add(boneParent.Name);
								boneParent = (xxFrame)boneParent.Parent;
							}
						}
					}
				}
			}

			for (int i = 0; i < frame.Count; i++)
			{
				SearchHierarchy(root, frame[i], meshNames, exportFrames);
			}
		}

		public static xxFrame FindFrame(string name, xxFrame root)
		{
			xxFrame frame = root;
			if ((frame != null) && (frame.Name == name))
			{
				return frame;
			}

			for (int i = 0; i < root.Count; i++)
			{
				if ((frame = FindFrame(name, root[i])) != null)
				{
					return frame;
				}
			}

			return null;
		}

		public static List<xxFrame> FindMeshFrames(xxFrame frame, List<string> nameList)
		{
			List<xxFrame> frameList = new List<xxFrame>(nameList.Count);
			FindMeshFrames(frame, frameList, nameList);
			return frameList;
		}

		static void FindMeshFrames(xxFrame frame, List<xxFrame> frameList, List<string> nameList)
		{
			if ((frame.Mesh != null) && nameList.Contains(frame.Name))
			{
				frameList.Add(frame);
			}

			for (int i = 0; i < frame.Count; i++)
			{
				FindMeshFrames(frame[i], frameList, nameList);
			}
		}

		public static List<xxFrame> FindMeshFrames(xxFrame frame)
		{
			List<xxFrame> frameList = new List<xxFrame>();
			FindMeshFrames(frame, frameList);
			return frameList;
		}

		static void FindMeshFrames(xxFrame frame, List<xxFrame> frameList)
		{
			if (frame.Mesh != null)
			{
				frameList.Add(frame);
			}

			for (int i = 0; i < frame.Count; i++)
			{
				FindMeshFrames(frame[i], frameList);
			}
		}

		public static void SetBoundingBox(xxFrame frame)
		{
			if (frame.Mesh == null)
			{
				frame.Bounds = new BoundingBox();
			}
			else
			{
				xxMesh meshList = frame.Mesh;
				Vector3 min = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
				Vector3 max = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
				for (int i = 0; i < meshList.SubmeshList.Count; i++)
				{
					List<xxVertex> vertList = meshList.SubmeshList[i].VertexList;
					for (int j = 0; j < vertList.Count; j++)
					{
						xxVertex vert = vertList[j];
						Vector3 pos = vert.Position;
						min = Vector3.Minimize(min, pos);
						max = Vector3.Maximize(max, pos);
					}
				}
				frame.Bounds = new BoundingBox(min, max);
			}
		}

		public static void CopyUnknowns(xxFrame src, xxFrame dest)
		{
			dest.Name2 = src.Name2;
			dest.Unknown1 = (byte[])src.Unknown1.Clone();
			dest.Unknown2 = (byte[])src.Unknown2.Clone();
		}

		public static void CreateUnknowns(xxFrame frame, int xxFormat)
		{
			if (xxFormat >= 7)
			{
				frame.Unknown1 = new byte[32];
				frame.Unknown2 = new byte[64];
			}
			else
			{
				frame.Unknown1 = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
				frame.Unknown2 = new byte[16] { 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			}

			if (xxFormat >= 6)
			{
				frame.Name2 = String.Empty;
			}
		}

		public static void CopyUnknowns(xxMesh src, xxMesh dest)
		{
			dest.NumVector2PerVertex = src.NumVector2PerVertex;
			dest.VertexListDuplicateUnknown = (byte[])src.VertexListDuplicateUnknown.Clone();
		}

		public static void CreateUnknowns(xxMesh mesh)
		{
			mesh.NumVector2PerVertex = 0;
			mesh.VertexListDuplicateUnknown = IsSkinned(mesh)
				? new byte[] { 0x1C, 0x11, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00 }
				: new byte[] { 0x12, 0x01, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00 };
		}

		public static void CopyUnknowns(xxSubmesh src, xxSubmesh dest, int xxFormat, byte xxMeshNumUnknownA)
		{
			List<xxVertex> srcVertexList = src.VertexList;
			List<xxVertex> destVertexList = dest.VertexList;

			dest.Unknown1 = (byte[])src.Unknown1.Clone();

			if (xxFormat >= 7)
			{
				dest.Unknown2 = (byte[])src.Unknown2.Clone();
			}

			if (xxMeshNumUnknownA > 0)
			{
				dest.Vector2Lists = new List<List<Vector2>>(destVertexList.Count);
				for (int j = 0; (j < destVertexList.Count) && (j < srcVertexList.Count); j++)
				{
					List<Vector2> vectorList = new List<Vector2>(xxMeshNumUnknownA);
					dest.Vector2Lists.Add(vectorList);
					for (byte k = 0; k < xxMeshNumUnknownA; k++)
					{
						vectorList.Add(src.Vector2Lists[j][k]);
					}
				}
				for (int j = srcVertexList.Count; j < destVertexList.Count; j++)
				{
					List<Vector2> vectorList = new List<Vector2>(xxMeshNumUnknownA);
					dest.Vector2Lists.Add(vectorList);
					for (byte k = 0; k < xxMeshNumUnknownA; k++)
					{
						vectorList.Add(new Vector2());
					}
				}
			}

			if (xxFormat >= 2)
			{
				dest.Unknown3 = (byte[])src.Unknown3.Clone();
			}

			if (xxFormat >= 7)
			{
				dest.Unknown4 = (byte[])src.Unknown4.Clone();

				if (xxFormat >= 7)
				{
					dest.Unknown5 = (byte[])src.Unknown5.Clone();
				}
			}
			else
			{
				if (xxFormat >= 3)
				{
					dest.Unknown4 = (byte[])src.Unknown4.Clone();
				}
				if (xxFormat >= 5)
				{
					dest.Unknown5 = (byte[])src.Unknown5.Clone();
				}
				if (xxFormat >= 6)
				{
					dest.Unknown6 = (byte[])src.Unknown6.Clone();
				}
			}

			if (xxFormat >= 4)
			{
				for (int j = 0; j < destVertexList.Count; j++)
				{
					if (j < srcVertexList.Count)
					{
						destVertexList[j].Unknown1 = (byte[])srcVertexList[j].Unknown1.Clone();
					}
					else
					{
						destVertexList[j].Unknown1 = new byte[20];
					}
				}
			}
		}

		public static void CreateUnknowns(xxSubmesh submesh, int xxFormat, byte numVector2PerVertex)
		{
			List<xxVertex> vertList = submesh.VertexList;

			submesh.Unknown1 = (xxFormat >= 7) ? new byte[64] : new byte[] { 0x0C, 0x64, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

			if (numVector2PerVertex > 0)
			{
				submesh.Vector2Lists = new List<List<Vector2>>(vertList.Count);
				for (int j = 0; j < vertList.Count; j++)
				{
					List<Vector2> vectorList = new List<Vector2>(numVector2PerVertex);
					submesh.Vector2Lists.Add(vectorList);
					for (byte k = 0; k < numVector2PerVertex; k++)
					{
						vectorList.Add(new Vector2());
					}
				}
			}

			if (xxFormat >= 2)
			{
				submesh.Unknown3 = new byte[100];
			}

			if (xxFormat >= 7)
			{
				submesh.Unknown4 = new byte[284];

				if (xxFormat >= 8)
				{
					submesh.Unknown5 = new byte[21];
				}
			}
			else
			{
				if (xxFormat >= 3)
				{
					submesh.Unknown4 = new byte[64];
				}
				if (xxFormat >= 5)
				{
					submesh.Unknown5 = new byte[20];
				}
				if (xxFormat >= 6)
				{
					submesh.Unknown6 = new byte[28];
				}
			}

			if (xxFormat >= 4)
			{
				for (int j = 0; j < vertList.Count; j++)
				{
					vertList[j].Unknown1 = new byte[20];
				}
			}
		}

		public static void CopyUnknowns(xxMaterial src, xxMaterial dest)
		{
			dest.Unknown1 = (byte[])src.Unknown1.Clone();
			for (int j = 0; j < src.Textures.Length; j++)
			{
				dest.Textures[j].Unknown1 = (byte[])src.Textures[j].Unknown1.Clone();
			}
		}

		public static void CreateUnknowns(xxMaterial mat, int xxFormat)
		{
			byte[] buf = (xxFormat < 0) ? new byte[4] : new byte[88];
			if (xxFormat < 0)
			{
				buf = new byte[4];
			}
			else
			{
				buf = new byte[88];
			}
			buf[1] = 0x01;
			buf[2] = 0x02;
			buf[3] = 0x03;
			mat.Unknown1 = buf;

			for (int i = 0; i < mat.Textures.Length; i++)
			{
				var matTex = mat.Textures[i];
				byte[] matTexBytes = new byte[16];
				if (matTex.Name != String.Empty)
				{
					string ext = Path.GetExtension(matTex.Name).ToLowerInvariant();
					if ((i == 0) && (ext == ".tga"))
					{
						matTexBytes[0] = 0x01;
					}
					if ((i == 1) && (ext == ".bmp"))
					{
						matTexBytes[0] = 0x07;
						matTexBytes[3] = 0x03;
					}
				}
				matTex.Unknown1 = matTexBytes;
			}
		}

		public static void CopyUnknowns(xxTexture src, xxTexture dest)
		{
			dest.Unknown1 = (byte[])src.Unknown1.Clone();
		}

		public static void CreateUnknowns(xxTexture tex)
		{
			tex.Unknown1 = new byte[4];
		}

		public static List<xxVertex> CreateVertexListDup(List<xxSubmesh> submeshList)
		{
			List<xxVertex> vertListDup = new List<xxVertex>(UInt16.MaxValue);
			List<VertexCompare> vertCompareList = new List<VertexCompare>(UInt16.MaxValue);
			for (int i = 0; i < submeshList.Count; i++)
			{
				xxSubmesh meshObj = submeshList[i];
				List<xxVertex> vertList = meshObj.VertexList;
				for (int j = 0; j < vertList.Count; j++)
				{
					xxVertex vert = vertList[j];
					VertexCompare vertCompare = new VertexCompare(vertCompareList.Count, vert.Position, vert.Normal, vert.BoneIndices, vert.Weights3);
					int idx = vertCompareList.BinarySearch(vertCompare);
					if (idx < 0)
					{
						vertCompareList.Insert(~idx, vertCompare);
						vert.Index = vertCompareList.Count;
						vertListDup.Add(vert.Clone());
					}
					else
					{
						vert.Index = vertCompareList[idx].index;
					}
				}
			}
			vertListDup.TrimExcess();
			return vertListDup;
		}

		public static List<xxBone> MergeBoneList(List<xxBone> boneList1, List<xxBone> boneList2, out byte[] boneList2IdxMap)
		{
			boneList2IdxMap = new byte[boneList2.Count];
			Dictionary<string, int> boneDic = new Dictionary<string, int>();
			List<xxBone> mergedList = new List<xxBone>(boneList1.Count + boneList2.Count);
			for (int i = 0; i < boneList1.Count; i++)
			{
				xxBone xxBone = boneList1[i].Clone();
				boneDic.Add(xxBone.Name, i);
				mergedList.Add(xxBone);
			}
			for (int i = 0; i < boneList2.Count; i++)
			{
				xxBone xxBone = boneList2[i].Clone();
				int boneIdx;
				if (boneDic.TryGetValue(xxBone.Name, out boneIdx))
				{
					mergedList[boneIdx] = xxBone;
				}
				else
				{
					boneIdx = mergedList.Count;
					mergedList.Add(xxBone);
					boneDic.Add(xxBone.Name, boneIdx);
				}
				xxBone.Index = boneIdx;
				boneList2IdxMap[i] = (byte)boneIdx;
			}
			return mergedList;
		}

		public static void MergeFrame(xxFrame oldFrame, xxFrame newFrame)
		{
			for (int i = oldFrame.Count - 1; i >= 0; i--)
			{
				xxFrame oldChild = oldFrame[i];
				xxFrame newChild = null;
				for (int j = 0; j < newFrame.Count; j++)
				{
					if (oldChild.Name == newFrame[j].Name)
					{
						newChild = newFrame[j];
						break;
					}
				}

				if (newChild == null)
				{
					newFrame.InsertChild(0, oldChild.Clone(true, true));
				}
				else
				{
					if ((newChild.Mesh == null) && (oldChild.Mesh != null))
					{
						newChild.Mesh = oldChild.Mesh.Clone(true, true, true);
						newChild.Bounds = oldChild.Bounds;
					}
					CopyUnknowns(oldChild, newChild);

					MergeFrame(oldChild, newChild);
				}
			}
		}

		public static void ConvertFormat(xxParser parser, int destFormat)
		{
			int srcFormat = parser.Format;
			if ((srcFormat < 1) && (destFormat >= 1))
			{
				byte[] headerBuf = new byte[26];
				headerBuf[0] = (byte)destFormat;
				parser.Header.CopyTo(headerBuf, 5);
				parser.Header = headerBuf;
			}
			else if ((srcFormat >= 1) && (destFormat < 1))
			{
				byte[] headerBuf = new byte[21];
				Array.Copy(parser.Header, 5, headerBuf, 0, headerBuf.Length);
				parser.Header = headerBuf;
			}

			ConvertFormat(parser.Frame, srcFormat, destFormat);

			for (int i = 0; i < parser.MaterialList.Count; i++)
			{
				ConvertFormat(parser.MaterialList[i], srcFormat, destFormat);
			}

			if ((srcFormat < 2) && (destFormat >= 2))
			{
				parser.Footer = new byte[10];
			}
			else if ((srcFormat >= 2) && (destFormat < 2))
			{
				parser.Footer = null;
			}

			parser.Format = destFormat;
		}

		public static void ConvertFormat(xxFrame frame, int srcFormat, int destFormat)
		{
			if ((srcFormat < 7) && (destFormat >= 7))
			{
				byte[] unknown1 = frame.Unknown1;
				frame.Unknown1 = new byte[32];
				Array.Copy(unknown1, frame.Unknown1, unknown1.Length);

				byte[] unknown2 = frame.Unknown2;
				frame.Unknown2 = new byte[64];
				Array.Copy(unknown2, frame.Unknown2, unknown2.Length);
			}
			else if ((srcFormat >= 7) && (destFormat < 7))
			{
				byte[] unknown1 = frame.Unknown1;
				frame.Unknown1 = new byte[16];
				Array.Copy(unknown1, frame.Unknown1, frame.Unknown1.Length);

				byte[] unknown2 = frame.Unknown2;
				frame.Unknown2 = new byte[16];
				Array.Copy(unknown2, frame.Unknown2, frame.Unknown2.Length);
			}

			if ((srcFormat < 6) && (destFormat >= 6))
			{
				frame.Name2 = String.Empty;
			}
			else if ((srcFormat >= 6) && (destFormat < 6))
			{
				frame.Name2 = null;
			}

			if (frame.Mesh != null)
			{
				for (int i = 0; i < frame.Mesh.SubmeshList.Count; i++)
				{
					xxSubmesh submesh = frame.Mesh.SubmeshList[i];

					if ((srcFormat < 7) && (destFormat >= 7))
					{
						byte[] unknown1 = submesh.Unknown1;
						submesh.Unknown1 = new byte[64];
						Array.Copy(unknown1, submesh.Unknown1, unknown1.Length);

						submesh.Unknown2 = new byte[20];
					}
					else if ((srcFormat >= 7) && (destFormat < 7))
					{
						byte[] unknown1 = submesh.Unknown1;
						submesh.Unknown1 = new byte[16];
						Array.Copy(unknown1, submesh.Unknown1, submesh.Unknown1.Length);

						submesh.Unknown2 = null;
					}

					if ((srcFormat < 2) && (destFormat >= 2))
					{
						submesh.Unknown3 = new byte[100];
					}
					else if ((srcFormat >= 2) && (destFormat < 2))
					{
						submesh.Unknown3 = null;
					}

					if ((srcFormat < 3) && ((destFormat >= 3) && (destFormat < 7)))
					{
						submesh.Unknown4 = new byte[64];
					}
					else if ((srcFormat < 3) && (destFormat >= 7))
					{
						submesh.Unknown4 = new byte[284];
					}
					else if (((srcFormat >= 3) && (srcFormat < 7)) && (destFormat < 3))
					{
						submesh.Unknown4 = null;
					}
					else if (((srcFormat >= 3) && (srcFormat < 7)) && (destFormat >= 7))
					{
						byte[] unknown4 = submesh.Unknown4;
						submesh.Unknown4 = new byte[284];
						Array.Copy(unknown4, submesh.Unknown4, unknown4.Length);
					}
					else if ((srcFormat >= 7) && (destFormat < 3))
					{
						submesh.Unknown4 = null;
					}
					else if ((srcFormat >= 7) && ((destFormat >= 3) && (destFormat < 7)))
					{
						byte[] unknown4 = submesh.Unknown4;
						submesh.Unknown4 = new byte[64];
						Array.Copy(unknown4, submesh.Unknown4, submesh.Unknown4.Length);
					}

					if ((srcFormat < 5) && ((destFormat >= 5) && (destFormat < 8)))
					{
						submesh.Unknown5 = new byte[20];
					}
					else if ((srcFormat < 5) && (destFormat >= 8))
					{
						submesh.Unknown5 = new byte[21];
					}
					else if (((srcFormat >= 5) && (srcFormat < 8)) && (destFormat < 5))
					{
						submesh.Unknown5 = null;
					}
					else if (((srcFormat >= 5) && (srcFormat < 8)) && (destFormat >= 8))
					{
						byte[] unknown5 = submesh.Unknown5;
						submesh.Unknown5 = new byte[21];
						Array.Copy(unknown5, submesh.Unknown5, unknown5.Length);
					}
					else if ((srcFormat >= 8) && (destFormat < 5))
					{
						submesh.Unknown5 = null;
					}
					else if ((srcFormat >= 8) && ((destFormat >= 5) && (destFormat < 8)))
					{
						byte[] unknown5 = submesh.Unknown5;
						submesh.Unknown5 = new byte[20];
						Array.Copy(unknown5, submesh.Unknown5, submesh.Unknown5.Length);
					}

					if ((srcFormat == 6) && (destFormat != 6))
					{
						submesh.Unknown6 = null;
					}
					else if ((srcFormat != 6) && (destFormat == 6))
					{
						submesh.Unknown6 = new byte[28];
					}

					ConvertFormat(submesh.VertexList, srcFormat, destFormat);
				}

				ConvertFormat(frame.Mesh.VertexListDuplicate, srcFormat, destFormat);
			}

			for (int i = 0; i < frame.Count; i++)
			{
				ConvertFormat(frame[i], srcFormat, destFormat);
			}
		}

		public static void ConvertFormat(List<xxVertex> vertexList, int srcFormat, int destFormat)
		{
			if ((srcFormat < 4) && (destFormat >= 4))
			{
				for (int i = 0; i < vertexList.Count; i++)
				{
					vertexList[i] = ConvertVertex((xxVertexInt)vertexList[i]);
				}
			}
			else if ((srcFormat >= 4) && (destFormat < 4))
			{
				for (int i = 0; i < vertexList.Count; i++)
				{
					vertexList[i] = ConvertVertex((xxVertexUShort)vertexList[i]);
				}
			}
		}

		public static xxVertexUShort ConvertVertex(xxVertexInt vertex)
		{
			xxVertexUShort vertShort = new xxVertexUShort();
			vertShort.Index = vertex.Index;
			vertShort.Position = vertex.Position;
			vertShort.Weights3 = (float[])vertex.Weights3.Clone();
			vertShort.Normal = vertex.Normal;
			vertShort.UV = (float[])vertex.UV.Clone();
			vertShort.Unknown1 = new byte[20];
			return vertShort;
		}

		public static xxVertexInt ConvertVertex(xxVertexUShort vertex)
		{
			xxVertexInt vertInt = new xxVertexInt();
			vertInt.Index = vertex.Index;
			vertInt.Position = vertex.Position;
			vertInt.Weights3 = (float[])vertex.Weights3.Clone();
			vertInt.Normal = vertex.Normal;
			vertInt.UV = (float[])vertex.UV.Clone();
			return vertInt;
		}

		public static void ConvertFormat(xxMaterial mat, int srcFormat, int destFormat)
		{
			if ((srcFormat < 0) && (destFormat >= 0))
			{
				byte[] unknown1 = mat.Unknown1;
				mat.Unknown1 = new byte[88];
				Array.Copy(unknown1, mat.Unknown1, unknown1.Length);
			}
			else if ((srcFormat >= 0) && (destFormat < 0))
			{
				byte[] unknown1 = mat.Unknown1;
				mat.Unknown1 = new byte[4];
				Array.Copy(unknown1, mat.Unknown1, mat.Unknown1.Length);
			}
		}

		public static void SetNumVector2PerVertex(xxMesh mesh, byte value)
		{
			int diff = value - mesh.NumVector2PerVertex;

			if (diff < 0)
			{
				if (value == 0)
				{
					for (int i = 0; i < mesh.SubmeshList.Count; i++)
					{
						mesh.SubmeshList[i].Vector2Lists = null;
					}
				}
				else
				{
					diff = Math.Abs(diff);
					for (int i = 0; i < mesh.SubmeshList.Count; i++)
					{
						var submesh = mesh.SubmeshList[i];
						for (int j = 0; j < submesh.VertexList.Count; j++)
						{
							var vectorList = submesh.Vector2Lists[j];
							vectorList.RemoveRange(vectorList.Count - diff, diff);
						}
					}
				}
			}
			else if (diff > 0)
			{
				for (int i = 0; i < mesh.SubmeshList.Count; i++)
				{
					var submesh = mesh.SubmeshList[i];
					if (submesh.Vector2Lists == null)
					{
						submesh.Vector2Lists = new List<List<Vector2>>(submesh.VertexList.Count);
						for (int j = 0; j < submesh.VertexList.Count; j++)
						{
							submesh.Vector2Lists.Add(new List<Vector2>(value));
							for (int k = 0; k < diff; k++)
							{
								submesh.Vector2Lists[j].Add(new Vector2());
							}
						}
					}
					else
					{
						for (int j = 0; j < submesh.VertexList.Count; j++)
						{
							for (int k = 0; k < diff; k++)
							{
								submesh.Vector2Lists[j].Add(new Vector2());
							}
						}
					}
				}
			}

			mesh.NumVector2PerVertex = value;
		}

		public static void CopyNormalsOrder(List<xxVertex> src, List<xxVertex> dest)
		{
			int len = (src.Count < dest.Count) ? src.Count : dest.Count;
			for (int i = 0; i < len; i++)
			{
				dest[i].Normal = src[i].Normal;
			}
		}

		public static void CopyNormalsNear(List<xxVertex> src, List<xxVertex> dest)
		{
			int len = (src.Count < dest.Count) ? src.Count : dest.Count;
			for (int i = 0; i < len; i++)
			{
				var destVert = dest[i];
				var destPos = destVert.Position;
				float minDistSq = Single.MaxValue;
				xxVertex nearestVert = null;
				foreach (xxVertex srcVert in src)
				{
					var srcPos = srcVert.Position;
					float[] diff = new float[] { destPos[0] - srcPos[0], destPos[1] - srcPos[1], destPos[2] - srcPos[2] };
					float distSq = (diff[0] * diff[0]) + (diff[1] * diff[1]) + (diff[2] * diff[2]);
					if (distSq < minDistSq)
					{
						minDistSq = distSq;
						nearestVert = srcVert;
					}
				}

				destVert.Normal = nearestVert.Normal;
			}
		}

		public static void CopyBonesOrder(List<xxVertex> src, List<xxVertex> dest)
		{
			int len = (src.Count < dest.Count) ? src.Count : dest.Count;
			for (int i = 0; i < len; i++)
			{
				dest[i].BoneIndices = (byte[])src[i].BoneIndices.Clone();
				dest[i].Weights3 = (float[])src[i].Weights3.Clone();
			}
		}

		public static void CopyBonesNear(List<xxVertex> src, List<xxVertex> dest)
		{
			int len = (src.Count < dest.Count) ? src.Count : dest.Count;
			for (int i = 0; i < len; i++)
			{
				var destVert = dest[i];
				var destPos = destVert.Position;
				float minDistSq = Single.MaxValue;
				xxVertex nearestVert = null;
				foreach (xxVertex srcVert in src)
				{
					var srcPos = srcVert.Position;
					float[] diff = new float[] { destPos[0] - srcPos[0], destPos[1] - srcPos[1], destPos[2] - srcPos[2] };
					float distSq = (diff[0] * diff[0]) + (diff[1] * diff[1]) + (diff[2] * diff[2]);
					if (distSq < minDistSq)
					{
						minDistSq = distSq;
						nearestVert = srcVert;
					}
				}

				destVert.BoneIndices = (byte[])nearestVert.BoneIndices.Clone();
				destVert.Weights3 = (float[])nearestVert.Weights3.Clone();
			}
		}

		public static void CalculateNormals(List<xxSubmesh> submeshes, float threshold)
		{
			var pairList = new List<Tuple<List<xxFace>, List<xxVertex>>>(submeshes.Count);
			for (int i = 0; i < submeshes.Count; i++)
			{
				pairList.Add(new Tuple<List<xxFace>, List<xxVertex>>(submeshes[i].FaceList, submeshes[i].VertexList));
			}
			CalculateNormals(pairList, threshold);
		}

		public static void CalculateNormals(List<Tuple<List<xxFace>, List<xxVertex>>> pairList, float threshold)
		{
			if (threshold < 0)
			{
				VertexRef[][] vertRefArray = new VertexRef[pairList.Count][];
				for (int i = 0; i < pairList.Count; i++)
				{
					List<xxVertex> vertList = pairList[i].Item2;
					vertRefArray[i] = new VertexRef[vertList.Count];
					for (int j = 0; j < vertList.Count; j++)
					{
						xxVertex vert = vertList[j];
						VertexRef vertRef = new VertexRef();
						vertRef.vert = vert;
						vertRef.norm = new Vector3();
						vertRefArray[i][j] = vertRef;
					}
				}

				for (int i = 0; i < pairList.Count; i++)
				{
					List<xxFace> faceList = pairList[i].Item1;
					for (int j = 0; j < faceList.Count; j++)
					{
						xxFace face = faceList[j];
						Vector3 v1 = vertRefArray[i][face.VertexIndices[1]].vert.Position - vertRefArray[i][face.VertexIndices[2]].vert.Position;
						Vector3 v2 = vertRefArray[i][face.VertexIndices[0]].vert.Position - vertRefArray[i][face.VertexIndices[2]].vert.Position;
						Vector3 norm = Vector3.Cross(v2, v1);
						norm.Normalize();
						for (int k = 0; k < face.VertexIndices.Length; k++)
						{
							vertRefArray[i][face.VertexIndices[k]].norm += norm;
						}
					}
				}

				for (int i = 0; i < vertRefArray.Length; i++)
				{
					for (int j = 0; j < vertRefArray[i].Length; j++)
					{
						Vector3 norm = vertRefArray[i][j].norm;
						norm.Normalize();
						vertRefArray[i][j].vert.Normal = norm;
					}
				}
			}
			else
			{
				int vertCount = 0;
				for (int i = 0; i < pairList.Count; i++)
				{
					vertCount += pairList[i].Item2.Count;
				}

				VertexRefComparerX vertRefComparerX = new VertexRefComparerX();
				List<VertexRef> vertRefListX = new List<VertexRef>(vertCount);
				VertexRef[][] vertRefArray = new VertexRef[pairList.Count][];
				for (int i = 0; i < pairList.Count; i++)
				{
					var vertList = pairList[i].Item2;
					vertRefArray[i] = new VertexRef[vertList.Count];
					for (int j = 0; j < vertList.Count; j++)
					{
						xxVertex vert = vertList[j];
						VertexRef vertRef = new VertexRef();
						vertRef.vert = vert;
						vertRef.norm = new Vector3();
						vertRefArray[i][j] = vertRef;
						vertRefListX.Add(vertRef);
					}
				}
				vertRefListX.Sort(vertRefComparerX);

				for (int i = 0; i < pairList.Count; i++)
				{
					var faceList = pairList[i].Item1;
					for (int j = 0; j < faceList.Count; j++)
					{
						xxFace face = faceList[j];
						Vector3 v1 = vertRefArray[i][face.VertexIndices[1]].vert.Position - vertRefArray[i][face.VertexIndices[2]].vert.Position;
						Vector3 v2 = vertRefArray[i][face.VertexIndices[0]].vert.Position - vertRefArray[i][face.VertexIndices[2]].vert.Position;
						Vector3 norm = Vector3.Cross(v2, v1);
						norm.Normalize();
						for (int k = 0; k < face.VertexIndices.Length; k++)
						{
							vertRefArray[i][face.VertexIndices[k]].norm += norm;
						}
					}
				}

				float squaredThreshold = threshold * threshold;
				while (vertRefListX.Count > 0)
				{
					VertexRef vertRef = vertRefListX[vertRefListX.Count - 1];
					List<VertexRef> dupList = new List<VertexRef>();
					List<VertexRef> dupListX = GetAxisDups(vertRef, vertRefListX, 0, threshold, null);
					foreach (VertexRef dupRef in dupListX)
					{
						if (((vertRef.vert.Position.Y - dupRef.vert.Position.Y) <= threshold) &&
							((vertRef.vert.Position.Z - dupRef.vert.Position.Z) <= threshold) &&
							((vertRef.vert.Position - dupRef.vert.Position).LengthSquared() <= squaredThreshold))
						{
							dupList.Add(dupRef);
						}
					}
					vertRefListX.RemoveAt(vertRefListX.Count - 1);

					Vector3 norm = vertRef.norm;
					foreach (VertexRef dupRef in dupList)
					{
						norm += dupRef.norm;
						vertRefListX.Remove(dupRef);
					}
					norm.Normalize();

					vertRef.vert.Normal = norm;
					foreach (VertexRef dupRef in dupList)
					{
						dupRef.vert.Normal = norm;
						vertRefListX.Remove(dupRef);
					}
				}
			}
		}

		private static List<VertexRef> GetAxisDups(VertexRef vertRef, List<VertexRef> compareList, int axis, float threshold, IComparer<VertexRef> binaryComparer)
		{
			List<VertexRef> dupList = new List<VertexRef>();
			int startIdx;
			if (binaryComparer == null)
			{
				startIdx = compareList.IndexOf(vertRef);
				if (startIdx < 0)
				{
					throw new Exception("Vertex wasn't found in the compare list");
				}
			}
			else
			{
				startIdx = compareList.BinarySearch(vertRef, binaryComparer);
				if (startIdx < 0)
				{
					startIdx = ~startIdx;
				}
				if (startIdx < compareList.Count)
				{
					VertexRef compRef = compareList[startIdx];
					if (System.Math.Abs(vertRef.vert.Position[axis] - compRef.vert.Position[axis]) <= threshold)
					{
						dupList.Add(compRef);
					}
				}
			}

			for (int i = startIdx + 1; i < compareList.Count; i++)
			{
				VertexRef compRef = compareList[i];
				if ((System.Math.Abs(vertRef.vert.Position[axis] - compRef.vert.Position[axis]) <= threshold))
				{
					dupList.Add(compRef);
				}
				else
				{
					break;
				}
			}
			for (int i = startIdx - 1; i >= 0; i--)
			{
				VertexRef compRef = compareList[i];
				if ((System.Math.Abs(vertRef.vert.Position[axis] - compRef.vert.Position[axis]) <= threshold))
				{
					dupList.Add(compRef);
				}
				else
				{
					break;
				}
			}
			return dupList;
		}

		public static bool RemoveUnusedBones(xxMesh mesh)
		{
			List<List<xxVertex>> vertexLists = new List<List<xxVertex>>(mesh.SubmeshList.Count);
			foreach (xxSubmesh submesh in mesh.SubmeshList)
			{
				vertexLists.Add(submesh.VertexList);
			}
			RemoveUnusedBones(vertexLists, mesh.BoneList);

			return (mesh.BoneList.Count > 0);
		}

		public static void RemoveUnusedBones(List<List<xxVertex>> vertexLists, List<xxBone> boneList)
		{
			byte[] boneMap = null;
			bool skinnedStart = (boneList.Count > 0);
			if (skinnedStart)
			{
				bool[] bonesUsed = IsBoneUsedList(vertexLists, boneList);

				boneMap = new byte[boneList.Count];
				int numRemoved = 0;
				for (int i = 0; i < bonesUsed.Length; i++)
				{
					int removeIdx = i - numRemoved;
					if (bonesUsed[i])
					{
						boneMap[i] = (byte)removeIdx;
						xxBone bone = boneList[removeIdx];
						bone.Index = removeIdx;
					}
					else
					{
						boneList.RemoveAt(removeIdx);
						numRemoved++;
					}
				}
			}

			bool skinnedEnd = (boneList.Count > 0);
			if (skinnedStart && skinnedEnd)
			{
				foreach (var vertList in vertexLists)
				{
					UpdateBoneIndices(vertList, boneMap);
				}
			}
			else if (skinnedStart && !skinnedEnd)
			{
				// skinned -> unskinned
				foreach (var vertList in vertexLists)
				{
					foreach (var vert in vertList)
					{
						vert.BoneIndices = new byte[4];
						vert.Weights3 = new float[3];
					}
				}
			}
		}

		private static bool[] IsBoneUsedList(List<List<xxVertex>> vertexLists, List<xxBone> boneList)
		{
			bool[] bonesUsed = new bool[boneList.Count];
			foreach (var vertList in vertexLists)
			{
				foreach (var vert in vertList)
				{
					foreach (byte boneIdx in vert.BoneIndices)
					{
						if (boneIdx != 0xFF)
						{
							bonesUsed[boneIdx] = true;
						}
					}
				}
			}
			return bonesUsed;
		}

		private static void UpdateBoneIndices(List<xxVertex> vertList, byte[] boneMap)
		{
			for (int i = 0; i < vertList.Count; i++)
			{
				xxVertex vert = vertList[i];
				byte[] boneIndices = (byte[])vert.BoneIndices.Clone();
				for (int j = 0; j < boneIndices.Length; j++)
				{
					if (boneIndices[j] != 0xFF)
					{
						boneIndices[j] = boneMap[boneIndices[j]];
					}
				}
				vert.BoneIndices = boneIndices;
			}
		}
	}
}
