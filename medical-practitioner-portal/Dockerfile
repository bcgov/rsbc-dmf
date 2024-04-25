FROM trion/ng-cli-karma AS ng-builder

COPY ./shared-portal-ui/ ./shared-portal-ui
COPY ./medical-practitioner-portal/ ./medical-practitioner-portal

WORKDIR /app/medical-practitioner-portal

RUN npm run build -- --base-href /medical-portal/ --deploy-url /medical-portal/ 

FROM caddy:alpine as final
COPY ./medical-practitioner-portal/Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/medical-practitioner-portal/ /site
ENV API_URL=
EXPOSE 2015