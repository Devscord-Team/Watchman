FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build 
WORKDIR /app
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get install -y nodejs

COPY src/Watchman.Web/ClientApp/package*.json Watchman.Web/ClientApp/
RUN npm install src/Watchman.Web/ClientApp/

COPY . ./
RUN dotnet publish src/Watchman.Web/Watchman.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Watchman.Web.dll"]
