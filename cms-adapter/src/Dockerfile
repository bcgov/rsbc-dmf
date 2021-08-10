FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Rsbc.Dmf.CaseManagement.Service/Rsbc.Dmf.CaseManagement.Service.csproj", "Rsbc.Dmf.CaseManagement.Service/"]
COPY ["Rsbc.Dmf.CaseManagement/Rsbc.Dmf.CaseManagement.csproj", "Rsbc.Dmf.CaseManagement/"]
COPY ["Rsbc.Dmf.CaseManagement.Tests/Rsbc.Dmf.CaseManagement.Tests.csproj", "Rsbc.Dmf.CaseManagement.Tests/"]
COPY ["Rsbc.Dmf.CaseManagement.sln", "./"]

RUN dotnet restore .
COPY . .
RUN dotnet test -c Release
RUN dotnet publish "Rsbc.Dmf.CaseManagement.Service/Rsbc.Dmf.CaseManagement.Service.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=https://*:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Rsbc.Dmf.CaseManagement.Service.dll"]