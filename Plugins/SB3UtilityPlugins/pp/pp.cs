using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SB3Utility
{
	public static partial class Plugins
	{
		/// <summary>
		/// Parses a .pp archive file from the specified path.
		/// </summary>
		/// <param name="path"><b>[DefaultVar]</b> Path of the file.</param>
		/// <returns>A ppParser that represents the .pp archive.</returns>
		[Plugin]
		public static ppParser OpenPP([DefaultVar]string path)
		{
			ppFormat format = ppFormat.GetFormat(path);
			if (format == null)
			{
				throw new Exception("Couldn't auto-detect the ppFormat");
			}
			return new ppParser(path, format);
		}

		/// <summary>
		/// Parses a .pp archive file from the specified path.
		/// </summary>
		/// <param name="path"><b>[DefaultVar]</b> Path of the file.</param>
		/// <param name="format"><b>(int)</b> Index of the ppFormat array</param>
		/// <returns>A ppParser that represents the .pp archive.</returns>
		[Plugin]
		public static ppParser OpenPP([DefaultVar]string path, double format)
		{
			return new ppParser(path, ppFormat.Array[(int)format]);
		}

		/// <summary>
		/// Extracts a subfile with the specified name and writes it to the specified path.
		/// </summary>
		/// <param name="parser"><b>[DefaultVar]</b> The ppParser with the subfile.</param>
		/// <param name="name">The name of the subfile.</param>
		/// <param name="path">The destination path to write the subfile.</param>
		[Plugin]
		public static void ExportSubfile([DefaultVar]ppParser parser, string name, string path)
		{
			for (int i = 0; i < parser.Subfiles.Count; i++)
			{
				if (parser.Subfiles[i].Name == name)
				{
					FileInfo file = new FileInfo(path);
					DirectoryInfo dir = file.Directory;
					if (!dir.Exists)
					{
						dir.Create();
					}

					using (FileStream fs = file.Create())
					{
						parser.Subfiles[i].WriteTo(fs);
					}
					break;
				}
			}
		}

		[Plugin]
		public static void ExportPP([DefaultVar]ppParser parser, string path)
		{
			DirectoryInfo dir = new DirectoryInfo(path);
			if (!dir.Exists)
			{
				dir.Create();
			}

			for (int i = 0; i < parser.Subfiles.Count; i++)
			{
				var subfile = parser.Subfiles[i];
				using (FileStream fs = File.Create(dir.FullName + @"\" + subfile.Name))
				{
					subfile.WriteTo(fs);
				}
			}
		}
	}
}
