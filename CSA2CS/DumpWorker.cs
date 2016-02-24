using System;
using System.Reflection;
using System.IO;

namespace CSA2CS
{
	public class DumpWorker
	{
		protected string outputRootPath;
		protected string inputPath;
		public DumpWorker (string inputPath, string outputRootPath)
		{
			this.inputPath = inputPath;
			this.outputRootPath = outputRootPath;
		}

		public bool CheckProc(out string err)
		{
			err = "";
			if (!File.Exists(inputPath))
			{
				err = "Input file not exist!";
				return false;
			}
			if (Directory.Exists(outputRootPath))
			{
				var entries = Directory.EnumerateFileSystemEntries(outputRootPath);
				using (var enumerator = entries.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						err = "Output directory is NOT empty!";
						return false;
					}
				}
			}
			return true;
		}

		public bool WorkProc()
		{
			try
			{
				var reference = new System.Collections.Generic.Dictionary<string, Assembly>();
				var assemby = AssemblyLoader.LoadAssembly(inputPath, ref reference);
				foreach (var kvp in reference)
				{
					AssemblyFabricParser.Parse(kvp.Value);
				}
				var entries = AssemblyFabricParser.Parse(assemby);

				foreach (var e in entries)
				{
					var ns = e.Namespace;
					if (String.IsNullOrEmpty(ns))
					{
						var fileName = FileCodeDumper.GetFileName(e.Name, ".cs");
						Debug.Log("Dump Code File : " + e.Type.Name + " ==> " + fileName, Debug.DEBUG_LEVEL_LOG);

						var content = FileCodeDumper.DumpCode(e);
						DumperIO.WriteFile(GetFilePath(fileName), content);
					}
					else
					{
						var parts = ns.Split('.');
						var fileName = FileCodeDumper.GetFileName(e.FullNameNoNamespace, ".cs");
						Debug.Log("Dump Code File : " + e.Type.Name + " ==> " + fileName, Debug.DEBUG_LEVEL_LOG);

						var content = FileCodeDumper.DumpCode(e);
						DumperIO.WriteFile(GetFilePath(fileName, parts), content);
					}
				}

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine("ERROR OCCURS ============================================================");
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.WriteLine("=========================================================================");
				return false;
			}
		}

		private string GetFilePath(string fileName, string[] dirs = null)
		{
			if (null == dirs)
			{
				return Path.Combine(outputRootPath, fileName);
			}
			else
			{
				var ret = outputRootPath;
				foreach (var d in dirs)
				{
					ret = Path.Combine(ret, d);
				}
				ret = Path.Combine(ret, fileName);
				return ret;
			}
		}
	}
}

