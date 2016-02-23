using System;
using System.Reflection;

namespace CSA2CS
{
	public static class MethodParameterDumper
	{
		public static void DumpParameter(ParameterInfo pi, DumpContext ctx)
		{
			if (pi.IsOut) ctx.Push(Consts.KEYWORD_OUT);
			else if (pi.ParameterType.IsByRef) ctx.Push(Consts.KEYWORD_REF);
			else if (pi.IsDefined(typeof(ParamArrayAttribute), false)) ctx.Push(Consts.KEYWORD_PARAMS);
			
			ctx.Push(TypeMetaHelper.GetTypeUsageName(pi.ParameterType, ctx));
			ctx.Push(' ');
			ctx.Push(pi.Name);
			if (pi.IsOptional)
			{
				ctx.Push(" = ");
				var value = pi.RawDefaultValue;
				if (null == value)
				{
					ctx.Push(Consts.KEYWORD_NULL);
				}
				else
				{
					var defaultValueType = value.GetType();
					if (defaultValueType.IsEnum)
					{
						ctx.Push(Enum.GetName(value.GetType(), value));
					}
					else if (defaultValueType.IsPrimitive)
					{
						ctx.Push(value.ToString());
					}
					else if (defaultValueType.IsValueType)
					{
						ctx.Push("default(");
						ctx.Push(TypeMetaHelper.GetTypeUsageName(defaultValueType, ctx));
						ctx.Push(")");
					}
					else
					{
						ctx.Push(Consts.KEYWORD_NULL);
					}
				}
			}
		}
	}
}

