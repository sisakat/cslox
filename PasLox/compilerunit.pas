unit compilerunit;

{$mode objfpc}

interface

uses
  Classes, SysUtils, scannerunit;

procedure Compile(Source : String);

implementation

procedure Compile(Source : String);
var
  Scanner : TScanner;
  Line    : Integer;
  Token   : TToken;
begin
  Scanner := TScanner.Create(Source);
  try
    Line := -1;
    while TRUE do
    begin
      Token := Scanner.ScanToken;
      if (Token.Line <> Line) then
      begin
        Write(Format('%0:4d', [Token.Line]));
        Line := Token.Line;
      end
      else
      begin
        Write('   | ');
      end;

      WriteLn(Format('%0:2d "%1:s"', [Ord(Token.TokenType), Token.Source]));

      if (Token.TokenType = TOKEN_EOF)   then Break;
      if (Token.TokenType = TOKEN_ERROR) then Break;
    end;
  finally
    FreeAndNil(Scanner);
  end;
end;

end.

