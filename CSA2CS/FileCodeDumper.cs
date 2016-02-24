using System;
namespace CSA2CS
{
	public static class FileCodeDumper
	{
		public static string DumpCode(TypeData data, string[] prefixs = null)
		{
			var ctx = new DumpContext();
			ctx.data = data;
			ctx.builder = Utils.GraspBuilder();
			ctx.indentLevel = 0;
			ctx.enclosingNamespace = data.Namespace;

			if (null != prefixs)
			{
				foreach (var p in prefixs)
				{
					ctx.Push(p);
					ctx.NewLine();
				}
			}

			var ns = data.Namespace;
			bool hasNamespace = !String.IsNullOrEmpty(ns);
			if (hasNamespace)
			{
				ctx.Push(Consts.KEYWORD_NAMESPACE);
				ctx.Push(ns);
				ctx.LeftBracket();
			}

			TypeCodeDumper.Dump(ctx);

			if (hasNamespace)
			{
				ctx.RightBracket();
			}

			var ret = ctx.builder.GetString();
			Utils.DropBuilder(ctx.builder);
			ctx = null;
			return ret;
		}

		private static System.Text.StringBuilder sb = new System.Text.StringBuilder();
		public static string GetFileName(string typeName, string extension = "")
		{
			foreach (var c in typeName.ToCharArray())
			{
				if (c == '+') sb.Append('_');
				else if (c == '.') sb.Append('_');
				else if (c == '<') sb.Append('_');
				else if (c == ',') sb.Append('_');
				else if (c == '>' || c == ' ') {}
				else sb.Append(c);
			}
			sb.Append(extension);
			var ret = sb.ToString();
			sb.Length = 0;
			return ret;
		}
	}
}

