using System;
using System.Collections.Generic;
using System.IO;
using SlimDX;

namespace SB3Utility
{
	public interface IObjInfo
	{
		void WriteTo(Stream stream);
	}

	#region .xx
	public class xxFrame : ObjChildren<xxFrame>, IObjChild, IObjInfo
	{
		public string Name { get; set; }
		public Matrix Matrix { get; set; }
		public byte[] Unknown1 { get; set; }
		public BoundingBox Bounds { get; set; }
		public byte[] Unknown2 { get; set; }
		public string Name2 { get; set; }
		public xxMesh Mesh { get; set; }

		public dynamic Parent { get; set; }
		
		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteName(Name);
			writer.Write(children.Count);
			writer.Write(Matrix);
			writer.Write(Unknown1);

			if (Mesh == null)
			{
				writer.Write((int)0);
			}
			else
			{
				writer.Write(Mesh.SubmeshList.Count);
			}

			writer.Write(Bounds.Minimum);
			writer.Write(Bounds.Maximum);
			writer.Write(Unknown2);

			if (Name2 != null)
			{
				writer.WriteName(Name2);
			}

			if (Mesh != null)
			{
				Mesh.WriteTo(stream);
			}

			for (int i = 0; i < children.Count; i++)
			{
				children[i].WriteTo(stream);
			}
		}

		public xxFrame Clone(bool mesh, bool childFrames)
		{
			xxFrame frame = new xxFrame();
			frame.InitChildren(children.Count);
			frame.Name = Name;
			frame.Matrix = Matrix;
			frame.Unknown1 = (byte[])Unknown1.Clone();
			frame.Bounds = Bounds;
			frame.Unknown2 = (byte[])Unknown2.Clone();
			if (Name2 != null)
			{
				frame.Name2 = Name2;
			}

			if (mesh && (Mesh != null))
			{
				frame.Mesh = Mesh.Clone(true, true, true);
			}
			if (childFrames)
			{
				for (int i = 0; i < children.Count; i++)
				{
					frame.AddChild(children[i].Clone(mesh, true));
				}
			}
			return frame;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class xxMesh : IObjInfo
	{
		public byte NumVector2PerVertex { get; set; }
		public List<xxSubmesh> SubmeshList { get; set; }
		public byte[] VertexListDuplicateUnknown { get; set; }
		public List<xxVertex> VertexListDuplicate { get; set; }
		public List<xxBone> BoneList { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(NumVector2PerVertex);

			for (int i = 0; i < SubmeshList.Count; i++)
			{
				SubmeshList[i].WriteTo(stream);
			}

			writer.Write((ushort)VertexListDuplicate.Count);
			writer.Write(VertexListDuplicateUnknown);
			for (int i = 0; i < VertexListDuplicate.Count; i++)
			{
				VertexListDuplicate[i].WriteTo(stream);
			}

			writer.Write(BoneList.Count);
			for (int i = 0; i < BoneList.Count; i++)
			{
				BoneList[i].WriteTo(stream);
			}
		}

		public xxMesh Clone(bool submeshes, bool vertexListDup, bool boneList)
		{
			xxMesh mesh = new xxMesh();
			mesh.SubmeshList = new List<xxSubmesh>(SubmeshList.Count);
			mesh.NumVector2PerVertex = NumVector2PerVertex;
			mesh.VertexListDuplicateUnknown = (byte[])VertexListDuplicateUnknown.Clone();
			mesh.VertexListDuplicate = new List<xxVertex>(VertexListDuplicate.Count);
			mesh.BoneList = new List<xxBone>(BoneList.Count);

			if (submeshes)
			{
				for (int i = 0; i < SubmeshList.Count; i++)
				{
					mesh.SubmeshList.Add(SubmeshList[i].Clone());
				}
			}
			if (vertexListDup)
			{
				for (int i = 0; i < VertexListDuplicate.Count; i++)
				{
					mesh.VertexListDuplicate.Add(VertexListDuplicate[i].Clone());
				}
			}
			if (boneList)
			{
				for (int i = 0; i < BoneList.Count; i++)
				{
					mesh.BoneList.Add(BoneList[i].Clone());
				}
			}
			return mesh;
		}
	}

	public class xxSubmesh : IObjInfo
	{
		public byte[] Unknown1 { get; set; }
		public int MaterialIndex { get; set; }
		public List<xxFace> FaceList { get; set; }
		public List<xxVertex> VertexList { get; set; }
		public byte[] Unknown2 { get; set; }
		public List<List<Vector2>> Vector2Lists { get; set; }
		public byte[] Unknown3 { get; set; }
		public byte[] Unknown4 { get; set; }
		public byte[] Unknown5 { get; set; }
		public byte[] Unknown6 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Unknown1);
			writer.Write(MaterialIndex);

			writer.Write(FaceList.Count * 3);
			for (int i = 0; i < FaceList.Count; i++)
			{
				FaceList[i].WriteTo(stream);
			}

			writer.Write(VertexList.Count);
			for (int i = 0; i < VertexList.Count; i++)
			{
				VertexList[i].WriteTo(stream);
			}

			writer.WriteIfNotNull(Unknown2);

			if (Vector2Lists != null)
			{
				for (int i = 0; i < Vector2Lists.Count; i++)
				{
					for (int j = 0; j < Vector2Lists[i].Count; j++)
					{
						writer.Write(Vector2Lists[i][j]);
					}
				}
			}

			writer.WriteIfNotNull(Unknown3);
			writer.WriteIfNotNull(Unknown4);
			writer.WriteIfNotNull(Unknown5);
			writer.WriteIfNotNull(Unknown6);
		}

