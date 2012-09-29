using System;
using System.Collections.Generic;
using System.Text;

namespace SB3Utility
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class DefaultVar : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class Plugin : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true)]
	public class PluginOpensFile : Attribute
	{
		public string FileExtension { get; protected set; }

		public PluginOpensFile(string ext)
		{
			this.FileExtension = ext;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class PluginTool : Attribute
	{
		public string Name { get; protected set; }
		public string Shortcut { get; protected set; }

		public PluginTool(string name, string shortcut)
		{
			this.Name = name;
			this.Shortcut = shortcut;
		}
	}
}
