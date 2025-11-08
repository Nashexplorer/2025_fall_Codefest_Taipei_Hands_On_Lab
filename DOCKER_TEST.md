# 本地 Docker 測試指南

在部署到 Cloud Run 之前，建議先在本地測試 Docker 映像。

## 前置需求

- Docker Desktop 已安裝並運行
- 應用程式已成功建置 (`dotnet build`)

## 🧪 本地測試步驟

### 1. 建置 Docker 映像

```powershell
# Windows PowerShell
docker build -t taipei-sports-api:local .
```

```bash
# Linux/macOS
docker build -t taipei-sports-api:local .
```

### 2. 執行容器

#### 使用預設連接字串

```powershell
docker run -p 8080:8080 taipei-sports-api:local
```

#### 使用自訂環境變數

```powershell
docker run -p 8080:8080 `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e ConnectionStrings__DefaultConnection="Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=Showcase@2025!;" `
  taipei-sports-api:local
```

Linux/macOS:
```bash
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e "ConnectionStrings__DefaultConnection=Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=Showcase@2025!;" \
  taipei-sports-api:local
```

### 3. 測試 API

開啟瀏覽器訪問：

- **Swagger UI**: http://localhost:8080/swagger
- **API 端點**: http://localhost:8080/api/parking-status

或使用命令列測試：

```powershell
# PowerShell
Invoke-RestMethod -Uri "http://localhost:8080/api/parking-ids" -Method Get

# 測試分頁查詢
Invoke-RestMethod -Uri "http://localhost:8080/api/parking-status?page=1&pageSize=5" -Method Get
```

```bash
# Linux/macOS/Git Bash
curl http://localhost:8080/api/parking-ids
curl "http://localhost:8080/api/parking-status?page=1&pageSize=5"
```

### 4. 查看容器日誌

```powershell
# 查看即時日誌
docker logs -f <container-id>

# 查看最後 50 行日誌
docker logs --tail 50 <container-id>
```

取得容器 ID:
```powershell
docker ps
```

### 5. 停止容器

```powershell
# 停止所有運行中的容器
docker stop $(docker ps -q)

# 或停止特定容器
docker stop <container-id>
```

### 6. 清理

```powershell
# 刪除容器
docker rm <container-id>

# 刪除映像
docker rmi taipei-sports-api:local

# 清理所有未使用的映像、容器和網路
docker system prune -a
```

## 🔍 進階測試

### 使用 Docker Compose（可選）

建立 `docker-compose.yml`:

```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=Showcase@2025!;
    restart: unless-stopped
```

執行：

```powershell
# 啟動
docker-compose up -d

# 查看日誌
docker-compose logs -f

# 停止
docker-compose down
```

### 檢查映像大小

```powershell
docker images taipei-sports-api:local
```

理想情況下，映像大小應該在 200-300 MB 之間。

### 進入容器內部（除錯）

```powershell
docker exec -it <container-id> /bin/bash
```

## 🐛 常見問題

### 問題 1: 建置失敗 "unable to prepare context"

**原因**: Docker 無法讀取檔案

**解決方案**:
```powershell
# 確保在專案根目錄
cd C:\Users\BARRYYANG\source\repos\TaipeiSportsApi

# 檢查 Dockerfile 是否存在
Test-Path Dockerfile
```

### 問題 2: 容器啟動後立即退出

**原因**: 應用程式錯誤或配置問題

**解決方案**:
```powershell
# 查看退出容器的日誌
docker logs <container-id>

# 或以互動模式執行
docker run -it -p 8080:8080 taipei-sports-api:local
```

### 問題 3: 無法連接到 localhost:8080

**原因**: 端口被佔用或容器未正確啟動

**解決方案**:
```powershell
# 檢查端口是否被佔用
netstat -ano | findstr :8080

# 使用不同端口
docker run -p 9090:8080 taipei-sports-api:local

# 然後訪問 http://localhost:9090
```

### 問題 4: 資料庫連接失敗

**原因**: 網路問題或憑證錯誤

**解決方案**:
1. 確認本機可以連接到資料庫：
   ```powershell
   Test-NetConnection -ComputerName 34.81.245.32 -Port 3306
   ```

2. 檢查環境變數是否正確設定

3. 查看容器日誌中的詳細錯誤訊息

### 問題 5: 映像太大

**原因**: 包含了不必要的檔案

**解決方案**:
- 確保 `.dockerignore` 檔案存在並正確配置
- 檢查是否使用了多階段建置（Dockerfile 已配置）

## ✅ 測試檢查清單

在部署到 Cloud Run 前，確認：

- [ ] Docker 映像成功建置
- [ ] 容器可以正常啟動
- [ ] 可以訪問 Swagger UI
- [ ] API 端點正常回應
- [ ] 資料庫連接正常
- [ ] 沒有錯誤日誌
- [ ] 記憶體使用合理（< 512 MB）

## 📊 效能測試

### 簡單負載測試

使用 PowerShell 進行簡單測試：

```powershell
# 發送 100 次請求
1..100 | ForEach-Object {
    Invoke-RestMethod -Uri "http://localhost:8080/api/parking-ids" -Method Get
    Write-Host "Request $_"
}
```

### 使用 Apache Bench (ab)

```bash
# 100 個請求，10 個並發
ab -n 100 -c 10 http://localhost:8080/api/parking-ids
```

### 監控資源使用

```powershell
# 查看容器資源使用統計
docker stats <container-id>
```

## 🎯 下一步

本地測試通過後，就可以部署到 Cloud Run 了！

請參閱：
- [快速部署指南](CLOUD_RUN_QUICKSTART.md)
- [完整部署指南](DEPLOYMENT_GUIDE.md)

