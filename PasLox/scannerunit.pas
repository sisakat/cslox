unit scannerunit;

{$mode objfpc}{H+}

interface

uses
  Classes, SysUtils;

type
  TTokenType = (
    // Single-character tokens
    TOKEN_LEFT_PAREN   ,
    TOKEN_RIGHT_PAREN  ,
    TOKEN_LEFT_BRACE   ,
    TOKEN_RIGHT_BRACE  ,
    TOKEN_COMMA        ,
    TOKEN_DOT          ,
    TOKEN_MINUS        ,
    TOKEN_PLUS         ,
    TOKEN_SEMICOLON    ,
    TOKEN_SLASH        ,
    TOKEN_STAR         ,

    // One or two character tokens
    TOKEN_BANG         ,
    TOKEN_BANG_EQUAL   ,
    TOKEN_EQUAL        ,
    TOKEN_EQUAL_EQUAL  ,
    TOKEN_GREATER      ,
    TOKEN_GREATER_EQUAL,
    TOKEN_LESS         ,
    TOKEN_LESS_EQUAL   ,

    // Literals
    TOKEN_IDENTIFIER   ,
    TOKEN_STRING       ,
    TOKEN_NUMBER       ,

    // Keywords
    TOKEN_AND          ,
    TOKEN_CLASS        ,
    TOKEN_ELSE         ,
    TOKEN_FALSE        ,
    TOKEN_FOR          ,
    TOKEN_FUN          ,
    TOKEN_IF           ,
    TOKEN_NIL          ,
    TOKEN_OR           ,
    TOKEN_PRINT        ,
    TOKEN_RETURN       ,
    TOKEN_SUPER        ,
    TOKEN_THIS         ,
    TOKEN_TRUE         ,
    TOKEN_VAR          ,
    TOKEN_WHILE        ,

    TOKEN_ERROR        ,
    TOKEN_EOF
  );

type
  TToken = class(TObject)
  private
    FSource    : String;
    FTokenType : TTokenType;
    FStart     : Integer;
    FLength    : Integer;
    FLine      : Integer;
  public
    property Source    : String     read FSource    write FSource   ;
    property TokenType : TTokenType read FTokenType write FTokenType;
    property Start     : Integer    read FStart     write FStart    ;
    property Length    : Integer    read FLength    write FLength   ;
    property Line      : Integer    read FLine      write FLine     ;
  end;

type
  TScanner = class(TObject)
  private
    FSource  : String ;
    FStart   : Integer;
    FCurrent : Integer;
    FLine    : Integer;

    function IdentifierType : TTokenType;
    function CheckKeyword(Start : Integer;
      Length : Integer; Rest : String; TokenType : TTokenType) : TTokenType;
  public
    constructor Create(Source : String);
    destructor  Destroy; override;

    procedure SkipWhitespace                   ;
    function  Peek                   : Char    ;
    function  PeekNext               : Char    ;
    function  Advance                : Char    ;
    function  Match(Expected : Char) : Boolean ;
    function  ScanToken              : TToken  ;
    function  IsAtEnd                : Boolean ;
    function  IsAlpha(c : Char)      : Boolean ;
    function  IsDigit(c : Char)      : Boolean ;

    function MakeString : TToken;
    function MakeNumber : TToken;
    function MakeIdentifier : TToken;
    function MakeToken(TokenType : TTokenType) : TToken;
    function ErrorToken(Message : String) : TToken;
  end;

implementation

constructor TScanner.Create(Source : String);
begin
  FSource  := Source;
  FStart   := 1;
  FCurrent := 1;
  FLine    := 1;
end;

destructor TScanner.Destroy;
begin
  inherited;
end;

