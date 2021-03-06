using System;
using System.Collections.Generic;
using System.Text;
using CSLox.Lexer;

namespace CSLox.Parsing
{
  public class AstPrinter : Expr.Visitor<string>, Stmt.Visitor<string>
  {
    public string Print(List<Stmt> statements)
    {
      var builder = new StringBuilder();
      foreach (var statement in statements)
      {
        builder.AppendLine(statement.Accept(this));
      }
      return builder.ToString();
    }

    public string Paranthesize(string lexeme, params Expr[] exprs)
    {
      var builder = new StringBuilder();
      builder.Append("(").Append(lexeme);
      foreach (var expr in exprs)
      {
        builder.Append(" ");
        builder.Append(expr.Accept(this));
      }
      builder.Append(")");
      return builder.ToString();
    }

    public string VisitBinaryExpr(Expr.Binary expr)
    {
      return Paranthesize(expr.Oper.Lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
    {
      return Paranthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Expr.Literal expr)
    {
      if (expr.Value == null) return "nil";
      return expr.Value.ToString();
    }

    public string VisitUnaryExpr(Expr.Unary expr)
    {
      return Paranthesize(expr.Oper.Lexeme, expr.Right);
    }

    public string VisitExpressionStmt(Stmt.Expression stmt)
    {
      return stmt.Expr.Accept(this);
    }

    public string VisitPrintStmt(Stmt.Print stmt)
    {
      return stmt.Expr.Accept(this);
    }

    public string VisitVariableExpr(Expr.Variable expr)
    {
      throw new NotImplementedException();
    }

    public string VisitVarStmt(Stmt.Var stmt)
    {
      throw new NotImplementedException();
    }

    public string VisitAssignExpr(Expr.Assign expr)
    {
      throw new NotImplementedException();
    }

    public string VisitBlockStmt(Stmt.Block stmt)
    {
      throw new NotImplementedException();
    }

    public string VisitIfStmt(Stmt.If stmt)
    {
      throw new NotImplementedException();
    }

    public string VisitLogicalExpr(Expr.Logical expr)
    {
      throw new NotImplementedException();
    }

    public string VisitWhileStmt(Stmt.While stmt)
    {
      throw new NotImplementedException();
    }

    public string VisitBreakStmt(Stmt.Break stmt)
    {
      throw new NotImplementedException();
    }

    public string VisitCallExpr(Expr.Call expr)
    {
      throw new NotImplementedException();
    }

    public string VisitFunctionStmt(Stmt.Function stmt)
    {
      throw new NotImplementedException();
    }

    public string VisitReturnStmt(Stmt.Return stmt)
    {
      throw new NotImplementedException();
    }

    public string VisitClassStmt(Stmt.Class stmt)
    {
      throw new NotImplementedException();
    }

    public string VisitGetExpr(Expr.Get expr)
    {
      throw new NotImplementedException();
    }

    public string VisitSetExpr(Expr.Set expr)
    {
      throw new NotImplementedException();
    }

    public string VisitThisExpr(Expr.This expr)
    {
      throw new NotImplementedException();
    }

    public string VisitSuperExpr(Expr.Super expr)
    {
      throw new NotImplementedException();
    }
  }
}