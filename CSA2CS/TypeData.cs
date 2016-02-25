using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSA2CS
{
	public class TypeData
	{
		public TypeData(System.Type type) { this.type = type; }

		protected bool isInited = false;
		protected System.Type type;
		protected string nameStr;
		protected string fullNameStr;
		protected string fullNameStrNoNamespace;
		protected string namespaceStr;

		public bool IsInited { get { return isInited; } }
		public System.Type Type { get { return type; } }
		public string UUID { get { return fullNameStr; } }
		public string Name { get { return nameStr; } }
		public string Namespace { get { return namespaceStr; } }
		public string FullName { get { return fullNameStr; } }
		public string FullNameNoNamespace { get { return fullNameStrNoNamespace; } }

		protected List<System.Type> nestedDelegates;
		public List<System.Type> NestedDelegates { get { return nestedDelegates; } }
		public bool HasNestedDelegate { get { return null != NestedDelegates; } }

		protected List<TypeData> nestedTypes;
		public List<TypeData> NestedTypes { get { return nestedTypes; } }
		public bool HasNestedType { get { return null != nestedTypes; } }

		public void InitFull(List<FieldInfo> constants,
		                     List<System.Type> nestedDelegates,
		                     List<MethodInfo> specialMethods,
		                     List<FieldInfo> staticFields,
		                     List<FieldInfo> instanceFields,
		                     List<ConstructorInfo> constructors,
		                     bool isExplicitFinalizer,
		                     List<PropertyInfo> staticProperties,
		                     List<PropertyInfo> instanceProperties,
		                     List<MethodInfo> staticMethods,
		                     List<MethodInfo> instanceMethods,
		                     List<FieldInfo> staticEvents,
		                     List<FieldInfo> instanceEvents,
		                     List<TypeData> nestedTypes)
		{
			this.constants = constants;
			this.nestedDelegates = nestedDelegates;
			this.specialMethods = specialMethods;
			this.staticFields = staticFields;
			this.instanceFields = instanceFields;
			this.constructors = constructors;
			this.isExplicitFinalizer = isExplicitFinalizer;
			this.staticProperties = staticProperties;
			this.instanceProperties = instanceProperties;
			this.staticEvents = staticEvents;
			this.instanceEvents = instanceEvents;
			this.staticMethods = staticMethods;
			this.instanceMethods = instanceMethods;
			this.nestedTypes = nestedTypes;

			InitBasic();
		}

		public void InitBasic()
		{
			nameStr = TypeMetaHelper.GetTypeDeclarationName(type);
			namespaceStr = String.IsNullOrEmpty(type.Namespace) ? String.Empty : type.Namespace;
			fullNameStrNoNamespace = nameStr;
			if (type.IsNested)
			{
				var nested = type;
				while (nested.IsNested)
				{
					nested = nested.DeclaringType;
					fullNameStrNoNamespace = TypeMetaHelper.GetTypeDeclarationName(nested) + "." + fullNameStrNoNamespace;
				}
			}
			fullNameStr = String.IsNullOrEmpty(namespaceStr) ? fullNameStrNoNamespace : (namespaceStr + "." + fullNameStrNoNamespace);

			Debug.Log("Init TypeData : " + type.Name + " ==> " + fullNameStr, Debug.DEBUG_LEVEL_LOG);

			isInited = true;
		}

		protected List<FieldInfo> constants;
		public List<FieldInfo> Constants { get { return constants; } }
		public bool HasConstants { get { return null != constants; } }

		protected List<FieldInfo> staticFields;
		public List<FieldInfo> StaticFields { get { return staticFields; } }
		public bool HasStaticFields { get { return null != staticFields; } }

		protected List<FieldInfo> instanceFields;
		public List<FieldInfo> InstanceFields { get { return instanceFields; } }
		public bool HasInstanceFields { get { return null != instanceFields; } }

		protected List<MethodInfo> specialMethods;
		public List<MethodInfo> SpecialMethods { get { return specialMethods; } }
		public bool HasSpecialMethods { get { return null != specialMethods; } }

		protected List<ConstructorInfo> constructors = new List<ConstructorInfo>();
		public List<ConstructorInfo> Constructors { get { return constructors; } }
		public bool HasConstructors { get { return null != constructors; } }

		protected bool isExplicitFinalizer = false;
		public bool IsExplicitFinalizer { get { return isExplicitFinalizer; } }

		protected List<PropertyInfo> staticProperties;
		public List<PropertyInfo> StaticProperties { get { return staticProperties; } }
		public bool HasStaticProperties { get { return null != staticProperties; } }

		protected List<PropertyInfo> instanceProperties;
		public List<PropertyInfo> InstanceProperties { get { return instanceProperties; } }
		public bool HasInstanceProperties { get { return null != instanceProperties; } }

		protected List<MethodInfo> staticMethods;
		public List<MethodInfo> StaticMethods { get { return staticMethods; } }
		public bool HasStaticMethods { get { return null != staticMethods; } }

		protected List<MethodInfo> instanceMethods;
		public List<MethodInfo> InstanceMethods { get { return instanceMethods; } }
		public bool HasInstanceMethods { get { return null != instanceMethods; } }

		protected List<FieldInfo> staticEvents;
		public List<FieldInfo> StaticEvents { get { return staticEvents; } }
		public bool HasStaticEvents { get { return null != staticEvents; } }

		protected List<FieldInfo> instanceEvents;
		public List<FieldInfo> InstanceEvents { get { return instanceEvents; } }
		public bool HasInstanceEvents { get { return null != instanceEvents; } }
	}
}