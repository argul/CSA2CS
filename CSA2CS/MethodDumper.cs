using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSA2CS
{
	public static class MethodDumper
	{
		public static string Privacy(MethodInfo info)
		{
			if (PrivacyHelper.IsPublic(info)) return Consts.KEYWORD_PUBLIC;
			else if (PrivacyHelper.IsPrivate(info)) return Consts.KEYWORD_PRIVATE;
			else if (PrivacyHelper.IsProtectedInternal(info)) return Consts.KEYWORD_PROTECTED + Consts.KEYWORD_INTERNAL;
			else if (PrivacyHelper.IsProtected(info)) return Consts.KEYWORD_PROTECTED;
			else if (PrivacyHelper.IsInternal(info)) return Consts.KEYWORD_INTERNAL;
			
			return "";
		}

		public static string Privacy(ConstructorInfo info)
		{
			if (PrivacyHelper.IsPublic(info)) return Consts.KEYWORD_PUBLIC;
			else if (PrivacyHelper.IsPrivate(info)) return Consts.KEYWORD_PRIVATE;
			else if (PrivacyHelper.IsProtectedInternal(info)) return Consts.KEYWORD_PROTECTED + Consts.KEYWORD_INTERNAL;
			else if (PrivacyHelper.IsProtected(info)) return Consts.KEYWORD_PROTECTED;
			else if (PrivacyHelper.IsInternal(info)) return Consts.KEYWORD_INTERNAL;
			
			return "";
		}

		public static string Trait(MethodInfo info)
		{
			if (TraitHelper.IsStatic(info)) return Consts.KEYWORD_STATIC;
			else if (TraitHelper.IsAbstract(info)) return Consts.KEYWORD_ABSTRACT;
			else if (TraitHelper.IsVirtualNoneOverride(info)) return Consts.KEYWORD_VIRTUAL;
			else if (TraitHelper.IsOverrideSealed(info)) return Consts.KEYWORD_SEALED + Consts.KEYWORD_OVERRIDE;
			else if (TraitHelper.IsOverrideNoneSealed(info)) return Consts.KEYWORD_OVERRIDE;

			return "";
		}

		public static void DumpMethod(MethodInfo mi, DumpContext ctx, bool isInterface)
		{
			ctx.NewLine();
			if (!isInterface)
			{
				ctx.Push(Privacy(mi));
				ctx.Push(Trait(mi));
			}
			ctx.Push(ReturnTypeStr(mi, ctx));
			ctx.Push(mi.Name);
			ctx.Push('(');
			
			List<ParameterInfo> outParams = null;
			var parameters = mi.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].IsOut)
				{
					if (null == outParams) outParams = new List<ParameterInfo>();
					outParams.Add(parameters[i]);
				}
				MethodParameterDumper.DumpParameter(parameters[i], ctx);
				if (i < parameters.Length - 1)
				{
					ctx.Push(", ");
				}
			}
			
			ctx.Push(')');
			if (isInterface || mi.IsAbstract)
			{
				ctx.Push(';');
			}
			else if (mi.ReturnType == typeof(void) && null == outParams)
			{
				ctx.Push(" { }");
			}
			else
			{
				ctx.LeftBracket();
				if (null != outParams)
				{
					foreach (var o in outParams)
					{
						ctx.NewLine();
						ctx.Push(o.Name);
						ctx.Push(" = ");
						ctx.Push("default(");
						ctx.Push(TypeMetaHelper.GetTypeUsageName(o.ParameterType, ctx));
						ctx.Push(");");
					}
				}

				if (mi.ReturnType == typeof(System.Collections.IEnumerator))
				{
					ctx.NewLine();
					ctx.Push("yield break;");
				}
				else if (mi.ReturnType != typeof(void))
				{
					ctx.NewLine();
					ctx.Push("return default(");
					ctx.Push(TypeMetaHelper.GetTypeUsageName(mi.ReturnType, ctx));
					ctx.Push(");");
				}
				
				ctx.RightBracket();
			}
		}

		public static void DumpConstructor(ConstructorInfo ci, DumpContext ctx)
		{
			ctx.NewLine();
			ctx.Push(Privacy(ci));
			ctx.Push(ctx.data.Name);
			ctx.Push('(');
			var parameters = ci.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				MethodParameterDumper.DumpParameter(parameters[i], ctx);
				if (i < parameters.Length - 1)
				{
					ctx.Push(", ");
				}
			}
			ctx.Push(") {}");
		}

		public static void DumpFinalizer(DumpContext ctx)
		{
			ctx.NewLine();
			ctx.Push('~');
			ctx.Push(ctx.data.Name);
			ctx.Push("() {}");
		}

		public static void DumpDelegateType(System.Type type, DumpContext ctx)
		{
			Assert.AssertIsTrue(type.BaseType == typeof(MulticastDelegate));
			var mi = type.GetMethod("Invoke");
			Assert.AssertNotNull(mi);

			ctx.NewLine();
			ctx.Push(Privacy(mi));
			ctx.Push(Consts.KEYWORD_DELEGATE);
			ctx.Push(ReturnTypeStr(mi, ctx));
			ctx.Push(TypeMetaHelper.GetTypeDeclarationName(type));
			ctx.Push('(');

			var parameters = mi.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				MethodParameterDumper.DumpParameter(parameters[i], ctx);
				if (i < parameters.Length - 1)
				{
					ctx.Push(", ");
				}
			}

			ctx.Push(");");
		}

		private static string ReturnTypeStr(MethodInfo info, DumpContext ctx)
		{
			if (info.ReturnType == typeof(void))
			{
				return Consts.KEYWORD_VOID;
			}
			else
			{
				var name = TypeMetaHelper.GetTypeUsageName(info.ReturnType, ctx);
				return name + " ";
			}
		}
	}
}

