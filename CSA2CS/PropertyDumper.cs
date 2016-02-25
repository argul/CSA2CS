using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSA2CS
{
	public static class PropertyDumper
	{
		public static void DumpProperty(PropertyInfo pi, DumpContext ctx)
		{
			ctx.NewLine();
			var compileName = TypeMetaHelper.GetTypeUsageName(pi.PropertyType, ctx);
			var privacyArr = Privacy(pi);

			ctx.Push(privacyArr[0]);
			ctx.Push(Trait(pi));
			ctx.Push(compileName);
			ctx.Push(' ');

			var indexParams = pi.GetIndexParameters();
			if (null != indexParams && indexParams.Length > 0)
				DumpIndexedName(pi, ctx, indexParams);
			else
				ctx.Push(pi.Name);
			
			ctx.LeftBracket();
			if (pi.CanRead)
			{
				ctx.NewLine();
				ctx.Push(privacyArr[1]);
				ctx.Push(Consts.KEYWORD_GET);
				ctx.Push("{ return default(");
				ctx.Push(compileName);
				ctx.Push("); }");
			}
			if (pi.CanWrite)
			{
				ctx.NewLine();
				ctx.Push(privacyArr[2]);
				ctx.Push(Consts.KEYWORD_SET);
				ctx.Push("{ }");
			}
			ctx.RightBracket();
		}

		private static void DumpIndexedName(PropertyInfo pi, DumpContext ctx, ParameterInfo[] indexParams)
		{
			if (pi.Name != "Item")
			{
				Debug.Log ("Unknown Indexed Property : " + pi.Name, Debug.DEBUG_LEVEL_ERROR);
			}
			ctx.Push("this[");
			int len = indexParams.Length;
			for (int i = 0; i < len; i++)
			{
				MethodParameterDumper.DumpParameter(indexParams[i], ctx);
				if (i != len - 1) ctx.Push(", ");
			}
			ctx.Push(']');
		}

		private static List<string> privacyPriorities = new List<string>
		{
			Consts.KEYWORD_PUBLIC,
			Consts.KEYWORD_INTERNAL,
			Consts.KEYWORD_PROTECTED,
			Consts.KEYWORD_PROTECTED_INTERNAL,
			Consts.KEYWORD_PRIVATE,
			String.Empty
		};
		private static string[] Privacy(PropertyInfo info)
		{
			var ret = new string[3] { String.Empty, String.Empty, String.Empty };
			if (info.CanRead && info.CanWrite)
			{
				var privacyRead = MethodDumper.Privacy(info.GetGetMethod(true));
				var privacyWrite = MethodDumper.Privacy(info.GetSetMethod(true));
				if (privacyRead == privacyWrite)
					ret[0] = privacyRead;
				else
				{
					var priorityRead = privacyPriorities.IndexOf(privacyRead);
					var priorityWrite = privacyPriorities.IndexOf(privacyWrite);
					if (priorityRead < priorityWrite)
					{
						ret[0] = privacyRead;
						ret[2] = privacyWrite;
					}
					else
					{
						ret[0] = privacyWrite;
						ret[1] = privacyRead;
					}
				}
			}
			else if (info.CanRead)
			{
				ret[0] = MethodDumper.Privacy(info.GetGetMethod(true));
			}
			else if (info.CanWrite)
			{
				ret[0] = MethodDumper.Privacy(info.GetSetMethod(true));
			}
			else Assert.Error("Oops! what is this? " + info.ToString());
			
			return ret;
		}
		
		private static string Trait(PropertyInfo info)
		{
			if (info.CanRead) return MethodDumper.Trait(info.GetGetMethod(true));
			else if (info.CanWrite) return MethodDumper.Trait(info.GetSetMethod(true));
			else Assert.Error("Oops! what is this? " + info.ToString());
			
			return "";
		}
	}
}

