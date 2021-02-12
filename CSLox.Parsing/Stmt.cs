using System;
using System.Collections.Generic;
using CSLox.Lexer;

namespace CSLox.Parsing
{
    public abstract class Stmt
    {
        public interface Visitor<R>
        {
            R VisitBlockStmt(Block stmt);
            R VisitBreakStmt(Break stmt);
            R VisitClassStmt(Class stmt);
            R VisitExpressionStmt(Expression stmt);
            R VisitFunctionStmt(Function stmt);
            R VisitIfStmt(If stmt);
            R VisitPrintStmt(Print stmt);
            R VisitVarStmt(Var stmt);
            R VisitReturnStmt(Return stmt);
            R VisitWhileStmt(While stmt);
        }
        public class Block : Stmt
        {
            readonly List<Stmt> statements;

            public List<Stmt> Statements => statements;

            public Block (List<Stmt> statements)
            {
                this.statements = statements;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
        }

        public class Break : Stmt
        {
            readonly Token loop;

            public Token Loop => loop;

            public Break (Token loop)
            {
                this.loop = loop;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBreakStmt(this);
            }
        }

        public class Class : Stmt
        {
            readonly Token name;
            readonly Expr.Variable superclass;
            readonly List<Stmt.Function> methods;

            public Token Name => name;
            public Expr.Variable Superclass => superclass;
            public List<Stmt.Function> Methods => methods;

            public Class (Token name, Expr.Variable superclass, List<Stmt.Function> methods)
            {
                this.name = name;
                this.superclass = superclass;
                this.methods = methods;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitClassStmt(this);
            }
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

        public class Function : Stmt
        {
            readonly Token name;
            readonly List<Token> parameters;
            readonly List<Stmt> body;

            public Token Name => name;
            public List<Token> Parameters => parameters;
            public List<Stmt> Body => body;

            public Function (Token name, List<Token> parameters, List<Stmt> body)
            {
                this.name = name;
                this.parameters = parameters;
                this.body = body;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }
        }

        public class If : Stmt
        {
            readonly Expr condition;
            readonly Stmt thenBranch;
            readonly Stmt elseBranch;

            public Expr Condition => condition;
            public Stmt ThenBranch => thenBranch;
            public Stmt ElseBranch => elseBranch;

            public If (Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitIfStmt(this);
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

        public class Return : Stmt
        {
            readonly Token keyword;
            readonly Expr value;

            public Token Keyword => keyword;
            public Expr Value => value;

            public Return (Token keyword, Expr value)
            {
                this.keyword = keyword;
                this.value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
        }

        public class While : Stmt
        {
            readonly Expr condition;
            readonly Stmt body;

            public Expr Condition => condition;
            public Stmt Body => body;

            public While (Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
        }


        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
