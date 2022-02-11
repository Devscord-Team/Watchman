#!/bin/bash

sudo snap alias dotnet-sdk.dotnet dotnet

dotnet build --configuration Release

dotnet test Watchman.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

curl -Os https://uploader.codecov.io/latest/linux/codecov
chmod +x codecov

./codecov -f "/Devscord.DiscordFramework.UnitTests/coverage.opencover.xml"
./codecov -f "/ProjectStructureTests/coverage.opencover.xml"
./codecov -f "/Statsman.Tests/coverage.opencover.xml"
./codecov -f "/Watchman.Discord.UnitTests/coverage.opencover.xml"
./codecov -f "/Watchman.DomainModel.UnitTests/coverage.opencover.xml"
./codecov -f "/Watchman.Integration.Tests/coverage.opencover.xml"
