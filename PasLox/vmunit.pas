unit vmunit;

{$mode objfpc}

interface

uses
  SysUtils, chunkunit, valueunit, commonunit, debugunit;

type
  TInterpretResult = (
    INTERPRET_OK,
    INTERPERT_COMPILE_ERROR,
    INTERPERT_RUNTIME_ERROR
  );

type
  TVM = class(TObject)
  private
    FChunk : TChunk;
    FIP    : Byte;

    function Run : TInterpretResult;
    function ReadByte : Byte;
    function ReadConstant : TValue;
  public
    constructor Create;
    destructor  Destroy; override;

    function Interpret(Chunk : TChunk) : TInterpretResult;
  end;

implementation

constructor TVM.Create;
begin

end; // Create()

destructor TVM.Destroy;
begin

end; // Destroy()

function TVM.ReadByte : Byte;
begin
  Result := FChunk.Code[FIP];
  Inc(FIP);
end; // ReadByte()

function TVM.ReadConstant : TValue;
begin
  Result := FChunk.Constants.Values[ReadByte];
end; // ReadConstant()

function TVM.Run : TInterpretResult;
begin
  Result := INTERPERT_RUNTIME_ERROR;

  while True do
  begin
    {$IFDEF DEBUG_TRACE_EXECUTION}
    DisassembleInstruction(FChunk, FIP);
    WriteLn('hello');
    {$ENDIF}

    case (TOpCode(ReadByte)) of
      OP_CONSTANT:
        begin
          PrintValue(ReadConstant);
          WriteLn; 
        end;
      OP_RETURN: 
        begin
          Result := INTERPRET_OK;
          Exit;
        end;
    end;
  end; // while ()
end; // Run()

function TVM.Interpret(Chunk : TChunk) : TInterpretResult;
begin
  FChunk := Chunk;
  FIP    := 0;
  Result := Run;
end; // Interpret()

begin
end.