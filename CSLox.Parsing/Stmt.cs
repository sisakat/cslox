using System;
using System.Collections.Generic;
using CSLox.Lexer;

namespace CSLox.Parsing
{
    public abstract class Stmt
    {
        public interface Visitor<R>
        {
            R VisitExpressionStmt(Expression stmt);
            R VisitPrintStmt(Print stmt);
            R VisitVarStmt(Var stmt);
        }
        public class Expression : Stmt
        {
            readonly Expr expr;

            public Expr Expr => expr;

            public Expression (Expr expr)
            {
                this.expr = expr;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        public class Print : Stmt
        {
            readonly Expr expr;

            public Expr Expr => expr;

            public Print (Expr expr)
            {
                this.expr = expr;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }

        public class Var : Stmt
        {
            readonly Token name;
            readonly Expr initializer;

            public Token Name => name;
            public Expr Initializer => initializer;

            public Var (Token name, Expr initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }


        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
