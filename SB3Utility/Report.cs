using System;

namespace SB3Utility
{
	public static class Report
	{
		public static event Action<string> Log;
		public static event Action<string> Status;

		public static void ReportLog(string msg)
		{
			if (Log != null)
			{
				Log(msg);
			}
		}

		public static void ReportStatus(string msg)
		{
			if (Status != null)
			{
				Status(msg);
			}
		}
	}
}
