using System;
using System.Collections.Generic;
using System.Text;

namespace SB3Utility
{
	[Plugin]
	public class Example
	{
		[Plugin]
		public static float DoStuff([DefaultVar]float a)
		{
			return a;
		}

		[Plugin]
		public float DoStuffInstance([DefaultVar]float a)
		{
			return a;
		}
	}
}
