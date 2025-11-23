# Stage 1 — Build Angular
FROM node:20 AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build -- --configuration production

# Stage 2 — Serve with Nginx
FROM nginx:stable

# Copy from the browser subfolder (Angular 17+)
COPY --from=build /app/dist/web-ui/browser /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]