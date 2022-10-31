unit chunkunit;

{$mode objfpc}

interface

uses
  utilunit, valueunit;

type
  TOpCode = (
    OP_CONSTANT,
    OP_ADD     ,
    OP_SUBTRACT,
    OP_MULTIPLY,
    OP_DIVIDE  ,
    OP_NEGATE  ,
    OP_RETURN
  );

type
  TChunk = class(TObject)
  private
    FCount      : Integer;
    FCapacity   : Integer;
    FCode       : array of Byte;
    FLines      : array of Integer;
    FConstants  : TValueArray;

    function  GetCode(i : Integer) : Byte;
    procedure SetCode(i : Integer; Value : Byte);
    function  GetLine(i : Integer) : Integer;
    procedure SetLine(i : Integer; Value : Integer);
  public
    constructor Create ;
    destructor  Destroy; override;
    procedure   WriteChunk (Value  : Byte   ; Line : Integer); overload;
    procedure   WriteChunk (OpCode : TOpCode; Line : Integer); overload;
    function    AddConstant(Value  : TValue ) : Integer;

    property Count              : Integer     read FCount     write FCount    ;
    property Capacity           : Integer     read FCapacity  write FCapacity ;
    property Constants          : TValueArray read FConstants write FConstants;
    property Code [i : Integer] : Byte        read GetCode    write SetCode   ;
    property Lines[i : Integer] : Integer     read GetLine    write SetLine   ;
  end;

implementation

uses
  SysUtils;

function TChunk.GetCode(i : Integer) : Byte;
begin
  Result := FCode[i];
end; // GetCode()

procedure TChunk.SetCode(i : Integer; Value : Byte);
begin
  FCode[i] := Value;
end; // SetCode()

function TChunk.GetLine(i : Integer) : Integer;
begin
  Result := FLines[i];
end; // GetLine()

procedure TChunk.SetLine(i : Integer; Value : Integer);
begin
  FLines[i] := Value;
end; // SetLine()

constructor TChunk.Create;
begin
  FCount      := 0;
  FCapacity   := 0;
  FConstants  := TValueArray.Create;

  SetLength(FCode , 0);
  SetLength(FLines, 0);
end; // Create()

destructor TChunk.Destroy;
begin
  FreeAndNil(FConstants);
end; // Destroy()

procedure TChunk.WriteChunk(Value : Byte; Line : Integer);
var
  OldCapacity : Integer;
begin
  if (FCapacity < FCount + 1) then
  begin
    OldCapacity := FCapacity;
    FCapacity   := GrowCapacity(OldCapacity);
    SetLength(FCode , FCapacity);
    SetLength(FLines, FCapacity);
  end; // if ()

  FCode [FCount] := Value;
  FLines[FCount] := Line ;
  Inc(FCount);
end; // WriteChunk()

procedure TChunk.WriteChunk(OpCode : TOpCode; Line : Integer);
begin
  WriteChunk(Ord(OpCode), Line);
end; // WriteChunk()

function TChunk.AddConstant(Value : TValue) : Integer;
begin
  FConstants.WriteValueArray(Value);
  Result := FConstants.Count - 1;
end; // AddConstant()

begin
end.
