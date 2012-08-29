using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		public static ImportedTexture ImportEmaTexture(Stream stream, string name)
		{
			return new Ema(stream, name).ImportedTexture();
		}

		[Plugin]
		public static ImportedTexture ImportTexture(Stream stream, string name)
		{
			return new ImportedTexture(stream, name);
		}

		[Plugin]
		public static ImportedTexture ImportTexture([DefaultVar]string path)
		{
			return new ImportedTexture(path);
		}
	}
}
