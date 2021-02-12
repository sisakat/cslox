using System;
using System.Collections.Generic;
using CSLox.Lexer;

namespace CSLox.Parsing
{
    public abstract class Expr
    {
        public interface Visitor<R>
        {
            R VisitAssignExpr(Assign expr);
            R VisitBinaryExpr(Binary expr);
            R VisitCallExpr(Call expr);
            R VisitGetExpr(Get expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitLogicalExpr(Logical expr);
            R VisitSetExpr(Set expr);
            R VisitSuperExpr(Super expr);
            R VisitThisExpr(This expr);
            R VisitUnaryExpr(Unary expr);
            R VisitVariableExpr(Variable expr);
        }
        public class Assign : Expr
        {
            readonly Token name;
            readonly Expr value;

            public Token Name => name;
            public Expr Value => value;

            public Assign (Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
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

        public class Call : Expr
        {
            readonly Expr callee;
            readonly Token paren;
            readonly List<Expr> arguments;

            public Expr Callee => callee;
            public Token Paren => paren;
            public List<Expr> Arguments => arguments;

            public Call (Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        public class Get : Expr
        {
            readonly Expr obj;
            readonly Token name;

            public Expr Obj => obj;
            public Token Name => name;

            public Get (Expr obj, Token name)
            {
                this.obj = obj;
                this.name = name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGetExpr(this);
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

        public class Logical : Expr
        {
            readonly Expr left;
            readonly Token oper;
            readonly Expr right;

            public Expr Left => left;
            public Token Oper => oper;
            public Expr Right => right;

            public Logical (Expr left, Token oper, Expr right)
            {
                this.left = left;
                this.oper = oper;
                this.right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }

        public class Set : Expr
        {
            readonly Expr obj;
            readonly Token name;
            readonly Expr value;

            public Expr Obj => obj;
            public Token Name => name;
            public Expr Value => value;

            public Set (Expr obj, Token name, Expr value)
            {
                this.obj = obj;
                this.name = name;
                this.value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSetExpr(this);
            }
        }

        public class Super : Expr
        {
            readonly Token keyword;
            readonly Token method;

            public Token Keyword => keyword;
            public Token Method => method;

            public Super (Token keyword, Token method)
            {
                this.keyword = keyword;
                this.method = method;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSuperExpr(this);
            }
        }

        public class This : Expr
        {
            readonly Token keyword;

            public Token Keyword => keyword;

            public This (Token keyword)
            {
                this.keyword = keyword;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitThisExpr(this);
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

        public class Variable : Expr
        {
            readonly Token name;

            public Token Name => name;

            public Variable (Token name)
            {
                this.name = name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }


        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
