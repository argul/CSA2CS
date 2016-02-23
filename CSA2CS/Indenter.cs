using System;
using System.Collections.Generic;
namespace CSA2CS
{
	public static class Indenter
	{
		private static Dictionary<int, string> indents = new Dictionary<int, string>();
		public static string GetIndentStr(int indent)
		{
			if (indent <= 0) return String.Empty;
			string ret = String.Empty;
			if (indents.TryGetValue(indent, out ret))
			{
				return ret;
			}
			else
			{
				for (int i = 0; i < indent; i++)
				{
					ret += Consts.TAB;
				}
				indents.Add(indent, ret);
				return ret;
			}
		}
	}
}

