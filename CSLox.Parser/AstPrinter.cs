using System;
using System.Collections.Generic;
using System.Text;
using CSLox.Lexer;

namespace CSLox.Parser
{
    public class AstPrinter : Expr.Visitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
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
    }
}