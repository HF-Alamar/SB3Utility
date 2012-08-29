using System;
using System.Collections.Generic;
using System.IO;
using SlimDX;

namespace SB3Utility
{
	public class xxParser : IWriteFile
	{
		public byte[] Header { get; set; }
		public xxFrame Frame { get; set; }
		public byte[] MaterialSectionUnknown { get; set; }
		public List<xxMaterial> MaterialList { get; set; }
		public List<xxTexture> TextureList { get; set; }
		public byte[] Footer { get; set; }

		public string Name { get; set; }
		public int Format { get; set; }

		protected BinaryReader reader;

		public xxParser(Stream stream, string name)
			: this(stream)
		{
			this.Name = name;
		}

		public xxParser(Stream stream)
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				this.reader = reader;

				Format = 0;
				byte[] formatBuf = reader.ReadBytes(5);
				if ((formatBuf[0] >= 0x01) && (BitConverter.ToInt32(formatBuf, 1) == 0))
				{
					Format = BitConverter.ToInt32(formatBuf, 0);
				}
				else
				{
					uint id = BitConverter.ToUInt32(formatBuf, 0);
					if ((id == 0x3F8F5C29) || (id == 0x3F90A3D7) || (id == 0x3F91EB85) || (id == 0x3F933333) ||
						(id == 0x3F947AE1) || (id == 0x3F95C28F) || (id == 0x3F970A3D) || (id == 0x3F99999A) ||
						(id == 0x3FA66666) || (id == 0x3FB33333))
					{
						Format = -1;
					}
				}

				byte[] headerBuf;
				int headerBufLen;
				if (Format >= 1)
				{
					headerBufLen = 26;
				}
				else
				{
					headerBufLen = 21;
				}
				headerBuf = new byte[headerBufLen];
				formatBuf.CopyTo(headerBuf, 0);
				reader.ReadBytes(headerBufLen - formatBuf.Length).CopyTo(headerBuf, formatBuf.Length);
				Header = headerBuf;

				this.Frame = ParseFrame();

				this.MaterialSectionUnknown = reader.ReadBytes(4);
				int numMaterials = reader.ReadInt32();
				this.MaterialList = new List<xxMaterial>(numMaterials);
				for (int i = 0; i < numMaterials; i++)
				{
					MaterialList.Add(ParseMaterial());
				}

				int numTextures = reader.ReadInt32();
				this.TextureList = new List<xxTexture>(numTextures);
				for (int i = 0; i < numTextures; i++)
				{
					TextureList.Add(ParseTexture());
				}

				if (Format >= 2)
				{
					Footer = reader.ReadBytes(10);
				}

				if (reader.ReadBytes(1).Length > 0)
				{
					throw new Exception("Parsing " + Name + " finished before the end of the file");
				}

