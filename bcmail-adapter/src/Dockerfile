#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

USER 0

RUN apt-get -y update && apt install -y libgdiplus
RUN ln -s /usr/lib/libgdiplus.so /lib/x86_64-linux-gnu/libgdiplus.so
RUN apt-get install -y --no-install-recommends zlib1g fontconfig libfreetype6 libx11-6 libxext6 libxrender1 wget gdebi
RUN wget https://github.com/wkhtmltopdf/wkhtmltopdf/releases/download/0.12.5/wkhtmltox_0.12.5-1.stretch_amd64.deb
RUN gdebi --n wkhtmltox_0.12.5-1.stretch_amd64.deb
RUN apt install -y libssl1.1
RUN ln -s /usr/local/lib/libwkhtmltox.so /usr/lib/libwkhtmltox.so

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY . .
RUN dotnet restore ./bcmail-adapter/src/Rsbc.Dmf.BcMailAdapter 

FROM build AS publish
RUN dotnet publish ./bcmail-adapter/src/Rsbc.Dmf.BcMailAdapter -c Release -o /app/publish

FROM base AS final
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