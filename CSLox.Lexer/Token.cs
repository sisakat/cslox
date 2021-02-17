using System;

namespace CSLox.Lexer
{
  public class Token
  {
    private readonly TokenType type;
    private readonly string lexeme;
    private readonly Object literal;
    private readonly int line;

    public TokenType Type => type;
    public string Lexeme => lexeme;
    public Object Literal => literal;
    public int Line => line;

    public Token(TokenType type,
        string lexeme,
        Object literal,
        int line)
    {
      this.type = type;
      this.lexeme = lexeme;
      this.literal = literal;
      this.line = line;
    }

    public override string ToString()
    {
      return $"{type} {lexeme} {literal}";
    }
  }
}