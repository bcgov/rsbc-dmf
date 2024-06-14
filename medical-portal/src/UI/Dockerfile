FROM trion/ng-cli-karma:17.0.10 AS ng-builder

COPY ./shared-portal-ui/ ./shared-portal-ui
COPY ./medical-portal/src/UI/ ./medical-portal

WORKDIR /app/shared-portal-ui

RUN npm install

WORKDIR /app/medical-portal

RUN npm install

WORKDIR /app/

RUN npm run build -- --base-href /medical-portal/ --deploy-url /medical-portal/ --configuration production --source-map

FROM caddy:alpine as final
COPY ./medical-portal/src/UI/Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /app/medical-portal/dist/browser/ /site
ENV API_URL=
EXPOSE 2015
