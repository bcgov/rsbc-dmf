FROM trion/ng-cli-karma:17.0.10 AS ng-builder

COPY ./shared-portal-ui/ ./shared-portal-ui
COPY ./partner-portal/src/UI/ ./partner-portal/src/UI

WORKDIR /app/shared-portal-ui

RUN npm install
RUN npm run build

WORKDIR /app/partner-portal/src/UI

RUN npm install

RUN npm run build -- --base-href /partner-portal/ --deploy-url /partner-portal/ --configuration production --source-map

FROM caddy:alpine as final
COPY ./partner-portal/src/UI/Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /app/partner-portal/src/UI/dist/browser/ /site
ENV API_URL=
EXPOSE 2015
