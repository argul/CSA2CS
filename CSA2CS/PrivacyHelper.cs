using System;
using System.Reflection;

namespace CSA2CS
{
	public static class PrivacyHelper
	{
		#region Type
		public static bool IsPublic(System.Type type)
		{
			return type.IsPublic || type.IsNestedPublic;
		}

		public static bool IsProtected(System.Type type)
		{
			return type.IsNestedFamily;
		}

		public static bool IsProtectedInternal(System.Type type)
		{
			return type.IsNestedFamORAssem;
		}

		public static bool IsInternal(System.Type type)
		{
			return type.IsNestedAssembly;
		}

		public static bool IsPrivate(System.Type type)
		{
			return type.IsNestedPrivate;
		}
		#endregion

		#region MethodInfo
		public static bool IsPublic(MethodInfo info)
		{
			return info.IsPublic;
		}

		public static bool IsProtected(MethodInfo info)
		{
			return info.IsFamily;
		}

		public static bool IsProtectedInternal(MethodInfo info)
		{
			return info.IsFamilyOrAssembly;
		}

		public static bool IsInternal(MethodInfo info)
		{
			return info.IsAssembly;
		}

		public static bool IsPrivate(MethodInfo info)
		{
			return info.IsPrivate;
		}
		#endregion

		#region FieldInfo
		public static bool IsPublic(FieldInfo info)
		{
			return info.IsPublic;
		}
		
		public static bool IsProtected(FieldInfo info)
		{
			return info.IsFamily;
		}
		
		public static bool IsProtectedInternal(FieldInfo info)
		{
			return info.IsFamilyOrAssembly;
		}
		
		public static bool IsInternal(FieldInfo info)
		{
			return info.IsAssembly;
		}
		
		public static bool IsPrivate(FieldInfo info)
		{
			return info.IsPrivate;
		}
		#endregion

		#region MethodInfo
		public static bool IsPublic(ConstructorInfo info)
		{
			return info.IsPublic;
		}
		
		public static bool IsProtected(ConstructorInfo info)
		{
			return info.IsFamily;
		}
		
		public static bool IsProtectedInternal(ConstructorInfo info)
		{
			return info.IsFamilyOrAssembly;
		}
		
		public static bool IsInternal(ConstructorInfo info)
		{
			return info.IsAssembly;
		}
		
		public static bool IsPrivate(ConstructorInfo info)
		{
			return info.IsPrivate;
		}
		#endregion
	}
}

