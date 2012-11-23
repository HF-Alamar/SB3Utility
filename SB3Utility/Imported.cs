using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;

namespace SB3Utility
{
	public interface IImported
	{
		List<ImportedFrame> FrameList { get; }
		List<ImportedMesh> MeshList { get; }
		List<ImportedMaterial> MaterialList { get; }
		List<ImportedTexture> TextureList { get; }
		List<ImportedAnimation> AnimationList { get; }
		List<ImportedMorph> MorphList { get; }
	}

	public class ImportedFrame : ObjChildren<ImportedFrame>, IObjChild
	{
		public string Name { get; set; }
		public Matrix Matrix { get; set; }

		public dynamic Parent { get; set; }
	}

	public class ImportedMesh
	{
		public string Name { get; set; }
		public List<ImportedSubmesh> SubmeshList { get; set; }
		public List<ImportedBone> BoneList { get; set; }
	}

	public class ImportedSubmesh
	{
		public List<ImportedVertex> VertexList { get; set; }
		public List<ImportedFace> FaceList { get; set; }
		public string Material { get; set; }
		public int Index { get; set; }
		public bool WorldCoords { get; set; }
	}

	public class ImportedVertex
	{
		public Vector3 Position { get; set; }
		public float[] Weights { get; set; }
		public byte[] BoneIndices { get; set; }
		public Vector3 Normal { get; set; }
		public float[] UV { get; set; }
	}

	public class ImportedFace
	{
		public int[] VertexIndices { get; set; }
	}

	public class ImportedBone
	{
		public string Name { get; set; }
		public Matrix Matrix { get; set; }
	}

	public class ImportedMaterial
	{
		public string Name { get; set; }
		public Color4 Diffuse { get; set; }
		public Color4 Ambient { get; set; }
		public Color4 Specular { get; set; }
		public Color4 Emissive { get; set; }
		public float Power { get; set; }
		public string[] Textures { get; set; }
	}

	public class ImportedTexture
	{
		public string Name { get; set; }
		public byte[] Data { get; set; }

		public ImportedTexture()
		{
		}

		public ImportedTexture(string path)
		{
			Name = Path.GetFileName(path);
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
			{
				Data = reader.ReadBytes((int)reader.BaseStream.Length);
			}
		}

		public ImportedTexture(Stream stream, string name)
		{
			Name = name;
			using (BinaryReader reader = new BinaryReader(stream))
			{
				Data = reader.ReadToEnd();
			}
		}
	}

	public class ImportedAnimation
	{
		public List<ImportedAnimationTrack> TrackList { get; set; }
	}

	public class ImportedAnimationTrack
	{
		public string Name { get; set; }
		public ImportedAnimationKeyframe[] Keyframes { get; set; }
	}

	public class ImportedAnimationKeyframe
	{
		public Vector3 Scaling { get; set; }
		public Quaternion Rotation { get; set; }
		public Vector3 Translation { get; set; }
	}

	public class ImportedMorph
	{
		/// <summary>
		/// Target mesh name
		/// </summary>
		public string Name { get; set; }
		public List<ImportedMorphKeyframe> KeyframeList { get; set; }
	}

	public class ImportedMorphKeyframe
	{
		/// <summary>
		/// Blend shape name
		/// </summary>
		public string Name { get; set; }
		public List<ImportedVertex> VertexList { get; set; }
	}

	public static class ImportedHelpers
	{
		public static ImportedFrame FindFrame(String name, ImportedFrame root)
		{
			ImportedFrame frame = root;
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

		public static ImportedMesh FindMesh(String frameName, IImported imported)
		{
			foreach (ImportedMesh mesh in imported.MeshList)
			{
				if (mesh.Name == frameName)
				{
					return mesh;
				}
			}

			return null;
		}

		public static ImportedMaterial FindMaterial(String name, IImported imported)
		{
			foreach (ImportedMaterial mat in imported.MaterialList)
			{
				if (mat.Name == name)
				{
					return mat;
				}
			}

			return null;
		}

		public static ImportedTexture FindTexture(String name, IImported imported)
		{
			if (name == null || name == String.Empty)
			{
				return null;
			}

			foreach (ImportedTexture tex in imported.TextureList)
			{
				if (tex.Name == name)
				{
					return tex;
				}
			}

			return null;
		}
	}
}
