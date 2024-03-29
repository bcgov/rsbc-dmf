#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG BUILD_ID
ARG BUILD_VERSION
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG BUILD_ID
ARG BUILD_VERSION
USER 0

RUN apt-get -y update && apt install -y libgdiplus
RUN ln -s /usr/lib/libgdiplus.so /lib/x86_64-linux-gnu/libgdiplus.so
RUN apt-get install -y --no-install-recommends zlib1g fontconfig libfreetype6 libx11-6 libxext6 libxrender1 wget gdebi

RUN wget http://http.us.debian.org/debian/pool/main/o/openssl/libssl1.1_1.1.1w-0+deb11u1_amd64.deb
RUN gdebi --n libssl1.1_1.1.1w-0+deb11u1_amd64.deb
RUN wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6.1-2/wkhtmltox_0.12.6.1-2.bullseye_amd64.deb
RUN gdebi --n wkhtmltox_0.12.6.1-2.bullseye_amd64.deb
RUN apt install -y libssl1.1
RUN ln -s /usr/local/lib/libwkhtmltox.so /usr/lib/libwkhtmltox.so

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_ID
ARG BUILD_VERSION
COPY . .
RUN dotnet restore ./bcmail-adapter/src/Rsbc.Dmf.BcMailAdapter 

FROM build AS publish
RUN dotnet publish ./bcmail-adapter/src/Rsbc.Dmf.BcMailAdapter /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID"  -c Release -o /app/publish

FROM base AS final
ARG BUILD_ID
ARG BUILD_VERSION
WORKDIR /app
COPY --from=publish /app/publish .

# RUN apt-get update -yq \
#    && apt-get install curl -yq \
#	&& curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v vs2019 -l /app/vsdbg

USER 1001

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENV HOME=/tmp/
ENTRYPOINT ["dotnet", "Rsbc.Dmf.BcMailAdapter.dll"]