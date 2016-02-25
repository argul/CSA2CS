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
			else if (PrivacyHelper.IsProtectedInternal(info)) return Consts.KEYWORD_PROTECTED_INTERNAL;
			else if (PrivacyHelper.IsProtected(info)) return Consts.KEYWORD_PROTECTED;
			else if (PrivacyHelper.IsInternal(info)) return Consts.KEYWORD_INTERNAL;
			
			return "";
		}

		public static string Privacy(ConstructorInfo info)
		{
			if (PrivacyHelper.IsPublic(info)) return Consts.KEYWORD_PUBLIC;
			else if (PrivacyHelper.IsPrivate(info)) return Consts.KEYWORD_PRIVATE;
			else if (PrivacyHelper.IsProtectedInternal(info)) return Consts.KEYWORD_PROTECTED_INTERNAL;
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
			// buggy : MethodBase.IsHideBySig always return true in mono env, so TEST_HIDE will fail!

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
			if (mi.IsGenericMethod) DumpGenericTypeArguments(mi, ctx);
			ctx.Push('(');
			bool isExtensionMethod = TraitHelper.IsExtensionMethod(mi);
			
			List<ParameterInfo> outParams = null;
			var parameters = mi.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (0 == i && isExtensionMethod) ctx.Push(Consts.KEYWORD_THIS);
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

		private static void DumpGenericTypeArguments(MethodInfo mi, DumpContext ctx)
		{
			Assert.AssertIsTrue(mi.IsGenericMethod);
			ctx.Push('<');
			var args = mi.GetGenericArguments();
			for (int i = 0; i < args.Length; i++)
			{
				ctx.Push(TypeMetaHelper.GetTypeDeclarationName(args[i]));
				if (i != args.Length - 1) ctx.Push(", ");
			}
			ctx.Push('>');
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

		public static void DumpSpecialMethod(MethodInfo info, DumpContext ctx)
		{
			Assert.AssertIsTrue(info.IsSpecialName);
			ctx.NewLine();
			ctx.Push(info.Name);
		}

//		private static Dictionary<string, string> overridedOperators = new Dictionary<string, string>()
//		{
//			op_Implicit
//			op_Explicit
//			op_Addition
//			op_Subtraction
//			op_Multiply
//			op_Division
//			op_Modulus
//			op_ExclusiveOr
//			op_BitwiseAnd
//			op_BitwiseOr
//			op_LogicalAnd
//			op_LogicalOr
//			op_Assign
//			op_LeftShift
//			op_RightShift
//			op_SignedRightShift
//			op_UnsignedRightShift
//			op_Equality
//			op_GreaterThan
//			op_LessThan
//			op_Inequality
//			op_GreaterThanOrEqual
//			op_LessThanOrEqual
//			op_MultiplicationAssignment
//			op_SubtractionAssignment
//			op_ExclusiveOrAssignment
//			op_LeftShiftAssignment
//			op_ModulusAssignment
//			op_AdditionAssignment
//			op_BitwiseAndAssignment
//			op_BitwiseOrAssignment
//			op_Comma
//			op_DivisionAssignment
//			op_Decrement
//			op_Increment
//			op_UnaryNegation
//			op_UnaryPlus
//			op_OnesComplement
//		};
	}
}

