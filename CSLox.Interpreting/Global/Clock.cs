using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    public class Clock : ICallable
    {
        public int Arity => 0;

        private static readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)((long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds) 
                / 1000.0;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}