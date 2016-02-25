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

		public static void DumpInstanceFlags(object obj, int debugLevel,
		                                     System.Predicate<PropertyInfo> flagFilter = null,
		                                     System.Func<PropertyInfo, object, string> dumper = null)
		{
			var type = obj.GetType();
			var properties = type.GetProperties(BindingFlags.Public |
			                                    BindingFlags.Instance);
			foreach (var p in properties)
			{
				if (null != flagFilter && !flagFilter.Invoke(p)) continue;
				if (!p.CanRead) continue;
				var str = "";
				object value = null;
				try
				{
					value = p.GetValue(obj, new object[0]);
				}
				catch (Exception e)
				{
					Debug.Log("Get Value Failed : " + p.Name, Debug.DEBUG_LEVEL_ERROR);
					Debug.Log(e.Message, Debug.DEBUG_LEVEL_ERROR);
					continue;
				}
				if (null != dumper)
				{
					str = dumper.Invoke(p, value);
				}
				else
				{
					str = (p.Name + " = " + value);
				}
				Debug.Log(str, debugLevel);
			}
		}
	}
}

