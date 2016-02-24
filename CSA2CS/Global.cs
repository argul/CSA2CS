using System;
using System.Collections.Generic;

namespace CSA2CS
{
	public static class Global
	{
		public static bool IGNORE_ANONYMOUS = true;
		public static int DEBUG_LEVEL = -1;

		public static TypeData FindTypeData(System.Type type)
		{
			TypeData ret = null;
			typeDataCluster.TryGetValue(type, out ret);
			return ret;
		}

		public static void RegisterTypeData(TypeData data)
		{
			typeDataCluster.Add(data.Type, data);
		}

		private static Dictionary<System.Type, TypeData> typeDataCluster = new Dictionary<Type, TypeData>();
	}
}