				this.reader = null;
			}
		}

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Header);

			Frame.WriteTo(stream);

			writer.Write(MaterialSectionUnknown);
			writer.Write(MaterialList.Count);
			for (int i = 0; i < MaterialList.Count; i++)
			{
				MaterialList[i].WriteTo(stream);
			}

			writer.Write(TextureList.Count);
			for (int i = 0; i < TextureList.Count; i++)
			{
				TextureList[i].WriteTo(stream);
			}

			if (Footer != null)
			{
				writer.Write(Footer);
			}
		}

		protected xxFrame ParseFrame()
		{
			xxFrame frame = new xxFrame();
			frame.Name = reader.ReadName();

			int numChildFrames = reader.ReadInt32();
			frame.InitChildren(numChildFrames);

			frame.Matrix = reader.ReadMatrix();
			frame.Unknown1 = (Format >= 7) ? reader.ReadBytes(32) : reader.ReadBytes(16);

			int numSubmeshes = reader.ReadInt32();

			frame.Bounds = new BoundingBox(reader.ReadVector3(), reader.ReadVector3());
			frame.Unknown2 = (Format >= 7) ? reader.ReadBytes(64) : reader.ReadBytes(16);

			if (Format >= 6)
			{
				frame.Name2 = reader.ReadName();
			}

			if (numSubmeshes > 0)
			{
				xxMesh mesh = new xxMesh();
				frame.Mesh = mesh;
				mesh.NumVector2PerVertex = reader.ReadByte();

				mesh.SubmeshList = new List<xxSubmesh>(numSubmeshes);
				for (int i = 0; i < numSubmeshes; i++)
				{
					xxSubmesh submesh = new xxSubmesh();
					mesh.SubmeshList.Add(submesh);

					submesh.Unknown1 = (Format >= 7) ? reader.ReadBytes(64) : reader.ReadBytes(16);
					submesh.MaterialIndex = reader.ReadInt32();

					submesh.FaceList = ParseFaceList();
					submesh.VertexList = ParseVertexList();

					if (Format >= 7)
					{
						submesh.Unknown2 = reader.ReadBytes(20);
					}

					if (mesh.NumVector2PerVertex > 0)
					{
						submesh.Vector2Lists = new List<List<Vector2>>(submesh.VertexList.Count);
						for (int j = 0; j < submesh.VertexList.Count; j++)
						{
							List<Vector2> vectorList = new List<Vector2>(mesh.NumVector2PerVertex);
							submesh.Vector2Lists.Add(vectorList);
							for (byte k = 0; k < mesh.NumVector2PerVertex; k++)
							{
								vectorList.Add(reader.ReadVector2());
							}
						}
					}

					if (Format >= 2)
					{
						submesh.Unknown3 = reader.ReadBytes(100); // 96 + 4
					}

					if (Format >= 7)
					{
						submesh.Unknown4 = reader.ReadBytes(284); // 256 + 28

						if (Format >= 8)
						{
							submesh.Unknown5 = reader.ReadBytes(21); // 1 + 4 + 12 + 4
						}
					}
					else
					{
						if (Format >= 3)
						{
							submesh.Unknown4 = reader.ReadBytes(64);
						}
						if (Format >= 5)
						{
							submesh.Unknown5 = reader.ReadBytes(20);
						}
						if (Format >= 6)
						{
							submesh.Unknown6 = reader.ReadBytes(28);
						}
					}
				}

				ushort numVerticesDup = reader.ReadUInt16();
				mesh.VertexListDuplicate = new List<xxVertex>(numVerticesDup);
				mesh.VertexListDuplicateUnknown = reader.ReadBytes(8);  // 4 + 4
				for (int i = 0; i < numVerticesDup; i++)
				{
					mesh.VertexListDuplicate.Add(ParseVertex());
				}

				mesh.BoneList = ParseBoneList();
			}

			for (int i = 0; i < numChildFrames; i++)
			{
				frame.AddChild(ParseFrame());
			}

			return frame;
		}

		protected List<xxFace> ParseFaceList()
		{
			int numFaces = reader.ReadInt32() / 3;
			List<xxFace> faceList = new List<xxFace>(numFaces);
			for (int i = 0; i < numFaces; i++)
			{
				xxFace face = new xxFace();
				faceList.Add(face);
				face.VertexIndices = reader.ReadUInt16Array(3);
			}
			return faceList;
		}

		protected List<xxVertex> ParseVertexList()
		{
			int numVertices = reader.ReadInt32();
			List<xxVertex> vertexList = new List<xxVertex>(numVertices);
			for (int i = 0; i < numVertices; i++)
			{
				vertexList.Add(ParseVertex());
			}
			return vertexList;
		}

		protected xxVertex ParseVertex()
		{
			xxVertex vertex;
			if (Format >= 4)
			{
				vertex = new xxVertexUShort();
				vertex.Index = reader.ReadUInt16();
			}
			else
			{
				vertex = new xxVertexInt();
				vertex.Index = reader.ReadInt32();
			}

			vertex.Position = reader.ReadVector3();
			vertex.Weights3 = reader.ReadSingleArray(3);
			vertex.BoneIndices = reader.ReadBytes(4);
			vertex.Normal = reader.ReadVector3();
			vertex.UV = reader.ReadSingleArray(2);

			if (Format >= 4)
			{
				vertex.Unknown1 = reader.ReadBytes(20);
			}
			return vertex;
		}

		protected List<xxBone> ParseBoneList()
		{
			int numBones = reader.ReadInt32();
			List<xxBone> boneList = new List<xxBone>(numBones);
			for (int i = 0; i < numBones; i++)
			{
				xxBone bone = new xxBone();
				boneList.Add(bone);

				bone.Name = reader.ReadName();
				bone.Index = reader.ReadInt32();
				bone.Matrix = reader.ReadMatrix();
			}
			return boneList;
		}

		protected xxMaterial ParseMaterial()
		{
			xxMaterial mat = new xxMaterial();
			mat.Name = reader.ReadName();
			mat.Diffuse = reader.ReadColor4();
			mat.Ambient = reader.ReadColor4();
			mat.Specular = reader.ReadColor4();
			mat.Emissive = reader.ReadColor4();
			mat.Power = reader.ReadSingle();

			mat.Textures = new xxMaterialTexture[4];
			for (int i = 0; i < mat.Textures.Length; i++)
			{
				mat.Textures[i] = new xxMaterialTexture();
				mat.Textures[i].Name = reader.ReadName();
				mat.Textures[i].Unknown1 = reader.ReadBytes(16);
			}

			if (Format < 0)
			{
				mat.Unknown1 = reader.ReadBytes(4);
			}
			else
			{
				mat.Unknown1 = reader.ReadBytes(88);
			}
			return mat;
		}

		protected xxTexture ParseTexture()
		{
			xxTexture tex = new xxTexture();
			tex.Name = reader.ReadName();
			tex.Unknown1 = reader.ReadBytes(4);
			tex.Width = reader.ReadInt32();
			tex.Height = reader.ReadInt32();
			tex.Depth = reader.ReadInt32();
			tex.MipLevels = reader.ReadInt32();
			tex.Format = reader.ReadInt32();
			tex.ResourceType = reader.ReadInt32();
			tex.ImageFileFormat = reader.ReadInt32();
			tex.Checksum = reader.ReadByte();

			int imgDataLen = reader.ReadInt32();
			tex.ImageData = reader.ReadBytes(imgDataLen);
			return tex;
		}
	}
}
