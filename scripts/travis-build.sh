#!/bin/bash
sudo snap alias dotnet-sdk.dotnet dotnet
cd src
dotnet build --configuration Release
dotnet test
