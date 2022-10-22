unit utilunit;

{$mode objfpc}

interface

function GrowCapacity(Capacity : Integer) : Integer;

implementation

function GrowCapacity(Capacity : Integer) : Integer;
begin
  if (Capacity < 8) then Result := 8
  else Result := Capacity * 2;
end; // GrowCapacity()

begin
end.