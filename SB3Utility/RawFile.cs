using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SB3Utility
{
	public class RawFile : IReadFile, IWriteFile
	{
		public string Name { get; set; }
		public string FilePath { get; protected set; }

		public RawFile(string path)
		{
			FilePath = path;
			Name = Path.GetFileName(FilePath);
		}

		public Stream CreateReadStream()
		{
			return File.OpenRead(FilePath);
		}

		public void WriteTo(Stream stream)
		{
			using (BinaryReader reader = new BinaryReader(CreateReadStream()))
			{
				BinaryWriter writer = new BinaryWriter(stream);
				byte[] buf;
				while ((buf = reader.ReadBytes(Utility.BufSize)).Length == Utility.BufSize)
				{
					writer.Write(buf);
				}
				writer.Write(buf);
			}
		}
	}
}
