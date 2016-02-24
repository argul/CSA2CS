using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSA2CS
{
	public static class FieldDumper
	{
		public static string Privacy(FieldInfo info)
		{
			if (PrivacyHelper.IsPublic(info)) return Consts.KEYWORD_PUBLIC;
			else if (PrivacyHelper.IsPrivate(info)) return Consts.KEYWORD_PRIVATE;
			else if (PrivacyHelper.IsProtectedInternal(info)) return Consts.KEYWORD_PROTECTED + Consts.KEYWORD_INTERNAL;
			else if (PrivacyHelper.IsProtected(info)) return Consts.KEYWORD_PROTECTED;
			else if (PrivacyHelper.IsInternal(info)) return Consts.KEYWORD_INTERNAL;
			
			return "";
		}

		public static void DumpConstants(List<FieldInfo> constants, DumpContext ctx)
		{
			foreach (var fi in constants)
			{
				ctx.NewLine();
				ctx.Push(Privacy(fi));
				ctx.Push(Consts.KEYWORD_CONST);
				ctx.Push(TypeMetaHelper.GetTypeUsageName(fi.FieldType, ctx));
				ctx.Push(' ');
				ctx.Push(fi.Name);
				ctx.Push(" = ");
				ValueDumper.Dump(fi.FieldType, fi.GetRawConstantValue(), ctx);
				ctx.Push(';');
			}
		}

		public static void DumpField(FieldInfo fi, DumpContext ctx, bool isEventField)
		{
			ctx.NewLine();
			ctx.Push(Privacy(fi));
			if (TraitHelper.IsStatic(fi))
			{
				ctx.Push(Consts.KEYWORD_STATIC);
			}
			if (isEventField)
			{
				ctx.Push(Consts.KEYWORD_EVENT);
			}
			ctx.Push(TypeMetaHelper.GetTypeUsageName(fi.FieldType, ctx));
			ctx.Push(' ');
			ctx.Push(fi.Name);
			ctx.Push(';');
		}
	}
}

