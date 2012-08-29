using System;
using System.Collections.Generic;
using System.Text;

namespace SB3Utility
{
	public static class ScriptHelper
	{
		public static void SetProperty(string obj, string name, object value)
		{
			Gui.Scripting.RunScript("SetProperty(obj=" + obj + ", name=\"" + name + "\", value=[" + value + "])");
		}

		public static string String(string variable, object value)
		{
			return variable + "=\"" + value + "\"";
		}

		public static string Bytes(string variable, object value)
		{
			return variable + "=[" + value + "]";
		}

		public static string Parameters(string[] parameters)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < parameters.Length; i++)
			{
				sb.Append(parameters[i]);
				sb.Append(", ");
			}
			return sb.ToString(0, sb.Length - 2);
		}
	}
}
