using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using SlimDX;

namespace SB3Utility
{
	[Plugin]
	public class xxEditor
	{
		public List<xxFrame> Frames { get; protected set; }
		public List<xxFrame> Meshes { get; protected set; }

		public xxParser Parser { get; protected set; }

		public xxEditor(xxParser parser)
		{
			Parser = parser;

			Frames = new List<xxFrame>();
			Meshes = new List<xxFrame>();
			InitFrames(parser.Frame);
		}

		void InitFrames(xxFrame frame)
		{
			Frames.Add(frame);

			if (frame.Mesh != null)
			{
				Meshes.Add(frame);
			}

			for (int i = 0; i < frame.Count; i++)
			{
				InitFrames(frame[i]);
			}
		}

		[Plugin]
		public int GetFrameId(string name)
		{
			for (int i = 0; i < Frames.Count; i++)
			{
				if (Frames[i].Name == name)
				{
					return i;
				}
			}

			return -1;
		}

		[Plugin]
		public int GetMeshId(string name)
		{
			for (int i = 0; i < Meshes.Count; i++)
			{
				if (Meshes[i].Name == name)
				{
					return i;
				}
			}

			return -1;
		}

		[Plugin]
		public int GetMaterialId(string name)
		{
			for (int i = 0; i < Parser.MaterialList.Count; i++)
			{
				if (Parser.MaterialList[i].Name == name)
				{
					return i;
				}
			}

			return -1;
		}

		[Plugin]
		public int GetTextureId(string name)
		{
			for (int i = 0; i < Parser.TextureList.Count; i++)
			{
				if (Parser.TextureList[i].Name == name)
				{
					return i;
				}
			}

			return -1;
		}

		[Plugin]
		public void SetFrameUnknowns(int id, byte[] unknown1, byte[] unknown2)
		{
			xxFrame frame = Frames[id];
			frame.Unknown1 = (byte[])unknown1.Clone();
			frame.Unknown2 = (byte[])unknown2.Clone();
		}

		[Plugin]
		public void SetMeshUnknowns(int id, byte[] numVector2, byte[] vertListDup)
		{
			xxFrame frame = Meshes[id];
			xx.SetNumVector2PerVertex(frame.Mesh, numVector2[0]);
			frame.Mesh.VertexListDuplicateUnknown = (byte[])vertListDup.Clone();
		}

		[Plugin]
		public void SetSubmeshUnknowns(int meshId, int submeshId,
			byte[] unknown1, byte[] unknown2, byte[] unknown3, byte[] unknown4, byte[] unknown5, byte[] unknown6)
		{
			xxSubmesh submesh = Meshes[meshId].Mesh.SubmeshList[submeshId];
			submesh.Unknown1 = (byte[])unknown1.Clone();
			if (Parser.Format >= 7)
			{
				submesh.Unknown2 = (byte[])unknown2.Clone();
			}
			if (Parser.Format >= 2)
			{
				submesh.Unknown3 = (byte[])unknown3.Clone();
			}
			if (Parser.Format >= 3)
			{
				submesh.Unknown4 = (byte[])unknown4.Clone();
			}
			if (Parser.Format >= 5)
			{
				submesh.Unknown5 = (byte[])unknown5.Clone();
			}
			if (Parser.Format == 6)
			{
				submesh.Unknown6 = (byte[])unknown6.Clone();
			}
		}

		[Plugin]
		public void SetMaterialUnknowns(int id, byte[] unknown1, byte[] tex1, byte[] tex2, byte[] tex3, byte[] tex4)
		{
			xxMaterial mat = Parser.MaterialList[id];
			mat.Unknown1 = (byte[])unknown1.Clone();
			mat.Textures[0].Unknown1 = (byte[])tex1.Clone();
			mat.Textures[1].Unknown1 = (byte[])tex2.Clone();
			mat.Textures[2].Unknown1 = (byte[])tex3.Clone();
			mat.Textures[3].Unknown1 = (byte[])tex4.Clone();
		}

		[Plugin]
		public void SetTextureUnknowns(int id, byte[] unknown1)
		{
			xxTexture tex = Parser.TextureList[id];
			tex.Unknown1 = (byte[])unknown1.Clone();
		}

		[Plugin]
		public void MoveFrame(int id, int parent, int index)
		{
			var srcFrame = Frames[id];
			var srcParent = (xxFrame)srcFrame.Parent;
			var destParent = Frames[parent];
			srcParent.RemoveChild(srcFrame);
			destParent.InsertChild(index, srcFrame);
		}

