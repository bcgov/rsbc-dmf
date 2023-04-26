### Global args
ARG PORT 2015

### Deploy
FROM caddy:2.4.6-alpine@sha256:15e576e7d00b1f41a648c5295a03677c24da5fede09131edcb1e6d809c7dc8aa
ARG PORT

# Copy Caddyfile and static files
COPY Caddyfile /etc/caddy/Caddyfile
COPY html/ /srv

# Port, Healthcheck and user
EXPOSE ${PORT}
HEALTHCHECK --interval=30s --timeout=3s \
    CMD curl -f http://localhost/:${PORT} || exit 1
USER app

