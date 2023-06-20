FROM trion/ng-cli-karma AS ng-builder
WORKDIR /src
COPY ./package*.json ./
RUN npm install
COPY ./ .
#RUN npm run lint
#RUN npm run test -- --no-watch --no-progress

RUN npm run build -- --base-href /driver-portal/ --deploy-url /driver-portal/ --configuration production

FROM caddy:latest as final
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/ /site
ENV API_URL=
EXPOSE 2015

USER 1001