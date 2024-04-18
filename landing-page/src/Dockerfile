FROM caddy:latest

# Copy Caddyfile and static files
COPY Caddyfile /etc/caddy/Caddyfile
COPY html/ /srv

# Port, Healthcheck and user
ENV API_URL=
EXPOSE 2015
USER 1001

