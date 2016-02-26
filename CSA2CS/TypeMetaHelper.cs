using System;
using System.Reflection;

namespace CSA2CS
{
	public static class TypeMetaHelper
	{
		public static bool IsDelegateType(System.Type type)
		{
			return type.BaseType == typeof(MulticastDelegate);
		}

		public static bool IsEventField(FieldInfo info)
		{
			// Seems mono is buggy on this, returns MemberTypes.Field for event.
			return info.MemberType == MemberTypes.Event;
		}

		public static bool IsDelegateField(FieldInfo info)
		{
			return IsDelegateType(info.FieldType);
		}

		public static bool IsCompilerGenerated(System.Type type)
		{
			var attrs = type.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
			return (null != attrs && attrs.Length > 0);
		}

		public static bool IsCompilerGenerated(MemberInfo info)
		{
			var attrs = info.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
			return (null != attrs && attrs.Length > 0);
		}

		public static string GetTypeDeclarationName(System.Type type)
		{
			if (type.IsArray)
			{
				return GetTypeDeclarationName(type.GetElementType()) + "[]";
			}
			else if (type.IsGenericParameter)
			{
				return type.Name;
			}
			else if (!type.IsGenericType)
			{
				return type.Name;
			}
			else
			{
				var rawName = type.Name;
				if (rawName.Contains("`"))
				{
					var typeArguments = type.GetGenericArguments();
					var strArr = new string[typeArguments.Length];
					for (int i = 0; i < typeArguments.Length; i++)
					{
						strArr[i] = GetTypeDeclarationName(typeArguments[i]);
					}
					var ret = type.Name.Remove(type.Name.IndexOf('`')) + "<" + string.Join(", ", strArr) + ">";
					return ret;
				}
				else
				{
					return type.Name;
				}
			}
		}

		public static string GetTypeUsageName(System.Type type, DumpContext ctx)
		{
			if (type.IsByRef)
			{
				return GetTypeUsageName(type.GetElementType(), ctx);
			}
			else if (type.IsArray)
			{
				return GetTypeUsageName(type.GetElementType(), ctx) + "[]";
			}
			else if (type.IsGenericParameter)
			{
				return type.Name;
			}
			else if (TypeMetaHelper.IsBuiltInType(type))
			{
				var ns = String.IsNullOrEmpty(type.Namespace) ? String.Empty : type.Namespace;
				if (!type.IsNested)
				{
					return ns + "." + TypeMetaHelper.GetTypeDeclarationName(type);
				}
				else
				{
					var prefix = "";
					var nested = type;
					while (nested.IsNested)
					{
						nested = nested.DeclaringType;
						prefix = TypeMetaHelper.GetTypeUsageName(nested, ctx) + "." + prefix;
					}
					return ns + "." + prefix + TypeMetaHelper.GetTypeDeclarationName(type);
				}
			}
			else
			{
				var data = Global.FindTypeData(type);
				Assert.AssertIsTrue(null != data, "Type data not found! : " + type.FullName);
				Assert.AssertIsTrue(data.IsInited, "Uninited Type : " + type.FullName);
				if (String.IsNullOrEmpty(ctx.enclosingNamespace) || String.IsNullOrEmpty(data.Namespace))
					return data.FullName;
				else if (String.Equals(ctx.enclosingNamespace, data.Namespace))
					return data.FullNameNoNamespace;
				else
					return data.FullName;
			}
		}

		private static bool IsBuiltInType(System.Type type)
		{
			return !String.IsNullOrEmpty(type.Namespace) && type.Namespace.StartsWith("System");
		}
	}
}

