using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    public class Function : ICallable
    {
        private readonly Stmt.Function declaration;

        public int Arity => declaration.Parameters.Count;

        public Function(Stmt.Function declaration)
        {
            this.declaration = declaration;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(interpreter.globals);
            for (int i = 0; i < declaration.Parameters.Count; i++)
            {
                environment.Define(declaration.Parameters[i].Lexeme,
                    arguments[i]);
            }

            try 
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            } catch (Return returnValue)
            {
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