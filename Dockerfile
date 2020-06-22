FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster-arm32v7 AS build 
WORKDIR /app
COPY . ./

RUN dotnet publish Watchman.Web.Server/Watchman.Web.Server.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim-arm32v7
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Watchman.Web.Server.dll"]