using System;

namespace CSA2CS
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length != 2)
			{
				PrintManual();
			}

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

		private static void PrintManual()
		{
			Console.WriteLine("TODO : print manual");
		}
	}
}