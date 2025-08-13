# Stage 1 - Dependencies
FROM node:22-alpine AS deps
WORKDIR /app

COPY package*.json ./
RUN --mount=type=cache,target=/root/.npm \
    npm install

# Stage 2 - Development
FROM node:22-alpine
WORKDIR /app

# Copy dependencies from deps stage
COPY --from=deps /app/node_modules ./node_modules

# Copy app source
COPY . .

EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host"]
