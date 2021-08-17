#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY . .
RUN dotnet restore ./document-storage-adapter/src/Pssg.DocumentStorageAdapter 

FROM build AS publish
RUN dotnet publish ./document-storage-adapter/src/Pssg.DocumentStorageAdapter -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update -yq \
    && apt-get install curl -yq \
	&& curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v vs2017u5 -l /app/vsdbg

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "Pssg.DocumentStorageAdapter.dll"]