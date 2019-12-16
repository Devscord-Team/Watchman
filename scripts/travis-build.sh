#!/bin/bash
sudo snap alias dotnet-sdk.dotnet dotnet
dotnet restore
dotnet build --configuration Release
dotnet test
