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
		public void setSubmeshReplacingOriginals(int meshId, int id, bool replaceOriginals)
		{
			ImportedSubmesh submesh = this.Meshes[meshId].SubmeshList[id];
			this.Meshes[meshId].setSubmeshEnabled(submesh, replaceOriginals);
		}
	}
}
