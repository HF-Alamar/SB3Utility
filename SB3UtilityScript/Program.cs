using System;
using System.Collections.Generic;
using System.Text;

namespace SB3Utility
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				if (args.Length <= 0)
				{
					Console.WriteLine("Usage: SB3UtilityScript \"scriptPath.txt\"");
				}
				else
				{
					for (int i = 0; i < args.Length; i++)
					{
						ScriptMain script = new ScriptMain();
						Report.Log += new Action<string>(Logger);
						script.LoadPlugin((string)script.ScriptEnvironment.Variables[ScriptExecutor.PluginDirectoryName] + "SB3UtilityPlugins.dll");
						script.RunScript(args[i]);
					}
				}
			}
			catch (Exception ex)
			{
				Exception inner = ex;
				while (inner != null)
				{
					Console.WriteLine(inner.Message);
					inner = inner.InnerException;
				}
			}
		}

		static void Logger(string s)
		{
			Console.WriteLine(s);
		}
	}
}
