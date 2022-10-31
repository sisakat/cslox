unit debugunit;

{$mode objfpc}

interface

uses
  chunkunit;

procedure DisassembleChunk      (Chunk : TChunk; Name   : String                  )          ;
function  DisassembleInstruction(Chunk : TChunk; Offset : Integer                 ) : Integer;
function  SimpleInstruction     (Name  : String; Offset : Integer                 ) : Integer;
function  ConstantInstruction   (Name  : String; Chunk  : TChunk; Offset : Integer) : Integer;

implementation

uses
  SysUtils, valueunit;

procedure DisassembleChunk(Chunk : TChunk; Name : String);
var
  Offset : Integer;
begin
  WriteLn(Format('== %0:s ==', [Name]));

  Offset := 0;
  while (Offset < Chunk.Count) do
  begin
    Offset := DisassembleInstruction(Chunk, Offset);
  end; // while ()
end; // DisassembleChunk()

function  DisassembleInstruction(Chunk : TChunk; Offset : Integer) : Integer;
var
  Value : Byte;
begin
  Write(Format('%.4d ', [Offset]));
  if (Offset > 0) and (Chunk.Lines[Offset] = Chunk.Lines[Offset - 1]) then
  begin
    Write('   | ');
  end
  else
  begin
    Write(Format('%0:4d ', [Chunk.Lines[Offset]]));
  end; // if ()

  Value := Chunk.Code[Offset];

  case TOpCode(Value) of
    OP_CONSTANT: Result := ConstantInstruction('OP_CONSTANT', Chunk, Offset);
    OP_ADD     : Result := SimpleInstruction  ('OP_ADD'     , Offset       );
    OP_SUBTRACT: Result := SimpleInstruction  ('OP_SUBTRACT', Offset       );
    OP_MULTIPLY: Result := SimpleInstruction  ('OP_MULTIPLY', Offset       );
    OP_DIVIDE  : Result := SimpleInstruction  ('OP_DIVIDE'  , Offset       );
    OP_NEGATE  : Result := SimpleInstruction  ('OP_NEGATE'  , Offset       );
    OP_RETURN  : Result := SimpleInstruction  ('OP_RETURN'  , Offset       );
    else WriteLn(Format('Unknown opcode %0:d', [Value]));
  end;
end; // DisassembleInstruction()

function SimpleInstruction(Name : String; Offset : Integer) : Integer;
begin
  WriteLn(Format('%0:s', [Name]));
  Result := Offset + 1;
end; // SimpleInstruction()

function ConstantInstruction (Name : String; Chunk : TChunk; Offset : Integer) : Integer;
var
  Constant : Byte;
begin
  Constant := Chunk.Code[Offset + 1];
  Write(Format('%0:-16s %1:.4d "', [Name, Constant]));
  PrintValue(Chunk.Constants.Values[Constant]);
  WriteLn('"');
  Result := Offset + 2;
end; // ConstantInstruction()

begin
end.
