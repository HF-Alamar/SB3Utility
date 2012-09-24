using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace SB3Utility
{
	public enum CopyFrameMethod
	{
		MergeFrame,
		AddFrame,
		ReplaceFrame
	}

	public enum CopyMeshMethod
	{
		Replace,
		CopyNear,
		CopyOrder
	}

	public static partial class xx
	{
		public static xxFrame CreateFrame(ImportedFrame frame)
		{
			xxFrame xxFrame = new xxFrame();
			xxFrame.Matrix = frame.Matrix;
			xxFrame.Bounds = new BoundingBox();
			xxFrame.Name = frame.Name;

			xxFrame.InitChildren(frame.Count);
			for (int i = 0; i < frame.Count; i++)
			{
				xxFrame.AddChild(CreateFrame(frame[i]));
			}

			return xxFrame;
		}

		public static void CopyOrCreateUnknowns(xxFrame dest, xxFrame root, int xxFormat)
		{
			xxFrame src = FindFrame(dest.Name, root);
			if (src == null)
			{
				CreateUnknowns(dest, xxFormat);
			}
			else
			{
				CopyUnknowns(src, dest);
			}

			for (int i = 0; i < dest.Count; i++)
			{
				CopyOrCreateUnknowns(dest[i], root, xxFormat);
			}
		}

		public static List<xxBone> CreateBoneList(List<ImportedBone> boneList)
		{
			List<xxBone> xxBoneList = new List<xxBone>(boneList.Count);
			for (int i = 0; i < boneList.Count; i++)
			{
				xxBone xxBone = new xxBone();
				xxBone.Name = boneList[i].Name;
				xxBone.Matrix = boneList[i].Matrix;
				xxBone.Index = i;
				xxBoneList.Add(xxBone);
			}
			return xxBoneList;
		}

		public static xxMesh CreateMesh(WorkspaceMesh mesh, int xxFormat, out string[] materialNames, out int[] indices, out bool[] worldCoords, out bool[] replaceSubmeshesOption)
		{
			int numUncheckedSubmeshes = 0;
			foreach (ImportedSubmesh submesh in mesh.SubmeshList)
			{
				if (!mesh.isSubmeshEnabled(submesh))
					numUncheckedSubmeshes++;
			}
			int numSubmeshes = mesh.SubmeshList.Count - numUncheckedSubmeshes;
			materialNames = new string[numSubmeshes];
			indices = new int[numSubmeshes];
			worldCoords = new bool[numSubmeshes];
			replaceSubmeshesOption = new bool[numSubmeshes];

			xxMesh xxMesh = new xxMesh();
			xxMesh.BoneList = CreateBoneList(mesh.BoneList);

			xxMesh.SubmeshList = new List<xxSubmesh>(mesh.SubmeshList.Count);
			for (int i = 0, submeshIdx = 0; i < numSubmeshes; i++, submeshIdx++)
			{
				while (!mesh.isSubmeshEnabled(mesh.SubmeshList[submeshIdx]))
					submeshIdx++;

				xxSubmesh xxSubmesh = new xxSubmesh();
				xxMesh.SubmeshList.Add(xxSubmesh);

				xxSubmesh.MaterialIndex = -1;
				materialNames[i] = mesh.SubmeshList[submeshIdx].Material;
				indices[i] = mesh.SubmeshList[submeshIdx].Index;
				worldCoords[i] = mesh.SubmeshList[submeshIdx].WorldCoords;
				replaceSubmeshesOption[i] = mesh.isSubmeshReplacingOriginal(mesh.SubmeshList[submeshIdx]);

				List<ImportedVertex> vertexList = mesh.SubmeshList[submeshIdx].VertexList;
				List<xxVertex> xxVertexList = new List<xxVertex>(vertexList.Count);
				for (int j = 0; j < vertexList.Count; j++)
				{
					ImportedVertex vert = vertexList[j];
					xxVertex xxVertex;
					if (xxFormat >= 4)
					{
						xxVertex = new xxVertexUShort();
					}
					else
					{
						xxVertex = new xxVertexInt();
					}

					xxVertex.Index = j;
					xxVertex.Normal = vert.Normal;
					xxVertex.UV = (float[])vert.UV.Clone();
					xxVertex.Weights3 = new float[3] { vert.Weights[0], vert.Weights[1], vert.Weights[2] };
					xxVertex.BoneIndices = (byte[])vert.BoneIndices.Clone();
					xxVertex.Position = vert.Position;
					xxVertexList.Add(xxVertex);
				}
				xxSubmesh.VertexList = xxVertexList;

				List<ImportedFace> faceList = mesh.SubmeshList[submeshIdx].FaceList;
				List<xxFace> xxFaceList = new List<xxFace>(faceList.Count);
				for (int j = 0; j < faceList.Count; j++)
				{
					int[] vertexIndices = faceList[j].VertexIndices;
					xxFace xxFace = new xxFace();
					xxFace.VertexIndices = new ushort[3] { (ushort)vertexIndices[0], (ushort)vertexIndices[1], (ushort)vertexIndices[2] };
					xxFaceList.Add(xxFace);
				}
				xxSubmesh.FaceList = xxFaceList;
			}

			xxMesh.VertexListDuplicate = CreateVertexListDup(xxMesh.SubmeshList);
			return xxMesh;
		}

		public static void ReplaceMesh(xxFrame frame, xxParser parser, WorkspaceMesh mesh, bool merge, CopyMeshMethod normalsMethod, CopyMeshMethod bonesMethod)
		{
			Matrix transform = Matrix.Identity;
			xxFrame transformFrame = frame;
			while (transformFrame != null)
			{
				transform = transformFrame.Matrix * transform;
				transformFrame = (xxFrame)transformFrame.Parent;
			}
			transform.Invert();

			string[] materialNames;
			int[] indices;
			bool[] worldCoords;
			bool[] replaceSubmeshesOption;
			xxMesh xxMesh = CreateMesh(mesh, parser.Format, out materialNames, out indices, out worldCoords, out replaceSubmeshesOption);

			if (frame.Mesh == null)
			{
				CreateUnknowns(xxMesh);
			}
			else
			{
				CopyUnknowns(frame.Mesh, xxMesh);

				if ((bonesMethod == CopyMeshMethod.CopyOrder) || (bonesMethod == CopyMeshMethod.CopyNear))
				{
					xxMesh.BoneList = new List<xxBone>(frame.Mesh.BoneList.Count);
					for (int i = 0; i < frame.Mesh.BoneList.Count; i++)
					{
						xxMesh.BoneList.Add(frame.Mesh.BoneList[i].Clone());
					}
				}
			}

			xxSubmesh[] replaceSubmeshes = (frame.Mesh == null) ? null : new xxSubmesh[frame.Mesh.SubmeshList.Count];
			List<xxSubmesh> addSubmeshes = new List<xxSubmesh>(xxMesh.SubmeshList.Count);
			for (int i = 0; i < xxMesh.SubmeshList.Count; i++)
			{
				for (int j = 0; j < parser.MaterialList.Count; j++)
				{
					if (parser.MaterialList[j].Name == materialNames[i])
					{
						xxMesh.SubmeshList[i].MaterialIndex = j;
						break;
					}
				}

				xxSubmesh xxSubmesh = xxMesh.SubmeshList[i];
				List<xxVertex> xxVertexList = xxSubmesh.VertexList;
				if (worldCoords[i])
				{
					for (int j = 0; j < xxVertexList.Count; j++)
					{
						xxVertexList[j].Position = Vector3.TransformCoordinate(xxVertexList[j].Position, transform);
					}
				}

				xxSubmesh baseSubmesh = null;
				int idx = indices[i];
				if ((frame.Mesh != null) && (idx >= 0) && (idx < frame.Mesh.SubmeshList.Count))
				{
					baseSubmesh = frame.Mesh.SubmeshList[idx];
					CopyUnknowns(baseSubmesh, xxSubmesh, parser.Format, xxMesh.NumVector2PerVertex);
				}
				else
				{
					CreateUnknowns(xxSubmesh, parser.Format, xxMesh.NumVector2PerVertex);
				}

				if (baseSubmesh != null)
				{
					if (normalsMethod == CopyMeshMethod.CopyOrder)
					{
						xx.CopyNormalsOrder(baseSubmesh.VertexList, xxSubmesh.VertexList);
					}
					else if (normalsMethod == CopyMeshMethod.CopyNear)
					{
						xx.CopyNormalsNear(baseSubmesh.VertexList, xxSubmesh.VertexList);
					}

					if (bonesMethod == CopyMeshMethod.CopyOrder)
					{
						xx.CopyBonesOrder(baseSubmesh.VertexList, xxSubmesh.VertexList);
					}
					else if (bonesMethod == CopyMeshMethod.CopyNear)
					{
						xx.CopyBonesNear(baseSubmesh.VertexList, xxSubmesh.VertexList);
					}
				}

				if ((baseSubmesh != null) && merge && replaceSubmeshesOption[i])
				{
					replaceSubmeshes[idx] = xxSubmesh;
				}
				else
				{
					addSubmeshes.Add(xxSubmesh);
				}
			}

			if ((frame.Mesh != null) && merge)
			{
				xxMesh.SubmeshList = new List<xxSubmesh>(replaceSubmeshes.Length + addSubmeshes.Count);
				List<xxSubmesh> copiedSubmeshes = new List<xxSubmesh>(replaceSubmeshes.Length);
				for (int i = 0; i < replaceSubmeshes.Length; i++)
				{
					if (replaceSubmeshes[i] == null)
					{
						xxSubmesh xxSubmesh = frame.Mesh.SubmeshList[i].Clone();
						copiedSubmeshes.Add(xxSubmesh);
						xxMesh.SubmeshList.Add(xxSubmesh);
					}
					else
					{
						xxMesh.SubmeshList.Add(replaceSubmeshes[i]);
					}
				}
				xxMesh.SubmeshList.AddRange(addSubmeshes);

				if ((frame.Mesh.BoneList.Count == 0) && (xxMesh.BoneList.Count > 0))
				{
					for (int i = 0; i < copiedSubmeshes.Count; i++)
					{
						List<xxVertex> vertexList = copiedSubmeshes[i].VertexList;
						for (int j = 0; j < vertexList.Count; j++)
						{
							vertexList[j].BoneIndices = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
						}
					}
				}
				else if ((frame.Mesh.BoneList.Count > 0) && (xxMesh.BoneList.Count == 0))
				{
					for (int i = 0; i < replaceSubmeshes.Length; i++)
					{
						if (replaceSubmeshes[i] != null)
						{
							List<xxVertex> vertexList = replaceSubmeshes[i].VertexList;
							for (int j = 0; j < vertexList.Count; j++)
							{
								vertexList[j].BoneIndices = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
							}
						}
					}
					for (int i = 0; i < addSubmeshes.Count; i++)
					{
						List<xxVertex> vertexList = addSubmeshes[i].VertexList;
						for (int j = 0; j < vertexList.Count; j++)
						{
							vertexList[j].BoneIndices = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
						}
					}
				}
				else if ((frame.Mesh.BoneList.Count > 0) && (xxMesh.BoneList.Count > 0))
				{
					byte[] boneIdxMap;
					xxMesh.BoneList = MergeBoneList(frame.Mesh.BoneList, xxMesh.BoneList, out boneIdxMap);
					for (int i = 0; i < replaceSubmeshes.Length; i++)
					{
						if (replaceSubmeshes[i] != null)
						{
							List<xxVertex> vertexList = replaceSubmeshes[i].VertexList;
							for (int j = 0; j < vertexList.Count; j++)
							{
								byte[] boneIndices = vertexList[j].BoneIndices;
								vertexList[j].BoneIndices = new byte[4];
								for (int k = 0; k < 4; k++)
								{
									vertexList[j].BoneIndices[k] = boneIndices[k] < 0xFF ? boneIdxMap[boneIndices[k]] : (byte)0xFF;
								}
							}
						}
					}
					for (int i = 0; i < addSubmeshes.Count; i++)
					{
						List<xxVertex> vertexList = addSubmeshes[i].VertexList;
						for (int j = 0; j < vertexList.Count; j++)
						{
							byte[] boneIndices = vertexList[j].BoneIndices;
							vertexList[j].BoneIndices = new byte[4];
							for (int k = 0; k < 4; k++)
							{
								vertexList[j].BoneIndices[k] = boneIndices[k] < 0xFF ? boneIdxMap[boneIndices[k]] : (byte)0xFF;
							}
						}
					}
				}
			}

			if ((xxMesh.NumVector2PerVertex > 0) || ((frame.Mesh != null) && merge))
			{
				xxMesh.VertexListDuplicate = CreateVertexListDup(xxMesh.SubmeshList);
			}

			frame.Mesh = xxMesh;
			SetBoundingBox(frame);
		}

		public static xxMaterial CreateMaterial(ImportedMaterial material)
		{
			xxMaterial xxMat = new xxMaterial();
			xxMat.Name = material.Name;
			xxMat.Diffuse = material.Diffuse;
			xxMat.Ambient = material.Ambient;
			xxMat.Specular = material.Specular;
			xxMat.Emissive = material.Emissive;
			xxMat.Power = material.Power;

			xxMat.Textures = new xxMaterialTexture[4];
			for (int i = 0; i < xxMat.Textures.Length; i++)
			{
				xxMat.Textures[i] = new xxMaterialTexture();
				if ((material.Textures != null) && (i < material.Textures.Length) && (material.Textures[i] != null))
				{
					xxMat.Textures[i].Name = material.Textures[i];
				}
				else
				{
					xxMat.Textures[i].Name = String.Empty;
				}
			}
			return xxMat;
		}

		public static void ReplaceMaterial(xxParser parser, ImportedMaterial material)
		{
			xxMaterial mat = xx.CreateMaterial(material);

			bool found = false;
			for (int i = 0; i < parser.MaterialList.Count; i++)
			{
				if (parser.MaterialList[i].Name == material.Name)
				{
					CopyUnknowns(parser.MaterialList[i], mat);

					parser.MaterialList.RemoveAt(i);
					parser.MaterialList.Insert(i, mat);
					found = true;
					break;
				}
			}

			if (!found)
			{
				CreateUnknowns(mat, parser.Format);
				parser.MaterialList.Add(mat);
			}
		}

		public static xxTexture CreateTexture(ImportedTexture texture)
		{
			var imgInfo = ImageInformation.FromMemory(texture.Data);

			xxTexture xxTex = new xxTexture();
			xxTex.Width = imgInfo.Width;
			xxTex.Height = imgInfo.Height;
			xxTex.Depth = imgInfo.Depth;
			xxTex.Format = (int)imgInfo.Format;
			xxTex.ImageFileFormat = (int)imgInfo.ImageFileFormat;
			xxTex.MipLevels = imgInfo.MipLevels;
			xxTex.Name = texture.Name;
			xxTex.ResourceType = (int)imgInfo.ResourceType;

			byte checksum = 0;
			for (int i = 0; i < texture.Data.Length; i += 32)
			{
				checksum += texture.Data[i];
			}
			xxTex.Checksum = checksum;

			if (imgInfo.ImageFileFormat == ImageFileFormat.Bmp)
			{
				xxTex.ImageData = (byte[])texture.Data.Clone();
				xxTex.ImageData[0] = 0;
				xxTex.ImageData[1] = 0;
			}
			else if(imgInfo.ImageFileFormat == ImageFileFormat.Tga)
			{
				byte[] tgaHeader = new byte[18];
				Array.Copy(texture.Data, tgaHeader, tgaHeader.Length);
				int imgdataLen = tgaHeader[16] / 8 * BitConverter.ToInt16(tgaHeader, 12) * BitConverter.ToInt16(tgaHeader, 14);
				xxTex.ImageData = new byte[tgaHeader.Length + imgdataLen];
				Array.Copy(texture.Data, xxTex.ImageData, tgaHeader.Length);
				Array.Copy(texture.Data, tgaHeader.Length + tgaHeader[0], xxTex.ImageData, tgaHeader.Length, imgdataLen);
				xxTex.ImageData[0] = 0;
			}
			else
			{
				xxTex.ImageData = (byte[])texture.Data.Clone();
			}

			return xxTex;
		}

		public static void ReplaceTexture(xxParser parser, ImportedTexture texture)
		{
			xxTexture tex = xx.CreateTexture(texture);

			bool found = false;
			for (int i = 0; i < parser.TextureList.Count; i++)
			{
				if (parser.TextureList[i].Name == texture.Name)
				{
					CopyUnknowns(parser.TextureList[i], tex);

					parser.TextureList.RemoveAt(i);
					parser.TextureList.Insert(i, tex);
					found = true;
					break;
				}
			}

			if (!found)
			{
				CreateUnknowns(tex);
				parser.TextureList.Add(tex);
			}
		}
	}
}
