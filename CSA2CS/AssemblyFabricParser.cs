using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSA2CS
{
	public static class AssemblyFabricParser
	{
		// Not thread safe!
		public static List<TypeData> Parse(Assembly assembly)
		{
			var ret = new List<TypeData>();
			var types = assembly.GetTypes();
			foreach (var type in types)
			{
				if (type.IsPrimitive) continue;

				var data = Global.FindTypeData(type);
				if (null == data) 
				{
					data = new TypeData(type);
					Global.RegisterTypeData(data);
				}
				if (!data.IsInited)
				{
					if (data.Type.IsEnum)
					{
						ParseEnum(data);
					}
					else
					{
						ParseTypeFabric(data);
					}
				}

				if (!type.IsNested)
				{
					ret.Add(data);
				}
			}
			return ret;
		}

		private static List<FieldInfo> fieldList = new List<FieldInfo>();
		private static List<System.Type> typeList = new List<System.Type>();
		private static List<TypeData> typeDataList= new List<TypeData>();
		private static List<PropertyInfo> propertyList = new List<PropertyInfo>();
		private static List<MethodInfo> methodList = new List<MethodInfo>();

		private static void ParseEnum(TypeData data)
		{
			data.InitBasic();
		}

		private static void ParseTypeFabric(TypeData data)
		{
			var type = data.Type;
			var fields = type.GetFields(BindingFlags.DeclaredOnly |
			                            BindingFlags.Static |
			                            BindingFlags.Instance | 
			                            BindingFlags.Public |
			                            BindingFlags.NonPublic);

			var nestedTypes = type.GetNestedTypes(BindingFlags.NonPublic |
			                                      BindingFlags.Public);

			var properties = type.GetProperties(BindingFlags.Public |
			                                    BindingFlags.NonPublic |
			                                    BindingFlags.Instance | 
			                                    BindingFlags.Static |
			                                    BindingFlags.DeclaredOnly);

			var methods = type.GetMethods(BindingFlags.Public |
			                              BindingFlags.NonPublic |
			                              BindingFlags.Instance |
			                              BindingFlags.Static |
			                              BindingFlags.DeclaredOnly);

			var constructors = type.GetConstructors(BindingFlags.NonPublic | 
			                              BindingFlags.Public | 
			                              BindingFlags.Instance | 
			                              BindingFlags.DeclaredOnly);

			data.InitFull(GetConstants(fields),
			          GetNestedDelegates(nestedTypes),
			          GetStaticFields(fields),
			          GetInstanceFields(fields),
			          GetConstructors(constructors),
			          IsExplicitFinalizer(type),
			          GetStaticProperties(properties),
			          GetInstanceProperties(properties),
			          GetStaticMethods(methods),
			          GetInstanceMethods(methods),
			          GetStaticEvents(fields),
			          GetInstanceEvents(fields),
			          GetNestedTypes(nestedTypes));

			if (data.HasNestedType)
			{
				foreach (var nested in data.NestedTypes)
				{
					if (nested.IsInited)
					{
						ParseTypeFabric(nested);
					}
				}
			}
		}

		private static List<FieldInfo> GetConstants(FieldInfo[] fields)
		{
			return FieldFilter(fields, (fi)=>{
				return fi.IsLiteral;
			});
		}

		private static List<FieldInfo> GetStaticFields(FieldInfo[] fields)
		{
			return FieldFilter(fields, (fi)=>{
				return !fi.IsLiteral && fi.IsStatic && !TypeMetaHelper.IsEventField(fi);
			});
		}

		private static List<FieldInfo> GetInstanceFields(FieldInfo[] fields)
		{
			return FieldFilter(fields, (fi)=>{
				return !fi.IsLiteral && !fi.IsStatic && !TypeMetaHelper.IsEventField(fi);
			});
		}

		private static List<FieldInfo> GetStaticEvents(FieldInfo[] fields)
		{
			return FieldFilter(fields, (fi)=>{
				return !fi.IsLiteral && fi.IsStatic && TypeMetaHelper.IsEventField(fi);
			});
		}

		private static List<FieldInfo> GetInstanceEvents(FieldInfo[] fields)
		{
			return FieldFilter(fields, (fi)=>{
				return !fi.IsLiteral && !fi.IsStatic && TypeMetaHelper.IsEventField(fi);
			});
		}

		private static List<FieldInfo> FieldFilter(FieldInfo[] fields, System.Predicate<FieldInfo> filter)
		{
			var ret = fieldList;
			ret.Clear();
			
			foreach (var fi in fields)
			{
				if (Global.IGNORE_ANONYMOUS && TypeMetaHelper.IsAnonymousName(fi.Name)) continue;
				if (filter.Invoke(fi)) ret.Add(fi);
			}

			ret.Sort(FieldInfoComparer);
			return ret.Count > 0 ? ret.Clone<FieldInfo>() : null;
		}

		private static List<System.Type> GetNestedDelegates(System.Type[] nestedTypes)
		{
			var ret = typeList;
			ret.Clear();
			
			foreach (var nested in nestedTypes)
			{
				if (Global.IGNORE_ANONYMOUS && TypeMetaHelper.IsAnonymousName(nested.Name)) continue;
				if (TypeMetaHelper.IsDelegateType(nested))
				{
					ret.Add(nested);
				}
			}
			
			return ret.Count > 0 ? ret.Clone<System.Type>() : null;
		}

		private static List<TypeData> GetNestedTypes(System.Type[] nestedTypes)
		{
			var ret = typeDataList;
			ret.Clear();
			
			foreach (var nested in nestedTypes)
			{
				if (Global.IGNORE_ANONYMOUS && TypeMetaHelper.IsAnonymousName(nested.Name)) continue;
				if (TypeMetaHelper.IsDelegateType(nested)) continue;

				var data = Global.FindTypeData(nested);
				if (null == data)
				{
					data = new TypeData(nested);
					Global.RegisterTypeData(data);
				}
				ret.Add(data);
			}
			
			return ret.Count > 0 ? ret.Clone<TypeData>() : null;
		}

		private static List<ConstructorInfo> GetConstructors(ConstructorInfo[] constructors)
		{
			if (constructors.Length <= 0) return null;
			var ret = new List<ConstructorInfo>();
			ret.AddRange(constructors);
			ret.Sort(ConstructorInfoComparer);
			return ret;
		}

		private static bool IsExplicitFinalizer(System.Type type)
		{
			var mi = type.GetMethod(Consts.FINALIZER_METHOD_NAME, 
			                        BindingFlags.NonPublic |
			                        BindingFlags.Instance |
			                        BindingFlags.DeclaredOnly);
			return null != mi;
		}

		private static List<PropertyInfo> GetStaticProperties(PropertyInfo[] properties)
		{
			var ret = propertyList;
			ret.Clear();

			foreach (var pi in properties)
			{
				if (pi.CanRead)
				{
					if (pi.GetGetMethod(true).IsStatic)
					{
						ret.Add(pi);
					}
				}
				else if (pi.CanWrite)
				{
					if (pi.GetSetMethod(true).IsStatic)
					{
						ret.Add(pi);
					}
				}
			}

			ret.Sort(PropertyInfoComparer);
			return ret.Count > 0 ? ret.Clone<PropertyInfo>() : null;
		}

		private static List<PropertyInfo> GetInstanceProperties(PropertyInfo[] properties)
		{
			var ret = propertyList;
			ret.Clear();
			
			foreach (var pi in properties)
			{
				if (pi.CanRead)
				{
					if (!pi.GetGetMethod(true).IsStatic)
					{
						ret.Add(pi);
					}
				}
				else if (pi.CanWrite)
				{
					if (!pi.GetSetMethod(true).IsStatic)
					{
						ret.Add(pi);
					}
				}
			}

			ret.Sort(PropertyInfoComparer);
			return ret.Count > 0 ? ret.Clone<PropertyInfo>() : null;
		}

		private static List<MethodInfo> GetStaticMethods(MethodInfo[] methods)
		{
			var ret = methodList;
			ret.Clear();

			foreach (var mi in methods)
			{
				if (TraitHelper.IsSpecialMethod(mi)) continue;
				if (Global.IGNORE_ANONYMOUS && TypeMetaHelper.IsAnonymousName(mi.Name)) continue;
				if (mi.IsStatic)
				{
					ret.Add(mi);
				}
			}

			ret.Sort(MethodInfoComparer);
			return ret.Count > 0 ? ret.Clone<MethodInfo>() : null;
		}

		private static List<MethodInfo> GetInstanceMethods(MethodInfo[] methods)
		{
			var ret = methodList;
			ret.Clear();
			
			foreach (var mi in methods)
			{
				if (TraitHelper.IsSpecialMethod(mi)) continue;
				if (mi.IsStatic) continue;
				if (Global.IGNORE_ANONYMOUS && TypeMetaHelper.IsAnonymousName(mi.Name)) continue;

				ret.Add(mi);
			}

			ret.Sort(MethodInfoComparer);
			return ret.Count > 0 ? ret.Clone<MethodInfo>() : null;
		}

		public static List<T> Clone<T>(this List<T> list)
		{
			var ret = new List<T>();
			ret.AddRange(list);
			return ret;
		}

		private static int FieldInfoComparer(FieldInfo lhr, FieldInfo rhr)
		{
			var p1 = FieldPriority(lhr);
			var p2 = FieldPriority(rhr);
			return p1 > p2 ? -1 : 1;
		}

		private static int MethodInfoComparer(MethodInfo lhr, MethodInfo rhr)
		{
			var p1 = MethodPriority(lhr);
			var p2 = MethodPriority(rhr);
			return p1 > p2 ? -1 : 1;
		}

		private static int PropertyInfoComparer(PropertyInfo lhr, PropertyInfo rhr)
		{
			int p1 = 0;
			int p2 = 0;
			if (lhr.CanRead)
				p1 = MethodPriority(lhr.GetGetMethod(true));
			else if (lhr.CanWrite)
				p1 = MethodPriority(lhr.GetSetMethod(true));

			if (rhr.CanRead)
				p2 = MethodPriority(rhr.GetGetMethod(true));
			else if (rhr.CanWrite)
				p2 = MethodPriority(rhr.GetSetMethod(true));

			return p1 > p2 ? -1 : 1;
		}

		private static int ConstructorInfoComparer(ConstructorInfo lhr, ConstructorInfo rhr)
		{
			var p1 = ConstructorPriority(lhr);
			var p2 = ConstructorPriority(rhr);
			return p1 > p2 ? -1 : 1;
		}

		private static int FieldPriority(FieldInfo info)
		{
			var ret = 0;
			if (PrivacyHelper.IsPublic(info)) ret += 5000;
			else if (PrivacyHelper.IsInternal(info)) ret += 4000;
			else if (PrivacyHelper.IsProtected(info)) ret += 3000;
			else if (PrivacyHelper.IsProtectedInternal(info)) ret += 2000;
			else if (PrivacyHelper.IsPrivate(info)) ret += 1000;

			return ret;
		}

		private static int MethodPriority(MethodInfo info)
		{
			var ret = 0;
			if (PrivacyHelper.IsPublic(info)) ret += 500;
			else if (PrivacyHelper.IsInternal(info)) ret += 400;
			else if (PrivacyHelper.IsProtected(info)) ret += 300;
			else if (PrivacyHelper.IsProtectedInternal(info)) ret += 200;
			else if (PrivacyHelper.IsPrivate(info)) ret += 100;

			if (TraitHelper.IsStatic(info)) ret += 5000;
			else if (TraitHelper.IsAbstract(info)) ret += 4000;
			else if (TraitHelper.IsVirtualNoneOverride(info)) ret += 3000;
			else if (TraitHelper.IsOverrideNoneSealed(info)) ret += 2000;
			else if (TraitHelper.IsOverrideSealed(info)) ret += 1000;

			return ret;
		}

		private static int ConstructorPriority(ConstructorInfo info)
		{
			var ret = 0;
			if (PrivacyHelper.IsPublic(info)) ret += 5000;
			else if (PrivacyHelper.IsInternal(info)) ret += 4000;
			else if (PrivacyHelper.IsProtected(info)) ret += 3000;
			else if (PrivacyHelper.IsProtectedInternal(info)) ret += 2000;
			else if (PrivacyHelper.IsPrivate(info)) ret += 1000;
			
			return ret;
		}
	}
}

