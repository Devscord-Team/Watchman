#!/bin/bash

sudo snap alias dotnet-sdk.dotnet dotnet

dotnet build --configuration Release

dotnet test Watchman.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverageOutput=cover.xml

curl -Os https://uploader.codecov.io/latest/linux/codecov
chmod +x codecov

./codecov -f "/Devscord.DiscordFramework.UnitTests/cover.xml"
./codecov -f "/ProjectStructureTests/cover.xml"
./codecov -f "/Statsman.Tests/cover.xml"
./codecov -f "/Watchman.Discord.UnitTests/cover.xml"
./codecov -f "/Watchman.DomainModel.UnitTests/cover.xml"
./codecov -f "/Watchman.Integration.Tests/cover.xml"
