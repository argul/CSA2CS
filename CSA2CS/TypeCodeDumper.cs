using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSA2CS
{
	public static class TypeCodeDumper
	{
		public static void Dump(DumpContext ctx)
		{
			Assert.AssertIsTrue(ctx.data.IsInited);
			if (ctx.data.Type.IsPrimitive)
			{
				Assert.Error("Attempt to dump primitive type!");
			}
			else if (ctx.data.Type.IsEnum)
			{
				DumpEnum(ctx);
			}
			else if (ctx.data.Type.BaseType == typeof(MulticastDelegate))
			{
				MethodDumper.DumpDelegateType(ctx.data.Type, ctx);
			}
			else
			{
				DumpComposedType(ctx);
			}
		}

		private static void DumpEnum(DumpContext ctx)
		{
			ctx.NewLine();

			if (ctx.data.Type.IsPublic) ctx.Push(Consts.KEYWORD_PUBLIC);
			else if (ctx.data.Type.IsNotPublic) ctx.Push(Consts.KEYWORD_PRIVATE);

			ctx.Push(Consts.KEYWORD_ENUM);
			ctx.Push(ctx.data.Name);

			ctx.LeftBracket();
			
			var names = System.Enum.GetNames(ctx.data.Type);
			var values = System.Enum.GetValues(ctx.data.Type);
			int len = names.Length;
			
			for (int i = 0; i < len; i++)
			{
				ctx.Push(Consts.NEWLINE);
				ctx.builder.Indent(ctx.indentLevel);
				ctx.Push(names[i]);
				ctx.Push(" = ");
				ctx.Push(((int)values.GetValue(i)).ToString());
				if (i != len - 1) ctx.Push(',');
			}

			ctx.RightBracket();
		}

		private static void DumpComposedType(DumpContext ctx)
		{
			var type = ctx.data.Type;
			ctx.NewLine();

			DumpDeclaration(ctx);

			ctx.LeftBracket();
			ctx.SwallowNewline = true;

			DumpConstants(ctx);
			if (ctx.data.HasNestedDelegate)
			{
				ctx.NewLine();
				foreach (var del in ctx.data.NestedDelegates)
				{
					MethodDumper.DumpDelegateType(del, ctx);
				}
			}

			DumpConstructors(ctx);
			DumpFinalizer(ctx);
			DumpStaticEvents(ctx);
			DumpStaticProperties(ctx);
			DumpStaticMethods(ctx);
			DumpStaticFields(ctx);

			DumpInstanceEvents(ctx);
			DumpInstanceProperties(ctx);
			DumpInstanceMethods(ctx, type.IsInterface);
			DumpInstanceFields(ctx);

			if (ctx.data.HasNestedType)
			{
				ctx.NewLine();
				var saved = ctx.data;
				foreach (var tp in ctx.data.NestedTypes)
				{
					ctx.data = tp;
					Dump(ctx);
				}
				ctx.data = saved;
			}

			ctx.RightBracket();
		}

		private static void DumpDeclaration(DumpContext ctx)
		{
			TypeDeclarationDumper.DumpDeclaration(ctx);
		}

		private static void DumpConstants(DumpContext ctx)
		{
			if (!ctx.data.HasConstants) return;
			ctx.NewLine();

			FieldDumper.DumpConstants(ctx.data.Constants, ctx);
		}

		private static void DumpConstructors(DumpContext ctx)
		{
			if (!ctx.data.HasConstructors) return;
			ctx.NewLine();

			foreach (var ci in ctx.data.Constructors)
			{
				MethodDumper.DumpConstructor(ci, ctx);
			}
		}

		private static void DumpFinalizer(DumpContext ctx)
		{
			if (!ctx.data.IsExplicitFinalizer) return;
			ctx.NewLine();

			MethodDumper.DumpFinalizer(ctx);
		}

		private static void DumpStaticEvents(DumpContext ctx)
		{
			if (!ctx.data.HasStaticEvents) return;
			ctx.NewLine();

			foreach (var fi in ctx.data.StaticEvents)
			{
				FieldDumper.DumpField(fi, ctx, true);
			}
		}

		private static void DumpStaticProperties(DumpContext ctx)
		{
			if (!ctx.data.HasStaticProperties) return;
			ctx.NewLine();

			foreach (var pi in ctx.data.StaticProperties)
			{
				PropertyDumper.DumpProperty(pi, ctx);
			}
		}

		private static void DumpStaticMethods(DumpContext ctx)
		{
			if (!ctx.data.HasStaticMethods) return;
			ctx.NewLine();

			foreach (var mi in ctx.data.StaticMethods)
			{
				MethodDumper.DumpMethod(mi, ctx, false);
			}
		}

		
		private static void DumpStaticFields(DumpContext ctx)
		{
			if (!ctx.data.HasStaticFields) return;
			ctx.NewLine();

			foreach (var fi in ctx.data.StaticFields)
			{
				FieldDumper.DumpField(fi, ctx, false);
			}
		}

		private static void DumpInstanceEvents(DumpContext ctx)
		{
			if (!ctx.data.HasStaticEvents) return;
			ctx.NewLine();
			
			foreach (var fi in ctx.data.InstanceEvents)
			{
				FieldDumper.DumpField(fi, ctx, true);
			}
		}
		
		private static void DumpInstanceProperties(DumpContext ctx)
		{
			if (!ctx.data.HasInstanceProperties) return;
			ctx.NewLine();
			
			foreach (var pi in ctx.data.InstanceProperties)
			{
				PropertyDumper.DumpProperty(pi, ctx);
			}
		}
		
		private static void DumpInstanceMethods(DumpContext ctx, bool IsInterface)
		{
			if (!ctx.data.HasInstanceMethods) return;
			ctx.NewLine();
			
			foreach (var mi in ctx.data.InstanceMethods)
			{
				MethodDumper.DumpMethod(mi, ctx, IsInterface);
			}
		}
		
		private static void DumpInstanceFields(DumpContext ctx)
		{
			if (!ctx.data.HasInstanceFields) return;
			ctx.NewLine();
			
			foreach (var fi in ctx.data.InstanceFields)
			{
				FieldDumper.DumpField(fi, ctx, false);
			}
		}
	}
}

