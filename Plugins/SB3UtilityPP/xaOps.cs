using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SlimDX;

namespace SB3Utility
{
	public enum ReplaceAnimationMethod
	{
		Replace,
		Merge,
		Insert,
		Append
	}

	public static class xa
	{
		public static xaMorphIndexSet FindMorphIndexSet(string name, xaMorphSection section)
		{
			for (int i = 0; i < section.IndexSetList.Count; i++)
			{
				if (section.IndexSetList[i].Name == name)
				{
					return section.IndexSetList[i];
				}
			}
			return null;
		}

		public static xaMorphKeyframe FindMorphKeyFrame(string name, xaMorphSection section)
		{
			for (int i = 0; i < section.KeyframeList.Count; i++)
			{
				if (section.KeyframeList[i].Name == name)
				{
					return section.KeyframeList[i];
				}
			}
			return null;
		}

		public static int MorphMeshObjIdx(ushort[] meshIndices, xxMesh mesh)
		{
			int meshObjIdx = -1;
			if (mesh.SubmeshList.Count > 0)
			{
				if (mesh.SubmeshList.Count == 1)
				{
					if (ValidIndices(meshIndices, mesh.SubmeshList[0].VertexList))
					{
						meshObjIdx = 0;
					}
				}
				else
				{
					float maxModified = 0;
					for (int i = 0; i < mesh.SubmeshList.Count; i++)
					{
						if (ValidIndices(meshIndices, mesh.SubmeshList[i].VertexList))
						{
							float modified = (float)meshIndices.Length / mesh.SubmeshList[i].VertexList.Count;
							if (modified > maxModified)
							{
								maxModified = modified;
								meshObjIdx = i;
							}
						}
					}
				}
			}
			return meshObjIdx;
		}

		static bool ValidIndices(ushort[] meshIndices, List<xxVertex> vertList)
		{
			bool valid = true;
			foreach (ushort index in meshIndices)
			{
				if (index >= vertList.Count)
				{
					valid = false;
					break;
				}
			}
			return valid;
		}

