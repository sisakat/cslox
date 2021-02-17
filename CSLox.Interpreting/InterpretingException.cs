using System;
using System.Collections.Generic;
using CSLox.Lexer;

namespace CSLox.Interpreting
{
  public class InterpretingException : Exception
  {
    public Token Token { get; private set; }

    public InterpretingException(Token token,
        string message) : base(message)
    {
      Token = token;
    }
  }
}