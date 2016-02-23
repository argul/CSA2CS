using System;
using System.Text;

namespace CSA2CS
{
	public static class Utils
	{
		// TODO : memory pool
		public static CodeBuilder GraspBuilder()
		{
			return new CodeBuilder();
		}

		public static void DropBuilder(CodeBuilder builder)
		{
		}
	}

	public class CodeBuilder
	{
		public CodeBuilder()
		{
			substance = new StringBuilder();
		}
		public StringBuilder substance;

		public void Indent(int indent)
		{
			substance.Append(Indenter.GetIndentStr(indent));
		}

		public string GetString(bool flush = true)
		{
			var ret = substance.ToString();
			if (flush) substance.Length = 0;
			return ret;
		}
	}
}