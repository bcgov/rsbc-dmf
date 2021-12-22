FROM trion/ng-cli-karma AS ng-builder
WORKDIR /src
COPY ./medical-portal/package*.json ./
RUN npm install
COPY ./medical-portal/ .
RUN npm run lint
RUN npm run test -- --no-watch --no-progress

RUN npm run build -- --base-href /rsbc-dfp-medical-portal/ --deploy-url /rsbc-dfp-medical-portal/ --configuration production

FROM caddy:alpine as final
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/medical-portal/ /site
ENV API_URL=
EXPOSE 2015