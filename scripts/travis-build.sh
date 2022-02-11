#!/bin/bash

sudo snap alias dotnet-sdk.dotnet dotnet

dotnet build --configuration Release

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

curl -Os https://uploader.codecov.io/latest/linux/codecov
chmod +x codecov
./codecov
