#Driver-Portal

## Run

To start the app on port 3020, which OAuth has a redirect uri entry for, and to run with ssl certificate, use the following:
npm start

## Development Environment

The Driver Portal application has the following requirements:

- node
- .NET 6

## Docker Build

cd rsbc-dmf

# build driver-portal-api backend
docker build --tag driver-portal-api --build-arg BUILD_ID --build-arg BUILD_VERSION="1.0.2.40" . --file ./driver-portal/src/Dockerfile

# build driver-portal-api frontend
docker build . --file ./driver-portal/src/ClientApp/Dockerfile --tag driver-portal-api

# to debug docker make sure you add tail -f entrypoint
docker run -rm --name driver-portal-api driver-portal-api
docker exec -it driver-portal-api bash
docker stop driver-portal-api
docker rm driver-portal-api
docker image rm driver-portal-api
