using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace SB3Utility
{
	public class XmlComments
	{
		public string Name;
		public string Description;
		public Dictionary<string, string> Parameters;
		public string Returns = null;

		public XmlComments(XElement element)
		{
			string ns = element.Name.NamespaceName;
			XName xname = XName.Get("name", ns);

			Name = element.Attribute(xname).Value;
			Parameters = new Dictionary<string, string>();

			foreach (var child in element.Elements())
			{
				switch (child.Name.LocalName.ToLowerInvariant())
				{
					case "summary":
						Description = child.Value.Trim();
						break;
					case "param":
						Parameters.Add(child.Attribute(xname).Value, child.Value.Trim());
						break;
					case "returns":
						Returns = child.Value;
						break;
					default:
						break;
				}
			}
		}
	}
}
