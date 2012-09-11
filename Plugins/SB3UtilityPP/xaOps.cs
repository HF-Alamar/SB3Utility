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
					xaMorphKeyframe keyframe = FindMorphKeyFrame(wsMorph.Name, morphSection);
					if (keyframe == null)
					{
						Report.ReportLog("Warning: Couldn't find morph keyframe " + wsMorph.Name + ". Skipping this morph");
						continue;
					}

					List<ImportedVertex> vertList = wsMorph.VertexList;
					for (int i = 0; i < meshIndices.Length; i++)
					{
						Vector3 orgPos = new Vector3(keyframe.PositionList[morphIndices[i]].X, keyframe.PositionList[morphIndices[i]].Y, keyframe.PositionList[morphIndices[i]].Z),
							newPos = new Vector3(vertList[meshIndices[i]].Position.X, vertList[meshIndices[i]].Position.Y, vertList[meshIndices[i]].Position.Z);
						if ((orgPos - newPos).LengthSquared() >= minSquaredDistance)
							keyframe.PositionList[morphIndices[i]] = vertList[meshIndices[i]].Position;
						if (replaceNormals)
						{
							keyframe.NormalList[morphIndices[i]] = vertList[meshIndices[i]].Normal;
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
						if (clip.Name == wsMorphList.Name)
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

		public static void ReplaceAnimation(WorkspaceAnimation wsAnimation, xaParser parser, int resampleCount, ReplaceAnimationMethod replaceMethod, int insertPos)
		{
			if (parser.AnimationSection == null)
			{
				Report.ReportLog("The .xa file doesn't have an animation section. Skipping this animation");
				return;
			}

			Report.ReportLog("Replacing animation ...");
			List<KeyValuePair<string, xaAnimationKeyframe[]>> newTrackList = new List<KeyValuePair<string, xaAnimationKeyframe[]>>(wsAnimation.TrackList.Count);
			foreach (var wsTrack in wsAnimation.TrackList)
			{
				if (!wsAnimation.isTrackEnabled(wsTrack))
					continue;
				xaAnimationKeyframe[] newKeyframes = new xaAnimationKeyframe[resampleCount];
				if (wsTrack.Keyframes.Length == resampleCount)
				{
					for (int i = 0; i < wsTrack.Keyframes.Length; i++)
					{
						ImportedAnimationKeyframe keyframe = wsTrack.Keyframes[i];
						newKeyframes[i] = new xaAnimationKeyframe();
						newKeyframes[i].Index = i;
						newKeyframes[i].Rotation = keyframe.Rotation;
						CreateUnknowns(newKeyframes[i]);
						newKeyframes[i].Translation = keyframe.Translation;
						newKeyframes[i].Scaling = keyframe.Scaling;
					}
				}
				else
				{
					if (wsTrack.Keyframes.Length < 1)
					{
						xaAnimationKeyframe keyframe = new xaAnimationKeyframe();
						keyframe.Rotation = Quaternion.Identity;
						keyframe.Scaling = new Vector3(1, 1, 1);
						keyframe.Translation = new Vector3(0, 0, 0);
						CreateUnknowns(keyframe);

						for (int i = 0; i < newKeyframes.Length; i++)
						{
							keyframe.Index = i;
							newKeyframes[i] = keyframe;
						}
					}
					else if ((wsTrack.Keyframes.Length == 1) || (resampleCount == 1))
					{
						ImportedAnimationKeyframe keyframe = wsTrack.Keyframes[0];
						for (int i = 0; i < newKeyframes.Length; i++)
						{
							newKeyframes[i] = new xaAnimationKeyframe();
							newKeyframes[i].Index = i;
							newKeyframes[i].Rotation = keyframe.Rotation;
							CreateUnknowns(newKeyframes[i]);
							newKeyframes[i].Translation = keyframe.Translation;
							newKeyframes[i].Scaling = keyframe.Scaling;
						}
					}
					else
					{
/*						float animationLen = (float)(resampleCount - 1);
						Animation animation = new Animation("tempWorkspaceAnimation", animationLen);
						NodeAnimationTrack nodeTrack = animation.CreateNodeTrack(0);
						NodeAnimationTrack interpolatedTrack = animation.CreateNodeTrack(1);
						for (int i = 0; i < wsTrack.Keyframes.Length; i++)
						{
							float timePos = i * animationLen / (wsTrack.Keyframes.Length - 1);
							TransformKeyFrame ogreKeyframe = nodeTrack.CreateNodeKeyFrame(timePos);
							ImportedAnimationKeyframe wsKeyframe = wsTrack.Keyframes[i];
							ogreKeyframe.Translate = wsKeyframe.Translation;
							ogreKeyframe.Rotation = wsKeyframe.Rotation;
							ogreKeyframe.Scale = wsKeyframe.Scaling;
						}
						for (int i = 0; i < newKeyframes.Length; i++)
						{
							TransformKeyFrame ogreKeyframe = interpolatedTrack.CreateNodeKeyFrame((float)i);
							nodeTrack.GetInterpolatedKeyFrame(new TimeIndex((float)i), ogreKeyframe);
							newKeyframes[i] = new xaAnimationKeyframe();
							newKeyframes[i].Index = i;
							newKeyframes[i].Rotation = ogreKeyframe.Rotation;
							newKeyframes[i].Scaling = ogreKeyframe.Scale;
							newKeyframes[i].Translation = ogreKeyframe.Translate;
							CreateUnknowns(newKeyframes[i]);
						}
						animation.DestroyAllTracks();
						animation.Dispose();*/
						Report.ReportLog("Interpolation of animation keyframes is not implemented.");
						return;
					}
				}

				newTrackList.Add(new KeyValuePair<string, xaAnimationKeyframe[]>(wsTrack.Name, newKeyframes));
			}

			List<xaAnimationTrack> animationNodeList = parser.AnimationSection.TrackList;
			Dictionary<string, xaAnimationTrack> animationNodeDic = null;
			if (replaceMethod != ReplaceAnimationMethod.Replace)
			{
				animationNodeDic = new Dictionary<string, xaAnimationTrack>();
				foreach (xaAnimationTrack animationNode in animationNodeList)
				{
					animationNodeDic.Add(animationNode.Name, animationNode);
				}
			}

			if (replaceMethod == ReplaceAnimationMethod.Replace)
			{
				animationNodeList.Clear();
				foreach (var newTrack in newTrackList)
				{
					xaAnimationTrack animationNode = new xaAnimationTrack();
					animationNodeList.Add(animationNode);
					animationNode.KeyframeList = new List<xaAnimationKeyframe>(newTrack.Value);
					animationNode.Name = newTrack.Key;
					CreateUnknowns(animationNode);
				}
			}
			else if (replaceMethod == ReplaceAnimationMethod.Merge)
			{
				foreach (var newTrack in newTrackList)
				{
					xaAnimationTrack animationNode;
					xaAnimationKeyframe[] origKeyframes = animationGetOriginalKeyframes(animationNodeDic, newTrack.Key, animationNodeList, out animationNode);
					xaAnimationKeyframe[] destKeyframes;
					int newEnd = insertPos + newTrack.Value.Length;
					if (origKeyframes.Length < insertPos)
					{
						destKeyframes = new xaAnimationKeyframe[newEnd];
						animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, origKeyframes.Length);
						animationNormalizeTrack(origKeyframes, destKeyframes, insertPos);
					}
					else
					{
						if (origKeyframes.Length < newEnd)
						{
							destKeyframes = new xaAnimationKeyframe[newEnd];
						}
						else
						{
							destKeyframes = new xaAnimationKeyframe[origKeyframes.Length];
							animationCopyKeyframeTransformArray(origKeyframes, newEnd, destKeyframes, newEnd, origKeyframes.Length - newEnd);
						}
						animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, insertPos);
					}

					animationCopyKeyframeTransformArray(newTrack.Value, 0, destKeyframes, insertPos, newTrack.Value.Length);
					animationNode.KeyframeList = new List<xaAnimationKeyframe>(destKeyframes);
				}
			}
			else if (replaceMethod == ReplaceAnimationMethod.Insert)
			{
				foreach (var newTrack in newTrackList)
				{
					xaAnimationTrack animationNode;
					xaAnimationKeyframe[] origKeyframes = animationGetOriginalKeyframes(animationNodeDic, newTrack.Key, animationNodeList, out animationNode); ;
					xaAnimationKeyframe[] destKeyframes;
					int newEnd = insertPos + newTrack.Value.Length;
					if (origKeyframes.Length < insertPos)
					{
						destKeyframes = new xaAnimationKeyframe[newEnd];
						animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, origKeyframes.Length);
						animationNormalizeTrack(origKeyframes, destKeyframes, insertPos);
					}
					else
					{
						destKeyframes = new xaAnimationKeyframe[origKeyframes.Length + newTrack.Value.Length];
						animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, insertPos);
						animationCopyKeyframeTransformArray(origKeyframes, insertPos, destKeyframes, newEnd, origKeyframes.Length - insertPos);
					}

					animationCopyKeyframeTransformArray(newTrack.Value, 0, destKeyframes, insertPos, newTrack.Value.Length);
					animationNode.KeyframeList = new List<xaAnimationKeyframe>(destKeyframes);
				}
			}
			else if (replaceMethod == ReplaceAnimationMethod.Append)
			{
				int maxKeyframes = 0;
				foreach (xaAnimationTrack animationNode in animationNodeList)
				{
					int numKeyframes = animationNode.KeyframeList.Count;
					if (numKeyframes > maxKeyframes)
					{
						maxKeyframes = numKeyframes;
					}
				}

				foreach (var newTrack in newTrackList)
				{
					xaAnimationTrack animationNode;
					xaAnimationKeyframe[] origKeyframes = animationGetOriginalKeyframes(animationNodeDic, newTrack.Key, animationNodeList, out animationNode);
					xaAnimationKeyframe[] destKeyframes = new xaAnimationKeyframe[maxKeyframes + newTrack.Value.Length];
					animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, origKeyframes.Length);
					if (origKeyframes.Length < maxKeyframes)
					{
						animationNormalizeTrack(origKeyframes, destKeyframes, maxKeyframes);
					}

					animationCopyKeyframeTransformArray(newTrack.Value, 0, destKeyframes, maxKeyframes, newTrack.Value.Length);
					animationNode.KeyframeList = new List<xaAnimationKeyframe>(destKeyframes);
				}
			}
			else
			{
				Report.ReportLog("Error: Unexpected animation replace method " + replaceMethod + ". Skipping this animation");
				return;
			}
		}

		private static void animationNormalizeTrack(xaAnimationKeyframe[] origKeyframes, xaAnimationKeyframe[] destKeyframes, int count)
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

		private static void animationCopyKeyframeTransformArray(xaAnimationKeyframe[] src, int srcIdx, xaAnimationKeyframe[] dest, int destIdx, int count)
		{
			for (int i = 0; i < count; i++)
			{
				xaAnimationKeyframe keyframe = src[srcIdx + i];
				keyframe.Index = destIdx + i;
				dest[destIdx + i] = keyframe;
			}
		}

		private static xaAnimationKeyframe[] animationGetOriginalKeyframes(Dictionary<string, xaAnimationTrack> animationNodeDic, string trackName, List<xaAnimationTrack> animationNodeList, out xaAnimationTrack animationNode)
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
