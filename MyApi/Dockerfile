# 建立執行環境
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 建立階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 先複製 csproj 進行 restore（加快建置）
COPY *.csproj ./
RUN dotnet restore

# 再複製其餘原始碼
COPY . .
RUN dotnet publish -c Release -o /app/publish

# 最終階段
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyApi.dll"]
