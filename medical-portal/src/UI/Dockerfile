FROM trion/ng-cli-karma:17.0.10 AS ng-builder

COPY ./shared-portal-ui/ ./shared-portal-ui
COPY ./medical-portal/src/UI/ ./medical-portal/src/UI

WORKDIR /app/shared-portal-ui

RUN npm install
RUN npm run build

WORKDIR /app/medical-portal/src/UI

RUN npm install

RUN npm run build -- --base-href /medical-portal/ --deploy-url /medical-portal/ --configuration production --source-map

FROM caddy:alpine as final
COPY ./medical-portal/src/UI/Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /app/medical-portal/src/UI/dist/browser/ /site
ENV API_URL=
EXPOSE 2015
