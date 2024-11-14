# version 17.1.0 introduced breaking changes to RUN npm install, try change 17.0.10 tag to latest on subsequent versions
FROM trion/ng-cli-karma:17.0.10 AS ng-builder

COPY ./shared-portal-ui/ ./shared-portal-ui
COPY ./driver-portal/src/UI ./driver-portal/src/UI

WORKDIR /app/shared-portal-ui

RUN npm install
RUN npm run build

WORKDIR /app/driver-portal/src/UI

RUN npm install

RUN npm run build -- --base-href /driver-portal/ --deploy-url /driver-portal/ --configuration production

FROM caddy:latest as final
COPY ./driver-portal/src/UI/Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /app/driver-portal/src/UI/dist/ /site
ENV API_URL=
EXPOSE 2015

USER 1001
