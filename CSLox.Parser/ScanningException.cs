using System;
using System.Collections.Generic;
using CSLox.Lexer;

namespace CSLox.Parser
{
    public class ParsingException : Exception 
    {
        public Token Token { get; private set; }
        
        public ParsingException(Token token,
            string message) : base(message)
        {
            Token = token;
        }
    }
}