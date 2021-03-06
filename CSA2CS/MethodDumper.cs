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

		public static void DumpMethod(MethodInfo mi, DumpContext ctx)
		{
			var dllImportAttr = mi.GetCustomAttributes(typeof(System.Runtime.InteropServices.DllImportAttribute), false);
			if (null != dllImportAttr && dllImportAttr.Length > 0)
			{
				DumpDllImportMethod(mi, ctx, dllImportAttr);
			}
			else if (IsOperatorMethod(mi))
			{
				if (customizedOperDumpers.ContainsKey(mi.Name))
					customizedOperDumpers[mi.Name].Invoke(mi, ctx);
				else
					DumpManagedMethod(mi, ctx);
			}
			else
			{
				DumpManagedMethod(mi, ctx);
			}
		}

		private static void DumpDllImportMethod(MethodInfo mi, DumpContext ctx, object[] attrs)
		{
			ctx.NewLine();
			if (attrs.Length > 1)
			{
				Debug.Log("Multiple DllImportAttributes : " + mi.Name, Debug.DEBUG_LEVEL_ERROR);
			}
			else
			{
				//[DllImport("test.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
				var attr = attrs[0] as System.Runtime.InteropServices.DllImportAttribute;
				ctx.Push("[DllImport(\"");
				ctx.Push(attr.Value);
				ctx.Push("\", CharSet = CharSet.");
				ctx.Push(attr.CharSet.ToString());
				ctx.Push(", CallingConvention = CallingConvention.");
				ctx.Push(attr.CallingConvention.ToString());
				ctx.Push(")]");

				ctx.NewLine();
				//public static extern int TEST(IntPtr hWnd, String text, String caption, uint type);
				ctx.Push(Privacy(mi));
				ctx.Push(Consts.KEYWORD_STATIC);
				ctx.Push(Consts.KEYWORD_EXTERN);
				ctx.Push(ReturnTypeStr(mi, ctx));
				ctx.Push(mi.Name);
				ctx.Push('(');

				var parameters = mi.GetParameters();
				for (int i = 0; i < parameters.Length; i++)
				{
					MethodParameterDumper.DumpParameter(parameters[i], ctx);
					if (i < parameters.Length - 1) ctx.Push(", ");
				}

				ctx.Push(");");
			}
		}

		private static void DumpManagedMethod(MethodInfo mi, DumpContext ctx)
		{
			bool isInterface = ctx.data.Type.IsInterface;
			ctx.NewLine();
			if (!isInterface)
			{
				ctx.Push(Privacy(mi));
				ctx.Push(Trait(mi));
			}
			ctx.Push(ReturnTypeStr(mi, ctx));
			DumpMethodDeclareName(mi, ctx);
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
				if (i < parameters.Length - 1) ctx.Push(", ");
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

		private static void DumpMethodDeclareName(MethodInfo mi, DumpContext ctx)
		{
			if (IsOperatorMethod(mi))
			{
				ctx.Push(Consts.KEYWORD_OPERATOR);
				ctx.Push(operatorMethodNames[mi.Name]);
				ctx.Push(' ');
			}
			else
				ctx.Push(mi.Name);

			if (mi.IsGenericMethod) DumpGenericTypeArguments(mi, ctx);
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

		public static bool IsOperatorMethod(MethodInfo mi)
		{
			return operatorMethodNames.ContainsKey(mi.Name);
		}
		private static Dictionary<string, Action<MethodInfo, DumpContext>> customizedOperDumpers = new Dictionary<string, Action<MethodInfo, DumpContext>>()
		{
			{ 
				"op_Implicit", (mi, ctx)=>{
					ctx.NewLine();
					ctx.Push(Privacy(mi));
					ctx.Push(Consts.KEYWORD_STATIC);
					ctx.Push("implicit ");
					ctx.Push(Consts.KEYWORD_OPERATOR);
					ctx.Push(ctx.data.Name);
					ctx.Push('(');
					var parameters = mi.GetParameters();
					Assert.AssertIsTrue(1 == parameters.Length);
					MethodParameterDumper.DumpParameter(parameters[0], ctx);
					ctx.Push(')');
					ctx.Push(" { return default(");
					ctx.Push(TypeMetaHelper.GetTypeUsageName(parameters[0].ParameterType, ctx));
					ctx.Push("); }");
				}
			},
			{ 
				"op_Explicit", (mi, ctx)=>{
					ctx.NewLine();
					ctx.Push(Privacy(mi));
					ctx.Push(Consts.KEYWORD_STATIC);
					ctx.Push("explicit ");
					ctx.Push(Consts.KEYWORD_OPERATOR);
					ctx.Push(ctx.data.Name);
					ctx.Push('(');
					var parameters = mi.GetParameters();
					Assert.AssertIsTrue(1 == parameters.Length);
					MethodParameterDumper.DumpParameter(parameters[0], ctx);
					ctx.Push(')');
					ctx.Push(" { return default(");
					ctx.Push(ctx.data.Name);
					ctx.Push("); }");
				}
			}
		};
		private static Dictionary<string, string> operatorMethodNames = new Dictionary<string, string>()
		{
			{ "op_Implicit", "" },
			{ "op_Explicit", "" },
			{ "op_Addition", "+" },
			{ "op_Subtraction", "-" },
			{ "op_Multiply", "*" },
			{ "op_Division", "/" },
			{ "op_Modulus", "%" },
			{ "op_ExclusiveOr", "^" },
			{ "op_BitwiseAnd", "&" },
			{ "op_BitwiseOr", "|" },
			{ "op_LogicalAnd", "&&" },
			{ "op_LogicalOr", "||" },
			{ "op_Assign", "=" },
			{ "op_LeftShift", "<<" },
			{ "op_RightShift", ">>" },
			{ "op_SignedRightShift", ">>" },
			{ "op_UnsignedRightShift", "<<" },
			{ "op_Equality", "==" },
			{ "op_GreaterThan", ">" },
			{ "op_LessThan", "<" },
			{ "op_Inequality", "!=" },
			{ "op_GreaterThanOrEqual", ">=" },
			{ "op_LessThanOrEqual", "<=" },
			{ "op_MultiplicationAssignment", "*=" },
			{ "op_SubtractionAssignment", "-=" },
			{ "op_ExclusiveOrAssignment", "^=" },
			{ "op_LeftShiftAssignment", "<<=" },
			{ "op_RightShiftAssignment", ">>=" },
			{ "op_ModulusAssignment", "%=" },
			{ "op_AdditionAssignment", "+=" },
			{ "op_BitwiseAndAssignment", "&=" },
			{ "op_BitwiseOrAssignment", "|=" },
			{ "op_Comma", "," },
			{ "op_DivisionAssignment", "/=" },
			{ "op_Decrement", "--" },
			{ "op_Increment", "++" },
			{ "op_UnaryNegation", "-" },
			{ "op_UnaryPlus", "+" },
			{ "op_OnesComplement", "~" }
		};
	}
}

