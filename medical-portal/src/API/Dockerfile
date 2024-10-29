ARG BUILD_ID
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG BUILD_ID

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY . .
RUN dotnet restore "./medical-portal/src/API/RSBC.DMF.MedicalPortal.API/"
RUN dotnet test "./medical-portal/src/API/RSBC.DMF.MedicalPortal.API/" /p:InformationalVersion="$BUILD_ID" -c Release

FROM build AS publish
ARG BUILD_ID

RUN dotnet publish /p:InformationalVersion="$BUILD_ID" "./medical-portal/src/API/RSBC.DMF.MedicalPortal.API/RSBC.DMF.MedicalPortal.API.csproj" -c Release -o /app/publish --no-restore 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# copied from https://github.com/bcgov/emcr-dfa-portal/blob/develop/pdf-service/Dockerfile
RUN apt-get update && ln -s /usr/lib/libgdiplus.so /lib/x86_64-linux-gnu/libgdiplus.so
RUN apt-get install -y --no-install-recommends zlib1g fontconfig libfreetype6 libx11-6 libxext6 libxrender1 wget gdebi
RUN wget https://github.com/wkhtmltopdf/wkhtmltopdf/releases/download/0.12.5/wkhtmltox_0.12.5-1.stretch_amd64.deb
RUN wget http://archive.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2_amd64.deb
RUN dpkg -i libssl1.1_1.1.1f-1ubuntu2_amd64.deb
RUN gdebi --n wkhtmltox_0.12.5-1.stretch_amd64.deb
RUN ln -s /usr/local/lib/libwkhtmltox.so /usr/lib/libwkhtmltox.so

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "RSBC.DMF.MedicalPortal.API.dll"]