# 專案結構說明

## 目錄結構

```
TaipeiSportsApi/
│
├── Data/                           # 資料存取層
│   └── ApplicationDbContext.cs    # Entity Framework Core DbContext
│
├── Models/                         # 資料模型
│   └── TaipeiParkingStatus.cs     # 停車場狀態實體類別
│
├── Properties/                     # 專案屬性
│   └── launchSettings.json        # 啟動設定（端口、環境變數等）
│
├── bin/                           # 編譯輸出目錄（不在版本控制中）
├── obj/                           # 中間編譯檔案（不在版本控制中）
│
├── .gitignore                     # Git 忽略檔案
├── appsettings.json               # 應用程式設定
├── appsettings.Development.json   # 開發環境設定
├── Program.cs                     # 應用程式進入點和 API 路由定義
├── TaipeiSportsApi.csproj         # 專案檔案
├── TaipeiSportsApi.sln            # 解決方案檔案
├── TaipeiSportsApi.http           # HTTP 請求測試檔案
├── README.md                      # 專案說明文件
├── QUICKSTART.md                  # 快速啟動指南
└── PROJECT_STRUCTURE.md           # 本檔案
```

## 核心檔案說明

### Program.cs

應用程式的主要進入點，包含：

- **服務配置**: 
  - Entity Framework Core 配置
  - Swagger/OpenAPI 配置
  - 依賴注入設定

- **API 端點定義**: 
  - 所有 Minimal API 路由
  - 請求處理邏輯
  - 回應格式定義

- **中介軟體配置**:
  - HTTPS 重定向
  - Swagger UI
  - 錯誤處理

### Models/TaipeiParkingStatus.cs

資料模型類別，對應資料庫表 `taipei_parking_status`：

- 使用 Data Annotations 定義欄位屬性
- 複合主鍵：`Id` + `UpdateTime`
- 包含所有停車場狀態相關欄位
- 10 個保留欄位供未來擴充

### Data/ApplicationDbContext.cs

Entity Framework Core 的資料庫上下文：

- 繼承自 `DbContext`
- 定義 `TaipeiParkingStatuses` DbSet
- 配置資料模型（如複合主鍵）
- 處理資料庫連接和查詢

### appsettings.json

應用程式配置檔案：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=xxxxx;"
  }
}
```

**重要**: 不要將包含敏感資訊的配置檔案提交到版本控制！

### TaipeiSportsApi.csproj

專案檔案，定義：

- 目標框架：.NET 8.0
- NuGet 套件依賴：
  - `Microsoft.AspNetCore.OpenApi` - OpenAPI 支援
  - `Swashbuckle.AspNetCore` - Swagger UI
  - `Pomelo.EntityFrameworkCore.MySql` - MySQL 資料庫提供者
  - `Microsoft.EntityFrameworkCore.Design` - EF Core 工具

### TaipeiSportsApi.http

REST Client 測試檔案：

- Visual Studio 和 VS Code 可直接執行
- 包含所有 API 端點的測試範例
- 方便開發和測試

## 架構模式

### Minimal API

專案使用 ASP.NET Core 8 的 Minimal API 模式：

**優點**:
- 簡潔的程式碼結構
- 減少樣板程式碼
- 高效能
- 適合小型到中型 API

**特點**:
- 路由定義直接在 `Program.cs`
- 使用 Lambda 表達式處理請求
- 支援依賴注入
- 內建 OpenAPI/Swagger 支援

### 資料存取層

使用 Entity Framework Core：

**優點**:
- 類型安全的查詢
- LINQ 支援
- 自動參數化查詢（防止 SQL 注入）
- 變更追蹤
- 資料庫遷移支援

### 依賴注入

ASP.NET Core 內建的 DI 容器：

```csharp
// 註冊服務
builder.Services.AddDbContext<ApplicationDbContext>(options => ...);

