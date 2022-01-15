FROM node:13-alpine as build
WORKDIR /app
ENV PATH /app/node_modules/.bin:$PATH
COPY . /app
RUN npm install react-scripts@5.0.0 -g -silent
RUN npm install
RUN npm i -D --save-exact mini-css-extract-plugin@2.4.5
RUN npm run build

FROM nginx:1.16.0-alpine
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]