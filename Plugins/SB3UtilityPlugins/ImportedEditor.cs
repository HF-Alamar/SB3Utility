using System;
using System.Collections.Generic;
using System.Text;

namespace SB3Utility
{
	[Plugin]
	public class ImportedEditor
	{
		public IImported Imported { get; protected set; }
		public List<ImportedFrame> Frames { get; protected set; }
		public List<WorkspaceMesh> Meshes { get; protected set; }
		public List<WorkspaceMorph> Morphs { get; protected set; }

		public ImportedEditor(IImported imported)
		{
			Imported = imported;

			if ((Imported.FrameList != null) && (Imported.FrameList.Count > 0))
			{
				Frames = new List<ImportedFrame>();
				foreach (var frame in Imported.FrameList)
				{
					InitFrames(frame);
				}
			}

			if (Imported.MeshList != null && Imported.MeshList.Count > 0)
			{
				Meshes = new List<WorkspaceMesh>(Imported.MeshList.Count);
				foreach (ImportedMesh mesh in Imported.MeshList)
				{
					WorkspaceMesh wsMesh = new WorkspaceMesh(mesh);
					Meshes.Add(wsMesh);
				}
			}

			if (Imported.MorphList != null && Imported.MorphList.Count > 0)
			{
				Morphs = new List<WorkspaceMorph>(Imported.MorphList.Count);
				foreach (ImportedMorph morph in Imported.MorphList)
				{
					WorkspaceMorph wsMorph = new WorkspaceMorph(morph);
					Morphs.Add(wsMorph);
				}
			}
		}

		void InitFrames(ImportedFrame frame)
		{
			Frames.Add(frame);

			foreach (var child in frame)
			{
				InitFrames(child);
			}
		}

		[Plugin]
		public void setSubmeshEnabled(int meshId, int id, bool enabled)
		{
			ImportedSubmesh submesh = this.Meshes[meshId].SubmeshList[id];
			this.Meshes[meshId].setSubmeshEnabled(submesh, enabled);
		}

		[Plugin]
		public void setSubmeshReplacingOriginal(int meshId, int id, bool replaceOriginal)
		{
			ImportedSubmesh submesh = this.Meshes[meshId].SubmeshList[id];
			this.Meshes[meshId].setSubmeshReplacingOriginal(submesh, replaceOriginal);
		}

		[Plugin]
		public void setMorphKeyframeEnabled(int morphId, int id, bool enabled)
		{
			ImportedMorphKeyframe keyframe = this.Morphs[morphId].KeyframeList[id];
			this.Morphs[morphId].setMorphKeyframeEnabled(keyframe, enabled);
		}

		[Plugin]
		public void setMorphKeyframeNewName(int morphId, int id, string newName)
		{
			ImportedMorphKeyframe keyframe = this.Morphs[morphId].KeyframeList[id];
			this.Morphs[morphId].setMorphKeyframeNewName(keyframe, newName);
		}
	}
}
