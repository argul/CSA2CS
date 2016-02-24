using System;
using System.Reflection;

namespace CSA2CS
{
	public static class Debug
	{
		public static int DEBUG_LEVEL = DEBUG_LEVEL_TEST;
		public const int DEBUG_LEVEL_TEST = -1000;
		public const int DEBUG_LEVEL_FATAL_ERROR = 0;
		public const int DEBUG_LEVEL_ERROR = 1;
		public const int DEBUG_LEVEL_WARNING = 2;
		public const int DEBUG_LEVEL_LOG = 3;
		public const int DEBUG_LEVEL_TRIVIAL = 4;
		public const int DEBUG_LEVEL_VERBOSE = 5;

		public static void Log(string str, int debugLevel)
		{
			if (DEBUG_LEVEL >= debugLevel)
			{
				Console.WriteLine(str);
			}
			if (debugLevel <= DEBUG_LEVEL_ERROR && Global.INTERRUPT_ON_ERROR)
			{
				throw new Exception(str);
			}
		}

		public static void DumpTypeFlags(System.Type type, int debugLevel,
		                                 System.Predicate<PropertyInfo> flagFilter = null,
		                                 System.Func<PropertyInfo, object, string> dumper = null)
		{
			var properties = type.GetProperties(BindingFlags.Public |
			                                    BindingFlags.Instance);
			foreach (var p in properties)
			{
				if (null != flagFilter && !flagFilter.Invoke(p)) continue;
				var str = "";
				if (null != dumper)
				{
					str = dumper.Invoke(p, p.GetValue(type, new object[0]));
				}
				else
				{
					str = (p.Name + " = " + p.GetValue(type, new object[0]));
				}
				Debug.Log(str, debugLevel);
			}
		}
	}
}

