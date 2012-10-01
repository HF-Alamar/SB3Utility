using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace SB3Utility
{
	public class JchStream : Stream
	{
		private Stream stream;
		private CompressionMode mode;
		private bool leaveOpen;

		private List<byte> readBuf;
		private bool hasReadHeader;
		private byte compressByte;
		private int copyPos;
		private int fileSize;
		private int totalRead;

		private MemoryStream writeBuf;
		private int[] byteCount;

		public JchStream(Stream stream, CompressionMode mode)
			: this(stream, mode, false)
		{
		}

		public JchStream(Stream stream, CompressionMode mode, bool leaveOpen)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			if (mode == CompressionMode.Decompress)
			{
				if (!stream.CanRead)
				{
					throw new ArgumentException("The base stream is not writeable.", "stream");
				}
			}
			else if (mode == CompressionMode.Compress)
			{
				if (!stream.CanWrite)
				{
					throw new ArgumentException("The base stream is not readable.", "stream");
				}
			}
			else
			{
				throw new ArgumentException("Enum value was out of legal range.", "mode");
			}

			this.stream = stream;
			this.mode = mode;
			this.leaveOpen = leaveOpen;
			if (mode == CompressionMode.Decompress)
			{
				readBuf = new List<byte>(0x20000);
				hasReadHeader = false;
				totalRead = 0;
			}
			else
			{
				writeBuf = new MemoryStream();
				byteCount = new int[256];
			}
		}

		public Stream BaseStream
		{
			get
			{
				return stream;
			}
		}

		public override bool CanRead
		{
			get
			{
				return (stream != null) && (mode == CompressionMode.Decompress);
			}
		}

		public override bool CanWrite
		{
			get
			{
				return (stream != null) && (mode == CompressionMode.Compress);
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("This operation is not supported.");
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException("This operation is not supported.");
			}
			set
			{
				throw new NotSupportedException("This operation is not supported.");
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (stream == null)
			{
				throw new ObjectDisposedException("stream");
			}
			if (mode != CompressionMode.Decompress)
			{
				throw new NotSupportedException("This operation is not supported.");
			}
			return base.BeginRead(buffer, offset, count, callback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (stream == null)
			{
				throw new ObjectDisposedException("stream");
			}
			if (mode != CompressionMode.Compress)
			{
				throw new NotSupportedException("This operation is not supported.");
			}
			return base.BeginWrite(buffer, offset, count, callback, state);
		}

		public override void Close()
		{
			if (stream != null)
			{
				if (mode == CompressionMode.Decompress)
				{
					readBuf = null;
				}
				else
				{
					int maxByteCount = 0;
					for (int i = 0; i < byteCount.Length; i++)
					{
						if (byteCount[i] > maxByteCount)
						{
							compressByte = (byte)i;
							maxByteCount = byteCount[i];
						}
					}

					BinaryWriter writer = new BinaryWriter(stream);
					writer.Write(compressByte);
					writer.Write((int)writeBuf.Length);

					byte[] inputBuf = writeBuf.GetBuffer();
					int bufLength = (int)writeBuf.Length;
					int windowStart = 0;
					int windowEnd = 0;
					for (int i = 0; i < bufLength; )
					{
						Match maxMatch = new Match();
						int maxMatchLength = 0;
						for (int windowIdx = windowStart; windowIdx < windowEnd; windowIdx++)
						{
							int blockSize = 0;
							int numBlocks = 1;
							if (inputBuf[i] == inputBuf[windowIdx])
							{
								for (int k = 1; k < 0xFF; k++)
								{
									blockSize++;
									int offset = i + k;
									if ((offset >= bufLength) || ((windowIdx + k) >= windowEnd) || (inputBuf[offset] != inputBuf[windowIdx + k]))
									{
										break;
									}
								}

								for (int k = 1; k < 0xFF; k++)
								{
									bool addBlock = true;
									for (int m = 0; m < blockSize; m++)
									{
										int offset = i + (blockSize * k) + m;
										if ((offset >= bufLength) || (inputBuf[offset] != inputBuf[windowIdx + m]))
										{
											addBlock = false;
											break;
										}
									}
									if (addBlock)
									{
										numBlocks++;
									}
									else
									{
										break;
									}
								}

								int matchLength = blockSize * numBlocks;
								if (matchLength > maxMatchLength)
								{
									maxMatch.blockSize = blockSize;
									maxMatch.numBlocks = numBlocks;
									maxMatch.distance = i - windowIdx;
									maxMatch.length = matchLength;
									maxMatchLength = matchLength;
								}
							}
						}

						int windowOffset = 1;
						if (maxMatchLength > 4)
						{
							writer.Write(compressByte);
							if (maxMatch.distance == compressByte)
							{
								writer.Write((byte)0);
							}
							else
							{
								writer.Write((byte)maxMatch.distance);
							}
							writer.Write((byte)maxMatch.blockSize);
							writer.Write((byte)maxMatch.numBlocks);
							i += maxMatch.length;
							windowOffset = maxMatch.length;
						}
						else
						{
							WriteSingleByte(writer, inputBuf[i], compressByte);
							i++;
						}

						windowEnd += windowOffset;
						int windowSize = windowEnd - windowStart;
						if (windowSize >= 0xFF)
						{
							windowStart += windowSize - 0xFF;
						}
					}

					writeBuf.Close();
					writeBuf = null;
					byteCount = null;
				}

				if (!leaveOpen)
				{
					stream.Close();
				}

				stream = null;
			}
		}

		private class Match
		{
			public int distance;
			public int length;
			public int blockSize;
			public int numBlocks;
		}

		private static void WriteSingleByte(BinaryWriter writer, byte b, byte compressedByte)
		{
			writer.Write(b);

			if (b == compressedByte)
			{
				writer.Write(b);
			}
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return base.EndRead(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			base.EndWrite(asyncResult);
		}

		public override void Flush()
		{
			if (stream == null)
			{
				throw new ObjectDisposedException("stream");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (stream == null)
			{
				throw new ObjectDisposedException("stream");
			}
			if (mode != CompressionMode.Decompress)
			{
				throw new NotSupportedException("This operation is not supported.");
			}

			BinaryReader reader = new BinaryReader(stream);
			if (!hasReadHeader)
			{
				compressByte = reader.ReadByte();
				fileSize = reader.ReadInt32();

				hasReadHeader = true;
			}

			if (totalRead >= fileSize)
			{
				return 0;
			}

			if ((totalRead + count) > fileSize)
			{
				count = fileSize - totalRead;
			}

			while (readBuf.Count - copyPos < count)
			{
				byte b = reader.ReadByte();
				if (b == compressByte)
				{
					byte distance = reader.ReadByte();
					if (distance == compressByte)
					{
						readBuf.Add(distance);
					}
					else
					{
						if (distance == 0)
						{
							distance = compressByte;
						}
						byte blockSize = reader.ReadByte();
						byte numBlocks = reader.ReadByte();
						byte[] block = new byte[blockSize];
						readBuf.CopyTo(readBuf.Count - distance, block, 0, blockSize);

						for (int i = 0; i < numBlocks; i++)
						{
							readBuf.AddRange(block);
						}
					}
				}
				else
				{
					readBuf.Add(b);
				}
			}

			readBuf.CopyTo(copyPos, buffer, offset, count);
			copyPos += count;
			if (copyPos > 255)
			{
				int remove = copyPos - 255;
				readBuf.RemoveRange(0, remove);
				copyPos -= remove;
			}

			totalRead += count;
			return count;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (stream == null)
			{
				throw new ObjectDisposedException("stream");
			}
			if (mode != CompressionMode.Compress)
			{
				throw new NotSupportedException("This operation is not supported.");
			}

			for (int i = 0; i < count; i++)
			{
				byteCount[buffer[offset + i]]++;
			}

			writeBuf.Write(buffer, offset, count);
		}
	}
}
