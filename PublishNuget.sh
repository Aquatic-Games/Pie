#!/bin/bash

if [ $# -ne 2 ]
then
  echo "Required 2 arguments, <version number> <api key>"
  exit 1
fi

rm -f Nuget/*

dotnet pack -c Release -o Nuget/ -p:PackageVersion="$1"
if [ $? -ne 0 ]
then
  echo "Pack failed. Not pushing."
  exit 1
fi

#dotnet nuget push Nuget/*.nupkg -k "$2" -s https://api.nuget.org/v3/index.json