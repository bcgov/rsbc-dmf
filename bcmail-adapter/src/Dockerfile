#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

RUN apk --no-cache add curl

RUN \
  echo "**** install packages ****" && \
  if [ -z ${LIBREOFFICE_VERSION+x} ]; then \
    LIBREOFFICE_VERSION=$(curl -sL "http://dl-cdn.alpinelinux.org/alpine/v3.16/community/x86_64/APKINDEX.tar.gz" | tar -xz -C /tmp \
    && awk '/^P:libreoffice$/,/V:/' /tmp/APKINDEX | sed -n 2p | sed 's/^V://'); \
  fi && \
  apk add --no-cache \
    libreoffice==${LIBREOFFICE_VERSION} \
    openjdk8-jre \
    tint2 && \
  echo "**** openbox tweaks ****" && \
  sed -i \
    's/NLMC/NLIMC/g' \
    /etc/xdg/openbox/rc.xml && \
  echo "**** cleanup ****" && \
  rm -rf \
    /tmp/*

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

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "Rsbc.Dmf.BcMailAdapter.dll"]