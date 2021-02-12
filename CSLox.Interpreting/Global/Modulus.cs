using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    public class Modulus : ICallable
    {
        public int Arity => 2;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            if (arguments.Count == 2)
            {
                return (double)arguments[0] % (double)arguments[1];
            }

            return null;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}