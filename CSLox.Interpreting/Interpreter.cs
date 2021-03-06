﻿using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
  public delegate void Error(Token token, string message);
  public class Interpreter : Expr.Visitor<Object>, Stmt.Visitor<Object>
  {
    internal readonly Environment globals = new Environment();
    private Environment environment;
    private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();
    private int breaking = 0;

    public event Error OnError;

    public Interpreter()
    {
      environment = globals;

      globals.Define("clock", new Clock());
      globals.Define("readLine", new ReadLine());
      globals.Define("mod", new Modulus());
    }

    public void Interpret(List<Stmt> statements)
    {
      foreach (var statement in statements)
      {
        Execute(statement);
      }
    }

    private void Execute(Stmt statement)
    {
      statement?.Accept(this);
    }

    internal void Resolve(Expr expr, int depth)
    {
      locals[expr] = depth;
    }

    internal void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
      Environment previous = this.environment;
      try
      {
        this.environment = environment;

        foreach (var statement in statements)
        {
          if (breaking > 0)
          {
            throw new Break();
          }
          Execute(statement);
        }
      }
      finally
      {
        this.environment = previous;
      }
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
      object left = Evaluate(expr.Left);
      object right = Evaluate(expr.Right);

      switch (expr.Oper.Type)
      {
        case TokenType.GREATER:
          CheckNumberOperand(expr.Oper, left, right);
          return (double)left > (double)right;
        case TokenType.GREATER_EQUAL:
          CheckNumberOperand(expr.Oper, left, right);
          return (double)left >= (double)right;
        case TokenType.LESS:
          CheckNumberOperand(expr.Oper, left, right);
          return (double)left < (double)right;
        case TokenType.LESS_EQUAL:
          CheckNumberOperand(expr.Oper, left, right);
          return (double)left <= (double)right;
        case TokenType.BANG_EQUAL:
          return !IsEqual(left, right);
        case TokenType.EQUAL_EQUAL:
          return IsEqual(left, right);
        case TokenType.MINUS:
          CheckNumberOperand(expr.Oper, left, right);
          return (double)left - (double)right;
        case TokenType.PLUS:
          if (left is double && right is double)
          {
            return (double)left + (double)right;
          }

          if (left is string && right is string)
          {
            return (string)left + (string)right;
          }

          throw new InterpretingException(expr.Oper,
              "Operands must be two numbers or two strings.");
        case TokenType.SLASH:
          CheckNumberOperand(expr.Oper, left, right);
          return (double)left / (double)right;
        case TokenType.STAR:
          CheckNumberOperand(expr.Oper, left, right);
          return (double)left * (double)right;
      }

      return null;
    }

    public object VisitCallExpr(Expr.Call expr)
    {
      object callee = Evaluate(expr.Callee);

      List<object> arguments = new List<object>();
      foreach (var argument in expr.Arguments)
      {
        arguments.Add(Evaluate(argument));
      }

      if (!(callee is ICallable))
      {
        throw new InterpretingException(expr.Paren,
            "Can only call functions and classes.");
      }

      ICallable function = (ICallable)callee;
      if (arguments.Count != function.Arity)
      {
        throw new InterpretingException(expr.Paren,
            $"Expected {function.Arity} arguments but got {arguments.Count}.");
      }
      return function.Call(this, arguments);
    }

    public object VisitGetExpr(Expr.Get expr)
    {
      object obj = Evaluate(expr.Obj);
      if (obj is LoxInstance)
      {
        return ((LoxInstance)obj).Get(expr.Name);
      }

      throw new InterpretingException(expr.Name,
          "Only instances have properties.");
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
      return Evaluate(expr.Expression);
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
      return expr.Value;
    }

    public object VisitLogicalExpr(Expr.Logical expr)
    {
      object left = Evaluate(expr.Left);

      if (expr.Oper.Type == TokenType.OR)
      {
        if (IsTruthy(left)) return left;
      }
      else
      {
        if (!IsTruthy(left)) return left;
      }

      return Evaluate(expr.Right);
    }

    public object VisitSetExpr(Expr.Set expr)
    {
      object obj = Evaluate(expr.Obj);

      if (!(obj is LoxInstance))
      {
        throw new InterpretingException(expr.Name,
            "Only instances have fields.");
      }

      object value = Evaluate(expr.Value);
      ((LoxInstance)obj).Set(expr.Name, value);
      return value;
    }

    public object VisitSuperExpr(Expr.Super expr)
    {
      int distance;
      if (locals.TryGetValue(expr, out distance))
      {
        LoxClass superclass = (LoxClass)environment.GetAt(distance, "super");
        LoxInstance obj = (LoxInstance)environment.GetAt(distance - 1, "this");

        LoxFunction method = null;
        if (expr.Method != null)
        {
          method = superclass.FindMethod(expr.Method.Lexeme);
        }
        else
        {
          method = superclass.FindMethod(superclass.Name);
        }

        if (method == null)
        {
          throw new InterpretingException(expr.Method,
              $"Undefined property '{expr.Method.Lexeme}'.");
        }

        return method.Bind(obj);
      }

      return null;
    }

    public object VisitThisExpr(Expr.This expr)
    {
      return LookUpVariable(expr.Keyword, expr);
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
      object right = Evaluate(expr.Right);

      switch (expr.Oper.Type)
      {
        case TokenType.BANG:
          return !IsTruthy(right);
        case TokenType.MINUS:
          CheckNumberOperand(expr.Oper, right);
          return -(double)right;
        default:
          return null;
      }
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
      object value = Evaluate(expr.Value);

      int distance;
      if (locals.TryGetValue(expr, out distance))
      {
        environment.AssignAt(distance, expr.Name, value);
      }
      else
      {
        globals.Assign(expr.Name, value);
      }

      return value;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
      return LookUpVariable(expr.Name, expr);
    }

    private object LookUpVariable(Token name, Expr expr)
    {
      int distance;
      if (locals.TryGetValue(expr, out distance))
      {
        return environment.GetAt(distance, name.Lexeme);
      }
      else
      {
        return globals.Get(name);
      }
    }

    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
      Evaluate(stmt.Expr);
      return null;
    }

    public object VisitFunctionStmt(Stmt.Function stmt)
    {
      var function = new LoxFunction(stmt,
          environment,
          false);
      environment.Define(stmt.Name.Lexeme, function);
      return null;
    }

    public object VisitIfStmt(Stmt.If stmt)
    {
      if (IsTruthy(Evaluate(stmt.Condition)))
      {
        Execute(stmt.ThenBranch);
      }
      else if (stmt.ElseBranch != null)
      {
        Execute(stmt.ElseBranch);
      }

      return null;
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
      object value = Evaluate(stmt.Expr);
      Console.WriteLine(value);
      return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
      object value = null;
      if (stmt.Value != null) value = Evaluate(stmt.Value);
      throw new Return(value);
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
      object value = null;
      if (stmt.Initializer != null)
      {
        value = Evaluate(stmt.Initializer);
      }

      environment.Define(stmt.Name.Lexeme, value);
      return null;
    }

    public object VisitWhileStmt(Stmt.While stmt)
    {
      try
      {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
          Execute(stmt.Body);
        }
      }
      catch (Break)
      {
        breaking--;
      }

      return null;
    }

    public object VisitBlockStmt(Stmt.Block stmt)
    {
      ExecuteBlock(stmt.Statements, new Environment(environment));
      return null;
    }

    public object VisitClassStmt(Stmt.Class stmt)
    {
      object superclass = null;
      if (stmt.Superclass != null)
      {
        superclass = Evaluate(stmt.Superclass);
        if (!(superclass is LoxClass))
        {
          throw new InterpretingException(stmt.Superclass.Name,
              "Superclass must be a class.");
        }
      }

      environment.Define(stmt.Name.Lexeme, null);

      if (stmt.Superclass != null)
      {
        environment = new Environment(environment);
        environment.Define("super", superclass);
      }

      Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
      foreach (var method in stmt.Methods)
      {
        var function = new LoxFunction(method,
            environment,
            method.Name.Lexeme == stmt.Name.Lexeme);
        methods[method.Name.Lexeme] = function;
      }

      var lclass = new LoxClass(stmt.Name.Lexeme,
          (LoxClass)superclass,
          methods);

      if (stmt.Superclass != null)
      {
        environment = environment.Enclosing;
      }

      environment.Assign(stmt.Name, lclass);
      return null;
    }

    public object VisitBreakStmt(Stmt.Break stmt)
    {
      if (stmt.Loop == null)
      {
        breaking = 1;
      }
      else
      {
        breaking = (int)(double)stmt.Loop.Literal;
      }

      return null;
    }

    private bool IsEqual(object a, object b)
    {
      if (a == null && b == null) return true;
      if (a == null) return false;

      return a.Equals(b);
    }

    private bool IsTruthy(Object obj)
    {
      if (obj == null) return false;
      if (obj is bool) return (bool)obj;
      return true;
    }

    private void CheckNumberOperand(Token oper, object operand)
    {
      if (operand is double) return;
      throw new InterpretingException(oper, "Operand must be a number");
    }

    private void CheckNumberOperand(Token oper, object left, object right)
    {
      if (left is double && right is double) return;
      throw new InterpretingException(oper, "Operands must be numbers.");
    }

    private object Evaluate(Expr expression)
    {
      return expression.Accept(this);
    }
  }
}
