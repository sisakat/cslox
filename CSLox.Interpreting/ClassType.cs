using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    public enum ClassType
    {
        NONE,
        CLASS,
        SUBCLASS
    }
}