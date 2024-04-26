FROM trion/ng-cli-karma AS ng-builder

COPY ./shared-portal-ui/ ./shared-portal-ui
COPY ./medical-practitioner-portal/ ./medical-practitioner-portal

WORKDIR /app/shared-portal-ui

RUN npm install 

WORKDIR /app/medical-practitioner-portal

RUN npm install 

RUN npm run build -- --base-href /medical-portal/ --deploy-url /medical-portal/ 

FROM caddy:alpine as final
COPY ./medical-practitioner-portal/Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /app/medical-practitioner-portal/dist/browser/ /site
ENV API_URL=
EXPOSE 2015