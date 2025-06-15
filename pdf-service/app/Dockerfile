# Stage 1: Build Stage
##################################################### 
FROM node:22-slim AS build

WORKDIR /app
RUN npm install -g npm@11.4.1
COPY package*.json ./

RUN npm ci --include=dev
COPY . ./
RUN npm run build

# Stage 2: Production Stage
#####################################################
FROM node:22-slim AS production

WORKDIR /app

# Embed browser in the image
ENV PLAYWRIGHT_BROWSERS_PATH=0
ENV NODE_ENV=production

COPY package*.json ./
RUN npm install -g npm@11.4.1 && \
    npm ci --omit=dev && \
    npx playwright install chromium --with-deps

# Copy built application and static assets
COPY --from=build /app/dist ./dist
COPY --from=build /app/static ./static
COPY openapi.yaml ./openapi.yaml
COPY fallback ./fallback

EXPOSE 3000

CMD ["node", "dist/server.js"]