unit valueunit;

{$mode objfpc}

interface

uses
  SysUtils;

type
  TValue = Double;

type
  TValueArray = class(TObject)
  private
    FCapacity : Integer;
    FCount    : Integer;
    FValues   : array of TValue;

    function  GetValues(i : Integer) : TValue;
    procedure SetValues(i : Integer; Value : TValue);
  public
    constructor Create;
    destructor  Destroy; override;
    procedure   WriteValueArray(Value : TValue);

    property Count               : Integer read FCount    write FCount   ;
    property Capacity            : Integer read FCapacity write FCapacity;
    property Values[i : Integer] : TValue  read GetValues write SetValues;
  end;

procedure PrintValue(Value : TValue);

implementation

uses
  utilunit;

constructor TValueArray.Create;
begin
  FCapacity := 0;
  FCount    := 0;
  SetLength(FValues, 0);
end; // Create()

destructor TValueArray.Destroy;
begin
end; // TValueArray()

function TValueArray.GetValues(i : Integer) : TValue;
begin
  Result := FValues[i];
end; // GetValues()

procedure TValueArray.SetValues(i : Integer; Value : TValue);
begin
  FValues[i] := Value;
end; // SetValues()

procedure TValueArray.WriteValueArray(Value : TValue);
var
  OldCapacity : Integer;
begin
  if (FCapacity < FCount + 1) then
  begin
    OldCapacity := FCapacity;
    FCapacity := GrowCapacity(OldCapacity);
    SetLength(FValues, FCapacity);
  end; // if ()

  FValues[FCount] := Value;
  Inc(FCount);
end; // WriteValueArray()

procedure PrintValue(Value : TValue);
begin
  Write(Format('%0:f', [Value]));
end;

begin
end.