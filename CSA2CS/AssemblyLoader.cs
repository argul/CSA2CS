using System;
using System.Reflection;
using System.IO;

namespace CSA2CS
{
	public static class AssemblyLoader
	{
		public static Assembly LoadAssembly(string path)
		{
			return Assembly.LoadFile(path);
		}

		public static Assembly LoadAssembly(byte[] data)
		{
			return Assembly.Load(data);
		}
	}
}