FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OAuthServer/OAuthServer.csproj", "OAuthServer/"]
COPY ["oauth-server.sln", "./"]

RUN dotnet restore .
COPY . .
RUN dotnet test -c Release
RUN dotnet publish "OAuthServer/OAuthServer.csproj" -c Release -o /app/publish --no-restore
COPY ["OAuthServer/Data/config.json", "/app/publish/Data/"]

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
COPY --from=build /app/publish .
RUN chmod a+rwx -R ./Data
ENTRYPOINT ["dotnet", "OAuthServer.dll"]