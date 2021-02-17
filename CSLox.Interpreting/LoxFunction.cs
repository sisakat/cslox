using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
  public class LoxFunction : ICallable
  {
    private readonly Stmt.Function declaration;
    private readonly Environment closure;
    private readonly bool isInitializer;

    public int Arity => declaration.Parameters.Count;

    public LoxFunction(Stmt.Function declaration,
        Environment closure,
        bool isInitializer)
    {
      this.declaration = declaration;
      this.closure = closure;
      this.isInitializer = isInitializer;
    }

    public LoxFunction Bind(LoxInstance instance)
    {
      var environment = new Environment(closure);
      environment.Define("this", instance);
      return new LoxFunction(declaration, environment, isInitializer);
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
      var environment = new Environment(closure);
      for (int i = 0; i < declaration.Parameters.Count; i++)
      {
        environment.Define(declaration.Parameters[i].Lexeme,
            arguments[i]);
      }

      try
      {
        interpreter.ExecuteBlock(declaration.Body, environment);
      }
      catch (Return returnValue)
      {
        if (isInitializer) return closure.GetAt(0, "this");
        return returnValue.Value;
      }

      return null;
    }

    public override string ToString()
    {
      return $"<fn {declaration.Name.Lexeme}>";
    }
  }
}