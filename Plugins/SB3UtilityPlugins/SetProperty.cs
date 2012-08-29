using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		public static void SetProperty(object obj, string name, object value)
		{
			var type = obj.GetType();
			type.GetProperty(name).SetValue(obj, value, null);
		}

		[Plugin]
		public static void SetIndexed(object obj, int index, object value)
		{
			var type = obj.GetType();
			if (type.IsArray)
			{
				((Array)obj).SetValue(value, index);
			}
			else
			{
				var attributes = type.GetCustomAttributes(typeof(DefaultMemberAttribute), true);
				if (attributes.Length > 0)
				{
					var indexerName = ((DefaultMemberAttribute)attributes[0]).MemberName;
					type.GetProperty(indexerName).SetValue(obj, value, new object[] { index });
				}
				else
				{
					throw new Exception(obj.ToString() + " can't be indexed.");
				}
			}
		}
	}
}
