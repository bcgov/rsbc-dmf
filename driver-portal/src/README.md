#Driver-Portal

## Overview

Backend API for the Driver Portal application
See ClientApp/README.md for more information on the frontend

## Development Environment

The Driver Portal application has the following requirements:

- node
- .NET 6

## Docker Build

change directory to the repository root folder 'rsbc-dmf'

# build driver-portal-api
`docker build --tag driver-portal-api --build-arg BUILD_ID --build-arg BUILD_VERSION="1.0.2.40" . --file ./driver-portal/src/Dockerfile`

# build driver-portal-ui
`docker build . --file ./driver-portal/src/ClientApp/Dockerfile --tag driver-portal-ui`

# to debug docker
```bash
docker run -rm --name driver-portal-api driver-portal-api
# add tail -f entrypoint to docker otherwise it will not stay running
# useful if you want to look at folder structure
docker exec -it driver-portal-api bash
docker stop driver-portal-api
```