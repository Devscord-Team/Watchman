FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build 
WORKDIR /app

RUN apt-get update
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get install -y nodejs

COPY Watchman.Web/ClientApp/package*.json Watchman.Web/ClientApp/
RUN npm install Watchman.Web/ClientApp/

COPY . ./
RUN dotnet publish Watchman.Web/Watchman.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Watchman.Web.dll"]
