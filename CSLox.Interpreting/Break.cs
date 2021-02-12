using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    public class Break : Exception
    {
        public Break() : base()
        {
        }
    }
}