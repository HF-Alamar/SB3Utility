using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SlimDX;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		[PluginOpensFile(".fbx")]
		public static void WorkspaceFbx(string path, string variable)
		{
			string importVar = Gui.Scripting.GetNextVariable("importFbx");
			var importer = (Fbx.Importer)Gui.Scripting.RunScript(importVar + " = ImportFbx(path=\"" + path + "\", EulerFilter=" + (bool)Gui.Config["FbxImportAnimationEulerFilter"] + ", filterPrecision=" + (float)Gui.Config["FbxImportAnimationFilterPrecision"]+ ")");

			string editorVar = Gui.Scripting.GetNextVariable("importedEditor");
			var editor = (ImportedEditor)Gui.Scripting.RunScript(editorVar + " = ImportedEditor(" + importVar + ")");

			new FormWorkspace(path, importer, editorVar, editor);
		}

		[Plugin]
		public static void ExportFbx([DefaultVar]xxParser xxParser, object[] meshNames, object[] xaParsers, int startKeyframe, int endKeyframe, bool linear, string path, string exportFormat, bool allFrames, bool skins)
		{
			List<xaParser> xaParserList = null;
			if (xaParsers != null)
			{
				xaParserList = new List<xaParser>(Utility.Convert<xaParser>(xaParsers));
			}

			List<xxFrame> meshFrames = xx.FindMeshFrames(xxParser.Frame, new List<string>(Utility.Convert<string>(meshNames)));
			Fbx.Exporter.Export(path, xxParser, meshFrames, xaParserList, startKeyframe, endKeyframe, linear, exportFormat, allFrames, skins);
		}

		[Plugin]
		public static void ExportMorphFbx([DefaultVar]xxParser xxparser, string path, xxFrame meshFrame, xaParser xaparser, xaMorphClip morphClip, string exportFormat)
		{
			Fbx.Exporter.ExportMorph(path, xxparser, meshFrame, morphClip, xaparser, exportFormat);
		}

		[Plugin]
		public static Fbx.Importer ImportFbx([DefaultVar]string path, bool EulerFilter, double filterPrecision)
		{
			return new Fbx.Importer(path, EulerFilter, (float)filterPrecision);
		}
	}

	public static class FbxUtility
	{
		public static Vector3 QuaternionToEuler(Quaternion q)
		{
			return Fbx.QuaternionToEuler(q);
		}

		public static Quaternion EulerToQuaternion(Vector3 v)
		{
			return Fbx.EulerToQuaternion(v);
		}

		public static Matrix SRTToMatrix(Vector3 scale, Vector3 euler, Vector3 translate)
		{
			return Matrix.Scaling(scale) * Matrix.RotationQuaternion(EulerToQuaternion(euler)) * Matrix.Translation(translate);
		}

		public static Vector3[] MatrixToSRT(Matrix m)
		{
			Quaternion q;
			Vector3[] srt = new Vector3[3];
			m.Decompose(out srt[0], out q, out srt[2]);
			srt[1] = QuaternionToEuler(q);
			return srt;
		}

		public static void Export(String path, IImported imp, int startKeyframe, int endKeyframe, bool linear, bool EulerFilter, float filterPrecision, String exportFormat, bool allFrames, bool skins)
		{
			Fbx.Exporter.Export(path, imp, startKeyframe, endKeyframe, linear, EulerFilter, filterPrecision, exportFormat, allFrames, skins);
		}

		public static List<KeyValuePair<string, ImportedAnimationKeyframe[]>> CopyAnimation(WorkspaceAnimation wsAnimation, int resampleCount, bool linear)
		{
			List<KeyValuePair<string, ImportedAnimationKeyframe[]>> newTrackList = new List<KeyValuePair<string, ImportedAnimationKeyframe[]>>(wsAnimation.TrackList.Count);
			List<Tuple<ImportedAnimationTrack, ImportedAnimationKeyframe[]>> interpolateTracks = new List<Tuple<ImportedAnimationTrack, ImportedAnimationKeyframe[]>>();
			foreach (var wsTrack in wsAnimation.TrackList)
			{
				if (!wsAnimation.isTrackEnabled(wsTrack))
					continue;
				ImportedAnimationKeyframe[] newKeyframes;
				if (resampleCount < 0 || wsTrack.Keyframes.Length == resampleCount)
				{
					newKeyframes = new ImportedAnimationKeyframe[wsTrack.Keyframes.Length];
					for (int i = 0; i < wsTrack.Keyframes.Length; i++)
					{
						ImportedAnimationKeyframe keyframe = wsTrack.Keyframes[i];
						if (keyframe == null)
							continue;

						newKeyframes[i] = new ImportedAnimationKeyframe();
						newKeyframes[i].Rotation = keyframe.Rotation;
						newKeyframes[i].Translation = keyframe.Translation;
						newKeyframes[i].Scaling = keyframe.Scaling;
					}
				}
				else
				{
					newKeyframes = new ImportedAnimationKeyframe[resampleCount];
					if (wsTrack.Keyframes.Length < 1)
					{
						ImportedAnimationKeyframe keyframe = new ImportedAnimationKeyframe();
						keyframe.Rotation = Quaternion.Identity;
						keyframe.Scaling = new Vector3(1, 1, 1);
						keyframe.Translation = new Vector3(0, 0, 0);

						for (int i = 0; i < newKeyframes.Length; i++)
						{
							newKeyframes[i] = keyframe;
						}
					}
					else
					{
						interpolateTracks.Add(new Tuple<ImportedAnimationTrack, ImportedAnimationKeyframe[]>(wsTrack, newKeyframes));
					}
				}

				newTrackList.Add(new KeyValuePair<string, ImportedAnimationKeyframe[]>(wsTrack.Name, newKeyframes));
			}
			if (resampleCount >= 0)
			{
				Fbx.InterpolateKeyframes(interpolateTracks, resampleCount, linear);
			}
			return newTrackList;
		}

		public static void ReplaceAnimation(ReplaceAnimationMethod replaceMethod, int insertPos, List<KeyValuePair<string, ImportedAnimationKeyframe[]>> newTrackList, ImportedAnimation iAnim, Dictionary<string, ImportedAnimationTrack> animationNodeDic, bool negateQuaternionFlips)
		{
			if (replaceMethod == ReplaceAnimationMethod.Replace)
			{
				foreach (var newTrack in newTrackList)
				{
					ImportedAnimationTrack iTrack = new ImportedAnimationTrack();
					iAnim.TrackList.Add(iTrack);
					iTrack.Name = newTrack.Key;
					iTrack.Keyframes = newTrack.Value;
				}
			}
			else if (replaceMethod == ReplaceAnimationMethod.Merge)
			{
				foreach (var newTrack in newTrackList)
				{
					ImportedAnimationTrack animationNode;
					ImportedAnimationKeyframe[] origKeyframes = FbxUtility.animationGetOriginalKeyframes(animationNodeDic, newTrack.Key, iAnim, out animationNode);
					ImportedAnimationKeyframe[] destKeyframes;
					int newEnd = insertPos + newTrack.Value.Length;
					if (origKeyframes.Length < insertPos)
					{
						destKeyframes = new ImportedAnimationKeyframe[newEnd];
						FbxUtility.animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, origKeyframes.Length);
						FbxUtility.animationNormalizeTrack(origKeyframes, destKeyframes, insertPos);
					}
					else
					{
						if (origKeyframes.Length < newEnd)
						{
							destKeyframes = new ImportedAnimationKeyframe[newEnd];
						}
						else
						{
							destKeyframes = new ImportedAnimationKeyframe[origKeyframes.Length];
							FbxUtility.animationCopyKeyframeTransformArray(origKeyframes, newEnd, destKeyframes, newEnd, origKeyframes.Length - newEnd);
						}
						FbxUtility.animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, insertPos);
					}

					FbxUtility.animationCopyKeyframeTransformArray(newTrack.Value, 0, destKeyframes, insertPos, newTrack.Value.Length);
					animationNode.Keyframes = destKeyframes;
				}
			}
			else if (replaceMethod == ReplaceAnimationMethod.Insert)
			{
				foreach (var newTrack in newTrackList)
				{
					ImportedAnimationTrack animationNode;
					ImportedAnimationKeyframe[] origKeyframes = FbxUtility.animationGetOriginalKeyframes(animationNodeDic, newTrack.Key, iAnim, out animationNode);
					ImportedAnimationKeyframe[] destKeyframes;
					int newEnd = insertPos + newTrack.Value.Length;
					if (origKeyframes.Length < insertPos)
					{
						destKeyframes = new ImportedAnimationKeyframe[newEnd];
						FbxUtility.animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, origKeyframes.Length);
						FbxUtility.animationNormalizeTrack(origKeyframes, destKeyframes, insertPos);
					}
					else
					{
						destKeyframes = new ImportedAnimationKeyframe[origKeyframes.Length + newTrack.Value.Length];
						FbxUtility.animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, insertPos);
						FbxUtility.animationCopyKeyframeTransformArray(origKeyframes, insertPos, destKeyframes, newEnd, origKeyframes.Length - insertPos);
					}

					FbxUtility.animationCopyKeyframeTransformArray(newTrack.Value, 0, destKeyframes, insertPos, newTrack.Value.Length);
					animationNode.Keyframes = destKeyframes;
				}
			}
			else if (replaceMethod == ReplaceAnimationMethod.Append)
			{
				foreach (var newTrack in newTrackList)
				{
					ImportedAnimationTrack animationNode;
					ImportedAnimationKeyframe[] origKeyframes = FbxUtility.animationGetOriginalKeyframes(animationNodeDic, newTrack.Key, iAnim, out animationNode);
					ImportedAnimationKeyframe[] destKeyframes = new ImportedAnimationKeyframe[origKeyframes.Length + newTrack.Value.Length];
					FbxUtility.animationCopyKeyframeTransformArray(origKeyframes, 0, destKeyframes, 0, origKeyframes.Length);
					FbxUtility.animationCopyKeyframeTransformArray(newTrack.Value, 0, destKeyframes, origKeyframes.Length, newTrack.Value.Length);
					animationNode.Keyframes = destKeyframes;
				}
			}
			else
			{
				Report.ReportLog("Error: Unexpected animation replace method " + replaceMethod + ". Skipping this animation");
			}

			if (negateQuaternionFlips)
			{
				foreach (var newTrack in iAnim.TrackList)
				{
					Quaternion lastQ = Quaternion.Identity;
					for (int i = 0, lastUsed_keyIndex = -1; i < newTrack.Keyframes.Length; i++)
					{
						ImportedAnimationKeyframe iKeyframe = newTrack.Keyframes[i];
						if (iKeyframe == null)
							continue;

						Quaternion q = iKeyframe.Rotation;
						if (lastUsed_keyIndex >= 0)
						{
							bool diffX = Math.Sign(lastQ.X) != Math.Sign(q.X);
							bool diffY = Math.Sign(lastQ.Y) != Math.Sign(q.Y);
							bool diffZ = Math.Sign(lastQ.Z) != Math.Sign(q.Z);
							bool diffW = Math.Sign(lastQ.W) != Math.Sign(q.W);
							if (diffX && diffY && diffZ && diffW)
							{
								q.X = -q.X;
								q.Y = -q.Y;
								q.Z = -q.Z;
								q.W = -q.W;

								iKeyframe.Rotation = q;
							}
						}
						lastQ = q;
						lastUsed_keyIndex = i;
					}
				}
			}
		}

		public static void animationNormalizeTrack(ImportedAnimationKeyframe[] origKeyframes, ImportedAnimationKeyframe[] destKeyframes, int count)
		{
			ImportedAnimationKeyframe keyframeCopy;
			if (origKeyframes.Length > 0)
			{
				keyframeCopy = origKeyframes[origKeyframes.Length - 1];
			}
			else
			{
				keyframeCopy = new ImportedAnimationKeyframe();
				keyframeCopy.Rotation = Quaternion.Identity;
				keyframeCopy.Scaling = new Vector3(1, 1, 1);
				keyframeCopy.Translation = new Vector3(0, 0, 0);
			}
			for (int j = origKeyframes.Length; j < count; j++)
			{
				destKeyframes[j] = keyframeCopy;
			}
		}

		public static void animationCopyKeyframeTransformArray(ImportedAnimationKeyframe[] src, int srcIdx, ImportedAnimationKeyframe[] dest, int destIdx, int count)
		{
			for (int i = 0; i < count; i++)
			{
				ImportedAnimationKeyframe keyframe = src[srcIdx + i];
				dest[destIdx + i] = keyframe;
			}
		}

		public static ImportedAnimationKeyframe[] animationGetOriginalKeyframes(Dictionary<string, ImportedAnimationTrack> animationNodeDic, string trackName, ImportedAnimation anim, out ImportedAnimationTrack animationNode)
		{
			ImportedAnimationKeyframe[] origKeyframes;
			if (animationNodeDic.TryGetValue(trackName, out animationNode))
			{
				origKeyframes = animationNode.Keyframes;
			}
			else
			{
				animationNode = new ImportedAnimationTrack();
				anim.TrackList.Add(animationNode);
				animationNode.Name = trackName;
				origKeyframes = new ImportedAnimationKeyframe[0];
			}
			return origKeyframes;
		}
	}
}
