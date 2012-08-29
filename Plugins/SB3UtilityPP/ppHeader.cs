using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SB3Utility
{
	public enum ppHeaderIdx
	{
		SB3,
		SMFigure,
		SMRetail,
		AG3,
		AAJCH,
		Wakeari
	}

	public abstract class ppHeader
	{
		public static ppHeader[] Array = new ppHeader[] {
			new ppHeader_SB3(),
			new ppHeader_SMFigure(),
			new ppHeader_SMRetail(),
			new ppHeader_AG3(),
			new ppHeader_AAJCH(),
			new ppHeader_Wakeari()
		};

		public abstract int HeaderSize(int numFiles);
		public abstract List<IWriteFile> ReadHeader(string path, ppFormat format);
		public abstract void WriteHeader(Stream stream, List<IWriteFile> files, int[] sizes, object[] metadata);
		public abstract ppHeader TryHeader(string path);
		public abstract ppFormat[] ppFormats { get; }
	}

	public class ppHeader_SB3 : ppHeader
	{
		public override ppFormat[] ppFormats
		{
			get
			{
				return new ppFormat[] {
					ppFormat.Array[(int)ppFormatIdx.SB3]
				};
			}
		}

		public override int HeaderSize(int numFiles)
		{
			return (36 * numFiles) + 8;
		}

		public override List<IWriteFile> ReadHeader(string path, ppFormat format)
		{
			List<IWriteFile> subfiles = null;
			using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
			{
				int numFiles = binaryReader.ReadInt32();

				subfiles = new List<IWriteFile>(numFiles);
				binaryReader.ReadInt32();  // total size

				// get filenames
				for (int i = 0; i < numFiles; i++)
				{
					byte[] nameBuf = binaryReader.ReadBytes(0x20);
					for (int j = 0; j < nameBuf.Length; j++)
					{
						nameBuf[j] = (byte)(~nameBuf[j] + 1);
					}

					ppSubfile subfile = new ppSubfile(path);
					subfile.ppFormat = format;
					subfile.Name = Utility.EncodingShiftJIS.GetString(nameBuf).TrimEnd(new char[] { '\0' });
					subfiles.Add(subfile);
				}

				// get filesizes
				int offset = HeaderSize(numFiles);  // start of first file data
				for (int i = 0; i < numFiles; i++)
				{
					ppSubfile subfile = (ppSubfile)subfiles[i];
					subfile.offset = offset;
					subfile.size = binaryReader.ReadInt32();
					offset += subfile.size;
				}
			}

			return subfiles;
		}

		public override void WriteHeader(Stream stream, List<IWriteFile> files, int[] sizes, object[] metadata)
		{
			byte[] headerBuf = new byte[HeaderSize(files.Count)];
			BinaryWriter headerWriter = new BinaryWriter(new MemoryStream(headerBuf));

			headerWriter.Write(files.Count);
			headerWriter.BaseStream.Seek(4, SeekOrigin.Current);  // placeholder for total size

			// names
			for (int i = 0; i < files.Count; i++)
			{
				byte[] nameBuf = new byte[0x20];
				Utility.EncodingShiftJIS.GetBytes(files[i].Name).CopyTo(nameBuf, 0);
				for (int j = 0; j < nameBuf.Length; j++)
				{
					nameBuf[j] = (byte)(~nameBuf[j] + 1);
				}

				headerWriter.Write(nameBuf);
			}

			// file sizes
			int totalSize = 0;
			for (int i = 0; i < files.Count; i++)
			{
				headerWriter.Write(sizes[i]);
				totalSize += sizes[i];
			}

			// total size
			headerWriter.BaseStream.Seek(4, SeekOrigin.Begin);
			headerWriter.Write(totalSize);

			headerWriter.Flush();
			stream.Write(headerBuf, 0, headerBuf.Length);
		}

		public override ppHeader TryHeader(string path)
		{
			using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
			{
				int numFiles = binaryReader.ReadInt32();
				int headerSizeTemp = HeaderSize(numFiles);
				if ((numFiles > 0) && (headerSizeTemp > 0) && (headerSizeTemp <= binaryReader.BaseStream.Length))
				{
					int totalSizeRead = binaryReader.ReadInt32();
					binaryReader.BaseStream.Seek(numFiles * 0x20, SeekOrigin.Current);
					int totalSize = 0;
					for (int i = 0; i < numFiles; i++)
					{
						int filesize = binaryReader.ReadInt32();
						if (filesize < 0)
						{
							break;
						}
						totalSize += filesize;
						if (totalSize >= binaryReader.BaseStream.Length)
						{
							break;
						}
					}

					if ((totalSizeRead == totalSize) && ((totalSize + headerSizeTemp) == binaryReader.BaseStream.Length))
					{
						return this;
					}
				}

				return null;
			}
		}
	}

	public class ppHeader_AAJCH : ppHeader_SMRetail
	{
		public override ppFormat[] ppFormats
		{
			get
			{
				return new ppFormat[] {
					ppFormat.Array[(int)ppFormatIdx.AAJCH]
				};
			}
		}

		public override byte FirstByte
		{
			get { return 0x04; }
		}
	}

	public class ppHeader_AG3 : ppHeader_SMRetail
	{
		public override ppFormat[] ppFormats
		{
			get
			{
				return new ppFormat[] {
					ppFormat.Array[(int)ppFormatIdx.YuushaRetail],
					ppFormat.Array[(int)ppFormatIdx.AHMRetail],
					ppFormat.Array[(int)ppFormatIdx.HakoRetail],
					ppFormat.Array[(int)ppFormatIdx.AG3Retail],
					ppFormat.Array[(int)ppFormatIdx.DG],
					ppFormat.Array[(int)ppFormatIdx.SMSweets],
					ppFormat.Array[(int)ppFormatIdx.SM2Retail],
					ppFormat.Array[(int)ppFormatIdx.BestCollection]
				};
			}
		}

		public override byte FirstByte
		{
			get { return 0x03; }
		}
	}

	public abstract class ppHeader_SMBase : ppHeader
	{
		public abstract byte[] DecryptHeaderBytes(byte[] buf, byte[] SMFigTable);

		public virtual byte FirstByte
		{
			get { return 0x01; }
		}

		public override int HeaderSize(int numFiles)
		{
			return (268 * numFiles) + 9;
		}

		public List<IWriteFile> ReadHeader(string path, ppFormat format, byte[] SMFigTable)
		{
			List<IWriteFile> subfiles = null;
			using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
			{
				DecryptHeaderBytes(binaryReader.ReadBytes(1), SMFigTable);  // first byte
				int numFiles = BitConverter.ToInt32(DecryptHeaderBytes(binaryReader.ReadBytes(4), SMFigTable), 0);
				byte[] buf = DecryptHeaderBytes(binaryReader.ReadBytes(numFiles * 268), SMFigTable);

				subfiles = new List<IWriteFile>(numFiles);
				for (int i = 0; i < numFiles; i++)
				{
					int offset = i * 268;
					ppSubfile subfile = new ppSubfile(path);
					subfile.ppFormat = format;
					subfile.Name = Utility.EncodingShiftJIS.GetString(buf, offset, 260).TrimEnd(new char[] { '\0' });
					subfile.size = BitConverter.ToInt32(buf, offset + 260);
					subfile.offset = BitConverter.ToInt32(buf, offset + 264);
					subfiles.Add(subfile);
				}
			}

			return subfiles;
		}

		public void WriteHeader(Stream stream, List<IWriteFile> files, int[] sizes, byte[] SMFigTable)
		{
			byte[] headerBuf = new byte[HeaderSize(files.Count)];
			BinaryWriter headerWriter = new BinaryWriter(new MemoryStream(headerBuf));

			headerWriter.Write(DecryptHeaderBytes(new byte[] { FirstByte }, SMFigTable));
			headerWriter.Write(DecryptHeaderBytes(BitConverter.GetBytes(files.Count), SMFigTable));

			byte[] fileHeaderBuf = new byte[268 * files.Count];
			int fileOffset = headerBuf.Length;
			for (int i = 0; i < files.Count; i++)
			{
				int idx = i * 268;
				Utility.EncodingShiftJIS.GetBytes(files[i].Name).CopyTo(fileHeaderBuf, idx);
				BitConverter.GetBytes(sizes[i]).CopyTo(fileHeaderBuf, idx + 260);
				BitConverter.GetBytes(fileOffset).CopyTo(fileHeaderBuf, idx + 264);
				fileOffset += sizes[i];
			}

			headerWriter.Write(DecryptHeaderBytes(fileHeaderBuf, SMFigTable));
			headerWriter.Write(DecryptHeaderBytes(BitConverter.GetBytes(headerBuf.Length), SMFigTable));
			headerWriter.Flush();
			stream.Write(headerBuf, 0, headerBuf.Length);
		}

		public ppHeader TryHeader(string path, byte[] SMFigTable)
		{
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
			{
				byte[] readFirstByte = reader.ReadBytes(1);
				byte firstByteDecrypted = DecryptHeaderBytes(readFirstByte, SMFigTable)[0];

				if (firstByteDecrypted == FirstByte)
				{
					int numFiles = BitConverter.ToInt32(DecryptHeaderBytes(reader.ReadBytes(4), SMFigTable), 0);

					int headerSizeTemp = HeaderSize(numFiles);
					if ((numFiles > 0) && (headerSizeTemp > 0) && (headerSizeTemp <= reader.BaseStream.Length))
					{
						DecryptHeaderBytes(reader.ReadBytes(numFiles * 268), SMFigTable);
						int headerSize = BitConverter.ToInt32(DecryptHeaderBytes(reader.ReadBytes(4), SMFigTable), 0);

						if (headerSize == headerSizeTemp)
						{
							return this;
						}
					}
				}

				return null;
			}
		}
	}

	public class ppHeader_SMFigure : ppHeader_SMBase
	{
		public override ppFormat[] ppFormats
		{
			get
			{
				return new ppFormat[] {
					ppFormat.Array[(int)ppFormatIdx.SMFigure]
				};
			}
		}

		public override List<IWriteFile> ReadHeader(string path, ppFormat format)
		{
			return ReadHeader(path, format, InitTableFigure());
		}

		public override void WriteHeader(Stream stream, List<IWriteFile> files, int[] sizes, object[] metadata)
		{
			base.WriteHeader(stream, files, sizes, InitTableFigure());
		}

		public override ppHeader TryHeader(string path)
		{
			return TryHeader(path, InitTableFigure());
		}

		private static byte[] InitTableFigure()
		{
			return new byte[]
			{
				0x04, 0x0A, 0x06,
				0x02, 0x0D, 0x09, 0x00, 0x0E, 0x06, 0x01, 0x0C, 0x08, 0x05, 0x0B, 0x0F, 0x07, 0x0A, 0x04, 0x03,
				0x04, 0x0B, 0x03, 0x01, 0x0F, 0x00, 0x0D, 0x0A, 0x0E, 0x08, 0x05, 0x09, 0x0C, 0x02, 0x06, 0x07,
				0x03, 0x06, 0x00, 0x0F, 0x0E, 0x09, 0x05, 0x0C, 0x08, 0x02, 0x0D, 0x0A, 0x07, 0x01, 0x04, 0x0B,
				0x08, 0x04, 0x01, 0x06, 0x0D, 0x09, 0x00, 0x0E, 0x0A, 0x05, 0x02, 0x0F, 0x07, 0x0B, 0x03, 0x0C,
				0x05, 0x03, 0x0D, 0x02, 0x00, 0x0A, 0x0E, 0x0F, 0x09, 0x0B, 0x07, 0x01, 0x0C, 0x06, 0x08, 0x04,
				0x0B, 0x08, 0x0D, 0x07, 0x0A, 0x0F, 0x0C, 0x03, 0x01, 0x0E, 0x04, 0x00, 0x06, 0x02, 0x09, 0x05,
				0x06, 0x02, 0x0A, 0x0E, 0x01, 0x09, 0x03, 0x0C, 0x00, 0x05, 0x08, 0x0D, 0x0F, 0x04, 0x07, 0x0B
			};
		}

		public override byte[] DecryptHeaderBytes(byte[] buf, byte[] table)
		{
			byte byte_7 = table[0];
			byte byte_8 = table[1];
			byte byte_9 = table[2];
			for (int i = 0; i < buf.Length; i++)
			{
				byte var_2F = table[(buf[i] & 0x0f) + 3];
				byte var_26 = table[var_2F + 0x13];
				byte var_5 = table[var_26 + 0x33];
				byte var_2D = table[var_5 + 0x53];
				byte var_25 = table[var_2D + 0x63];
				byte var_2E = table[var_25 + 0x43];
				buf[i] = (byte)((table[var_2E + 0x23] & 0x0f) | (buf[i] & 0xf0));
				ShuffleTableFigure(table, ref byte_7, ref byte_8, ref byte_9);
			}

			return buf;
		}

		private static void ShuffleTableFigure(byte[] table, ref byte byte_7, ref byte byte_8, ref byte byte_9)
		{
			byte var_A;
			byte var_B = 0x0f;

			var_A = table[3];
			for (int i = 0; i < var_B; i++)
			{
				table[3 + i] = table[3 + i + 1];
			}
			table[3 + var_B] = var_A;
			byte_7 = (byte)((byte_7 + 1) & 0x0f);

			for (int i = 0; i < 0x10; i++)
			{
				table[0x13 + i + 0x10] = (byte)((table[0x13 + i + 0x10] + var_B) & 0x0f);
			}
			if (byte_7 != 0)
			{
				return;
			}

			var_A = table[0x13];
			for (int i = 0; i < var_B; i++)
			{
				table[0x13 + i] = table[0x13 + i + 1];
			}
			table[0x13 + var_B] = var_A;
			byte_8 = (byte)((byte_8 + 1) & 0x0f);

			for (int i = 0; i < 0x10; i++)
			{
				table[0x33 + i + 0x10] = (byte)((table[0x33 + i + 0x10] + var_B) & 0x0f);
			}
			if (byte_8 != 0)
			{
				return;
			}

			var_A = table[0x33];
			for (int i = 0; i < var_B; i++)
			{
				table[0x33 + i] = table[0x33 + i + 1];
			}
			table[0x33 + var_B] = var_A;
			byte_9 = (byte)((byte_9 + 1) & 0x0f);

			for (int i = 0; i < 0x10; i++)
			{
				table[0x53 + i + 0x10] = (byte)((table[0x53 + i + 0x10] + var_B) & 0x0f);
			}
		}
	}

	public class ppHeader_SMRetail : ppHeader_SMBase
	{
		public override ppFormat[] ppFormats
		{
			get
			{
				return new ppFormat[] {
					ppFormat.Array[(int)ppFormatIdx.SMRetail],
					ppFormat.Array[(int)ppFormatIdx.RGF],
					ppFormat.Array[(int)ppFormatIdx.HakoTrial],
					ppFormat.Array[(int)ppFormatIdx.SMTrial],
					ppFormat.Array[(int)ppFormatIdx.AG3Welcome],
					ppFormat.Array[(int)ppFormatIdx.YuushaTrial],
					ppFormat.Array[(int)ppFormatIdx.EskMate],
					ppFormat.Array[(int)ppFormatIdx.AHMFigure],
					ppFormat.Array[(int)ppFormatIdx.AHMTrial],
					ppFormat.Array[(int)ppFormatIdx.SM2Trial],
					ppFormat.Array[(int)ppFormatIdx.SBZ],
					ppFormat.Array[(int)ppFormatIdx.CharacolleAlicesoft],
					ppFormat.Array[(int)ppFormatIdx.CharacolleBaseson],
					ppFormat.Array[(int)ppFormatIdx.CharacolleKey],
					ppFormat.Array[(int)ppFormatIdx.AATrial],
					ppFormat.Array[(int)ppFormatIdx.AARetail]
				};
			}
		}

		public override List<IWriteFile> ReadHeader(string path, ppFormat format)
		{
			return ReadHeader(path, format, null);
		}

		public override void WriteHeader(Stream stream, List<IWriteFile> files, int[] sizes, object[] metadata)
		{
			base.WriteHeader(stream, files, sizes, null);
		}

		public override ppHeader TryHeader(string path)
		{
			return TryHeader(path, null);
		}

		public static byte[] DecryptHeaderBytes(byte[] buf)
		{
			byte[] table = new byte[]
			{
				0xFA, 0x49, 0x7B, 0x1C, // var48
				0xF9, 0x4D, 0x83, 0x0A,
				0x3A, 0xE3, 0x87, 0xC2, // var24
				0xBD, 0x1E, 0xA6, 0xFE
			};

			byte var28;
			for (int var4 = 0; var4 < buf.Length; var4++)
			{
				var28 = (byte)(var4 & 0x7);
				table[var28] += table[8 + var28];
				buf[var4] ^= table[var28];
			}

			return buf;
		}

		public override byte[] DecryptHeaderBytes(byte[] buf, byte[] SMFigTable)
		{
			return DecryptHeaderBytes(buf);
		}
	}

	public class ppHeader_Wakeari : ppHeader
	{
		public override ppFormat[] ppFormats
		{
			get
			{
				return new ppFormat[] {
					ppFormat.Array[(int)ppFormatIdx.Wakeari],
					ppFormat.Array[(int)ppFormatIdx.LoveGirl],
					ppFormat.Array[(int)ppFormatIdx.Hero]
				};
			}
		}

		const byte FirstByte = 0x01;
		const int Version = 0x6C;
		byte[] ppVersionBytes = Encoding.ASCII.GetBytes("[PPVER]\0");

		public override int HeaderSize(int numFiles)
		{
			return (288 * numFiles) + 9 + 12;
		}

		public override List<IWriteFile> ReadHeader(string path, ppFormat format)
		{
			List<IWriteFile> subfiles = null;
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
			{
				byte[] versionHeader = reader.ReadBytes(8);
				int version = BitConverter.ToInt32(ppHeader_SMRetail.DecryptHeaderBytes(reader.ReadBytes(4)), 0);

				ppHeader_SMRetail.DecryptHeaderBytes(reader.ReadBytes(1));  // first byte
				int numFiles = BitConverter.ToInt32(ppHeader_SMRetail.DecryptHeaderBytes(reader.ReadBytes(4)), 0);
				byte[] buf = ppHeader_SMRetail.DecryptHeaderBytes(reader.ReadBytes(numFiles * 288));

				subfiles = new List<IWriteFile>(numFiles);
				for (int i = 0; i < numFiles; i++)
				{
					int offset = i * 288;
					ppSubfile subfile = new ppSubfile(path);
					subfile.ppFormat = format;
					subfile.Name = Utility.EncodingShiftJIS.GetString(buf, offset, 260).TrimEnd(new char[] { '\0' });
					subfile.size = BitConverter.ToInt32(buf, offset + 260);
					subfile.offset = BitConverter.ToInt32(buf, offset + 264);

					Metadata metadata = new Metadata();
					metadata.LastBytes = new byte[20];
					System.Array.Copy(buf, offset + 268, metadata.LastBytes, 0, 20);
					subfile.Metadata = metadata;

					subfiles.Add(subfile);
				}
			}
			return subfiles;
		}

		public override void WriteHeader(Stream stream, List<IWriteFile> files, int[] sizes, object[] metadata)
		{
			byte[] headerBuf = new byte[HeaderSize(files.Count)];
			BinaryWriter writer = new BinaryWriter(new MemoryStream(headerBuf));

			writer.Write(ppVersionBytes);
			writer.Write(ppHeader_SMRetail.DecryptHeaderBytes(BitConverter.GetBytes(Version)));
			
			writer.Write(ppHeader_SMRetail.DecryptHeaderBytes(new byte[] { FirstByte }));
			writer.Write(ppHeader_SMRetail.DecryptHeaderBytes(BitConverter.GetBytes(files.Count)));

			byte[] fileHeaderBuf = new byte[288 * files.Count];
			int fileOffset = headerBuf.Length;
			for (int i = 0; i < files.Count; i++)
			{
				int idx = i * 288;
				Utility.EncodingShiftJIS.GetBytes(files[i].Name).CopyTo(fileHeaderBuf, idx);
				BitConverter.GetBytes(sizes[i]).CopyTo(fileHeaderBuf, idx + 260);
				BitConverter.GetBytes(fileOffset).CopyTo(fileHeaderBuf, idx + 264);

				Metadata wakeariMetadata = (Metadata)metadata[i];
				System.Array.Copy(wakeariMetadata.LastBytes, 0, fileHeaderBuf, idx + 268, 20);
				BitConverter.GetBytes(sizes[i]).CopyTo(fileHeaderBuf, idx + 284);

				fileOffset += sizes[i];
			}

			writer.Write(ppHeader_SMRetail.DecryptHeaderBytes(fileHeaderBuf));
			writer.Write(ppHeader_SMRetail.DecryptHeaderBytes(BitConverter.GetBytes(headerBuf.Length)));
			writer.Flush();
			stream.Write(headerBuf, 0, headerBuf.Length);
		}

		public override ppHeader TryHeader(string path)
		{
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
			{
				byte[] version = reader.ReadBytes(8);
				for (int i = 0; i < version.Length; i++)
				{
					if (ppVersionBytes[i] != version[i])
					{
						return null;
					}
				}
				return this;
			}
		}

		public struct Metadata
		{
			public byte[] LastBytes { get; set; }
		}
	}
}
