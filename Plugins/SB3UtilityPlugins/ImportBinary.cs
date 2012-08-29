using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		public static byte[] ImportBinary([DefaultVar]string path)
		{
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
			{
				return reader.ReadBytes((int)reader.BaseStream.Length);
			}
		}

		[Plugin]
		public static byte[] ImportBinary([DefaultVar]string path, int offset, int count)
		{
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
			{
				reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				return reader.ReadBytes(count);
			}
		}
	}
}
