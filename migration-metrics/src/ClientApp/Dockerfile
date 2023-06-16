FROM trion/ng-cli-karma AS ng-builder
WORKDIR /src
COPY ./migration-metrics/src/ClientApp/package*.json ./
RUN npm install
COPY ./migration-metrics/src/ClientApp/ .
RUN npm run lint
RUN npm run test -- --no-watch --no-progress

RUN npm run build -- --base-href /rsbc-dfp-migration-metrics/ --deploy-url /rsbc-dfp-migration-metrics/ --configuration production

FROM caddy:alpine as final
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/migration-metrics/ /site
ENV API_URL=
EXPOSE 2015