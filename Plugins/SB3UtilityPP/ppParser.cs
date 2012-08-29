using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.ComponentModel;

namespace SB3Utility
{
	public class ppParser
	{
		public string FilePath { get; protected set; }
		public ppFormat Format { get; set; }
		public List<IWriteFile> Subfiles { get; protected set; }

		private string destPath;
		private bool keepBackup;

		public ppParser(string path, ppFormat format)
		{
			this.Format = format;
			this.FilePath = path;
			this.Subfiles = format.ppHeader.ReadHeader(path, format);
		}

		public BackgroundWorker WriteArchive(string destPath, bool keepBackup, bool background)
		{
			this.destPath = destPath;
			this.keepBackup = keepBackup;

			BackgroundWorker worker = new BackgroundWorker();
			worker.WorkerSupportsCancellation = true;
			worker.WorkerReportsProgress = true;

			worker.DoWork += new DoWorkEventHandler(writeArchiveWorker_DoWork);

			if (!background)
			{
				writeArchiveWorker_DoWork(worker, new DoWorkEventArgs(null));
			}

			return worker;
		}

		void writeArchiveWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = (BackgroundWorker)sender;
			string backup = null;

			try
			{
				DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(destPath));
				if (!dir.Exists)
				{
					dir.Create();
				}

				if (File.Exists(destPath))
				{
					backup = Utility.GetDestFile(dir, Path.GetFileNameWithoutExtension(destPath) + ".bak", Path.GetExtension(destPath));
					File.Move(destPath, backup);

					if (destPath.Equals(this.FilePath, StringComparison.InvariantCultureIgnoreCase))
					{
						for (int i = 0; i < Subfiles.Count; i++)
						{
							ppSubfile subfile = Subfiles[i] as ppSubfile;
							if ((subfile != null) && subfile.ppPath.Equals(this.FilePath, StringComparison.InvariantCultureIgnoreCase))
							{
								subfile.ppPath = backup;
							}
						}
					}
				}

				using (BinaryWriter writer = new BinaryWriter(File.Create(destPath)))
				{
					writer.BaseStream.Seek(Format.ppHeader.HeaderSize(Subfiles.Count), SeekOrigin.Begin);
					int offset = (int)writer.BaseStream.Position;
					int[] sizes = new int[Subfiles.Count];
					object[] metadata = new object[Subfiles.Count];

					for (int i = 0; i < Subfiles.Count; i++)
					{
						if (worker.CancellationPending)
						{
							e.Cancel = true;
							break;
						}

						worker.ReportProgress(i * 100 / Subfiles.Count);

						ppSubfile subfile = Subfiles[i] as ppSubfile;
						if ((subfile != null) && (subfile.ppFormat == this.Format))
						{
							using (BinaryReader reader = new BinaryReader(File.OpenRead(subfile.ppPath)))
							{
								reader.BaseStream.Seek(subfile.offset, SeekOrigin.Begin);

								int readSteps = subfile.size / Utility.BufSize;
								for (int j = 0; j < readSteps; j++)
								{
									writer.Write(reader.ReadBytes(Utility.BufSize));
								}
								writer.Write(reader.ReadBytes(subfile.size % Utility.BufSize));
							}
							metadata[i] = subfile.Metadata;
						}
						else
						{
							Stream stream = Format.WriteStream(writer.BaseStream);
							Subfiles[i].WriteTo(stream);
							metadata[i] = Format.FinishWriteTo(stream);
						}

						int pos = (int)writer.BaseStream.Position;
						sizes[i] = pos - offset;
						offset = pos;
					}

					if (!e.Cancel)
					{
						writer.BaseStream.Seek(0, SeekOrigin.Begin);
						Format.ppHeader.WriteHeader(writer.BaseStream, Subfiles, sizes, metadata);
					}
				}

				if (e.Cancel)
				{
					RestoreBackup(destPath, backup);
				}
				else
				{
					this.FilePath = destPath;

					if ((backup != null) && !keepBackup)
					{
						File.Delete(backup);
					}
				}
			}
			catch (Exception ex)
			{
				RestoreBackup(destPath, backup);
				Utility.ReportException(ex);
			}
		}

		void RestoreBackup(string destPath, string backup)
		{
			if (File.Exists(destPath))
			{
				File.Delete(destPath);

				if (backup != null)
				{
					File.Move(backup, destPath);

					if (destPath.Equals(this.FilePath, StringComparison.InvariantCultureIgnoreCase))
					{
						for (int i = 0; i < Subfiles.Count; i++)
						{
							ppSubfile subfile = Subfiles[i] as ppSubfile;
							if ((subfile != null) && subfile.ppPath.Equals(backup, StringComparison.InvariantCultureIgnoreCase))
							{
								subfile.ppPath = this.FilePath;
							}
						}
					}
				}
			}
		}
	}
}
