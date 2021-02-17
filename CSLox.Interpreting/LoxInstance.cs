using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
  public class LoxInstance
  {
    private readonly LoxClass loxClass;
    private readonly Dictionary<string, object> fields
        = new Dictionary<string, object>();

    public LoxInstance(LoxClass loxClass)
    {
      this.loxClass = loxClass;
    }

    public object Get(Token name)
    {
      if (fields.ContainsKey(name.Lexeme))
      {
        return fields[name.Lexeme];
      }

      var method = loxClass.FindMethod(name.Lexeme);
      if (method != null) return method.Bind(this);

      throw new InterpretingException(name,
          $"Undefined property '{name.Lexeme}'.");
    }

    public void Set(Token name, object value)
    {
      fields[name.Lexeme] = value;
    }

    public override string ToString()
    {
      return loxClass.Name + " instance";
    }
  }
}