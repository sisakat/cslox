using System;
using System.Collections.Generic;
using CSLox.Lexer;

namespace CSLox.Parser
{
    public abstract class Expr
    {
        public interface Visitor<R>
        {
            R VisitBinaryExpr(Binary expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitUnaryExpr(Unary expr);
        }
        public class Binary : Expr
        {
            readonly Expr left;
            readonly Token oper;
            readonly Expr right;

            public Expr Left => left;
            public Token Oper => oper;
            public Expr Right => right;

            public Binary (Expr left, Token oper, Expr right)
            {
                this.left = left;
                this.oper = oper;
                this.right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }
        public class Grouping : Expr
        {
            readonly Expr expression;

            public Expr Expression => expression;

            public Grouping (Expr expression)
            {
                this.expression = expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }
        public class Literal : Expr
        {
            readonly Object value;

            public Object Value => value;

            public Literal (Object value)
            {
                this.value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }
        public class Unary : Expr
        {
            readonly Token oper;
            readonly Expr right;

            public Token Oper => oper;
            public Expr Right => right;

            public Unary (Token oper, Expr right)
            {
                this.oper = oper;
                this.right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
