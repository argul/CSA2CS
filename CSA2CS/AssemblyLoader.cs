using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace CSA2CS
{
	public static class AssemblyLoader
	{
		public static Assembly LoadAssembly(string path, ref Dictionary<string, Assembly> reference)
		{
			Debug.Log("Load Assembly File : " + path, Debug.DEBUG_LEVEL_LOG);
			var ret = Assembly.LoadFile(path);
			var refNames = ret.GetReferencedAssemblies();
			if (refNames.Length > 0)
			{
				foreach (var an in refNames)
				{
					DumpAssemblyNameInfo(an, Debug.DEBUG_LEVEL_VERBOSE);
					var name = an.Name;
					if (reference.ContainsKey(name)) continue;
					var refPath = Path.Combine(Path.GetDirectoryName(path), name + ".dll");
					if (File.Exists(refPath))
					{
						reference.Add(name, null);
						var refAssembly = LoadAssembly(refPath, ref reference);
						reference[name] = refAssembly;
					}
					else
					{
						Debug.Log("Referenced Assembly Not Found : " + refPath, Debug.DEBUG_LEVEL_ERROR);
					}
				}
			}

			return ret;
		}

		public static Assembly LoadAssembly(byte[] data)
		{
			return Assembly.Load(data);
		}

		private static void DumpAssemblyNameInfo(AssemblyName assemblyName, int debugLevel)
		{
			Debug.Log("Dump AssemblyName Info : " + assemblyName.Name, debugLevel);
			var properties = assemblyName.GetType().GetProperties(BindingFlags.Public |
			                                                      BindingFlags.Instance);
			foreach (var p in properties)
			{
				Debug.Log(p.Name + " = " + p.GetValue(assemblyName, new object[0]), debugLevel);
			}
		}
	}
}