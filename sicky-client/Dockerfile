FROM node:12.16.1-alpine as build-stage
RUN apk add git
WORKDIR /app
COPY package.json package-lock.json /app/
RUN npm ci && npm cache clean --force
COPY public  /app/public
COPY src /app/src
RUN npm run build

FROM nginx:1.15
RUN apt-get update && apt-get install -y vim
COPY --from=build-stage /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
