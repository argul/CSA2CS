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
			Debug.Log("Load Assembly File : " + path);
			var ret = Assembly.LoadFile(path);
			var refNames = ret.GetReferencedAssemblies();
			if (refNames.Length > 0)
			{
				foreach (var an in refNames)
				{
					var name = an.Name;
					if (reference.ContainsKey(name)) continue;
					var refPath = Path.Combine(Path.GetDirectoryName(path), name);
					if (File.Exists(refPath))
					{
						var refAssembly = LoadAssembly(refPath, ref reference);
						reference.Add(name, refAssembly);
					}
				}
			}

			return ret;
		}

		public static Assembly LoadAssembly(byte[] data)
		{
			return Assembly.Load(data);
		}
	}
}