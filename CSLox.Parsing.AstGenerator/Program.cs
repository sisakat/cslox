﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSLox.Parsing.AstGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(64);
            }

            string outputDirectory = args[0];

            DefineAst(outputDirectory, "Expr", new List<string>() {
                "Binary   : Expr left, Token oper, Expr right",
                "Grouping : Expr expression",
                "Literal  : Object value",
                "Unary    : Token oper, Expr right",
                "Variable : Token name"
            });

            DefineAst(outputDirectory, "Stmt", new List<string>() {
                "Expression : Expr expr",
                "Print      : Expr expr",
                "Var        : Token name, Expr initializer"
            });
        }

        private static void DefineAst(string outputDirectory, string baseName,
            List<string> types)
        {
            string path = Path.Combine(outputDirectory, $"{baseName}.cs");
            var builder = new StringBuilder();

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using CSLox.Lexer;");
            builder.AppendLine("");
            builder.AppendLine("namespace CSLox.Parsing");
            builder.AppendLine("{");

            builder.AppendLine($"    public abstract class {baseName}");
            builder.AppendLine("    {");

            DefineVisitor(builder, baseName, types);

            foreach (string type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim();
                DefineType(builder, baseName, className, fields);
            }

            builder.AppendLine("");
            builder.AppendLine("        public abstract R Accept<R>(Visitor<R> visitor);");

            builder.AppendLine("    }");

            builder.AppendLine("}");

            File.WriteAllText(path, builder.ToString());
        }

        private static void DefineType(StringBuilder builder,
            string baseName,
            string className,
            string fieldList)
        {
            builder.AppendLine($"        public class {className} : {baseName}");
            builder.AppendLine("        {");

            string[] fields = fieldList.Split(new string[] { ", " }, 
                StringSplitOptions.RemoveEmptyEntries);

            // private fields
            foreach (var field in fields) 
            {
                builder.AppendLine($"            readonly {field};");
            }

            builder.AppendLine("");

            // properties
            foreach (var field in fields) 
            {
                builder.AppendLine($"            public {field.Split(' ')[0]} {FirstCharUpper(field.Split(' ')[1])} => {field.Split(' ')[1]};");
            }

            builder.AppendLine("");

            // constructor
            builder.AppendLine($"            public {className} ({fieldList})");
            builder.AppendLine("            {");
            foreach (var field in fields)
            {
                string name = field.Split(' ')[1];
                builder.AppendLine($"                this.{name} = {name};");
            }

            builder.AppendLine("            }");

            // visitor override
            builder.AppendLine("");
            builder.AppendLine($"            public override R Accept<R>(Visitor<R> visitor)");
            builder.AppendLine("            {");
            builder.AppendLine($"                return visitor.Visit{className}{baseName}(this);");
            builder.AppendLine("            }");

            builder.AppendLine("        }");
            builder.AppendLine("");
        }

        private static void DefineVisitor(StringBuilder builder,
            string baseName,
            List<string> types)
        {
            builder.AppendLine("        public interface Visitor<R>");
            builder.AppendLine("        {");
            
            foreach (var type in types) 
            {
                string typeName = type.Split(':')[0].Trim();
                builder.AppendLine(
                    $"            R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});"
                );
            }

            builder.AppendLine("        }");
        }

        private static string FirstCharUpper(string s)
        {
            return (s.ToUpper())[0] + s.Substring(1);
        }
    }
}
