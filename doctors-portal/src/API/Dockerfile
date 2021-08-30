FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RSBC.DMF.DoctorsPortal.API/RSBC.DMF.DoctorsPortal.API.csproj", "RSBC.DMF.DoctorsPortal.API/"]
COPY ["RSBC.Tests.Unit.DMF.DoctorsPortal.API/RSBC.Tests.Unit.DMF.DoctorsPortal.API.csproj", "RSBC.Tests.Unit.DMF.DoctorsPortal.API/"]
COPY ["doctors-portal.sln", "./"]

RUN dotnet restore .
COPY . .
RUN dotnet test -c Release
RUN dotnet publish "RSBC.DMF.DoctorsPortal.API/RSBC.DMF.DoctorsPortal.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RSBC.DMF.DoctorsPortal.API.dll"]