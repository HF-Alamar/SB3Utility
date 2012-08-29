using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		public static xxParser OpenXX([DefaultVar]ppParser parser, string name)
		{
			for (int i = 0; i < parser.Subfiles.Count; i++)
			{
				if (parser.Subfiles[i].Name == name)
				{
					IReadFile subfile = parser.Subfiles[i] as IReadFile;
					if (subfile != null)
					{
						return new xxParser(subfile.CreateReadStream(), subfile.Name);
					}

					break;
				}
			}
			return null;
		}

		[Plugin]
		public static xxParser OpenXX([DefaultVar]string path)
		{
			return new xxParser(File.OpenRead(path), Path.GetFileName(path));
		}

		[Plugin]
		public static void ConvertXX([DefaultVar]xxParser parser, int format)
		{
			xx.ConvertFormat(parser, format);
		}
	}
}
