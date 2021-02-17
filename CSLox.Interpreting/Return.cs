using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
  public class Return : Exception
  {
    private object value;
    public object Value => value;

    public Return(object value) : base()
    {
      this.value = value;
    }
  }
}