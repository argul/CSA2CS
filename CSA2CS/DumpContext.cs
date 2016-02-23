using System;
namespace CSA2CS
{
	public class DumpContext
	{
		public TypeData data;
		public CodeBuilder builder;
		public int indentLevel;
		public string enclosingNamespace;
		private bool swallowNewline = false;
		public bool SwallowNewline { set { swallowNewline = value; } }

		public void Push(string s)
		{
			builder.substance.Append(s);
		}

		public void Push(char c)
		{
			builder.substance.Append(c);
		}

		public void NewLine()
		{
			if (swallowNewline)
			{
				swallowNewline = false;
			}
			else
			{
				Push(Consts.NEWLINE);
				builder.Indent(indentLevel);
			}
		}
		
		public void LeftBracket()
		{
			Push(Consts.NEWLINE);
			builder.Indent(indentLevel);
			Push('{');
			indentLevel++;
		}
		
		public void RightBracket()
		{
			indentLevel--;
			Push(Consts.NEWLINE);
			builder.Indent(indentLevel);
			Push('}');
		}
	}
}