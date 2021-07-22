FROM trion/ng-cli-karma AS ng-builder
WORKDIR /src
COPY ./doctors-portal/package*.json ./
RUN npm install
COPY ./doctors-portal/ .
RUN npm run lint
RUN npm run test -- --no-watch --no-progress

RUN npm run build -- --base-href /sbc-dfp-medical-portal/ --deploy-url /sbc-dfp-medical-portal/ --configuration production

FROM caddy:alpine as final
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/doctors-portal/ /site
ENV API_URL=
EXPOSE 2015