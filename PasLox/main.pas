program main;

{$mode objfpc}

uses
  SysUtils, chunkunit, debugunit, valueunit, vmunit;

var
  VM       : TVM;
  Chunk    : TChunk;
  Constant : Integer;
begin
  VM    := TVM.Create;
  Chunk := TChunk.Create;
  try
    Constant := Chunk.AddConstant(1.2);
    Chunk.WriteChunk(OP_CONSTANT, 123); 
    Chunk.WriteChunk(Constant, 123);
    Chunk.WriteChunk(OP_RETURN, 123);
    DisassembleChunk(Chunk, 'Test Chunk');
    VM.Interpret(Chunk);
  finally
    FreeAndNil(VM);
    FreeAndNil(Chunk);
  end;
end.