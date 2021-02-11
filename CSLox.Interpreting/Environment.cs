using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    internal class Environment
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            throw new InterpretingException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Define(string name, object value)
        {
            if (values.ContainsKey(name))
            {
                values[name] = value;
            } else
            {
                values.Add(name, value);
            }
        }
    }
}