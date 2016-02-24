using System;

namespace CSA2CS
{
	public static class ValueDumper
	{
		public static void Dump(System.Type type, object value, DumpContext ctx)
		{
			if (type.IsEnum)
			{
				Assert.AssertIsTrue(null != value);
				ctx.Push(value.ToString());
			}
			else if (type.IsPrimitive)
			{
				if (type == typeof(char))
				{
					ctx.Push('\'');
					ctx.Push(value.ToString());
					ctx.Push('\'');
				}
				else if (type == typeof(float))
				{
					ctx.Push(value.ToString());
					ctx.Push('f');
				}
				else if (type == typeof(double))
				{
					ctx.Push(value.ToString());
					ctx.Push('d');
				}
				else
				{
					ctx.Push(value.ToString());
				}
			}
			else if (type == typeof(string))
			{
				if (null == value)
				{
					ctx.Push(Consts.KEYWORD_NULL);
				}
				else
				{
					ctx.Push('\"');
					ctx.Push(value.ToString());
					ctx.Push('\"');
				}
			}
			else if (type.IsValueType)
			{
				ctx.Push("default(");
				ctx.Push(TypeMetaHelper.GetTypeUsageName(type, ctx));
				ctx.Push(')');
			}
			else
			{
				ctx.Push(Consts.KEYWORD_NULL);
			}
		}
	}
}

