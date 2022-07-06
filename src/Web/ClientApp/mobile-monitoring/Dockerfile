FROM node:latest AS build
WORKDIR /usr/app/client
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:latest
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /usr/app/client/dist/mobile-monitoring /usr/share/nginx/html