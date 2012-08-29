using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		public static xaParser OpenXA([DefaultVar]ppParser parser, string name)
		{
			for (int i = 0; i < parser.Subfiles.Count; i++)
			{
				if (parser.Subfiles[i].Name == name)
				{
					IReadFile subfile = parser.Subfiles[i] as IReadFile;
					if (subfile != null)
					{
						return new xaParser(subfile.CreateReadStream(), subfile.Name);
					}

					break;
				}
			}
			return null;
		}

		[Plugin]
		public static xaParser OpenXA([DefaultVar]string path)
		{
			return new xaParser(File.OpenRead(path), Path.GetFileName(path));
		}
	}
}
