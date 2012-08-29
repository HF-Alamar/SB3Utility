using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SB3Utility
{
	public class ScriptMain
	{
		public ScriptExecutor ScriptEnvironment { get; protected set; }

		public ScriptMain()
		{
			ScriptEnvironment = new ScriptExecutor();
		}

		public void LoadPlugin(string path)
		{
			PluginManager.LoadPlugin(path);
		}

		public object RunScript(string path)
		{
			ScriptParser parser = new ScriptParser(path);
			return ScriptEnvironment.RunScript(parser);
		}

		public object RunScript(Stream stream, string scriptName)
		{
			ScriptParser parser = new ScriptParser(stream, scriptName);
			return ScriptEnvironment.RunScript(parser);
		}
	}
}
