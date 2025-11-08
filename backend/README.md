# 共餐API

這是一個使用 ASP.NET Core 8 Minimal API 開發的共餐活動查詢 API。

## 技術堆疊

- **框架**: ASP.NET Core 8 Minimal API
- **資料庫**: MySQL 8.0
- **ORM**: Entity Framework Core with Pomelo MySQL Provider
- **文檔**: Swagger/OpenAPI

## 資料庫連接

- **主機**: 34.81.245.32
- **端口**: 3306
- **資料庫**: taipeipass_db
- **用戶**: demo_user

## 安裝步驟

1. 確保已安裝 .NET 8 SDK
2. 還原 NuGet 套件:
```bash
dotnet restore
```

3. 執行應用程式:
```bash
dotnet run
```

應用程式將在 `https://localhost:5001` (HTTPS) 和 `http://localhost:5000` (HTTP) 上運行。

## API 端點

### 1. 取得所有停車場狀態（分頁）
```
GET /api/parking-status?page=1&pageSize=10
```

### 2. 根據 ID 取得最新的停車場狀態
```
GET /api/parking-status/{id}
```

### 3. 根據 ID 和更新時間取得特定停車場狀態
```
GET /api/parking-status/{id}/{updateTime}
```
範例: `/api/parking-status/P001/2025-11-05T10:30:00`

### 4. 取得特定停車場的歷史記錄
```
GET /api/parking-status/{id}/history?page=1&pageSize=20
```

### 5. 新增停車場狀態
```
POST /api/parking-status
Content-Type: application/json

{
  "id": "P001",
  "updateTime": "2025-11-05T10:30:00",
  "availableCar": 50,
  "availableMotor": 20,
  "availableBus": 5,
  "availablePregnancy": 3,
  "availableHandicap": 2,
  "availableHeavyMotor": 1,
  "chargeTotal": 100,
  "chargeBusy": 30,
  "chargeIdle": 70
}
```

### 6. 更新停車場狀態
```
PUT /api/parking-status/{id}/{updateTime}
Content-Type: application/json

{
  "availableCar": 45,
  "availableMotor": 18
}
```

### 7. 刪除停車場狀態
```
DELETE /api/parking-status/{id}/{updateTime}
```

### 8. 取得所有不重複的停車場 ID
```
GET /api/parking-ids
```

### 9. 取得所有共餐活動（分頁）
```
GET /api/meals?page=1&pageSize=10
```

### 10. 根據 ID 取得共餐活動
```
GET /api/meals/{id}
```
範例: `/api/meals/meal-001`

回應範例:
```json
{
  "id": "meal-001",
  "title": "蔬食共餐：減碳午餐會",
  "description": "歡迎大家帶一道蔬食料理，一起分享、交流、減碳！",
  "image_url": "https://example.com/meal001.jpg",
  "location": "台北市信義區松高路 11 號",
  "latitude": 25.033,
  "longitude": 121.5645,
  "host_user_id": "user-456",
  "capacity": 10,
  "current_participants": 4,
  "diet_type": "素食",
  "is_dine_in": true,
  "start_time": "2025-11-20T12:00:00",
  "end_time": "2025-11-20T14:00:00",
  "signup_deadline": "2025-11-18T23:59:59",
  "created_at": "2025-11-08T15:00:00",
  "updated_at": "2025-11-08T15:00:00",
  "tags": ["蔬食", "永續", "社區"],
  "status": "open",
  "notes": "請自備環保餐具",
  "reserved1": null,
  "reserved2": null,
  "reserved3": null,
  "reserved4": null,
  "reserved5": null
}
```

## 資料模型

### TaipeiParkingStatus

| 欄位名稱 | 類型 | 說明 |
|---------|------|------|
| Id | string | 停車場 ID (主鍵) |
| UpdateTime | DateTime | 更新時間 (主鍵) |
| AvailableCar | int? | 可用汽車車位 |
| AvailableMotor | int? | 可用機車車位 |
| AvailableBus | int? | 可用巴士車位 |
| AvailablePregnancy | int? | 可用孕婦車位 |
| AvailableHandicap | int? | 可用身障車位 |
| AvailableHeavyMotor | int? | 可用重型機車車位 |
| ChargeTotal | int? | 充電樁總數 |
| ChargeBusy | int? | 使用中充電樁數 |
| ChargeIdle | int? | 閒置充電樁數 |
| Reserved1-10 | string? | 保留欄位 |

