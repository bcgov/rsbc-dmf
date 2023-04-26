FROM caddy:2.6.3-alpine as final
COPY Caddyfile /etc/caddy/Caddyfile
COPY html/ /site/
ENV LOG_LEVEL=INFO
