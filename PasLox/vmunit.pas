unit vmunit;

{$mode objfpc}

interface

uses
  SysUtils, chunkunit, valueunit, commonunit, debugunit, compilerunit;

type
  TInterpretResult = (
    INTERPRET_OK,
    INTERPERT_COMPILE_ERROR,
    INTERPERT_RUNTIME_ERROR
  );

type
  TVM = class(TObject)
  private
    FChunk    : TChunk;
    FIP       : Byte;
    FStack    : array[0..256] of TValue;
    FStackTop : Integer;

    function Run : TInterpretResult;
    function ReadByte : Byte;
    function ReadConstant : TValue;
    procedure ResetStack;
  public
    constructor Create;
    destructor  Destroy; override;

    function Interpret(Chunk  : TChunk) : TInterpretResult; overload;
    function Interpret(Source : String) : TInterpretResult; overload;
    procedure Push(Value : TValue);
    function Pop : TValue;
  end;

implementation

constructor TVM.Create;
begin
  ResetStack;
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

procedure TVM.ResetStack;
begin
  FStackTop := 0;
end; // ResetStack()

function TVM.Run : TInterpretResult;
var
  i    : Integer;
  a, b : TValue;
begin
  Result := INTERPERT_RUNTIME_ERROR;

  while True do
  begin
    {$IFDEF DEBUG_TRACE_EXECUTION}
    Write('          ');
    for i:=0 to FStackTop-1 do
    begin
      Write('[ ');
      PrintValue(FStack[i]);
      Write(' ]');
    end; // for ()
    WriteLn;
    DisassembleInstruction(FChunk, FIP);
    {$ENDIF}

    case (TOpCode(ReadByte)) of
      OP_CONSTANT:
        begin
          Push(ReadConstant);
        end;
      OP_ADD:
        begin
          b := Pop;
          a := Pop;
          Push(a + b);
        end;
      OP_SUBTRACT:
        begin
          b := Pop;
          a := Pop;
          Push(a - b);
        end;
      OP_MULTIPLY:
        begin
          b := Pop;
          a := Pop;
          Push(a * b);
        end;
      OP_DIVIDE:
        begin
          b := Pop;
          a := Pop;
          Push(a / b);
        end;
      OP_NEGATE:
        begin
          FStack[FStackTop-1] := -FStack[FStackTop-1];
        end;
      OP_RETURN:
        begin
          PrintValue(Pop);
          WriteLn;
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

function TVM.Interpret(Source : String) : TInterpretResult;
begin
  Compile(Source);
  Result := INTERPRET_OK;
end; // Interpret()

procedure TVM.Push(Value : TValue);
begin
  FStack[FStackTop] := Value;
  Inc(FStackTop);
end; // Push()

function TVM.Pop : TValue;
begin
  Dec(FStackTop);
  Result := FStack[FStackTop];
end; // Pop()

begin
end.
