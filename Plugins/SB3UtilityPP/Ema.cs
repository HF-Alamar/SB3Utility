using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace SB3Utility
{
	public class Ema : IWriteFile
	{
		public string Name { get; set; }
		public byte[] Data { get; set; }

		public Ema()
		{
		}

		public Ema(Stream stream, string name)
			: this(stream)
		{
			this.Name = name;
		}

		public Ema(Stream stream)
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				Data = reader.ReadToEnd();
			}
		}

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Data);
		}

		public ImportedTexture ImportedTexture()
		{
			string ext = Encoding.ASCII.GetString(Data, 8, 4).ToLowerInvariant();
			ImportedTexture importedTex = new ImportedTexture();
			importedTex.Name = Path.GetFileNameWithoutExtension(Name) + ext;
			importedTex.Data = CreateImageData();
			return importedTex;
		}

		public void Export(string path)
		{
			using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
			{
				writer.Write(CreateImageData());
			}
		}

		byte[] CreateImageData()
		{
			string ext = Encoding.ASCII.GetString(Data, 8, 4).ToLowerInvariant();
			byte[] buf = new byte[Data.Length - 13];
			Array.Copy(Data, 13, buf, 0, buf.Length);
			if (ext.ToLowerInvariant() == ".bmp")
			{
				buf[0] = (byte)'B';
				buf[1] = (byte)'M';
			}
			return buf;
		}

		public static Ema Import(string path)
		{
			return Import(File.OpenRead(path), Path.GetFileNameWithoutExtension(path) + ".ema");
		}

		public static Ema Import(Stream stream, string name)
		{
			Ema ema = Import(stream);
			ema.Name = name;
			return ema;
		}

		public static Ema Import(Stream stream)
		{
			Ema ema = new Ema();
			using (BinaryReader reader = new BinaryReader(stream))
			{
				byte[] imgData = reader.ReadToEnd();
				var imgInfo = ImageInformation.FromMemory(imgData);

				ema.Data = new byte[imgData.Length + 13];
				BinaryWriter dataWriter = new BinaryWriter(new MemoryStream(ema.Data));
				dataWriter.Write(imgInfo.Width);
				dataWriter.Write(imgInfo.Height);

				string ext = Enum.GetName(typeof(ImageFileFormat), imgInfo.ImageFileFormat).ToLowerInvariant();
				dataWriter.Write((byte)'.');
				dataWriter.Write(Encoding.ASCII.GetBytes(ext));
				dataWriter.Write((byte)0);

				if (imgInfo.ImageFileFormat == ImageFileFormat.Bmp)
				{
					dataWriter.Write((short)0);
					dataWriter.Write(imgData, 2, imgData.Length - 2);
				}
				else
				{
					dataWriter.Write(imgData);
				}
			}
			return ema;
		}
	}
}
