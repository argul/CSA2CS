using System;
using System.Reflection;

namespace CSA2CS
{
	public static class PropertyDumper
	{
		public static void DumpProperty(PropertyInfo pi, DumpContext ctx)
		{
			ctx.NewLine();
			var compileName = TypeMetaHelper.GetTypeUsageName(pi.PropertyType, ctx);
			ctx.Push(Privacy(pi));
			ctx.Push(Trait(pi));
			ctx.Push(compileName);
			ctx.Push(' ');
			ctx.Push(pi.Name);
			ctx.LeftBracket();
			if (pi.CanRead)
			{
				ctx.NewLine();
				ctx.Push(Consts.KEYWORD_GET);
				ctx.Push("{ return default(");
				ctx.Push(compileName);
				ctx.Push("); }");
			}
			if (pi.CanWrite)
			{
				ctx.NewLine();
				ctx.Push(Consts.KEYWORD_SET);
				ctx.Push("{ }");
			}
			ctx.RightBracket();
		}

		private static string Privacy(PropertyInfo info)
		{
			if (info.CanRead) return MethodDumper.Privacy(info.GetGetMethod(true));
			else if (info.CanWrite) return MethodDumper.Privacy(info.GetSetMethod(true));
			else Assert.Error("Oops! what is this? " + info.ToString());
			
			return "";
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

