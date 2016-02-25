using System;
using System.Reflection;

namespace CSA2CS
{
	public static class TraitHelper
	{
		#region Type
		public static bool IsStatic(System.Type type)
		{
			return type.IsSealed && type.IsAbstract;
		}

		public static bool IsAbstract(System.Type type)
		{
			return type.IsAbstract;
		}

		public static bool IsSealed(System.Type type)
		{
			return type.IsSealed;
		}
		#endregion

		#region MethodInfo
		public static bool IsStatic(MethodInfo info)
		{
			return info.IsStatic;
		}

		public static bool IsAbstract(MethodInfo info)
		{
			return info.IsAbstract;
		}

		public static bool IsVirtualNoneOverride(MethodInfo info)
		{
			return info.IsVirtual && info.GetBaseDefinition().DeclaringType == info.DeclaringType;
		}

		public static bool IsOverrideSealed(MethodInfo info)
		{
			return info.IsVirtual && info.GetBaseDefinition().DeclaringType != info.DeclaringType && info.IsFinal;
		}

		public static bool IsOverrideNoneSealed(MethodInfo info)
		{
			return info.IsVirtual && info.GetBaseDefinition().DeclaringType != info.DeclaringType && !info.IsFinal;
		}

		public static bool IsSpecialMethod(MethodInfo info)
		{
			if (info.IsSpecialName) return true;
			if (info.Name == Consts.FINALIZER_METHOD_NAME) return true;
			return false;
		}

		// https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.extensionattribute.aspx
		public static bool IsExtensionMethod(MethodInfo info)
		{
			var attrs = info.GetCustomAttributes(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false);
			return (null != attrs && attrs.Length > 0);
		}
		#endregion

		#region FieldInfo
		public static bool IsStatic(FieldInfo info)
		{
			return info.IsStatic;
		}
		#endregion
	}
}

