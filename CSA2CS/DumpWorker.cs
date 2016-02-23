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
				var assemby = AssemblyLoader.LoadAssembly(inputPath);
				var entries = AssemblyFabricParser.Parse(assemby);
				foreach (var e in entries)
				{
					var ns = e.Namespace;
					var content = FileCodeDumper.DumpCode(e);
					if (String.IsNullOrEmpty(ns))
					{
						var fileName = GetFileName(e.Type.Name, ".cs");
						DumperIO.WriteFile(GetFilePath(fileName), content);
					}
					else
					{
						var parts = ns.Split('.');
						var fileName = GetFileName(e.FullNameNoNamespace, ".cs");
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

		private static System.Text.StringBuilder sb = new System.Text.StringBuilder();
		private string GetFileName(string typeName, string extension = "")
		{
			foreach (var c in typeName.ToCharArray())
			{
				if (c == '+') sb.Append('_');
				else if (c == '.') sb.Append('_');
				else if (c == '<') sb.Append('_');
				else if (c == ',') sb.Append('_');
				else if (c == '>' || c == ' ') {}
				else sb.Append(c);
			}
			sb.Append(extension);
			var ret = sb.ToString();
			sb.Length = 0;
			return ret;
		}
	}
}