procedure TScanner.SkipWhitespace;
begin
  while TRUE do
  begin
    case Peek of
      // #9  ... tab
      // #13 ... carriage return
      // #10 ... linefeed
      ' ', #9, #13: Advance;
      #10:
        begin
          Inc(FLine);
          Advance;
        end;
      '/':
        begin
          if (PeekNext = '/') then
          begin
            // A comment goes until the end of the line.
            while (Peek <> #10) and (not IsAtEnd) do Advance;
          end;
        end
      else Exit;
    end;
  end;
end;

function TScanner.Peek : Char;
begin
  Result := FSource[FCurrent];
end;

function TScanner.PeekNext : Char;
begin
  if (IsAtEnd) then Result := #0
  else Result := FSource[FCurrent+1];
end;

function TScanner.Advance : Char;
begin
  Result := FSource[FCurrent];
  Inc(FCurrent);
end;

function TScanner.Match(Expected : Char) : Boolean;
begin
  if (IsAtEnd                      ) then Result := FALSE;
  if (FSource[FCurrent] <> Expected) then Result := FALSE;
  Inc(FCurrent);
  Result := TRUE;
end;

function TScanner.ScanToken : TToken;
var
  c : Char;
begin
  SkipWhitespace;
  Result := ErrorToken('Unexpected character.');

  FStart := FCurrent;
  if (IsAtEnd) then
  begin
    Result := MakeToken(TOKEN_EOF);
    Exit;
  end;

  c := Advance;

  if (IsAlpha(c)) then
  begin
    Result := MakeIdentifier;
    Exit;
  end;

  if (IsDigit(c)) then
  begin
    Result := MakeNumber;
    Exit;
  end;

  case c of
    '(': Result := MakeToken(TOKEN_LEFT_PAREN  );
    ')': Result := MakeToken(TOKEN_RIGHT_PAREN );
    '{': Result := MakeToken(TOKEN_LEFT_BRACE  );
    '}': Result := MakeToken(TOKEN_RIGHT_BRACE );
    ';': Result := MakeToken(TOKEN_SEMICOLON   );
    ',': Result := MakeToken(TOKEN_COMMA       );
    '.': Result := MakeToken(TOKEN_DOT         );
    '-': Result := MakeToken(TOKEN_MINUS       );
    '+': Result := MakeToken(TOKEN_PLUS        );
    '/': Result := MakeToken(TOKEN_SLASH       );
    '*': Result := MakeToken(TOKEN_STAR        );
    '!':
      begin
        if (Match('=')) then Result := MakeToken(TOKEN_BANG_EQUAL)
        else                 Result := MakeToken(TOKEN_BANG      );
      end;
    '=':
      begin
        if (Match('=')) then Result := MakeToken(TOKEN_EQUAL_EQUAL)
        else                 Result := MakeToken(TOKEN_EQUAL      );
      end;
    '<':
      begin
        if (Match('=')) then Result := MakeToken(TOKEN_LESS_EQUAL)
        else                 Result := MakeToken(TOKEN_LESS      );
      end;
    '>':
      begin
        if (Match('=')) then Result := MakeToken(TOKEN_GREATER_EQUAL)
        else                 Result := MakeToken(TOKEN_GREATER      );
      end;
    '"': Result := MakeString;
  end;
end;

function TScanner.IsAtEnd : Boolean;
begin
  Result := FSource[FCurrent] = #0;
end;

function TScanner.IsAlpha(c : Char) : Boolean;
begin
  Result := ((c >= 'a') and (c <= 'z')) or
            ((c >= 'A') and (c <= 'Z')) or
            (c = '_');
end;

function TScanner.IsDigit(c : Char) : Boolean;
begin
  Result := (c >= '0') and (c <= '9');
end;

function TScanner.MakeString : TToken;
begin
  while (Peek <> '"') and (not IsAtEnd) do
  begin
    if (Peek = #10) then Inc(FLine);
    Advance;
  end;

  if (IsAtEnd) then
  begin
    Result := ErrorToken('Unterminated string.');
    Exit;
  end;

  // The closing quote.
  Advance;
  Result := MakeToken(TOKEN_STRING);
end;

function TScanner.MakeNumber : TToken;
begin
  while (IsDigit(Peek)) do Advance;

  // Look for a fractional part.
  if (Peek = '.') and (IsDigit(PeekNext)) then
  begin
    // Consume the "."
    Advance;

    while (IsDigit(Peek)) do Advance;
  end;

  Result := MakeToken(TOKEN_NUMBER);
end;

function TScanner.MakeIdentifier : TToken;
begin
  while (IsAlpha(Peek)) or (IsDigit(Peek)) do Advance;
  Result := MakeToken(IdentifierType);
end;

function TScanner.MakeToken(TokenType : TTokenType) : TToken;
begin
  Result := TToken.Create;
  Result.TokenType := TokenType;
  Result.Start     := FStart;
  Result.Length    := FCurrent - FStart;
  Result.Line      := FLine;
  Result.Source    := AnsiString(FSource).Substring(FStart-1, Result.Length);
end;

function TScanner.ErrorToken(Message : String) : TToken;
begin
  Result := TToken.Create;
  Result.TokenType := TOKEN_ERROR;
  Result.Start     := 1;
  Result.Length    := Length(Message);
  Result.Line      := FLine;
  Result.Source    := Message;
end;

function TScanner.IdentifierType : TTokenType;
begin
  Result := TOKEN_IDENTIFIER;
  case FSource[FStart] of
    'a': Result := CheckKeyword(1, 2, 'nd'   , TOKEN_AND   );
    'c': Result := CheckKeyword(1, 4, 'lass' , TOKEN_CLASS );
    'e': Result := CheckKeyword(1, 3, 'lse'  , TOKEN_ELSE  );
    'f':
      begin
        if (FCurrent - FStart > 1) then
        begin
          case FSource[FStart + 1] of
            'a': Result := CheckKeyword(2, 3, 'lse', TOKEN_FALSE);
            'o': Result := CheckKeyword(2, 1, 'r'  , TOKEN_FOR  );
            'u': Result := CheckKeyword(2, 1, 'n'  , TOKEN_FUN  );
          end;
        end;
      end;
    'i': Result := CheckKeyword(1, 1, 'f'    , TOKEN_IF    );
    'n': Result := CheckKeyword(1, 2, 'il'   , TOKEN_NIL   );
    'o': Result := CheckKeyword(1, 1, 'r'    , TOKEN_OR    );
    'p': Result := CheckKeyword(1, 4, 'rint' , TOKEN_PRINT );
    'r': Result := CheckKeyword(1, 5, 'eturn', TOKEN_RETURN);
    's': Result := CheckKeyword(1, 4, 'uper' , TOKEN_SUPER );
    't':
      begin
        if (FCurrent - FStart > 1) then
        begin
          case FSource[FStart + 1] of
            'h': Result := CheckKeyword(2, 2, 'is', TOKEN_THIS);
            'r': Result := CheckKeyword(2, 2, 'ue', TOKEN_TRUE);
          end;
        end;
      end;
    'v': Result := CheckKeyword(1, 2, 'ar'   , TOKEN_VAR   );
    'w': Result := CheckKeyword(1, 4, 'hile' , TOKEN_WHILE );
  end;
end;

function TScanner.CheckKeyword(Start : Integer;
  Length : Integer; Rest : String; TokenType : TTokenType) : TTokenType;
begin
  Result := TOKEN_IDENTIFIER;
  if (AnsiString(FSource).Substring(FStart - 1 + Start, Length) = Rest) then
    Result := TokenType;
end;

end.