		public static void ReplaceMorph(string destMorphName, xaParser parser, WorkspaceMorph wsMorphList, string newMorphName, bool replaceNormals, float minSquaredDistance)
		{
			if (parser.MorphSection == null)
			{
				Report.ReportLog("The .xa file doesn't have a morph section. Skipping these morphs");
				return;
			}

			xaMorphSection morphSection = parser.MorphSection;
			xaMorphIndexSet indices = FindMorphIndexSet(destMorphName, morphSection);
			if (indices == null)
			{
				Report.ReportLog("Couldn't find morph clip " + destMorphName + ". Skipping these morphs");
				return;
			}

			Report.ReportLog("Replacing morphs ...");
			try
			{
				ushort[] meshIndices = indices.MeshIndices;
				ushort[] morphIndices = indices.MorphIndices;
				foreach (ImportedMorphKeyframe wsMorph in wsMorphList.KeyframeList)
				{
					if (!wsMorphList.isMorphKeyframeEnabled(wsMorph))
						continue;

					List<ImportedVertex> vertList = wsMorph.VertexList;
					xaMorphKeyframe keyframe = FindMorphKeyFrame(wsMorph.Name, morphSection);
					if (keyframe == null)
					{
						keyframe = new xaMorphKeyframe();
						keyframe.Name = wsMorph.Name;
						keyframe.PositionList = new List<Vector3>(new Vector3[wsMorph.VertexList.Count]);
						keyframe.NormalList = new List<Vector3>(new Vector3[wsMorph.VertexList.Count]);
						for (int i = 0; i < meshIndices.Length; i++)
						{
							keyframe.PositionList[morphIndices[i]] = vertList[meshIndices[i]].Position;
							keyframe.NormalList[morphIndices[i]] = vertList[meshIndices[i]].Normal;
						}
						morphSection.KeyframeList.Add(keyframe);
					}
					else
					{
						for (int i = 0; i < meshIndices.Length; i++)
						{
							Vector3 orgPos = new Vector3(keyframe.PositionList[morphIndices[i]].X, keyframe.PositionList[morphIndices[i]].Y, keyframe.PositionList[morphIndices[i]].Z),
								newPos = new Vector3(vertList[meshIndices[i]].Position.X, vertList[meshIndices[i]].Position.Y, vertList[meshIndices[i]].Position.Z);
							if ((orgPos - newPos).LengthSquared() >= minSquaredDistance)
							{
								keyframe.PositionList[morphIndices[i]] = vertList[meshIndices[i]].Position;
								if (replaceNormals)
								{
									keyframe.NormalList[morphIndices[i]] = vertList[meshIndices[i]].Normal;
								}
							}
						}
					}

					string morphNewName = wsMorphList.getMorphKeyframeNewName(wsMorph);
					if (morphNewName != String.Empty)
					{
						for (int i = 0; i < morphSection.ClipList.Count; i++)
						{
							xaMorphClip clip = morphSection.ClipList[i];
							for (int j = 0; j < clip.KeyframeRefList.Count; j++)
							{
								xaMorphKeyframeRef keyframeRef = clip.KeyframeRefList[j];
								if (keyframeRef.Name == wsMorph.Name)
									keyframeRef.Name = morphNewName;
							}
						}
						keyframe.Name = morphNewName;
					}
				}
				if (newMorphName != String.Empty)
				{
					for (int i = 0; i < morphSection.ClipList.Count; i++)
					{
						xaMorphClip clip = morphSection.ClipList[i];
						if (clip.Name == destMorphName)
						{
							clip.Name = newMorphName;
							break;
						}
					}
					indices.Name = newMorphName;
				}
			}
			catch (Exception ex)
			{
				Report.ReportLog("Error replacing morphs: " + ex.Message);
			}
		}

		public static void CalculateNormals(xaParser parser, xxFrame meshFrame, string morphClip, string keyframe, float threshold)
		{
			HashSet<Tuple<xaMorphClip, xaMorphKeyframe>> keyframes = new HashSet<Tuple<xaMorphClip, xaMorphKeyframe>>();
			foreach (xaMorphClip clip in parser.MorphSection.ClipList)
			{
				if (morphClip != null && clip.Name != morphClip)
					continue;

				if (keyframe != null)
				{
					xaMorphKeyframe xaKeyframe = FindMorphKeyFrame(keyframe, parser.MorphSection);
					if (xaKeyframe == null)
					{
						throw new Exception("keyframe " + keyframe + " not found in morph clip " + morphClip);
					}
					keyframes.Add(new Tuple<xaMorphClip, xaMorphKeyframe>(clip, xaKeyframe));
					break;
				}
				else
				{
					foreach (xaMorphKeyframeRef morphRef in clip.KeyframeRefList)
					{
						xaMorphKeyframe xaKeyframe = FindMorphKeyFrame(morphRef.Name, parser.MorphSection);
						keyframes.Add(new Tuple<xaMorphClip, xaMorphKeyframe>(clip, xaKeyframe));
					}
				}
			}
			if (keyframes.Count == 0)
			{
				Report.ReportLog("No keyframe for mesh " + meshFrame.Name + " to calculate normals for found.");
				return;
			}

			foreach (var tup in keyframes)
			{
				xaMorphIndexSet set = FindMorphIndexSet(tup.Item1.Name, parser.MorphSection);
				CalculateNormals(parser, meshFrame, tup.Item2, set, threshold);
			}
		}

