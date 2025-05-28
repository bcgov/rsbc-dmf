# Stage 1: Build Stage
##################################################### 
FROM node:22-slim AS build

# Set working directory
WORKDIR /app

# update npm
RUN npm install -g npm@11.3.0

# Copy dependency files
COPY package*.json ./

# Install all dependencies (including devDependencies)
RUN npm install

# Install all dependencies
RUN npm ci --include=dev

# Copy the rest of the files
COPY . ./

# Build the application (TypeScript)
RUN npm run build

# Stage 2: Production Stage
#####################################################
FROM node:22-slim AS production

# Install OS deps for Playwright
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates fonts-liberation libasound2 libatk-bridge2.0-0 \
    libatk1.0-0 libcups2 libdbus-1-3 libdrm2 libgbm1 libgtk-3-0 \
    libnspr4 libnss3 libxcomposite1 libxdamage1 libxrandr2 \
    xdg-utils wget gnupg && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy only necessary files
COPY package*.json ./

# update npm
# install browser
# Install only production dependencies
RUN npm install -g npm@11.3.0 && \
    npm ci --omit=dev && \
    npx playwright install chromium

# Copy built application and static assets
COPY --from=build /app/dist ./dist
COPY --from=build /app/static ./static

# Expose the port
EXPOSE 3000

# Set environment variables
ENV NODE_ENV=production

# Start the server
CMD ["node", "dist/server.js"]