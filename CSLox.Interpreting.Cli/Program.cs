﻿using System;
using CSLox.Lexer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSLox.Parsing;

namespace CSLox.Interpreting.Cli
{
  class Program
  {
    static Interpreter interpreter = new Interpreter();
    static bool hadError = false;

    static void Main(string[] args)
    {
      if (args.Length > 1)
      {
        Console.WriteLine("Usage: cslox [script]");
        System.Environment.Exit(64);
      }
      else if (args.Length == 1)
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
      if (hadError) System.Environment.Exit(65);
    }

    private static void RunPrompt()
    {
      while (true)
      {
        Console.Write("> ");
        string line = Console.ReadLine();
        if (string.IsNullOrEmpty(line)) break;
        Run(line, true);
        hadError = false;
      }
    }

    private static void Run(string source, bool repl = false)
    {
      Scanner scanner = new Scanner(source);

      try
      {
        // Scan (lexing) tokens
        var tokens = scanner.ScanTokens();

        // Parse tokens into an AST
        var parser = new Parser(tokens.ToList());
        parser.OnError += (token, message) =>
        {
          Error(token.Line, message);
        };

        var statements = parser.Parse();

        // Convert single expressions into print statements (for REPL)
        if (repl && statements.Count == 1)
        {
          var statement = statements[0];
          if (statement is Stmt.Expression)
          {
            statements.RemoveAt(0);
            statements.Add(new Stmt.Print(((Stmt.Expression)statement).Expr));
          }
        }

        // Semantic analysis for variables and scopes
        var resolver = new Resolver(interpreter);
        resolver.Resolve(statements);

        // Interpret and run
        interpreter.Interpret(statements);
      }
      catch (ScanningException ex)
      {
        Error(ex.Line, ex.Message);
      }
      catch (ParsingException ex)
      {
        Error(ex.Token, ex.Message);
      }
      catch (InterpretingException ex)
      {
        Error(ex.Token, ex.Message);
      }
    }

    private static void Error(int line, string message)
    {
      Report(line, "", message);
    }

    private static void Error(Token token, string message)
    {
      if (token.Type == TokenType.EOF)
      {
        Report(token.Line, " at end", message);
      }
      else
      {
        Report(token.Line, $" at '{token.Lexeme}'", message);
      }
    }

    private static void Report(int line, string where, string message)
    {
      Console.WriteLine($"[line {line}] Error{where}: {message}");
      hadError = true;
    }
  }
}
