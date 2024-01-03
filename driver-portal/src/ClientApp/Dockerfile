FROM trion/ng-cli-karma AS ng-builder
COPY ./package*.json ./

COPY ./shared-portal-ui/ ./shared-portal-ui
COPY ./driver-portal/src/ClientApp ./driver-portal/src/ClientApp

WORKDIR ./driver-portal/src/ClientApp

RUN npm install

#RUN npm run lint
#RUN npm run test -- --no-watch --no-progress

RUN npm run build -- --base-href /driver-portal/ --deploy-url /driver-portal/ --configuration production

FROM caddy:latest as final
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/ /site
ENV API_URL=
EXPOSE 2015

USER 1001