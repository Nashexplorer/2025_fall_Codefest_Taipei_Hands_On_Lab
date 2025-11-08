# 快速啟動指南

## 前置需求

- .NET 8.0 SDK 或更高版本
- MySQL 8.0 資料庫（已提供遠端連接）

## 快速開始

### 1. 克隆或下載專案

```bash
cd TaipeiSportsApi
```

### 2. 還原 NuGet 套件

```bash
dotnet restore
```

### 3. 檢查資料庫連接設定

確認 `appsettings.json` 中的連接字串正確：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=xxxxx;"
  }
}
```

### 4. 執行應用程式

```bash
dotnet run
```

或使用監視模式（程式碼變更時自動重新載入）：

```bash
dotnet watch run
```

### 5. 訪問 API

應用程式啟動後，可以通過以下方式訪問：

- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger

## 測試 API

### 使用 Swagger UI

1. 在瀏覽器中打開 https://localhost:5001/swagger
2. 展開任意端點
3. 點擊 "Try it out"
4. 填寫參數
5. 點擊 "Execute"

### 使用 HTTP 檔案（Visual Studio / VS Code）

專案包含 `TaipeiSportsApi.http` 檔案，可以直接在 IDE 中執行：

1. 在 VS Code 中安裝 "REST Client" 擴充套件
2. 開啟 `TaipeiSportsApi.http` 檔案
3. 點擊 "Send Request" 連結

### 使用 curl

```bash
# 取得所有停車場狀態
curl -X GET "http://localhost:5000/api/parking-status?page=1&pageSize=10"

# 取得特定停車場最新狀態
curl -X GET "http://localhost:5000/api/parking-status/P001"

# 新增停車場狀態
curl -X POST "http://localhost:5000/api/parking-status" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "P001",
    "updateTime": "2025-11-05T10:30:00",
    "availableCar": 50,
    "availableMotor": 20
  }'
```

### 使用 PowerShell

```powershell
# 取得所有停車場狀態
Invoke-RestMethod -Uri "http://localhost:5000/api/parking-status?page=1&pageSize=10" -Method Get

# 取得特定停車場最新狀態
Invoke-RestMethod -Uri "http://localhost:5000/api/parking-status/P001" -Method Get

# 新增停車場狀態
$body = @{
    id = "P001"
    updateTime = "2025-11-05T10:30:00"
    availableCar = 50
    availableMotor = 20
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/parking-status" -Method Post -Body $body -ContentType "application/json"
```

## API 端點總覽

| 方法 | 端點 | 說明 |
|------|------|------|
| GET | `/api/parking-status` | 取得所有停車場狀態（分頁） |
| GET | `/api/parking-status/{id}` | 取得特定停車場最新狀態 |
| GET | `/api/parking-status/{id}/{updateTime}` | 取得特定時間的停車場狀態 |
| GET | `/api/parking-status/{id}/history` | 取得停車場歷史記錄 |
| POST | `/api/parking-status` | 新增停車場狀態 |
| PUT | `/api/parking-status/{id}/{updateTime}` | 更新停車場狀態 |
| DELETE | `/api/parking-status/{id}/{updateTime}` | 刪除停車場狀態 |
| GET | `/api/parking-ids` | 取得所有停車場 ID |

## 常見問題

### 1. 無法連接到資料庫

確認：
- MySQL 伺服器正在運行
- 防火牆允許連接到端口 3306
- 連接字串中的憑證正確

### 2. HTTPS 憑證錯誤

在開發環境中信任本機憑證：

```bash
dotnet dev-certs https --trust
```

### 3. 端口已被佔用

修改 `Properties/launchSettings.json` 中的端口設定：

```json
{
  "applicationUrl": "https://localhost:5051;http://localhost:5050"
}
```

## 開發建議

### 啟用熱重載

使用 `dotnet watch` 命令自動重新編譯和重新啟動應用程式：

```bash
dotnet watch run
```

### 查看詳細日誌

修改 `appsettings.Development.json`：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### 使用 Entity Framework Core 工具

```bash
# 安裝全域工具
dotnet tool install --global dotnet-ef

# 查看資料庫上下文資訊
dotnet ef dbcontext info

# 列出所有 Migrations
dotnet ef migrations list
```

## 生產環境部署

### 發佈應用程式

```bash
dotnet publish -c Release -o ./publish
```

### 環境變數

在生產環境中，建議使用環境變數來儲存連接字串：

```bash
export ConnectionStrings__DefaultConnection="Server=...;Database=...;"
```

或在 `appsettings.Production.json` 中設定（不要提交到版本控制）。

## 下一步

- 閱讀完整的 [README.md](README.md)
- 探索 [Swagger UI](https://localhost:5001/swagger)
- 查看 API 範例請求 [TaipeiSportsApi.http](TaipeiSportsApi.http)

## 支援

如有問題，請參考：
- [ASP.NET Core 文檔](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core 文檔](https://docs.microsoft.com/ef/core)
- [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)

