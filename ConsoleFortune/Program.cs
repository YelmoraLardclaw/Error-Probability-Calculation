using System;
using System.IO;
using yLibrary.Voronoi;
using yLibrary.Voronoi.Formatter;

namespace ConsoleFortune
{
    class Program
    {
        private static string arguments;

        static void Main(string[] args)
        {
            foreach (string s in args)
                Console.WriteLine(s);

            if (args.Length == 2)
                Execute(args[0], args[1]);
            else if (args.Length == 0)
                ShowHelp();
            else
                ShowError();
        }

        private static void Execute(string inputFile, string outputFile)
        {
            TimeSpan workTime = TimeSpan.MinValue;
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Input file does not exist.");
                return;
            }

            try
            {
                AlyshevReader reader = new AlyshevReader(inputFile);
                Site[] sites = reader.Read();
                Fortune fortuneCalculator = new Fortune(sites);
                VoronoiDiagram diagram = fortuneCalculator.Calculate();
                AlyshevWriter writer = new AlyshevWriter(outputFile, "G");
                writer.Write(diagram);
            }
            catch (Exception Ex)
            {
                Console.Beep(1000, 600);
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                Console.ReadKey();
            }
        }

        private static void ShowHelp()
        {
            throw new NotImplementedException("Show help is not yet implemented.");
        }

        private static void ShowError()
        {
            throw new NotImplementedException("Show error is not yet implemented.");
        }
    }
}