		[Plugin]
		public void RemoveFrame(int id)
		{
			var frame = Frames[id];
			var parent = (xxFrame)frame.Parent;
			if (parent == null)
			{
				throw new Exception("The root frame can't be removed");
			}

			parent.RemoveChild(frame);

			Frames.Clear();
			Meshes.Clear();
			InitFrames(Parser.Frame);
		}

		[Plugin]
		public void SetFrameName(int id, string name)
		{
			Frames[id].Name = name;
		}

		[Plugin]
		public void SetFrameName2(int id, string name)
		{
			Frames[id].Name2 = name;
		}

		[Plugin]
		public void SetFrameSRT(int id, double sX, double sY, double sZ, double rX, double rY, double rZ, double tX, double tY, double tZ)
		{
			Frames[id].Matrix = FbxUtility.SRTToMatrix(new Vector3((float)sX, (float)sY, (float)sZ), new Vector3((float)rX, (float)rY, (float)rZ), new Vector3((float)tX, (float)tY, (float)tZ));
		}

		[Plugin]
		public void SetFrameMatrix(int id,
			double m11, double m12, double m13, double m14,
			double m21, double m22, double m23, double m24,
			double m31, double m32, double m33, double m34,
			double m41, double m42, double m43, double m44)
		{
			xxFrame frame = Frames[id];
			Matrix m = new Matrix();

			m.M11 = (float)m11;
			m.M12 = (float)m12;
			m.M13 = (float)m13;
			m.M14 = (float)m14;

			m.M21 = (float)m21;
			m.M22 = (float)m22;
			m.M23 = (float)m23;
			m.M24 = (float)m24;

			m.M31 = (float)m31;
			m.M32 = (float)m32;
			m.M33 = (float)m33;
			m.M34 = (float)m34;

			m.M41 = (float)m41;
			m.M42 = (float)m42;
			m.M43 = (float)m43;
			m.M44 = (float)m44;

			frame.Matrix = m;
		}

		[Plugin]
		public void AddFrame(ImportedFrame srcFrame, int destParentId, int meshMatOffset)
		{
			xxFrame newFrame = xx.CreateFrame(srcFrame);
			xx.CopyOrCreateUnknowns(newFrame, Parser.Frame, Parser.Format);
			MeshMatOffset(newFrame, meshMatOffset);

			AddFrame(newFrame, destParentId);
		}

		[Plugin]
		public void AddFrame(xxFrame srcFrame, int srcFormat, int destParentId, int meshMatOffset)
		{
			var newFrame = srcFrame.Clone(true, true);
			xx.ConvertFormat(newFrame, srcFormat, Parser.Format);
			MeshMatOffset(newFrame, meshMatOffset);

			AddFrame(newFrame, destParentId);
		}

		void AddFrame(xxFrame newFrame, int destParentId)
		{
			if (destParentId < 0)
			{
				Parser.Frame = newFrame;
			}
			else
			{
				Frames[destParentId].AddChild(newFrame);
			}

			Frames.Clear();
			Meshes.Clear();
			InitFrames(Parser.Frame);
		}

		[Plugin]
		public void ReplaceFrame(ImportedFrame srcFrame, int destParentId, int meshMatOffset)
		{
			xxFrame newFrame = xx.CreateFrame(srcFrame);
			xx.CopyOrCreateUnknowns(newFrame, Parser.Frame, Parser.Format);
			MeshMatOffset(newFrame, meshMatOffset);

			ReplaceFrame(newFrame, destParentId);
		}

		[Plugin]
		public void ReplaceFrame(xxFrame srcFrame, int srcFormat, int destParentId, int meshMatOffset)
		{
			var newFrame = srcFrame.Clone(true, true);
			xx.ConvertFormat(newFrame, srcFormat, Parser.Format);
			MeshMatOffset(newFrame, meshMatOffset);

			ReplaceFrame(newFrame, destParentId);
		}

		void ReplaceFrame(xxFrame newFrame, int destParentId)
		{
			if (destParentId < 0)
			{
				Parser.Frame = newFrame;
			}
			else
			{
				var destParent = Frames[destParentId];
				bool found = false;
				for (int i = 0; i < destParent.Count; i++)
				{
					var dest = destParent[i];
					if (dest.Name == newFrame.Name)
					{
						destParent.RemoveChild(i);
						destParent.InsertChild(i, newFrame);
						found = true;
						break;
					}
				}

				if (!found)
				{
					destParent.AddChild(newFrame);
				}
			}

			Frames.Clear();
			Meshes.Clear();
			InitFrames(Parser.Frame);
		}

