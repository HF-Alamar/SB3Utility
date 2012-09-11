using System;
using System.Collections.Generic;

namespace SB3Utility
{
	public class WorkspaceMesh : ImportedMesh
	{
		protected class AdditionalSubmeshOptions
		{
			public bool Enabled = true;
			public bool ReplaceOriginal = true;
		}
		protected Dictionary<ImportedSubmesh, AdditionalSubmeshOptions> SubmeshOptions { get; set; }

		public WorkspaceMesh(ImportedMesh importedMesh) :
			base()
		{
			this.Name = importedMesh.Name;
			this.SubmeshList = importedMesh.SubmeshList;
			this.BoneList = importedMesh.BoneList;

			this.SubmeshOptions = new Dictionary<ImportedSubmesh, AdditionalSubmeshOptions>(importedMesh.SubmeshList.Count);
			foreach (ImportedSubmesh submesh in importedMesh.SubmeshList)
			{
				AdditionalSubmeshOptions options = new AdditionalSubmeshOptions();
				this.SubmeshOptions.Add(submesh, options);
			}
		}

		public bool isSubmeshEnabled(ImportedSubmesh submesh)
		{
			AdditionalSubmeshOptions options;
			if (this.SubmeshOptions.TryGetValue(submesh, out options))
			{
				return options.Enabled;
			}
			throw new Exception("Submesh not found");
		}

		public void setSubmeshEnabled(ImportedSubmesh submesh, bool enabled)
		{
			AdditionalSubmeshOptions options;
			if (this.SubmeshOptions.TryGetValue(submesh, out options))
			{
				options.Enabled = enabled;
				return;
			}
			throw new Exception("Submesh not found");
		}

		public bool isSubmeshReplacingOriginal(ImportedSubmesh submesh)
		{
			AdditionalSubmeshOptions options;
			if (this.SubmeshOptions.TryGetValue(submesh, out options))
			{
				return options.ReplaceOriginal;
			}
			throw new Exception("Submesh not found");
		}

		public void setSubmeshReplacingOriginal(ImportedSubmesh submesh, bool replaceOriginal)
		{
			AdditionalSubmeshOptions options;
			if (this.SubmeshOptions.TryGetValue(submesh, out options))
			{
				options.ReplaceOriginal = replaceOriginal;
				return;
			}
			throw new Exception("Submesh not found");
		}
	}

	public class WorkspaceMorph : ImportedMorph
	{
		protected class AdditionalMorphKeyframeOptions
		{
			public bool Enabled = true;
			public string NewName;
		}
		protected Dictionary<ImportedMorphKeyframe, AdditionalMorphKeyframeOptions> MorphKeyframeOptions { get; set; }

		public WorkspaceMorph(ImportedMorph importedMorph) :
			base()
		{
			this.Name = importedMorph.Name;
			this.KeyframeList = importedMorph.KeyframeList;

			this.MorphKeyframeOptions = new Dictionary<ImportedMorphKeyframe, AdditionalMorphKeyframeOptions>(importedMorph.KeyframeList.Count);
			foreach (ImportedMorphKeyframe keyframe in importedMorph.KeyframeList)
			{
				AdditionalMorphKeyframeOptions options = new AdditionalMorphKeyframeOptions();
				this.MorphKeyframeOptions.Add(keyframe, options);
			}
		}

		public bool isMorphKeyframeEnabled(ImportedMorphKeyframe keyframe)
		{
			AdditionalMorphKeyframeOptions options;
			if (this.MorphKeyframeOptions.TryGetValue(keyframe, out options))
			{
				return options.Enabled;
			}
			throw new Exception("Morph keyframe not found");
		}

		public void setMorphKeyframeEnabled(ImportedMorphKeyframe keyframe, bool enabled)
		{
			AdditionalMorphKeyframeOptions options;
			if (this.MorphKeyframeOptions.TryGetValue(keyframe, out options))
			{
				options.Enabled = enabled;
				return;
			}
			throw new Exception("Morph keyframe not found");
		}

		public string getMorphKeyframeNewName(ImportedMorphKeyframe keyframe)
		{
			AdditionalMorphKeyframeOptions options;
			if (this.MorphKeyframeOptions.TryGetValue(keyframe, out options))
			{
				return options.NewName != null ? options.NewName : String.Empty;
			}
			throw new Exception("Morph keyframe not found");
		}

		public void setMorphKeyframeNewName(ImportedMorphKeyframe keyframe, string newName)
		{
			AdditionalMorphKeyframeOptions options;
			if (this.MorphKeyframeOptions.TryGetValue(keyframe, out options))
			{
				options.NewName = newName;
				return;
			}
			throw new Exception("Morph keyframe not found");
		}
	}

	public class WorkspaceAnimation : ImportedAnimation
	{
		protected class AdditionalTrackOptions
		{
			public bool Enabled = true;
		}
		protected Dictionary<ImportedAnimationTrack, AdditionalTrackOptions> TrackOptions { get; set; }

		public WorkspaceAnimation(ImportedAnimation importedAnimation) :
			base()
		{
			this.TrackList = importedAnimation.TrackList;

			this.TrackOptions = new Dictionary<ImportedAnimationTrack, AdditionalTrackOptions>(importedAnimation.TrackList.Count);
			foreach (ImportedAnimationTrack track in importedAnimation.TrackList)
			{
				AdditionalTrackOptions options = new AdditionalTrackOptions();
				this.TrackOptions.Add(track, options);
			}
		}

		public bool isTrackEnabled(ImportedAnimationTrack track)
		{
			AdditionalTrackOptions options;
			if (this.TrackOptions.TryGetValue(track, out options))
			{
				return options.Enabled;
			}
			throw new Exception("Track not found");
		}

		public void setTrackEnabled(ImportedAnimationTrack track, bool enabled)
		{
			AdditionalTrackOptions options;
			if (this.TrackOptions.TryGetValue(track, out options))
			{
				options.Enabled = enabled;
				return;
			}
			throw new Exception("Track not found");
		}
	}
}
