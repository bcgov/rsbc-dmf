FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
ARG BUILD_ID

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY . .
RUN dotnet restore "./doctors-portal/src/API/RSBC.DMF.DoctorsPortal.API/"
RUN dotnet test "./doctors-portal/src/API/RSBC.DMF.DoctorsPortal.API/" /p:InformationalVersion=$BUILD_ID -c Release

FROM build AS publish

RUN dotnet publish /p:InformationalVersion=$BUILD_ID "./doctors-portal/src/API/RSBC.DMF.DoctorsPortal.API/RSBC.DMF.DoctorsPortal.API.csproj" -c Release -o /app/publish --no-restore 

RUN echo "123 $BUILD_ID" > /app/publish/build_id

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "RSBC.DMF.DoctorsPortal.API.dll"]