#!/bin/bash
sudo snap alias dotnet-sdk.dotnet dotnet
dotnet build --configuration Release
dotnet test
