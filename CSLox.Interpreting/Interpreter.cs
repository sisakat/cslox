using System;
using System.Collections.Generic;
using CSLox.Lexer;
using CSLox.Parsing;

namespace CSLox.Interpreting
{
    public class Interpreter : Expr.Visitor<Object>, Stmt.Visitor<Object>
    {
        private Environment environment = new Environment();

        public void Interpret(List<Stmt> statements)
        {
            foreach (var statement in statements) 
            {
                Execute(statement);
            }
        }

        private void Execute(Stmt statement)
        {
            statement.Accept(this);
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

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
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

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            object value = Evaluate(stmt.Expr);
            Console.WriteLine(value);
            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.Name);
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
