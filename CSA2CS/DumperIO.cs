using System;
using System.IO;

namespace CSA2CS
{
	public static class DumperIO
	{
		public static void WriteFile(string path, string content)
		{
			var dirName = Path.GetDirectoryName(path);
			if (!Directory.Exists(dirName))
			{
				Directory.CreateDirectory(dirName);
			}
			Assert.AssertIsTrue(!File.Exists(path));
			File.WriteAllText(path, content);
		}
	}
}

