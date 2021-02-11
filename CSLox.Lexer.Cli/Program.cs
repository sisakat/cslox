using System;
using System.Collections.Generic;
using System.IO;

namespace CSLox.Lexer.Cli
{
    class Program
    {
        static bool hadError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                Environment.Exit(64);
            } else if (args.Length == 1) 
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path) 
        {
            string input = File.ReadAllText(path);
            Run(input);
            if (hadError) Environment.Exit(65);
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                Run(line);
                hadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);

            try
            {
                IEnumerable<Token> tokens = scanner.ScanTokens();

                foreach (Token token in tokens)
                {
                    Console.WriteLine(token);
                }
            } catch (ScanningException ex)
            {
                Error(ex.Line, ex.Message);
            }
        }

        private static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error{where}: {message}");
            hadError = true;
        }
    }
}