		[Plugin]
		public void MergeFrame(ImportedFrame srcFrame, int destParentId, int meshMatOffset)
		{
			xxFrame newFrame = xx.CreateFrame(srcFrame);
			xx.CopyOrCreateUnknowns(newFrame, Parser.Frame, Parser.Format);
			MeshMatOffset(newFrame, meshMatOffset);

			MergeFrame(newFrame, destParentId);
		}

		[Plugin]
		public void MergeFrame(xxFrame srcFrame, int srcFormat, int destParentId, int meshMatOffset)
		{
			var newFrame = srcFrame.Clone(true, true);
			xx.ConvertFormat(newFrame, srcFormat, Parser.Format);
			MeshMatOffset(newFrame, meshMatOffset);

			MergeFrame(newFrame, destParentId);
		}

		void MergeFrame(xxFrame newFrame, int destParentId)
		{
			xxFrame srcParent = new xxFrame();
			srcParent.InitChildren(1);
			srcParent.AddChild(newFrame);

			xxFrame destParent;
			if (destParentId < 0)
			{
				destParent = new xxFrame();
				destParent.InitChildren(1);
				destParent.AddChild(Parser.Frame);
			}
			else
			{
				destParent = Frames[destParentId];
			}

			MergeFrame(srcParent, destParent);

			if (destParentId < 0)
			{
				Parser.Frame = srcParent[0];
				srcParent.RemoveChild(0);
			}

			Frames.Clear();
			Meshes.Clear();
			InitFrames(Parser.Frame);
		}

		void MergeFrame(xxFrame srcParent, xxFrame destParent)
		{
			for (int i = 0; i < destParent.Count; i++)
			{
				var dest = destParent[i];
				for (int j = 0; j < srcParent.Count; j++)
				{
					var src = srcParent[j];
					if (src.Name == dest.Name)
					{
						MergeFrame(src, dest);

						srcParent.RemoveChild(j);
						destParent.RemoveChild(i);
						destParent.InsertChild(i, src);
						break;
					}
				}
			}

			if (srcParent.Name == destParent.Name)
			{
				while (destParent.Count > 0)
				{
					var dest = destParent[0];
					destParent.RemoveChild(0);
					srcParent.AddChild(dest);
				}
			}
			else
			{
				while (srcParent.Count > 0)
				{
					var src = srcParent[0];
					srcParent.RemoveChild(0);
					destParent.AddChild(src);
				}
			}
		}

		void MeshMatOffset(xxFrame frame, int offset)
		{
			if (frame.Mesh != null)
			{
				var submeshes = frame.Mesh.SubmeshList;
				for (int i = 0; i < submeshes.Count; i++)
				{
					submeshes[i].MaterialIndex += offset;
				}
			}

			for (int i = 0; i < frame.Count; i++)
			{
				MeshMatOffset(frame[i], offset);
			}
		}

		[Plugin]
		public void ReplaceMesh(WorkspaceMesh mesh, int frameId, bool merge, string normals, string bones)
		{
			var normalsMethod = (CopyMeshMethod)Enum.Parse(typeof(CopyMeshMethod), normals);
			var bonesMethod = (CopyMeshMethod)Enum.Parse(typeof(CopyMeshMethod), bones);
			xx.ReplaceMesh(Frames[frameId], Parser, mesh, merge, normalsMethod, bonesMethod);

			Frames.Clear();
			Meshes.Clear();
			InitFrames(Parser.Frame);
		}

		[Plugin]
		public void SetBoneName(int meshId, int boneId, string name)
		{
			xxBone bone = Meshes[meshId].Mesh.BoneList[boneId];
			bone.Name = name;
		}

		[Plugin]
		public void SetBoneSRT(int meshId, int boneId, double sX, double sY, double sZ, double rX, double rY, double rZ, double tX, double tY, double tZ)
		{
			xxBone bone = Meshes[meshId].Mesh.BoneList[boneId];
			bone.Matrix = FbxUtility.SRTToMatrix(new Vector3((float)sX, (float)sY, (float)sZ), new Vector3((float)rX, (float)rY, (float)rZ), new Vector3((float)tX, (float)tY, (float)tZ));
		}

