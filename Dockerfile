FROM mcr.microsoft.com/dotnet/core/sdk:3.1.102-buster-arm32v7 AS build
WORKDIR /app
COPY Watchman.Discord/Watchman.Discord.csproj Watchman.Discord/Watchman.Discord.csproj
COPY Devscord.DiscordFramework/Devscord.DiscordFramework.csproj Devscord.DiscordFramework/Devscord.DiscordFramework.csproj
COPY Watchman.Common/Watchman.Common.csproj Watchman.Common/Watchman.Common.csproj
COPY Watchman.Cqrs/Watchman.Cqrs.csproj Watchman.Cqrs/Watchman.Cqrs.csproj
COPY Watchman.DomainModel/Watchman.DomainModel.csproj Watchman.DomainModel/Watchman.DomainModel.csproj
COPY Watchman.Integrations/Watchman.Integrations.csproj Watchman.Integrations/Watchman.Integrations.csproj
COPY . ./

RUN dotnet build Watchman.Discord/Watchman.Discord.csproj

RUN dotnet publish Watchman.Discord/Watchman.Discord.csproj -c Release -o out
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.102-buster-arm32v7
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Watchman.Discord.dll"]
