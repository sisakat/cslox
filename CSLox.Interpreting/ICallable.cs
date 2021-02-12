using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    public interface ICallable 
    {
        int Arity { get; }
        object Call(Interpreter interpreter, List<object> arguments);
    }
}