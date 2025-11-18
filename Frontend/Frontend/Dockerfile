# ---------- 建置階段 ----------
FROM node:20-alpine AS build
WORKDIR /app

# 複製 package.json 並安裝依賴
COPY package*.json ./
RUN npm install

# 複製程式碼並執行 build
COPY . .

# 給 /app 權限
RUN chmod -R 777 /app

RUN npm run build

# ---------- 部署階段 ----------
FROM nginx:stable-alpine
# 移除預設的 nginx 靜態頁面
RUN rm -rf /usr/share/nginx/html/*

# 複製打包後的前端檔案
COPY --from=build /app/dist /usr/share/nginx/html

# 複製 nginx 設定檔（我們下面會建立這個）
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
