FROM node:20-alpine AS build
EXPOSE 8080

WORKDIR /src
ENV PATH=/src/node_modules/.bin:$PATH

COPY UI/package.json .
COPY UI/package-lock.json .
RUN npm install esbuild --ignore-scripts

COPY UI/ .
CMD ["npm", "run", "dev"]