		public xxSubmesh Clone()
		{
			xxSubmesh submesh = new xxSubmesh();
			submesh.Unknown1 = (byte[])Unknown1.Clone();
			submesh.MaterialIndex = MaterialIndex;
			submesh.FaceList = new List<xxFace>(FaceList.Count);
			for (int i = 0; i < FaceList.Count; i++)
			{
				submesh.FaceList.Add(FaceList[i].Clone());
			}
			submesh.VertexList = new List<xxVertex>(VertexList.Count);
			for (int i = 0; i < VertexList.Count; i++)
			{
				submesh.VertexList.Add(VertexList[i].Clone());
			}

			submesh.Unknown2 = Unknown2.CloneIfNotNull();

			if (Vector2Lists != null)
			{
				submesh.Vector2Lists = new List<List<Vector2>>(Vector2Lists.Count);
				for (int i = 0; i < Vector2Lists.Count; i++)
				{
					List<Vector2> vectorList = new List<Vector2>(Vector2Lists[i].Count);
					submesh.Vector2Lists.Add(vectorList);
					for (int j = 0; j < Vector2Lists[i].Count; j++)
					{
						vectorList.Add(Vector2Lists[i][j]);
					}
				}
			}

			submesh.Unknown3 = Unknown3.CloneIfNotNull();
			submesh.Unknown4 = Unknown4.CloneIfNotNull();
			submesh.Unknown5 = Unknown5.CloneIfNotNull();
			submesh.Unknown6 = Unknown6.CloneIfNotNull();

			return submesh;
		}
	}

	public class xxFace : IObjInfo
	{
		public ushort[] VertexIndices { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(VertexIndices);
		}

		public xxFace Clone()
		{
			xxFace face = new xxFace();
			face.VertexIndices = (ushort[])VertexIndices.Clone();
			return face;
		}
	}

	public class xxVertexUShort : xxVertex
	{
		protected override void WriteIndex(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write((ushort)Index);
		}

		protected override xxVertex InitClone()
		{
			return new xxVertexUShort();
		}
	}

	public class xxVertexInt : xxVertex
	{
		protected override void WriteIndex(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Index);
		}

		protected override xxVertex InitClone()
		{
			return new xxVertexInt();
		}
	}

	public abstract class xxVertex : IObjInfo
	{
		public int Index { get; set; }
		public Vector3 Position { get; set; }
		public float[] Weights3 { get; set; }
		public byte[] BoneIndices { get; set; }
		public Vector3 Normal { get; set; }
		public float[] UV { get; set; }
		public byte[] Unknown1 { get; set; }

		public float[] Weights4(bool skinned)
		{
			float[] w3 = Weights3;
			float[] w4;
			if (skinned)
			{
				w4 = new float[] { w3[0], w3[1], w3[2], 1f - (w3[0] + w3[1] + w3[2]) };
				if (w4[3] < 0)
				{
					w4[3] = 0;
				}
			}
			else
			{
				w4 = new float[4];
			}

			return w4;
		}

		protected abstract void WriteIndex(Stream stream);

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			WriteIndex(stream);
			writer.Write(Position);
			writer.Write(Weights3);
			writer.Write(BoneIndices);
			writer.Write(Normal);
			writer.Write(UV);

			writer.WriteIfNotNull(Unknown1);
		}

		protected abstract xxVertex InitClone();

		public xxVertex Clone()
		{
			xxVertex vertex = InitClone();
			vertex.Index = Index;
			vertex.Position = Position;
			vertex.Weights3 = (float[])Weights3.Clone();
			vertex.BoneIndices = (byte[])BoneIndices.Clone();
			vertex.Normal = Normal;
			vertex.UV = (float[])UV.Clone();
			vertex.Unknown1 = Unknown1.CloneIfNotNull();
			return vertex;
		}
	}

	public class xxBone : IObjInfo
	{
		public string Name { get; set; }
		public int Index { get; set; }
		public Matrix Matrix { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteName(Name);
			writer.Write(Index);
			writer.Write(Matrix);
		}

		public xxBone Clone()
		{
			xxBone bone = new xxBone();
			bone.Name = Name;
			bone.Index = Index;
			bone.Matrix = Matrix;
			return bone;
		}
	}

	public class xxMaterial : IObjInfo
	{
		public string Name { get; set; }
		public Color4 Diffuse { get; set; }
		public Color4 Ambient { get; set; }
		public Color4 Specular { get; set; }
		public Color4 Emissive { get; set; }
		public float Power { get; set; }
		public xxMaterialTexture[] Textures { get; set; }
		public byte[] Unknown1 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteName(Name);
			writer.Write(Diffuse);
			writer.Write(Ambient);
			writer.Write(Specular);
			writer.Write(Emissive);
			writer.Write(Power);

			for (int i = 0; i < Textures.Length; i++)
			{
				Textures[i].WriteTo(stream);
			}

			writer.Write(Unknown1);
		}

		public xxMaterial Clone()
		{
			xxMaterial mat = new xxMaterial();
			mat.Name = Name;
			mat.Diffuse = Diffuse;
			mat.Ambient = Ambient;
			mat.Specular = Specular;
			mat.Emissive = Emissive;
			mat.Power = Power;
			mat.Textures = new xxMaterialTexture[Textures.Length];
			for (int i = 0; i < Textures.Length; i++)
			{
				mat.Textures[i] = Textures[i].Clone();
			}
			mat.Unknown1 = (byte[])Unknown1.Clone();
			return mat;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class xxMaterialTexture : IObjInfo
	{
		public string Name { get; set; }
		public byte[] Unknown1 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteName(Name);
			writer.Write(Unknown1);
		}

		public xxMaterialTexture Clone()
		{
			xxMaterialTexture matTex = new xxMaterialTexture();
			matTex.Name = Name;
			matTex.Unknown1 = (byte[])Unknown1.Clone();
			return matTex;
		}
	}

	public class xxTexture : IObjInfo
	{
		public string Name { get; set; }
		public byte[] Unknown1 { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Depth { get; set; }
		public int MipLevels { get; set; }
		public int Format { get; set; }
		public int ResourceType { get; set; }
		public int ImageFileFormat { get; set; }
		public byte Checksum { get; set; }
		public byte[] ImageData { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteName(Name);
			writer.Write(Unknown1);

			writer.Write(Width);
			writer.Write(Height);
			writer.Write(Depth);
			writer.Write(MipLevels);
			writer.Write(Format);
			writer.Write(ResourceType);
			writer.Write(ImageFileFormat);
			writer.Write(Checksum);

			writer.Write(ImageData.Length);
			writer.Write(ImageData);
		}

		public xxTexture Clone()
		{
			xxTexture tex = new xxTexture();
			tex.Name = Name;
			tex.Unknown1 = (byte[])Unknown1.Clone();
			tex.Width = Width;
			tex.Height = Height;
			tex.Depth = Depth;
			tex.MipLevels = MipLevels;
			tex.Format = Format;
			tex.ResourceType = ResourceType;
			tex.ImageFileFormat = ImageFileFormat;
			tex.Checksum = Checksum;
			tex.ImageData = (byte[])ImageData.Clone();
			return tex;
		}

		public override string ToString()
		{
			return Name;
		}
	}
	#endregion

	#region .xa
	public class xaMaterialSection : IObjInfo
	{
		public List<xaMaterial> MaterialList { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);

			writer.Write(MaterialList.Count);
			for (int i = 0; i < MaterialList.Count; i++)
			{
				MaterialList[i].WriteTo(stream);
			}
		}
	}

	public class xaMaterial : IObjInfo
	{
		public string Name { get; set; }
		public List<xaMaterialColor> ColorList { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteName(Name);

			writer.Write(ColorList.Count);
			for (int i = 0; i < ColorList.Count; i++)
			{
				ColorList[i].WriteTo(stream);
			}
		}
	}

	public class xaMaterialColor : IObjInfo
	{
		public Color4 Diffuse { get; set; }
		public Color4 Ambient { get; set; }
		public Color4 Specular { get; set; }
		public Color4 Emissive { get; set; }
		public float Power { get; set; }
		public byte[] Unknown1 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Diffuse);
			writer.Write(Ambient);
			writer.Write(Specular);
			writer.Write(Emissive);
			writer.Write(Power);
			writer.Write(Unknown1);
		}
	}

	public class xaSection2 : IObjInfo
	{
		public List<xaSection2Item> ItemList { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);

			writer.Write(ItemList.Count);
			for (int i = 0; i < ItemList.Count; i++)
			{
				ItemList[i].WriteTo(stream);
			}
		}
	}

	public class xaSection2Item : IObjInfo
	{
		public byte[] Unknown1 { get; set; }
		public string Name { get; set; }
		public byte[] Unknown2 { get; set; }
		public byte[] Unknown3 { get; set; }
		public byte[] Unknown4 { get; set; }
		public List<xaSection2ItemBlock> ItemBlockList { get; set; }
		public byte[] Unknown5 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Unknown1);
			writer.WriteName(Name);
			writer.Write(Unknown2);
			writer.Write(Unknown3);
			writer.Write(ItemBlockList.Count);
			writer.Write(Unknown4);

			for (int i = 0; i < ItemBlockList.Count; i++)
			{
				ItemBlockList[i].WriteTo(stream);
			}

			writer.Write(Unknown5);
		}
	}

	public class xaSection2ItemBlock : IObjInfo
	{
		public byte[] Unknown1 { get; set; }
		public byte[] Unknown2 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Unknown1);
			writer.Write(Unknown2);
		}
	}

	public class xaMorphSection : IObjInfo
	{
		public List<xaMorphIndexSet> IndexSetList { get; set; }
		public List<xaMorphKeyframe> KeyframeList { get; set; }
		public List<xaMorphClip> ClipList { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);

			writer.Write(IndexSetList.Count);
			for (int i = 0; i < IndexSetList.Count; i++)
			{
				IndexSetList[i].WriteTo(stream);
			}

			writer.Write(KeyframeList.Count);
			for (int i = 0; i < KeyframeList.Count; i++)
			{
				KeyframeList[i].WriteTo(stream);
			}

			writer.Write(ClipList.Count);
			for (int i = 0; i < ClipList.Count; i++)
			{
				ClipList[i].WriteTo(stream);
			}
		}
	}

	public class xaMorphIndexSet : IObjInfo
	{
		public byte[] Unknown1 { get; set; }
		public ushort[] MeshIndices { get; set; }
		public ushort[] MorphIndices { get; set; }
		public string Name { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);

			writer.Write(Unknown1);

			writer.Write(MeshIndices.Length);
			for (int i = 0; i < MeshIndices.Length; i++)
			{
				writer.Write(MeshIndices[i]);
			}
			for (int i = 0; i < MorphIndices.Length; i++)
			{
				writer.Write(MorphIndices[i]);
			}

			writer.WriteName(Name);
		}
	}

	public class xaMorphKeyframe : IObjInfo
	{
		public List<Vector3> PositionList { get; set; }
		public List<Vector3> NormalList { get; set; }
		public string Name { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);

			writer.Write(PositionList.Count);
			for (int i = 0; i < PositionList.Count; i++)
			{
				writer.Write(PositionList[i]);
			}
			for (int i = 0; i < NormalList.Count; i++)
			{
				writer.Write(NormalList[i]);
			}

			writer.WriteName(Name);
		}
	}

	public class xaMorphClip : IObjInfo
	{
		public string MeshName { get; set; }
		public string Name { get; set; }
		public List<xaMorphKeyframeRef> KeyframeRefList { get; set; }
		public byte[] Unknown1 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteName(MeshName);
			writer.WriteName(Name);

			writer.Write(KeyframeRefList.Count);
			for (int i = 0; i < KeyframeRefList.Count; i++)
			{
				KeyframeRefList[i].WriteTo(stream);
			}

			writer.Write(Unknown1);
		}
	}

	public class xaMorphKeyframeRef : IObjInfo
	{
		public byte[] Unknown1 { get; set; }
		public int Index { get; set; }
		public byte[] Unknown2 { get; set; }
		public string Name { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Unknown1);
			writer.Write(Index);
			writer.Write(Unknown2);
			writer.WriteName(Name);
		}
	}

	public class xaSection4 : IObjInfo
	{
		public List<List<xaSection4Item>> ItemListList { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);

			writer.Write(ItemListList.Count);
			for (int i = 0; i < ItemListList.Count; i++)
			{
				writer.Write(ItemListList[i].Count);
				for (int j = 0; j < ItemListList[i].Count; j++)
				{
					ItemListList[i][j].WriteTo(stream);
				}
			}
		}
	}

	public class xaSection4Item : IObjInfo
	{
		public byte[] Unknown1 { get; set; }
		public byte[] Unknown2 { get; set; }
		public byte[] Unknown3 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Unknown1);
			writer.Write(Unknown2);
			writer.Write(Unknown3);
		}
	}

	public class xaAnimationSection : IObjInfo
	{
		public List<xaAnimationClip> ClipList { get; set; }
		public List<xaAnimationTrack> TrackList { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);

			for (int i = 0; i < ClipList.Count; i++)
			{
				ClipList[i].WriteTo(stream);
			}

			writer.Write(TrackList.Count);
			for (int i = 0; i < TrackList.Count; i++)
			{
				TrackList[i].WriteTo(stream);
			}
		}
	}

	public class xaAnimationClip : IObjInfo
	{
		public string Name { get; set; }
		public float Speed { get; set; }
		public byte[] Unknown1 { get; set; }
		public float Start { get; set; }
		public float End { get; set; }
		public byte[] Unknown2 { get; set; }
		public byte[] Unknown3 { get; set; }
		public byte[] Unknown4 { get; set; }
		public int Next { get; set; }
		public byte[] Unknown5 { get; set; }
		public byte[] Unknown6 { get; set; }
		public byte[] Unknown7 { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteNameWithoutLength(Name, 64);
			writer.Write(Speed);
			writer.Write(Unknown1);
			writer.Write(Start);
			writer.Write(End);
			writer.Write(Unknown2);
			writer.Write(Unknown3);
			writer.Write(Unknown4);
			writer.Write(Next);
			writer.Write(Unknown5);
			writer.Write(Unknown6);
			writer.Write(Unknown7);
		}
	}

	public class xaAnimationTrack : IObjInfo
	{
		public string Name { get; set; }
		public byte[] Unknown1 { get; set; }
		public List<xaAnimationKeyframe> KeyframeList { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.WriteName(Name);
			writer.Write(KeyframeList.Count);
			writer.Write(Unknown1);

			for (int i = 0; i < KeyframeList.Count; i++)
			{
				KeyframeList[i].WriteTo(stream);
			}
		}
	}

	public class xaAnimationKeyframe : IObjInfo
	{
		public int Index { get; set; }
		public Quaternion Rotation { get; set; }
		public byte[] Unknown1 { get; set; }
		public Vector3 Translation { get; set; }
		public Vector3 Scaling { get; set; }

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Index);
			writer.Write(Rotation);
			writer.Write(Unknown1);
			writer.Write(Translation);
			writer.Write(Scaling);
		}
	}
	#endregion
}
