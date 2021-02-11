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
            return Paranthesize(expr.Oper.Lexeme, expr.Left,  expr.Right);
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
    }
}