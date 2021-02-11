using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    internal class Environment
    {
        private readonly Environment enclosing;
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            if (enclosing != null) 
            {
                return enclosing.Get(name);
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

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new InterpretingException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}