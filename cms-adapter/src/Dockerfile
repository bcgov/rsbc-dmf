FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RSBC.DMF.CaseManagement.Service/RSBC.DMF.CaseManagement.Service.csproj", "RSBC.DMF.CaseManagement.Service/"]
COPY ["RSBC.DMF.CaseManagement/RSBC.DMF.CaseManagement.csproj", "RSBC.DMF.CaseManagement/"]
COPY ["RSBC.DMF.CaseManagement.Tests/RSBC.DMF.CaseManagement.Tests.csproj", "RSBC.DMF.CaseManagement.Tests/"]
COPY ["RSBC.DMF.CaseManagement.sln", "./"]

RUN dotnet restore .
COPY . .
RUN dotnet test -c Release
RUN dotnet publish "RSBC.DMF.CaseManagement.Service/RSBC.DMF.CaseManagement.Service.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RSBC.DMF.CaseManagement.Service"]