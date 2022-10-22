program main;

{$mode objfpc}

uses
  SysUtils, chunkunit, debugunit, valueunit;

var
  Chunk    : TChunk;
  Constant : Integer;
begin
  Chunk := TChunk.Create;
  try
    Constant := Chunk.AddConstant(1.2);
    Chunk.WriteChunk(OP_CONSTANT, 123); 
    Chunk.WriteChunk(Constant, 123);
    Chunk.WriteChunk(OP_RETURN, 123);
    DisassembleChunk(Chunk, 'Test Chunk');
  finally
    FreeAndNil(Chunk);
  end;
end.