using System;
using System.Text;

namespace CSA2CS
{
	public static class Utils
	{
		public static void StartUp()
		{
			Global.BuilderPool.RegisterCreator(CodeBuilder.POOL_TOKEN, ()=>{
				return new CodeBuilder();
			});
		}
		public static CodeBuilder GraspBuilder()
		{
			return (CodeBuilder)Global.BuilderPool.Spawn(CodeBuilder.POOL_TOKEN);
		}

		public static void DropBuilder(CodeBuilder builder)
		{
			Global.BuilderPool.Recycle(builder);
		}
	}

	public class CodeBuilder : IPoolUser
	{
		public const string POOL_TOKEN = "CodeBuilder";
		#region interface
		public string Token { get { return POOL_TOKEN; } }
		public void OnSpawn() { }
		public void OnRecycle() { substance.Length = 0; }
		#endregion

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