		private static void CalculateNormals(xaParser parser, xxFrame meshFrame, xaMorphKeyframe keyframe, xaMorphIndexSet set, float threshold)
		{
			xxMesh mesh = meshFrame.Mesh;
			ushort[] meshIndices = set.MeshIndices;
			ushort[] morphIndices = set.MorphIndices;
			int morphSubmeshIdx = MorphMeshObjIdx(meshIndices, mesh);
			if (morphSubmeshIdx < 0)
			{
				throw new Exception("no valid mesh object was found for the morph " + set.Name);
			}
			xxSubmesh submesh = mesh.SubmeshList[morphSubmeshIdx];
			List<xxVertex> morphedVertices = new List<xxVertex>(submesh.VertexList.Count);
			for (ushort i = 0; i < submesh.VertexList.Count; i++)
			{
				xxVertex vert = new xxVertexUShort();
				vert.Index = i;
				vert.Position = submesh.VertexList[i].Position;
				vert.Normal = submesh.VertexList[i].Normal;
				morphedVertices.Add(vert);
			}
			for (int i = 0; i < meshIndices.Length; i++)
			{
				morphedVertices[meshIndices[i]].Position = keyframe.PositionList[morphIndices[i]];
			}

			var pairList = new List<Tuple<List<xxFace>, List<xxVertex>>>(1);
			pairList.Add(new Tuple<List<xxFace>, List<xxVertex>>(submesh.FaceList, morphedVertices));
			xx.CalculateNormals(pairList, threshold);

			for (int i = 0; i < meshIndices.Length; i++)
			{
				keyframe.NormalList[morphIndices[i]] = morphedVertices[meshIndices[i]].Normal;
			}
		}

		public static void animationNormalizeTrack(xaAnimationKeyframe[] origKeyframes, xaAnimationKeyframe[] destKeyframes, int count)
		{
			xaAnimationKeyframe keyframeCopy;
			if (origKeyframes.Length > 0)
			{
				keyframeCopy = origKeyframes[origKeyframes.Length - 1];
			}
			else
			{
				keyframeCopy = new xaAnimationKeyframe();
				keyframeCopy.Rotation = Quaternion.Identity;
				keyframeCopy.Scaling = new Vector3(1, 1, 1);
				keyframeCopy.Translation = new Vector3(0, 0, 0);
				CreateUnknowns(keyframeCopy);
			}
			for (int j = origKeyframes.Length; j < count; j++)
			{
				keyframeCopy.Index = j;
				destKeyframes[j] = keyframeCopy;
			}
		}

		public static void CreateUnknowns(xaAnimationKeyframe keyframe)
		{
			keyframe.Unknown1 = new byte[8];
		}

		public static void CreateUnknowns(xaAnimationTrack track)
		{
			track.Unknown1 = new byte[4];
		}

		public static void CreateUnknowns(xaMorphKeyframeRef morphRef)
		{
			morphRef.Unknown1 = new byte[1];
			morphRef.Unknown2 = new byte[1];
		}

		public static void animationCopyKeyframeTransformArray(xaAnimationKeyframe[] src, int srcIdx, xaAnimationKeyframe[] dest, int destIdx, int count)
		{
			for (int i = 0; i < count; i++)
			{
				xaAnimationKeyframe keyframe = src[srcIdx + i];
				keyframe.Index = destIdx + i;
				dest[destIdx + i] = keyframe;
			}
		}

		public static xaAnimationKeyframe[] animationGetOriginalKeyframes(Dictionary<string, xaAnimationTrack> animationNodeDic, string trackName, List<xaAnimationTrack> animationNodeList, out xaAnimationTrack animationNode)
		{
			xaAnimationKeyframe[] origKeyframes;
			if (animationNodeDic.TryGetValue(trackName, out animationNode))
			{
				origKeyframes = animationNode.KeyframeList.ToArray();
			}
			else
			{
				animationNode = new xaAnimationTrack();
				animationNodeList.Add(animationNode);
				animationNode.Name = trackName;
				CreateUnknowns(animationNode);
				origKeyframes = new xaAnimationKeyframe[0];
			}
			return origKeyframes;
		}
	}
}
