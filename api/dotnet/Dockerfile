FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore
RUN dotnet tool install --global dotnet-ef --version 3.1.0
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy everything else and build
COPY . ./
# COPY localhost.pfx ./out/localhost.pfx
RUN dotnet publish -c Release -o out
# RUN dotnet ef database update

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

COPY --from=build-env /app/out ./

ENTRYPOINT ["dotnet", "Dmft.Api.dll"]

EXPOSE 80 433
