program main;

{$mode objfpc}

uses
  SysUtils, Classes, chunkunit, debugunit, valueunit, vmunit;

function ReadFile(FileName : String) : String;
var
  StringList : TStringList;
begin
  Result := '';
  StringList := TStringList.Create;
  try
    StringList.LoadFromFile(FileName);
    Result := StringList.Text;
  finally
    FreeAndNil(StringList);
  end;
end;

procedure Repl(VM : TVM);
var
  Line : String;
begin
  while TRUE do
  begin
    Write('> ');
    ReadLn(Line);
    VM.Interpret(Line);
  end;
end;

procedure RunFile(VM : TVM; FileName : String);
var
  Source          : String;
  InterpretResult : TInterpretResult;
begin
  Source          := ReadFile    (FileName);
  InterpretResult := VM.Interpret(Source  );

  if (InterpretResult = INTERPERT_COMPILE_ERROR) then Halt(65);
  if (InterpretResult = INTERPERT_RUNTIME_ERROR) then Halt(70);
end;

var
  VM       : TVM;
  Chunk    : TChunk;
  Constant : Integer;
begin
  VM    := TVM.Create;
  Chunk := TChunk.Create;
  try
    if      (ParamCount = 0) then Repl(VM)
    else if (ParamCount = 1) then RunFile(VM, ParamStr(1))
    else
    begin
      WriteLn('Usage: paslox [path]');
      ReadLn;
      Exit;
    end; // if ()
  finally
    FreeAndNil(VM);
    FreeAndNil(Chunk);
  end;
  ReadLn;
end.