**注意**: 此表使用複合主鍵 (Id + UpdateTime)

### MealEvent

| 欄位名稱 | 類型 | 說明 |
|---------|------|------|
| Id | string | 共餐活動 ID (主鍵) |
| Title | string | 活動標題 (必填) |
| Description | string? | 活動描述 |
| ImageUrl | string? | 活動圖片 URL |
| Location | string? | 活動地點 |
| Latitude | decimal(10,6)? | 緯度 |
| Longitude | decimal(10,6)? | 經度 |
| HostUserId | string | 主辦人用戶 ID (必填) |
| Capacity | int | 活動人數上限 (預設值: 0) |
| CurrentParticipants | int | 目前參與人數 (預設值: 0) |
| DietType | string? | 飲食習慣 (例如：素食、葷食) |
| IsDineIn | bool | 是否可內用 (預設值: true) |
| StartTime | DateTime | 活動開始時間 (必填) |
| EndTime | DateTime | 活動結束時間 (必填) |
| SignupDeadline | DateTime? | 報名截止時間 |
| CreatedAt | DateTime? | 建立時間 |
| UpdatedAt | DateTime? | 更新時間 |
| Tags | string? | 活動標籤 (JSON 格式陣列) |
| Status | string | 活動狀態 (open, closed, cancelled, full，預設值: open) |
| Notes | string? | 備註 |
| Reserved1 | string? | 預留欄位 1 |
| Reserved2 | string? | 預留欄位 2 |
| Reserved3 | string? | 預留欄位 3 |
| Reserved4 | string? | 預留欄位 4 |
| Reserved5 | string? | 預留欄位 5 |

## Swagger UI

啟動應用程式後，可以通過以下網址訪問 Swagger UI:
```
https://localhost:5001/swagger
```

## 開發工具

### 使用 Entity Framework Core 工具

如果需要進行資料庫遷移或其他操作:

```bash
# 安裝 EF Core 工具
dotnet tool install --global dotnet-ef

# 創建遷移
dotnet ef migrations add InitialCreate

# 更新資料庫
dotnet ef database update
```

## 設定檔

資料庫連接字串位於 `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=xxxxx;"
  }
}
```

## 測試 API

使用 curl 測試 API:

```bash
# 取得所有停車場狀態
curl -X GET "https://localhost:5001/api/parking-status?page=1&pageSize=10"

# 取得特定停車場最新狀態
curl -X GET "https://localhost:5001/api/parking-status/P001"

# 取得所有停車場 ID
curl -X GET "https://localhost:5001/api/parking-ids"

# 取得所有共餐活動
curl -X GET "https://localhost:5001/api/meals?page=1&pageSize=10"

# 取得特定共餐活動
curl -X GET "https://localhost:5001/api/meals/meal-001"
```

## ☁️ 部署到 GCP Cloud Run

此專案已準備好部署到 Google Cloud Run。

### 快速部署

```bash
# Windows PowerShell
.\deploy-cloudrun.ps1

# Linux/macOS
./deploy-cloudrun.sh
```

詳細部署指南請參閱：
- [快速開始](CLOUD_RUN_QUICKSTART.md) - 5 分鐘快速部署
- [完整指南](DEPLOYMENT_GUIDE.md) - 詳細的部署和配置說明

### 部署檔案

- `Dockerfile` - Docker 映像配置
- `.dockerignore` - Docker 忽略檔案
- `cloudbuild.yaml` - Cloud Build CI/CD 配置
- `deploy-cloudrun.ps1` - Windows 部署腳本
- `deploy-cloudrun.sh` - Linux/macOS 部署腳本
- `env.yaml.example` - 環境變數範例

## 授權

此專案僅供學習和開發使用。

