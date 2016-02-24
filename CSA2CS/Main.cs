using System;

namespace CSA2CS
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length < 2)
			{
				PrintManual();
			}

			var worker = new DumpWorker(args[0], args[1]);
			int debugLevel = 0;
			if (args.Length >= 3 && int.TryParse(args[2], out debugLevel))
			{
				Global.DEBUG_LEVEL = debugLevel;
			}

			string err = "";
			if (!worker.CheckProc(out err))
			{
				Console.WriteLine("Error : " + err);
			}
			else if (worker.WorkProc())
			{
				Console.WriteLine("Dump Success!");
			}
		}

		private static void PrintManual()
		{
			Console.WriteLine("TODO : print manual");
		}
	}
}