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
		}

		void InitFrames(ImportedFrame frame)
		{
			Frames.Add(frame);

			foreach (var child in frame)
			{
				InitFrames(child);
			}
		}
	}
}
