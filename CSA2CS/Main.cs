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
				return;
			}

			ParseArgs(args);

			StartUp();

			PerformDump(args);
		}

		private static void PrintManual()
		{
			Console.WriteLine("TODO : print manual");
		}

		private static void ParseArgs(string[] args)
		{
			int debugLevel = 0;
			if (args.Length >= 3 && int.TryParse(args[2], out debugLevel))
			{
				Debug.DEBUG_LEVEL = debugLevel;
			}
		}

		private static void StartUp()
		{
			Utils.StartUp();
		}

		private static void PerformDump(string[] args)
		{
			var worker = new DumpWorker(args[0], args[1]);
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
	}
}