		[Plugin]
		public void SetBoneMatrix(int meshId, int boneId,
			double m11, double m12, double m13, double m14,
			double m21, double m22, double m23, double m24,
			double m31, double m32, double m33, double m34,
			double m41, double m42, double m43, double m44)
		{
			xxBone bone = Meshes[meshId].Mesh.BoneList[boneId];
			Matrix m = new Matrix();

			m.M11 = (float)m11;
			m.M12 = (float)m12;
			m.M13 = (float)m13;
			m.M14 = (float)m14;

			m.M21 = (float)m21;
			m.M22 = (float)m22;
			m.M23 = (float)m23;
			m.M24 = (float)m24;

			m.M31 = (float)m31;
			m.M32 = (float)m32;
			m.M33 = (float)m33;
			m.M34 = (float)m34;

			m.M41 = (float)m41;
			m.M42 = (float)m42;
			m.M43 = (float)m43;
			m.M44 = (float)m44;

			bone.Matrix = m;
		}

		[Plugin]
		public void RemoveBone(int meshId, int boneId)
		{
			xxFrame frame = Meshes[meshId];
			frame.Mesh.BoneList.RemoveAt(boneId);
		}

		[Plugin]
		public void CopyBone(int meshId, int boneId)
		{
			xxFrame frame = Meshes[meshId];
			List<xxBone> boneList = frame.Mesh.BoneList;
			boneList.Add(boneList[boneId].Clone());
		}

		[Plugin]
		public void SetMaterialName(int id, string name)
		{
			Parser.MaterialList[id].Name = name;
		}

		[Plugin]
		public void SetMaterialPhong(int id, object[] diffuse, object[] ambient, object[] specular, object[] emissive, double shininess)
		{
			xxMaterial mat = Parser.MaterialList[id];
			mat.Diffuse = new Color4((float)(double)diffuse[3], (float)(double)diffuse[0], (float)(double)diffuse[1], (float)(double)diffuse[2]);
			mat.Ambient = new Color4((float)(double)ambient[3], (float)(double)ambient[0], (float)(double)ambient[1], (float)(double)ambient[2]);
			mat.Specular = new Color4((float)(double)specular[3], (float)(double)specular[0], (float)(double)specular[1], (float)(double)specular[2]);
			mat.Emissive = new Color4((float)(double)emissive[3], (float)(double)emissive[0], (float)(double)emissive[1], (float)(double)emissive[2]);
			mat.Power = (float)shininess;
		}

		[Plugin]
		public void RemoveMaterial(int id)
		{
			List<xxMaterial> materialList = Parser.MaterialList;

			int[] matIdxMap = new int[materialList.Count];
			for (int i = 0; i < id; i++)
			{
				matIdxMap[i] = i;
			}
			matIdxMap[id] = -1;
			for (int i = id + 1; i < materialList.Count; i++)
			{
				matIdxMap[i] = i - 1;
			}

			for (int i = 0; i < Meshes.Count; i++)
			{
				List<xxSubmesh> submeshList = Meshes[i].Mesh.SubmeshList;
				for (int j = 0; j < submeshList.Count; j++)
				{
					xxSubmesh submesh = submeshList[j];
					int matIdx = submesh.MaterialIndex;
					if ((matIdx >= 0) && (matIdx < materialList.Count))
					{
						submesh.MaterialIndex = matIdxMap[submesh.MaterialIndex];
					}
				}
			}

			Parser.MaterialList.RemoveAt(id);
		}

		[Plugin]
		public void CopyMaterial(int id)
		{
			Parser.MaterialList.Add(Parser.MaterialList[id].Clone());
		}

		[Plugin]
		public void MergeMaterial(ImportedMaterial mat)
		{
			xx.ReplaceMaterial(Parser, mat);
		}

		[Plugin]
		public void MergeMaterial(xxMaterial mat, int srcFormat)
		{
			var newMat = mat.Clone();
			xx.ConvertFormat(newMat, srcFormat, Parser.Format);

			bool found = false;
			for (int i = 0; i < Parser.MaterialList.Count; i++)
			{
				var oldMat = Parser.MaterialList[i];
				if (oldMat.Name == newMat.Name)
				{
					if (Parser.Format > srcFormat)
					{
						xx.CopyUnknowns(oldMat, newMat);
					}

					Parser.MaterialList.RemoveAt(i);
					Parser.MaterialList.Insert(i, newMat);
					found = true;
					break;
				}
			}

			if (!found)
			{
				Parser.MaterialList.Add(newMat);
			}
		}

