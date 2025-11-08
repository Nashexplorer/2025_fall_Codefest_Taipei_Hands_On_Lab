# Cloud Run 快速部署指南

## 🚀 5 分鐘內部署到 Cloud Run

### 前置條件

1. ✅ 安裝 [Google Cloud SDK](https://cloud.google.com/sdk/docs/install)
2. ✅ 安裝 [Docker Desktop](https://www.docker.com/products/docker-desktop)
3. ✅ 有一個 GCP 專案（已啟用計費）

### Windows 快速部署

```powershell
# 1. 登入 GCP
gcloud auth login

# 2. 設定專案 ID（替換成你的專案 ID）
$PROJECT_ID = "your-gcp-project-id"
gcloud config set project $PROJECT_ID

# 3. 啟用必要的 API
gcloud services enable cloudbuild.googleapis.com run.googleapis.com containerregistry.googleapis.com

# 4. 配置 Docker
gcloud auth configure-docker

# 5. 建置並部署（一行命令）
gcloud run deploy taipei-sports-api `
  --source . `
  --platform managed `
  --region asia-east1 `
  --allow-unauthenticated `
  --port 8080 `
  --memory 512Mi `
  --cpu 1
```

### 使用自動化腳本（更簡單）

```powershell
# 1. 編輯 deploy-cloudrun.ps1，修改專案 ID:
#    $PROJECT_ID = "your-gcp-project-id"

# 2. 執行腳本
.\deploy-cloudrun.ps1
```

### 部署完成！

執行後你會看到服務 URL，例如：

```
https://taipei-sports-api-xxxxx-xx.a.run.app
```

### 測試 API

```powershell
# 取得服務 URL
$URL = gcloud run services describe taipei-sports-api --region asia-east1 --format 'value(status.url)'

# 在瀏覽器開啟 Swagger
Start-Process "$URL/swagger"

# 或用 PowerShell 測試
Invoke-RestMethod -Uri "$URL/api/parking-ids"
```

## 🎯 常見任務

### 更新部署

```powershell
# 只需再次執行部署腳本
.\deploy-cloudrun.ps1
```

### 查看日誌

```powershell
gcloud run services logs tail taipei-sports-api
```

### 查看服務狀態

```powershell
gcloud run services describe taipei-sports-api --region asia-east1
```

### 刪除服務

```powershell
gcloud run services delete taipei-sports-api --region asia-east1
```

## ⚙️ 設定環境變數

### 方法 1: 命令列

```powershell
gcloud run services update taipei-sports-api `
  --set-env-vars "ASPNETCORE_ENVIRONMENT=Production"
```

### 方法 2: 使用設定檔

1. 複製範例檔案:
   ```powershell
   Copy-Item env.yaml.example env.yaml
   ```

2. 編輯 `env.yaml` 填入實際值

3. 部署時套用:
   ```powershell
   gcloud run deploy taipei-sports-api --env-vars-file=env.yaml
   ```

## 💡 常見問題

### Q: 如何降低成本？

A: Cloud Run 有免費額度，且會自動縮放到零。對於低流量應用，通常免費。

### Q: 如何提升效能？

A: 增加記憶體和 CPU:
```powershell
gcloud run services update taipei-sports-api --memory 1Gi --cpu 2
```

### Q: 如何設定自訂網域？

A: 在 Cloud Run Console 的服務詳情頁點擊 "Manage Custom Domains"

### Q: 資料庫連不上？

A: 檢查：
- MySQL 允許外部連接
- 防火牆規則
- 連接字串格式正確

## 📚 更多資訊

詳細部署指南請參閱 [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)

## 🎉 完成！

你的 API 現在已經運行在 Google Cloud 上了！