// 注入到端點
app.MapGet("/api/parking-status", async (ApplicationDbContext db) => {
    // db 自動由 DI 容器提供
});
```

## API 設計模式

### RESTful 設計

遵循 REST 原則：

- **資源導向**: `/api/parking-status`
- **HTTP 動詞**: GET, POST, PUT, DELETE
- **狀態碼**: 200 (成功), 201 (已建立), 404 (未找到), 409 (衝突)
- **分頁支援**: `?page=1&pageSize=10`

### 回應格式

**成功回應**:
```json
{
  "totalCount": 100,
  "page": 1,
  "pageSize": 10,
  "data": [...]
}
```

**錯誤回應**:
```json
{
  "message": "錯誤訊息"
}
```

### 複合主鍵處理

由於表使用複合主鍵 (Id + UpdateTime)，端點設計：

- **單一記錄**: `/api/parking-status/{id}/{updateTime}`
- **最新記錄**: `/api/parking-status/{id}`
- **歷史記錄**: `/api/parking-status/{id}/history`

## 擴充建議

### 新增服務層

對於更複雜的業務邏輯，建議新增服務層：

```
Services/
├── Interfaces/
│   └── IParkingService.cs
└── ParkingService.cs
```

### 新增 DTO (資料傳輸物件)

分離資料模型和 API 契約：

```
DTOs/
├── ParkingStatusDto.cs
├── CreateParkingStatusDto.cs
└── UpdateParkingStatusDto.cs
```

### 新增驗證

使用 FluentValidation 進行輸入驗證：

```
Validators/
└── ParkingStatusValidator.cs
```

### 新增錯誤處理中介軟體

集中處理例外：

```
Middleware/
└── ErrorHandlingMiddleware.cs
```

### 新增快取

使用 Redis 或記憶體快取提升效能：

```csharp
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost:6379";
});
```

### 新增認證授權

保護 API 端點：

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });
```

### 新增單元測試

```
TaipeiSportsApi.Tests/
├── Controllers/
├── Services/
└── IntegrationTests/
```

## 效能考量

### 資料庫索引

確保資料庫表有適當的索引：

```sql
-- 複合主鍵索引（已存在）
PRIMARY KEY (id, update_time)

-- 額外索引建議
CREATE INDEX idx_update_time ON taipei_parking_status(update_time DESC);
CREATE INDEX idx_id ON taipei_parking_status(id);
```

### 查詢最佳化

- 使用分頁避免載入過多資料
- 使用 `AsNoTracking()` 進行唯讀查詢
- 避免 N+1 查詢問題

### 連接池

Entity Framework Core 自動管理連接池，但可以調整：

```csharp
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
    mySqlOptions => {
        mySqlOptions.MaxBatchSize(100);
        mySqlOptions.CommandTimeout(30);
    });
```

## 安全性建議

1. **連接字串安全**:
   - 使用 User Secrets (開發環境)
   - 使用 Azure Key Vault (生產環境)
   - 不要提交到版本控制

2. **輸入驗證**:
   - 驗證所有輸入參數
   - 使用 DTO 和驗證器

3. **CORS 設定**:
   ```csharp
   builder.Services.AddCors(options => {
       options.AddPolicy("AllowSpecificOrigin",
           builder => builder.WithOrigins("https://example.com"));
   });
   ```

4. **速率限制**:
   使用 ASP.NET Core 的速率限制中介軟體

5. **HTTPS**:
   在生產環境強制使用 HTTPS

## 監控和日誌

### Application Insights

Azure 應用程式監控：

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog

結構化日誌記錄：

```csharp
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
```

### Health Checks

健康檢查端點：

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

app.MapHealthChecks("/health");
```

## 部署選項

1. **IIS** (Windows Server)
2. **Kestrel** (跨平台)
3. **Docker Container**
4. **Azure App Service**
5. **AWS Elastic Beanstalk**
6. **Kubernetes**

## 資源連結

- [ASP.NET Core 文檔](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Minimal APIs 概述](https://docs.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [Pomelo MySQL Provider](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)