		[Plugin]
		public void SetMaterialTexture(int id, int index, string name)
		{
			Parser.MaterialList[id].Textures[index].Name = name;
		}

		[Plugin]
		public void RemoveMesh(int id)
		{
			xxFrame frame = Meshes[id];
			frame.Mesh = null;
			frame.Bounds = new BoundingBox();

			Frames.Clear();
			Meshes.Clear();
			InitFrames(Parser.Frame);
		}

		[Plugin]
		public void RemoveSubmesh(int meshId, int submeshId)
		{
			List<xxSubmesh> submeshList = Meshes[meshId].Mesh.SubmeshList;
			if (submeshList.Count == 1)
			{
				RemoveMesh(meshId);
			}
			else
			{
				submeshList.RemoveAt(submeshId);
			}
		}

		[Plugin]
		public void SetSubmeshMaterial(int meshId, int submeshId, int material)
		{
			xxSubmesh submesh = Meshes[meshId].Mesh.SubmeshList[submeshId];
			submesh.MaterialIndex = material;
		}

		[Plugin]
		public void MinBones(int id)
		{
			xx.RemoveUnusedBones(Meshes[id].Mesh);
		}

		[Plugin]
		public void CalculateNormals(int id, double threshold)
		{
			xx.CalculateNormals(Meshes[id].Mesh.SubmeshList, (float)threshold);
		}

		[Plugin]
		public void ExportTexture(int id, string path)
		{
			xx.ExportTexture(Parser.TextureList[id], path);
		}

		[Plugin]
		public void AddTexture(ImportedTexture image)
		{
			xxTexture tex = xx.CreateTexture(image);
			xx.CreateUnknowns(tex);

			Parser.TextureList.Add(tex);
		}

		[Plugin]
		public void ReplaceTexture(int id, ImportedTexture image)
		{
			var oldTex = Parser.TextureList[id];

			var newTex = xx.CreateTexture(image);
			xx.CopyUnknowns(oldTex, newTex);

			Parser.TextureList.RemoveAt(id);
			Parser.TextureList.Insert(id, newTex);

			for (int i = 0; i < Parser.MaterialList.Count; i++)
			{
				var mat = Parser.MaterialList[i];
				for (int j = 0; j < mat.Textures.Length; j++)
				{
					var matTex = mat.Textures[j];
					if (matTex.Name == oldTex.Name)
					{
						matTex.Name = newTex.Name;
					}
				}
			}
		}

		[Plugin]
		public void MergeTexture(ImportedTexture tex)
		{
			xx.ReplaceTexture(Parser, tex);
		}

		[Plugin]
		public void MergeTexture(xxTexture tex)
		{
			var newTex = tex.Clone();

			bool found = false;
			for (int i = 0; i < Parser.TextureList.Count; i++)
			{
				var oldTex = Parser.TextureList[i];
				if (oldTex.Name == newTex.Name)
				{
					Parser.TextureList.RemoveAt(i);
					Parser.TextureList.Insert(i, newTex);
					found = true;
					break;
				}
			}

			if (!found)
			{
				Parser.TextureList.Add(newTex);
			}
		}

		[Plugin]
		public void RemoveTexture(int id)
		{
			var tex = Parser.TextureList[id];

			for (int i = 0; i < Parser.MaterialList.Count; i++)
			{
				var mat = Parser.MaterialList[i];
				for (int j = 0; j < mat.Textures.Length; j++)
				{
					var matTex = mat.Textures[j];
					if (matTex.Name == tex.Name)
					{
						matTex.Name = String.Empty;
					}
				}
			}

			Parser.TextureList.RemoveAt(id);
		}

		[Plugin]
		public void SetTextureName(int id, string name)
		{
			var tex = Parser.TextureList[id];

			for (int i = 0; i < Parser.MaterialList.Count; i++)
			{
				var mat = Parser.MaterialList[i];
				for (int j = 0; j < mat.Textures.Length; j++)
				{
					var matTex = mat.Textures[j];
					if (matTex.Name == tex.Name)
					{
						matTex.Name = name;
					}
				}
			}

			tex.Name = name;
		}
	}
}
