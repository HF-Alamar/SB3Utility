using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace SB3Utility
{
	[Plugin]
	public class ppEditor
	{
		public ppParser Parser { get; protected set; }

		public ppEditor(ppParser parser)
		{
			Parser = parser;
		}

		[Plugin]
		public void SetFormat(int id)
		{
			Parser.Format = ppFormat.Array[id];
		}

		[Plugin]
		public BackgroundWorker SavePP(bool keepBackup, bool background)
		{
			return SavePP(Parser.FilePath, keepBackup, background);
		}

		[Plugin]
		public BackgroundWorker SavePP(string path, bool keepBackup, bool background)
		{
			return Parser.WriteArchive(path, keepBackup, background);
		}

		[Plugin]
		public void ReplaceSubfile(IWriteFile file)
		{
			int index = FindSubfile(file.Name);
			if (index < 0)
			{
				throw new Exception("Couldn't find the subfile " + file.Name);
			}

			Parser.Subfiles.RemoveAt(index);
			Parser.Subfiles.Insert(index, file);
		}

		[Plugin]
		public void AddSubfile(string path)
		{
			Parser.Subfiles.Add(new RawFile(path));
		}

		[Plugin]
		public void RemoveSubfile(string name)
		{
			int index = FindSubfile(name);
			if (index < 0)
			{
				throw new Exception("Couldn't find the subfile " + name);
			}

			Parser.Subfiles.RemoveAt(index);
		}

		[Plugin]
		public string RenameSubfile(string subfile, string newName)
		{
			int index = FindSubfile(subfile);
			if (index < 0)
			{
				throw new Exception("Couldn't find the subfile " + subfile);
			}

			newName = newName.Trim();
			if (!Utility.ValidFilePath(newName))
			{
				throw new Exception("The name is invalid");
			}

			if (FindSubfile(newName) >= 0)
			{
				throw new Exception("A subfile with " + newName + " already exists");
			}

			Parser.Subfiles[index].Name = newName;
			return newName;
		}

		int FindSubfile(string name)
		{
			for (int i = 0; i < Parser.Subfiles.Count; i++)
			{
				if (Parser.Subfiles[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		[Plugin]
		public Stream ReadSubfile(string name)
		{
			int index = FindSubfile(name);
			if (index < 0)
			{
				throw new Exception("Couldn't find the subfile " + name);
			}

			var readFile = Parser.Subfiles[index] as IReadFile;
			if (readFile == null)
			{
				throw new Exception("The subfile " + name + " isn't readable");
			}

			return readFile.CreateReadStream();
		}
	}
}
