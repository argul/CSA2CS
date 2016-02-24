using System;

namespace CSA2CS
{
	public static class Debug
	{
		public static void Log(string str, int debugLevel = 0)
		{
			if (Global.DEBUG_LEVEL >= debugLevel)
			{
				Console.WriteLine(str);
			}
		}
	}
}

