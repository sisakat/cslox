using System;
using System.Collections.Generic;
using System.Text;
using CSLox.Lexer;

namespace CSLox.Parsing
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            Expr expr = Equality();

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable)
                {
                    Token name = ((Expr.Variable)expr).Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Stmt Statement()
        {
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());
            return ExpressionStatement();
        }

        private Stmt Declaration()
        {
            try 
            {
                if (Match(TokenType.VAR)) return VarDeclaration();
                return Statement();
            } catch (ParsingException)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt VarDeclaration() 
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private List<Stmt> Block()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token oper = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER,
                TokenType.GREATER_EQUAL,
                TokenType.LESS,
                TokenType.LESS_EQUAL))
            {
                Token oper = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token oper = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token oper = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token oper = Previous();
                Expr right = Unary();
                return new Expr.Unary(oper, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Expr.Literal(false);
            if (Match(TokenType.TRUE)) return new Expr.Literal(true);
            if (Match(TokenType.NIL)) return new Expr.Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw new ParsingException(Peek(), "Expected expression.");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        private ParsingException Error(Token token, string message)
        {
            return new ParsingException(token, message);
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }
    }
}