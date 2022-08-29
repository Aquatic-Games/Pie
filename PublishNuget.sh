#!/bin/bash

if [ $# -ne 2 ]
then
  echo "Required 2 arguments, <version number> <api key>"
  exit 0
fi

rm Nuget/*
dotnet pack -c Release -o Nuget/ -p:PackageVersion="$1"
if [ $? -ne 0 ]
then
  echo "Pack failed. Not pushing."
  exit 0
fi
dotnet nuget push Nuget/*.nupkg -k "$2" -s https://api.nuget.org/v3/index.json