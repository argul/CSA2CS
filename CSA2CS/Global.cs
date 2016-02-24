using System;
using System.Collections.Generic;

namespace CSA2CS
{
	public static class Global
	{
		public static bool IGNORE_ANONYMOUS = true;
		public static bool INTERRUPT_ON_ERROR = false;

		public static TypeData FindTypeData(System.Type type)
		{
			TypeData ret = null;
			typeDataCluster.TryGetValue(TypeGUID(type), out ret);
			return ret;
		}

		public static void RegisterTypeData(TypeData data)
		{
			Debug.Log("RegisterTypeData : " + data.Type.Name, Debug.DEBUG_LEVEL_VERBOSE);
			typeDataCluster.Add(TypeGUID(data.Type), data);
		}

		private static Dictionary<string, TypeData> typeDataCluster = new Dictionary<string, TypeData>();
		private static string TypeGUID(System.Type type)
		{
			if (type.IsNested)
			{
				return TypeGUID(type.DeclaringType) + '.' + type.Name;
			}
			else if (!String.IsNullOrEmpty(type.Namespace))
			{
				return type.Namespace + '.' + type.Name;
			}
			else
			{
				return type.Name;
			}
		}
	}
}