ARG BUILD_ID
ARG BUILD_VERSION
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_ID
ARG BUILD_VERSION
WORKDIR /src
COPY ["PidpAdapter.Client/PidpAdapter.Client", "PidpAdapter.Client/"]
COPY ["PidpAdapter.Service/PidpAdapter.Service.csproj", "PidpAdapter.Service/"]
COPY ["pidp-adapter.sln", "./"]

RUN dotnet restore .
COPY . .
RUN dotnet test -c Release
RUN dotnet publish "PidpAdapter.Service/PidpAdapter.Service.csproj" /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ARG BUILD_ID
ARG BUILD_VERSION

WORKDIR /app

#
# enable to install Visual Studio Debugger.
#
#RUN apt-get update -yq \
#    && apt-get install curl -yq \
#    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v vs2019 -l /app/vsdbg
#

EXPOSE 8080
ENV ASPNETCORE_URLS=https://*:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PidpAdapter.Service.dll"]