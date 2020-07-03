FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build 
WORKDIR /app
COPY . ./

RUN dotnet publish Watchman.Web/Watchman.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim-arm32v7
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Watchman.Web.dll"]
