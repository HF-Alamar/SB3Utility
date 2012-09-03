using System;
using System.Collections.Generic;

namespace SB3Utility
{
	[Plugin]
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
}
