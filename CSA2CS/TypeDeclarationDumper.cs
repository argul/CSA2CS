using System;
using System.Reflection;

namespace CSA2CS
{
	public static class TypeDeclarationDumper
	{
		public static void DumpDeclaration(DumpContext ctx)
		{
			if (ctx.data.Type.IsInterface)
			{
				DumpInterfaceDeclaration(ctx);
			}
			else if (ctx.data.Type.IsClass)
			{
				DumpClassDeclaration(ctx);
			}
			else
			{
				DumpStructDeclaration(ctx);
			}
		}

		private static void DumpInterfaceDeclaration(DumpContext ctx)
		{
			var type = ctx.data.Type;
			// Can't use the other variant here! some interface will all return false but they turn out to be internal.
			if (PrivacyHelper.IsPublic(type))
				ctx.Push(Consts.KEYWORD_PUBLIC);
			else
				ctx.Push(Consts.KEYWORD_INTERNAL);

			ctx.Push(Consts.KEYWORD_INTERFACE);
			ctx.Push(ctx.data.Name);
		}

		private static void DumpStructDeclaration(DumpContext ctx)
		{
			var type = ctx.data.Type;
			if (PrivacyHelper.IsPublic(type))
				ctx.Push(Consts.KEYWORD_PUBLIC);
			else if (PrivacyHelper.IsProtectedInternal(type))
				ctx.Push(Consts.KEYWORD_PROTECTED_INTERNAL);
			else if (PrivacyHelper.IsProtected(type))
				ctx.Push(Consts.KEYWORD_PROTECTED);
			else if (PrivacyHelper.IsInternal(type))
				ctx.Push(Consts.KEYWORD_INTERNAL);
			else if (PrivacyHelper.IsPrivate(type))
				ctx.Push(Consts.KEYWORD_PRIVATE);

			ctx.Push(Consts.KEYWORD_STRUCT);
			
			ctx.Push(ctx.data.Name);
		}

		private static void DumpClassDeclaration(DumpContext ctx)
		{
			var type = ctx.data.Type;
			if (PrivacyHelper.IsPublic(type))
				ctx.Push(Consts.KEYWORD_PUBLIC);
			else if (PrivacyHelper.IsProtectedInternal(type))
				ctx.Push(Consts.KEYWORD_PROTECTED_INTERNAL);
			else if (PrivacyHelper.IsProtected(type))
				ctx.Push(Consts.KEYWORD_PROTECTED);
			else if (PrivacyHelper.IsInternal(type))
				ctx.Push(Consts.KEYWORD_INTERNAL);
			else if (PrivacyHelper.IsPrivate(type))
				ctx.Push(Consts.KEYWORD_PRIVATE);
			
			if (TraitHelper.IsStatic(type))
				ctx.Push(Consts.KEYWORD_STATIC);
			else if (TraitHelper.IsSealed(type))
				ctx.Push(Consts.KEYWORD_SEALED);
			else if (TraitHelper.IsAbstract(type))
				ctx.Push(Consts.KEYWORD_ABSTRACT);

			ctx.Push(Consts.KEYWORD_CLASS);

			ctx.Push(ctx.data.Name);

			DumpInheritance(ctx);
		}

		private static bool IsDerivedClass(System.Type type)
		{
			if (!type.IsClass) return false;
			return (null != type.BaseType && type.BaseType != typeof(Object));
		}

		private static System.Type[] baseObjectInterfaces = typeof(Object).GetInterfaces();
		private static void DumpInheritance(DumpContext ctx)
		{
			var type = ctx.data.Type;
			var inherits = new System.Collections.Generic.List<System.Type>();
			var ignoreInterfaces = new System.Collections.Generic.List<System.Type>();
			if (IsDerivedClass(type))
			{
				inherits.Add(type.BaseType);
				ignoreInterfaces.AddRange(type.BaseType.GetInterfaces());
			}
			else
			{
				ignoreInterfaces.AddRange(baseObjectInterfaces);
			}

			foreach (var itf in type.GetInterfaces())
			{
				if (!ignoreInterfaces.Contains(itf))
				{
					inherits.Add(itf);
				}
			}

			var length = inherits.Count;
			if (length > 0)
			{
				ctx.Push(" : ");
				for (int i = 0; i < length; i++)
				{
					ctx.Push(TypeMetaHelper.GetTypeDeclarationName(inherits[i]));
					if (i != length - 1) ctx.Push(", ");
				}
			}
		}
	}
}

