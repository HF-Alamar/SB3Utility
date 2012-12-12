using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace SB3Utility
{
	public class RenderObjectXX : IDisposable, IRenderObject
	{
		private AnimationFrame rootFrame;
		private Device device;
		private VertexDeclaration tweeningVertDec;
		private List<AnimationFrame> meshFrames;
		private Material highlightMaterial;
		private Material nullMaterial = new Material();
		private int submeshNum = 0;
		private int numFrames = 0;

		private Texture[] Textures;
		private Material[] Materials;
		private Dictionary<int, int> MatTexIndices = new Dictionary<int, int>();

		public BoundingBox Bounds { get; protected set; }
		public AnimationController AnimationController { get; protected set; }
		public bool IsDisposed { get; protected set; }
		public HashSet<int> HighlightSubmesh { get; protected set; }

		const int BoneObjSize = 16;

		public RenderObjectXX(xxParser parser, HashSet<string> meshNames)
		{
			HighlightSubmesh = new HashSet<int>();
			highlightMaterial = new Material();
			highlightMaterial.Ambient = new Color4(1, 1, 1, 1);
			highlightMaterial.Diffuse = new Color4(1, 0, 1, 0);

			this.device = Gui.Renderer.Device;
			this.tweeningVertDec = new VertexDeclaration(this.device, TweeningMeshesVertexBufferFormat.ThreeStreams);

			Textures = new Texture[parser.TextureList.Count];
			Materials = new Material[parser.MaterialList.Count];

			rootFrame = CreateHierarchy(parser, meshNames, device, out meshFrames);

			AnimationController = new AnimationController(numFrames, 30, 30, 1);
			Frame.RegisterNamedMatrices(rootFrame, AnimationController);

			Bounds = meshFrames[0].Bounds;
			for (int i = 1; i < meshFrames.Count; i++)
			{
				Bounds = BoundingBox.Merge(Bounds, meshFrames[i].Bounds);
			}
		}

		~RenderObjectXX()
		{
			Dispose();
		}

		public void Dispose()
		{
			for (int i = 0; i < meshFrames.Count; i++)
			{
				MeshContainer mesh = meshFrames[i].MeshContainer;
				while (mesh != null)
				{
					if ((mesh.MeshData != null) && (mesh.MeshData.Mesh != null))
					{
						mesh.MeshData.Mesh.Dispose();
					}
					if (mesh is MorphMeshContainer)
					{
						MorphMeshContainer morphMesh = (MorphMeshContainer)mesh;
						if (morphMesh.StartBuffer != morphMesh.EndBuffer)
						{
							morphMesh.StartBuffer.Dispose();
						}
						if (morphMesh.EndBuffer != null)
						{
							morphMesh.EndBuffer.Dispose();
						}
						if (morphMesh.CommonBuffer != null)
						{
							morphMesh.CommonBuffer.Dispose();
						}
						if (morphMesh.IndexBuffer != null)
						{
							morphMesh.IndexBuffer.Dispose();
						}
					}

					for (int j = 0; j < Textures.Length; j++)
					{
						Texture tex = Textures[j];
						if ((tex != null) && !tex.Disposed)
						{
							tex.Dispose();
						}
					}

					mesh = mesh.NextMeshContainer;
				}
			}

			rootFrame.Dispose();
			AnimationController.Dispose();

			tweeningVertDec.Dispose();

			IsDisposed = true;
		}

		public void Render()
		{
			UpdateFrameMatrices(rootFrame, Matrix.Identity);

			for (int i = 0; i < meshFrames.Count; i++)
			{
				DrawMeshFrame(meshFrames[i]);
			}
		}

		public void ResetPose()
		{
			ResetPose(rootFrame);
		}

		private void DrawMeshFrame(AnimationFrame frame)
		{
			if (frame.MeshContainer is AnimationMeshContainer)
			{
				AnimationMeshContainer animMeshContainer = (AnimationMeshContainer)frame.MeshContainer;
				if (animMeshContainer.BoneNames.Length > 0)
				{
					device.SetRenderState(RenderState.VertexBlend, VertexBlend.Weights3);
					device.SetRenderState(RenderState.IndexedVertexBlendEnable, true);
					// uncomment to emphazise bone weights and darken everything else
					// device.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Color1);
					device.SetRenderState(RenderState.AmbientMaterialSource, ColorSource.Color1);

					for (int i = 0; i < animMeshContainer.BoneNames.Length; i++)
					{
						if (animMeshContainer.BoneFrames[i] != null)
						{
							device.SetTransform(i, animMeshContainer.BoneOffsets[i] * animMeshContainer.BoneFrames[i].CombinedTransform);
						}
					}
				}
				else
				{
					device.SetRenderState(RenderState.VertexBlend, VertexBlend.Disable);
					device.SetRenderState(RenderState.AmbientMaterialSource, ColorSource.Material);
					device.SetTransform(TransformState.World, frame.CombinedTransform);
				}

				submeshNum = 0;
				while (animMeshContainer != null)
				{
					DrawAnimationMeshContainer(animMeshContainer);
					animMeshContainer = (AnimationMeshContainer)animMeshContainer.NextMeshContainer;
					submeshNum++;
				}
			}
			else if (frame.MeshContainer is MorphMeshContainer)
			{
				MorphMeshContainer morphMeshContainer = (MorphMeshContainer)frame.MeshContainer;
				device.SetRenderState(RenderState.AmbientMaterialSource, ColorSource.Material);
				device.SetTransform(TransformState.World, frame.CombinedTransform);

				submeshNum = 0;
				DrawMorphMeshContainer(morphMeshContainer);
			}
		}

		private void DrawAnimationMeshContainer(AnimationMeshContainer meshContainer)
		{
			device.SetRenderState(RenderState.ZEnable, ZBufferType.UseZBuffer);
			device.SetRenderState(RenderState.Lighting, true);

			Cull culling = (Gui.Renderer.Culling) ? Cull.Counterclockwise : Cull.None;
			device.SetRenderState(RenderState.CullMode, culling);

			FillMode fill = (Gui.Renderer.Wireframe) ? FillMode.Wireframe : FillMode.Solid;
			device.SetRenderState(RenderState.FillMode, fill);

			int matIdx = meshContainer.MaterialIndex;
			device.Material = ((matIdx >= 0) && (matIdx < Materials.Length)) ? Materials[matIdx] : nullMaterial;

			int texIdx = meshContainer.TextureIndex;
			Texture tex = ((texIdx >= 0) && (texIdx < Textures.Length)) ? Textures[texIdx] : null;
			device.SetTexture(0, tex);

			meshContainer.MeshData.Mesh.DrawSubset(0);

			if (HighlightSubmesh.Contains(submeshNum))
			{
				device.SetRenderState(RenderState.ZEnable, ZBufferType.DontUseZBuffer);
				device.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
				device.Material = highlightMaterial;
				device.SetTexture(0, null);
				meshContainer.MeshData.Mesh.DrawSubset(0);
			}

			if (Gui.Renderer.ShowNormals)
			{
				device.SetRenderState(RenderState.ZEnable, ZBufferType.UseZBuffer);
				device.SetRenderState(RenderState.Lighting, false);
				device.Material = nullMaterial;
				device.SetTexture(0, null);
				device.VertexFormat = PositionBlendWeightsIndexedColored.Format;
				device.DrawUserPrimitives(PrimitiveType.LineList, meshContainer.NormalLines.Length / 2, meshContainer.NormalLines);
			}

			if (Gui.Renderer.ShowBones && (meshContainer.BoneLines != null))
			{
				device.SetRenderState(RenderState.ZEnable, ZBufferType.DontUseZBuffer);
				device.SetRenderState(RenderState.VertexBlend, VertexBlend.Weights1);
				device.SetRenderState(RenderState.Lighting, false);
				device.Material = nullMaterial;
				device.SetTexture(0, null);
				device.VertexFormat = PositionBlendWeightIndexedColored.Format;
				device.DrawUserPrimitives(PrimitiveType.LineList, meshContainer.BoneLines.Length / 2, meshContainer.BoneLines);
			}
		}

		private void DrawMorphMeshContainer(MorphMeshContainer meshContainer)
		{
			device.SetRenderState(RenderState.ZEnable, ZBufferType.UseZBuffer);
			device.SetRenderState(RenderState.Lighting, true);

			Cull culling = (Gui.Renderer.Culling) ? Cull.Counterclockwise : Cull.None;
			device.SetRenderState(RenderState.CullMode, culling);

			FillMode fill = (Gui.Renderer.Wireframe) ? FillMode.Wireframe : FillMode.Solid;
			device.SetRenderState(RenderState.FillMode, fill);

			int matIdx = meshContainer.MaterialIndex;
			device.Material = ((matIdx >= 0) && (matIdx < Materials.Length)) ? Materials[matIdx] : nullMaterial;

			int texIdx = meshContainer.TextureIndex;
			Texture tex = ((texIdx >= 0) && (texIdx < Textures.Length)) ? Textures[texIdx] : null;
			device.SetTexture(0, tex);

			device.SetRenderState(RenderState.VertexBlend, VertexBlend.Tweening);
			device.SetRenderState(RenderState.TweenFactor, meshContainer.TweenFactor);

			device.VertexDeclaration = tweeningVertDec;
			device.Indices = meshContainer.IndexBuffer;
			device.SetStreamSource(0, meshContainer.StartBuffer, 0, Marshal.SizeOf(typeof(TweeningMeshesVertexBufferFormat.Stream0)));
			device.SetStreamSource(1, meshContainer.EndBuffer, 0, Marshal.SizeOf(typeof(TweeningMeshesVertexBufferFormat.Stream1)));
			device.SetStreamSource(2, meshContainer.CommonBuffer, 0, Marshal.SizeOf(typeof(TweeningMeshesVertexBufferFormat.Stream2)));
			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshContainer.VertexCount, 0, meshContainer.FaceCount);

			if (HighlightSubmesh.Contains(submeshNum))
			{
				device.SetRenderState(RenderState.ZEnable, ZBufferType.DontUseZBuffer);
				device.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
				device.Material = highlightMaterial;
				device.SetTexture(0, null);
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshContainer.VertexCount, 0, meshContainer.FaceCount);
			}
		}

		private void ResetPose(AnimationFrame frame)
		{
			frame.TransformationMatrix = frame.OriginalTransform;

			if (frame.Sibling != null)
			{
				ResetPose((AnimationFrame)frame.Sibling);
			}

			if (frame.FirstChild != null)
			{
				ResetPose((AnimationFrame)frame.FirstChild);
			}
		}

		private void UpdateFrameMatrices(AnimationFrame frame, Matrix parentMatrix)
		{
			frame.CombinedTransform = frame.TransformationMatrix * parentMatrix;

			if (frame.Sibling != null)
			{
				UpdateFrameMatrices((AnimationFrame)frame.Sibling, parentMatrix);
			}

			if (frame.FirstChild != null)
			{
				UpdateFrameMatrices((AnimationFrame)frame.FirstChild, frame.CombinedTransform);
			}
		}

		private AnimationFrame CreateHierarchy(xxParser parser, HashSet<string> meshNames, Device device, out List<AnimationFrame> meshFrames)
		{
			meshFrames = new List<AnimationFrame>(meshNames.Count);
			HashSet<string> extractFrames = xx.SearchHierarchy(parser.Frame, meshNames);
			AnimationFrame rootFrame = CreateFrame(parser.Frame, parser, extractFrames, meshNames, device, Matrix.Identity, meshFrames);
			SetupBoneMatrices(rootFrame, rootFrame);
			return rootFrame;
		}

		private AnimationFrame CreateFrame(xxFrame frame, xxParser parser, HashSet<string> extractFrames, HashSet<string> meshNames, Device device, Matrix combinedParent, List<AnimationFrame> meshFrames)
		{
			AnimationFrame animationFrame = new AnimationFrame();
			animationFrame.Name = frame.Name;
			animationFrame.TransformationMatrix = frame.Matrix;
			animationFrame.OriginalTransform = animationFrame.TransformationMatrix;
			animationFrame.CombinedTransform = combinedParent * animationFrame.TransformationMatrix;

			xxMesh mesh = frame.Mesh;
			if (meshNames.Contains(frame.Name) && (mesh != null))
			{
				List<xxBone> boneList = mesh.BoneList;

				string[] boneNames = new string[boneList.Count];
				Matrix[] boneOffsets = new Matrix[boneList.Count];
				for (int i = 0; i < boneList.Count; i++)
				{
					xxBone bone = boneList[i];
					boneNames[i] = bone.Name;
					boneOffsets[i] = bone.Matrix;
				}

				AnimationMeshContainer[] meshContainers = new AnimationMeshContainer[mesh.SubmeshList.Count];
				Vector3 min = new Vector3(Single.MaxValue);
				Vector3 max = new Vector3(Single.MinValue);
				for (int i = 0; i < mesh.SubmeshList.Count; i++)
				{
					xxSubmesh submesh = mesh.SubmeshList[i];
					List<xxFace> faceList = submesh.FaceList;
					List<xxVertex> vertexList = submesh.VertexList;

					Mesh animationMesh = new Mesh(device, faceList.Count, vertexList.Count, MeshFlags.Managed, PositionBlendWeightsIndexedNormalTexturedColoured.Format);

					using (DataStream indexStream = animationMesh.LockIndexBuffer(LockFlags.None))
					{
						for (int j = 0; j < faceList.Count; j++)
						{
							ushort[] indices = faceList[j].VertexIndices;
							indexStream.Write(indices[0]);
							indexStream.Write(indices[2]);
							indexStream.Write(indices[1]);
						}
						animationMesh.UnlockIndexBuffer();
					}

					FillVertexBuffer(animationMesh, vertexList, -1);

					var normalLines = new PositionBlendWeightsIndexedColored[vertexList.Count * 2];
					for (int j = 0; j < vertexList.Count; j++)
					{
						xxVertex vertex = vertexList[j];

						normalLines[j * 2] = new PositionBlendWeightsIndexedColored(vertex.Position, vertex.Weights3, vertex.BoneIndices, Color.Yellow.ToArgb());
						normalLines[(j * 2) + 1] = new PositionBlendWeightsIndexedColored(vertex.Position + (vertex.Normal / 16), vertex.Weights3, vertex.BoneIndices, Color.Yellow.ToArgb());

						min = Vector3.Minimize(min, vertex.Position);
						max = Vector3.Maximize(max, vertex.Position);
					}

					AnimationMeshContainer meshContainer = new AnimationMeshContainer();
					meshContainer.Name = animationFrame.Name;
					meshContainer.MeshData = new MeshData(animationMesh);
					meshContainer.NormalLines = normalLines;
					meshContainers[i] = meshContainer;

					int matIdx = submesh.MaterialIndex;
					if ((matIdx >= 0) && (matIdx < parser.MaterialList.Count))
					{
						int texIdx;
						if (!MatTexIndices.TryGetValue(matIdx, out texIdx))
						{
							texIdx = -1;

							xxMaterial mat = parser.MaterialList[matIdx];
							Material materialD3D = new Material();
							materialD3D.Ambient = mat.Ambient;
							materialD3D.Diffuse = mat.Diffuse;
							materialD3D.Emissive = mat.Emissive;
							materialD3D.Specular = mat.Specular;
							materialD3D.Power = mat.Power;
							Materials[matIdx] = materialD3D;

							xxMaterialTexture matTex = mat.Textures[0];
							string matTexName = matTex.Name;
							if (matTexName != String.Empty)
							{
								for (int j = 0; j < parser.TextureList.Count; j++)
								{
									xxTexture tex = parser.TextureList[j];
									if (tex.Name == matTexName)
									{
										texIdx = j;
										if (Textures[j] == null)
										{
											ImportedTexture importedTex = xx.ImportedTexture(tex);
											Textures[j] = Texture.FromMemory(device, importedTex.Data);
										}
										break;
									}
								}
							}

							MatTexIndices.Add(matIdx, texIdx);
						}

						meshContainer.MaterialIndex = matIdx;
						meshContainer.TextureIndex = texIdx;
					}
				}

				for (int i = 0; i < (meshContainers.Length - 1); i++)
				{
					meshContainers[i].NextMeshContainer = meshContainers[i + 1];
				}
				for (int i = 0; i < meshContainers.Length; i++)
				{
					meshContainers[i].BoneNames = boneNames;
					meshContainers[i].BoneOffsets = boneOffsets;
				}

				min = Vector3.TransformCoordinate(min, animationFrame.CombinedTransform);
				max = Vector3.TransformCoordinate(max, animationFrame.CombinedTransform);
				animationFrame.Bounds = new BoundingBox(min, max);
				animationFrame.MeshContainer = meshContainers[0];
				meshFrames.Add(animationFrame);
			}

			for (int i = 0; i < frame.Count; i++)
			{
				xxFrame child = frame[i];
				if (extractFrames.Contains(child.Name))
				{
					AnimationFrame childAnimationFrame = CreateFrame(child, parser, extractFrames, meshNames, device, animationFrame.CombinedTransform, meshFrames);
					childAnimationFrame.Parent = animationFrame;
					animationFrame.AppendChild(childAnimationFrame);
				}
			}

			numFrames++;
			return animationFrame;
		}

		private void FillVertexBuffer(Mesh animationMesh, List<xxVertex> vertexList, int selectedBoneIdx)
		{
			using (DataStream vertexStream = animationMesh.LockVertexBuffer(LockFlags.None))
			{
				Color4 col = new Color4(1f, 1f, 1f);
				for (int i = 0; i < vertexList.Count; i++)
				{
					xxVertex vertex = vertexList[i];
					vertexStream.Write(vertex.Position.X);
					vertexStream.Write(vertex.Position.Y);
					vertexStream.Write(vertex.Position.Z);
					vertexStream.Write(vertex.Weights3[0]);
					vertexStream.Write(vertex.Weights3[1]);
					vertexStream.Write(vertex.Weights3[2]);
					vertexStream.Write(vertex.BoneIndices[0]);
					vertexStream.Write(vertex.BoneIndices[1]);
					vertexStream.Write(vertex.BoneIndices[2]);
					vertexStream.Write(vertex.BoneIndices[3]);
					vertexStream.Write(vertex.Normal.X);
					vertexStream.Write(vertex.Normal.Y);
					vertexStream.Write(vertex.Normal.Z);
					if (selectedBoneIdx >= 0)
					{
						col.Red = 0f; col.Green = 0f; col.Blue = 0f;
						byte[] boneIndices = vertex.BoneIndices;
						float[] boneWeights = vertex.Weights4(true);
						for (int j = 0; j < boneIndices.Length; j++)
						{
							if (boneIndices[j] == 0xFF)
							{
								continue;
							}

							byte boneIdx = boneIndices[j];
							if (boneIdx == selectedBoneIdx)
							{
/*								switch (cols)
								{
								case WeightsColourPreset.Greyscale:
									col.r = col.g = col.b = boneWeights[j];
									break;
								case WeightsColourPreset.Metal:
									col.r = boneWeights[j] > 0.666f ? 1f : boneWeights[j] * 1.5f;
									col.g = boneWeights[j] * boneWeights[j] * boneWeights[j];
									break;
								WeightsColourPreset.Rainbow:*/
									if (boneWeights[j] > 0.75f)
									{
										col.Red = 1f;
										col.Green = (1f - boneWeights[j]) * 2f;
										col.Blue = 0f;
									}
									else if (boneWeights[j] > 0.5f)
									{
										col.Red = 1f;
										col.Green = (1f - boneWeights[j]) * 2f;
										col.Blue = 0f;
									}
									else if (boneWeights[j] > 0.25f)
									{
										col.Red = (boneWeights[j] - 0.25f) * 4f;
										col.Green = 1f;
										col.Blue = 0f;
									}
									else
									{
										col.Green = boneWeights[j] * 4f;
										col.Blue = 1f - boneWeights[j] * 4f;
									}
/*									break;
								}*/
								break;
							}
						}
					}
					vertexStream.Write(col.ToArgb());
					vertexStream.Write(vertex.UV[0]);
					vertexStream.Write(vertex.UV[1]);
				}
				animationMesh.UnlockVertexBuffer();
			}
		}

		private void SetupBoneMatrices(AnimationFrame frame, AnimationFrame root)
		{
			AnimationMeshContainer mesh = (AnimationMeshContainer)frame.MeshContainer;
			if (mesh != null)
			{
				byte numBones = (byte)mesh.BoneNames.Length;
				AnimationFrame[] boneFrames = null;
				PositionBlendWeightIndexedColored[] boneLines = null;
				if (numBones > 0)
				{
					boneFrames = new AnimationFrame[numBones];
					var boneDic = new Dictionary<string, byte>();
					for (byte i = 0; i < numBones; i++)
					{
						string boneName = mesh.BoneNames[i];
						AnimationFrame bone = (AnimationFrame)root.FindChild(boneName);
						boneFrames[i] = bone;

						boneDic.Add(boneName, i);
					}

					float boneWidth = 0.05f;
					int boneColor = Color.CornflowerBlue.ToArgb();
					boneLines = new PositionBlendWeightIndexedColored[numBones * BoneObjSize];
					for (byte i = 0; i < numBones; i++)
					{
						AnimationFrame bone = boneFrames[i];

						byte boneParentId;
						if ((bone != null) && (bone.Parent != null) && boneDic.TryGetValue(bone.Parent.Name, out boneParentId))
						{
							Matrix boneMatrix = Matrix.Invert(mesh.BoneOffsets[i]);
							Matrix boneParentMatrix = Matrix.Invert(mesh.BoneOffsets[boneParentId]);

							Vector3 bonePos = Vector3.TransformCoordinate(new Vector3(), boneMatrix);
							Vector3 boneParentPos = Vector3.TransformCoordinate(new Vector3(), boneParentMatrix);

							Vector3 direction = bonePos - boneParentPos;
							float scale = boneWidth * (1 + direction.Length() / 2);
							Vector3 perpendicular = direction.Perpendicular();
							Vector3 cross = Vector3.Cross(direction, perpendicular);
							perpendicular = Vector3.Normalize(perpendicular) * scale;
							cross = Vector3.Normalize(cross) * scale;

							Vector3 bottomLeft = -perpendicular + -cross + boneParentPos;
							Vector3 bottomRight = -perpendicular + cross + boneParentPos;
							Vector3 topLeft = perpendicular + -cross + boneParentPos;
							Vector3 topRight = perpendicular + cross + boneParentPos;

							boneLines[i * BoneObjSize] = new PositionBlendWeightIndexedColored(bottomLeft, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 1] = new PositionBlendWeightIndexedColored(bottomRight, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 2] = new PositionBlendWeightIndexedColored(bottomRight, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 3] = new PositionBlendWeightIndexedColored(topRight, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 4] = new PositionBlendWeightIndexedColored(topRight, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 5] = new PositionBlendWeightIndexedColored(topLeft, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 6] = new PositionBlendWeightIndexedColored(topLeft, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 7] = new PositionBlendWeightIndexedColored(bottomLeft, boneParentId, boneColor);

							boneLines[(i * BoneObjSize) + 8] = new PositionBlendWeightIndexedColored(bottomLeft, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 9] = new PositionBlendWeightIndexedColored(bonePos, i, boneColor);
							boneLines[(i * BoneObjSize) + 10] = new PositionBlendWeightIndexedColored(bottomRight, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 11] = new PositionBlendWeightIndexedColored(bonePos, i, boneColor);
							boneLines[(i * BoneObjSize) + 12] = new PositionBlendWeightIndexedColored(topLeft, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 13] = new PositionBlendWeightIndexedColored(bonePos, i, boneColor);
							boneLines[(i * BoneObjSize) + 14] = new PositionBlendWeightIndexedColored(topRight, boneParentId, boneColor);
							boneLines[(i * BoneObjSize) + 15] = new PositionBlendWeightIndexedColored(bonePos, i, boneColor);
						}
					}
				}

				while (mesh != null)
				{
					if (mesh.NextMeshContainer == null)
					{
						mesh.BoneLines = boneLines;
					}

					mesh.BoneFrames = boneFrames;
					mesh = (AnimationMeshContainer)mesh.NextMeshContainer;
				}
			}

			if (frame.Sibling != null)
			{
				SetupBoneMatrices(frame.Sibling as AnimationFrame, root);
			}

			if (frame.FirstChild != null)
			{
				SetupBoneMatrices(frame.FirstChild as AnimationFrame, root);
			}
		}

		public void HighlightBone(xxMesh xxMesh, int boneIdx, bool show)
		{
			int submeshIdx = 0;
			for (AnimationMeshContainer mesh = (AnimationMeshContainer)meshFrames[0].MeshContainer;
				 mesh != null;
				 mesh = (AnimationMeshContainer)mesh.NextMeshContainer, submeshIdx++)
			{
				if (mesh.MeshData != null && mesh.MeshData.Mesh != null)
				{
					List<xxVertex> vertexList = xxMesh.SubmeshList[submeshIdx].VertexList;
					FillVertexBuffer(mesh.MeshData.Mesh, vertexList, show ? boneIdx : -1);
				}
				if (mesh.BoneLines != null)
				{
					for (int j = 0; j < BoneObjSize; j++)
					{
						mesh.BoneLines[boneIdx * BoneObjSize + j].Color = show ? Color.Crimson.ToArgb(): Color.CornflowerBlue.ToArgb();
					}
				}
			}
		}

		public float SetMorphKeyframe(xxFrame meshFrame, xaMorphIndexSet idxSet, xaMorphKeyframe keyframe, bool asStart)
		{
			foreach (AnimationFrame frame in meshFrames)
			{
				if (frame.Name == meshFrame.Name)
				{
					xxMesh xxMesh = meshFrame.Mesh;
					int meshObjIdx = xa.MorphMeshObjIdx(idxSet.MeshIndices, xxMesh);
					if (meshObjIdx < 0)
					{
						Report.ReportLog("no valid mesh object was found for the morph");
						return -1f;
					}
					MorphMeshContainer morphMesh = null;
					AnimationMeshContainer animMesh = frame.MeshContainer as AnimationMeshContainer;
					if (animMesh != null)
					{
						for (int i = 1; i < meshObjIdx; i++)
						{
							animMesh = (AnimationMeshContainer)animMesh.NextMeshContainer;
							if (animMesh == null)
								break;
						}
						if (animMesh == null)
						{
							Report.ReportLog("Bad submesh specified.");
							return -1f;
						}

						morphMesh = new MorphMeshContainer();
						morphMesh.FaceCount = xxMesh.SubmeshList[meshObjIdx].FaceList.Count;
						morphMesh.IndexBuffer = animMesh.MeshData.Mesh.IndexBuffer;

						morphMesh.VertexCount = xxMesh.SubmeshList[meshObjIdx].VertexList.Count;
						List<xxVertex> vertexList = xxMesh.SubmeshList[meshObjIdx].VertexList;
						VertexBuffer vertBuffer = CreateMorphVertexBuffer(idxSet, keyframe, vertexList);
						morphMesh.StartBuffer = morphMesh.EndBuffer = vertBuffer;

						int vertBufferSize = morphMesh.VertexCount * Marshal.SizeOf(typeof(TweeningMeshesVertexBufferFormat.Stream2));
						vertBuffer = new VertexBuffer(device, vertBufferSize, Usage.WriteOnly, VertexFormat.Texture1, Pool.Managed);
						using (DataStream vertexStream = vertBuffer.Lock(0, vertBufferSize, LockFlags.None))
						{
							for (int i = 0; i < vertexList.Count; i++)
							{
								xxVertex vertex = vertexList[i];
								vertexStream.Write(vertex.UV[0]);
								vertexStream.Write(vertex.UV[1]);
							}
							vertBuffer.Unlock();
						}
						morphMesh.CommonBuffer = vertBuffer;

						morphMesh.MaterialIndex = animMesh.MaterialIndex;
						morphMesh.TextureIndex = animMesh.TextureIndex;

						morphMesh.NextMeshContainer = animMesh;
						frame.MeshContainer = morphMesh;

						morphMesh.TweenFactor = 0.0f;
					}
					else
					{
						morphMesh = frame.MeshContainer as MorphMeshContainer;
						List<xxVertex> vertexList = xxMesh.SubmeshList[meshObjIdx].VertexList;
						VertexBuffer vertBuffer = CreateMorphVertexBuffer(idxSet, keyframe, vertexList);
						if (asStart)
						{
							if (morphMesh.StartBuffer != morphMesh.EndBuffer)
							{
								morphMesh.StartBuffer.Dispose();
							}
							morphMesh.StartBuffer = vertBuffer;
							morphMesh.TweenFactor = 0.0f;
						}
						else
						{
							if (morphMesh.StartBuffer != morphMesh.EndBuffer)
							{
								morphMesh.EndBuffer.Dispose();
							}
							morphMesh.EndBuffer = vertBuffer;
							morphMesh.TweenFactor = 1.0f;
						}
					}
					return morphMesh.TweenFactor;
				}
			}
			Report.ReportLog("Mesh frame " + meshFrame + " not displayed.");
			return -1f;
		}

		private VertexBuffer CreateMorphVertexBuffer(xaMorphIndexSet idxSet, xaMorphKeyframe keyframe, List<xxVertex> vertexList)
		{
			int vertBufferSize = keyframe.PositionList.Count * Marshal.SizeOf(typeof(TweeningMeshesVertexBufferFormat.Stream0));
			VertexBuffer vertBuffer = new VertexBuffer(device, vertBufferSize, Usage.WriteOnly, VertexFormat.Position | VertexFormat.Normal, Pool.Managed);
			Vector3[] positions = new Vector3[vertexList.Count];
			Vector3[] normals = new Vector3[vertexList.Count];
			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = vertexList[i].Position;
				normals[i] = vertexList[i].Normal;
			}
			ushort[] meshIndices = idxSet.MeshIndices;
			ushort[] morphIndices = idxSet.MorphIndices;
			List<Vector3> keyframePositions = keyframe.PositionList;
			List<Vector3> keyframeNormals = keyframe.NormalList;
			for (int i = 0; i < meshIndices.Length; i++)
			{
				positions[meshIndices[i]] = keyframePositions[morphIndices[i]];
				normals[meshIndices[i]] = keyframeNormals[morphIndices[i]];
			}

			using (DataStream vertexStream = vertBuffer.Lock(0, vertBufferSize, LockFlags.None))
			{
				for (int i = 0; i < positions.Length; i++)
				{
					Vector3 pos = positions[i];
					vertexStream.Write(pos.X);
					vertexStream.Write(pos.Y);
					vertexStream.Write(pos.Z);
					Vector3 normal = normals[i];
					vertexStream.Write(normal.X);
					vertexStream.Write(normal.Y);
					vertexStream.Write(normal.Z);
				}
				vertBuffer.Unlock();
			}

			return vertBuffer;
		}

		public float UnsetMorphKeyframe(xxFrame meshFrame, xaMorphIndexSet idxSet, bool asStart)
		{
			foreach (AnimationFrame frame in meshFrames)
			{
				if (frame.Name == meshFrame.Name)
				{
					xxMesh xxMesh = meshFrame.Mesh;
					int meshObjIdx = xa.MorphMeshObjIdx(idxSet.MeshIndices, xxMesh);
					if (meshObjIdx < 0)
					{
						Report.ReportLog("no valid mesh object was found for the morph");
						return -1f;
					}
					MeshContainer animMesh = frame.MeshContainer;
					for (int i = 1; i < meshObjIdx; i++)
					{
						animMesh = animMesh.NextMeshContainer;
						if (animMesh == null)
							break;
					}
					if (animMesh == null)
					{
						Report.ReportLog("Bad submesh specified.");
						return -1f;
					}
					MorphMeshContainer morphMesh = (MorphMeshContainer)animMesh;

					if (asStart)
					{
						if (morphMesh.StartBuffer != morphMesh.EndBuffer)
						{
							morphMesh.StartBuffer.Dispose();
							morphMesh.StartBuffer = morphMesh.EndBuffer;
						}
						else
						{
							frame.MeshContainer = morphMesh.NextMeshContainer;
						}
						morphMesh.TweenFactor = 1.0f;
					}
					else
					{
						if (morphMesh.StartBuffer != morphMesh.EndBuffer)
						{
							morphMesh.EndBuffer.Dispose();
							morphMesh.EndBuffer = morphMesh.StartBuffer;
						}
						else
						{
							frame.MeshContainer = morphMesh.NextMeshContainer;
						}
						morphMesh.TweenFactor = 0.0f;
					}
					return morphMesh.TweenFactor;
				}
			}
			Report.ReportLog("Mesh frame " + meshFrame + " not displayed.");
			return -1f;
		}

		public void SetTweenFactor(xxFrame meshFrame, xaMorphIndexSet idxSet, float tweenFactor)
		{
			foreach (AnimationFrame frame in meshFrames)
			{
				if (frame.Name == meshFrame.Name)
				{
					xxMesh xxMesh = meshFrame.Mesh;
					int meshObjIdx = xa.MorphMeshObjIdx(idxSet.MeshIndices, xxMesh);
					if (meshObjIdx < 0)
					{
						Report.ReportLog("no valid mesh object was found for the morph");
						return;
					}
					MeshContainer animMesh = frame.MeshContainer;
					for (int i = 1; i < meshObjIdx; i++)
					{
						animMesh = animMesh.NextMeshContainer;
						if (animMesh == null)
							break;
					}
					if (animMesh == null)
					{
						Report.ReportLog("Bad submesh specified.");
						return;
					}
					MorphMeshContainer morphMesh = (MorphMeshContainer)animMesh;

					morphMesh.TweenFactor = tweenFactor;
					return;
				}
			}
			Report.ReportLog("Mesh frame " + meshFrame + " not displayed.");
			return;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PositionBlendWeightIndexedColored
	{
		public Vector3 Position;
		public float Weight;
		public int BoneIndices;
		public int Color;

		public static readonly VertexFormat Format = VertexFormat.PositionBlend2 | VertexFormat.LastBetaUByte4 | VertexFormat.Diffuse;

		public PositionBlendWeightIndexedColored(Vector3 pos, byte boneIndex, int color)
		{
			Position = pos;
			Weight = 1;
			BoneIndices = boneIndex;
			Color = color;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PositionBlendWeightsIndexedColored
	{
		public Vector3 Position;
		public Vector3 Weights3;
		public int BoneIndices;
		public int Color;

		public static readonly VertexFormat Format = VertexFormat.PositionBlend4 | VertexFormat.LastBetaUByte4 | VertexFormat.Diffuse;

		public PositionBlendWeightsIndexedColored(Vector3 pos, float[] weights3, byte[] boneIndices, int color)
		{
			Position = pos;
			Weights3 = new Vector3(weights3[0], weights3[1], weights3[2]);
			BoneIndices = BitConverter.ToInt32(boneIndices, 0);
			Color = color;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PositionBlendWeightsIndexedNormalTexturedColoured
	{
		public Vector3 Position;
		public Vector3 Weights3;
		public int BoneIndices;
		public Vector3 Normal;
		public int Colour;
		public float U, V;

		public static readonly VertexFormat Format = VertexFormat.PositionBlend4 | VertexFormat.LastBetaUByte4 | VertexFormat.Normal | VertexFormat.Texture1 | VertexFormat.Diffuse;

		public static readonly VertexElement[] LikeFormat = new[] {
			new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
			new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.BlendWeight, 0),
			new VertexElement(0, 24, DeclarationType.Ubyte4, DeclarationMethod.Default, DeclarationUsage.BlendIndices, 0),
			new VertexElement(0, 28, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
			new VertexElement(0, 40, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
			new VertexElement(0, 44, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
			VertexElement.VertexDeclarationEnd
		};

		public PositionBlendWeightsIndexedNormalTexturedColoured(Vector3 pos, Vector3 norm, float[] weights3, byte[] boneIndices, Vector2 uv, int colour)
		{
			Position = pos;
			Normal = norm;
			Weights3 = new Vector3(weights3[0], weights3[1], weights3[2]);
			BoneIndices = BitConverter.ToInt32(boneIndices, 0);
			Colour = colour;
			U = uv[0];
			V = uv[1];
		}
	}

	public static class TweeningMeshesVertexBufferFormat
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct Stream0
		{
			public Vector3 Position;
			public Vector3 Normal;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct Stream1
		{
			public Vector3 Position;
			public Vector3 Normal;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct Stream2
		{
			public float U, V;
		}

		public static readonly VertexElement[] ThreeStreams = new[] {
			new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
			new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
			new VertexElement(1, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 1),
			new VertexElement(1, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 1),
			new VertexElement(2, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
			VertexElement.VertexDeclarationEnd
		};
	}

	public class AnimationFrame : Frame
	{
		public AnimationFrame Parent { get; set; }
		public Matrix OriginalTransform { get; set; }
		public Matrix CombinedTransform { get; set; }
		public BoundingBox Bounds { get; set; }

		public AnimationFrame()
		{
			OriginalTransform = Matrix.Identity;
			TransformationMatrix = Matrix.Identity;
			CombinedTransform = Matrix.Identity;
		}
	}

	public class AnimationMeshContainer : MeshContainer
	{
		public PositionBlendWeightsIndexedColored[] NormalLines { get; set; }
		public PositionBlendWeightIndexedColored[] BoneLines { get; set; }

		public string[] BoneNames { get; set; }
		public AnimationFrame[] BoneFrames { get; set; }
		public Matrix[] BoneOffsets { get; set; }

		public int MaterialIndex { get; set; }
		public int TextureIndex { get; set; }

		public AnimationMeshContainer()
		{
			MaterialIndex = -1;
			TextureIndex = -1;
		}
	}

	public class MorphMeshContainer : MeshContainer
	{
		public int MaterialIndex { get; set; }
		public int TextureIndex { get; set; }

		public int VertexCount { get; set; }
		public int FaceCount { get; set; }
		public VertexBuffer StartBuffer { get; set; }
		public VertexBuffer EndBuffer { get; set; }
		public VertexBuffer CommonBuffer { get; set; }
		public IndexBuffer IndexBuffer { get; set; }

		public float TweenFactor { get; set; }

		public MorphMeshContainer()
		{
			MaterialIndex = -1;
			TextureIndex = -1;

			TweenFactor = 0f;
		}
	}
}