FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build 
WORKDIR /app

COPY . ./
RUN dotnet publish Watchman.Web/Watchman.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Watchman.Web.dll"]
