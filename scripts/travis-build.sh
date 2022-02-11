#!/bin/bash

sudo snap alias dotnet-sdk.dotnet dotnet

dotnet build --configuration Release

dotnet test

curl -Os https://uploader.codecov.io/latest/linux/codecov
chmod +x codecov
./codecov
