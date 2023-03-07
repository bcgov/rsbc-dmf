#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

USER 0

RUN apt-get -y update && apt install -y -q \
		libreoffice \
        default-jre-headless \
		libreoffice-writer \
		ure \
		libreoffice-java-common \
		libreoffice-core \
		libreoffice-common \
		fonts-opensymbol \
		hyphen-en-us \
		fonts-dejavu \
		fonts-dejavu-core \
		fonts-dejavu-extra \
		fonts-droid-fallback \
		fonts-dustin \
		fonts-f500 \
		fonts-fanwood \
		fonts-freefont-ttf \
		fonts-liberation \
		fonts-lmodern \
		fonts-lyx \
		fonts-sil-gentium \
		fonts-texgyre \
		fonts-tlwg-purisa 

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
ENTRYPOINT ["dotnet", "Rsbc.Dmf.BcMailAdapter.dll"]