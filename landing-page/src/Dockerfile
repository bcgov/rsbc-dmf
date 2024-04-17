### Global args
ARG PORT 2015

### Deploy
FROM caddy:latest
ARG PORT

# Copy Caddyfile and static files
COPY Caddyfile /etc/caddy/Caddyfile
COPY html/ /srv

# Port, Healthcheck and user
EXPOSE ${PORT}
HEALTHCHECK --interval=30s --timeout=3s \
    CMD curl -f http://localhost/:${PORT} || exit 1
USER 1001

