FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
ARG BUILDID

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY . .
RUN dotnet restore "./doctors-portal/src/API/RSBC.DMF.DoctorsPortal.API/"
RUN dotnet test "./doctors-portal/src/API/RSBC.DMF.DoctorsPortal.API/" -c Release

FROM build AS publish
RUN echo $BUILDID
RUN dotnet publish /p:InformationalVersion=$BUILDID "./doctors-portal/src/API/RSBC.DMF.DoctorsPortal.API/RSBC.DMF.DoctorsPortal.API.csproj" -c Release -o /app/publish --no-restore 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "RSBC.DMF.DoctorsPortal.API.dll"]