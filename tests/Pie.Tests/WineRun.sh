#!/bin/bash

dotnet build -r win-x64

if [ $? -ne 0 ]; then
  exit 1
fi

WINEDEBUG=-all wine ./bin/Debug/net7.0/win-x64/Pie.Tests.exe