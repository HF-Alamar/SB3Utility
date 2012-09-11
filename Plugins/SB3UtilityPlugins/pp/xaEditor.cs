using System;
using System.Collections.Generic;
using System.Text;

namespace SB3Utility
{
	[Plugin]
	public class xaEditor
	{
		public xaParser Parser { get; protected set; }

		public xaEditor(xaParser parser)
		{
			Parser = parser;
		}

		[Plugin]
		public void ReplaceMorph(WorkspaceMorph morph, string destMorphName, string newName, bool replaceNormals, double minSquaredDistance)
		{
			xa.ReplaceMorph(destMorphName, Parser, morph, newName, replaceNormals, (float)minSquaredDistance);
		}

		[Plugin]
		public void ReplaceAnimation(WorkspaceAnimation animation, int resampleCount, string method, int insertPos)
		{
			var replaceMethod = (ReplaceAnimationMethod)Enum.Parse(typeof(ReplaceAnimationMethod), method);
			xa.ReplaceAnimation(animation, Parser, resampleCount, replaceMethod, insertPos);
		}
	}
}
