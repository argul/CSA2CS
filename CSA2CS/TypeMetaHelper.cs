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

		/*http://stackoverflow.com/questions/23228075/determine-if-methodinfo-represents-a-lambda-expression*/
		public static bool IsAnonymousName(string name)
		{
			return !System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(name);
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
				var typeArguments = type.GetGenericArguments();
				var strArr = new string[typeArguments.Length];
				for (int i = 0; i < typeArguments.Length; i++)
				{
					strArr[i] = GetTypeDeclarationName(typeArguments[i]);
				}
				var ret = type.Name.Remove(type.Name.IndexOf('`')) + "<" + string.Join(", ", strArr) + ">";
				return ret;
			}
		}

		public static string GetTypeUsageName(System.Type type, DumpContext ctx)
		{
			if (type.IsArray)
			{
				return GetTypeUsageName(type.GetElementType(), ctx) + "[]";
			}
			if (type.IsGenericParameter)
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

