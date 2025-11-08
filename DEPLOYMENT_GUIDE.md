# GCP Cloud Run 部署指南

這份指南將協助你將 TaipeiSportsApi 部署到 Google Cloud Platform (GCP) Cloud Run。

## 📋 前置需求

### 1. 安裝必要工具

- **Google Cloud SDK (gcloud CLI)**
  - Windows: 下載 [Google Cloud SDK Installer](https://cloud.google.com/sdk/docs/install)
  - 驗證安裝: `gcloud --version`

- **Docker Desktop**
  - 下載: [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop)
  - 驗證安裝: `docker --version`

### 2. GCP 帳號設定

1. 建立或選擇一個 GCP 專案
2. 確保已啟用計費功能
3. 記下你的專案 ID

## 🚀 部署步驟

### 方法 1: 使用自動化腳本（推薦）

#### Windows (PowerShell)

1. **編輯部署腳本**

   開啟 `deploy-cloudrun.ps1`，修改專案 ID:

   ```powershell
   $PROJECT_ID = "your-gcp-project-id"  # 改成你的 GCP 專案 ID
   ```

2. **執行部署**

   ```powershell
   # 授予執行權限（如果需要）
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

   # 執行部署腳本
   .\deploy-cloudrun.ps1
   ```

#### Linux/macOS (Bash)

1. **編輯部署腳本**

   開啟 `deploy-cloudrun.sh`，修改專案 ID:

   ```bash
   PROJECT_ID="your-gcp-project-id"  # 改成你的 GCP 專案 ID
   ```

2. **執行部署**

   ```bash
   # 授予執行權限
   chmod +x deploy-cloudrun.sh

   # 執行部署腳本
   ./deploy-cloudrun.sh
   ```

### 方法 2: 手動部署

#### 步驟 1: 登入 GCP

```bash
gcloud auth login
```

#### 步驟 2: 設定專案

```bash
# 設定你的專案 ID
export PROJECT_ID="your-gcp-project-id"
gcloud config set project $PROJECT_ID
```

#### 步驟 3: 啟用必要的 API

```bash
gcloud services enable cloudbuild.googleapis.com
gcloud services enable run.googleapis.com
gcloud services enable containerregistry.googleapis.com
```

#### 步驟 4: 配置 Docker 認證

```bash
gcloud auth configure-docker
```

#### 步驟 5: 建置 Docker 映像

```bash
docker build -t gcr.io/$PROJECT_ID/taipei-sports-api:latest .
```

#### 步驟 6: 推送映像到 GCR

```bash
docker push gcr.io/$PROJECT_ID/taipei-sports-api:latest
```

#### 步驟 7: 部署到 Cloud Run

```bash
gcloud run deploy taipei-sports-api \
  --image gcr.io/$PROJECT_ID/taipei-sports-api:latest \
  --platform managed \
  --region asia-east1 \
  --allow-unauthenticated \
  --port 8080 \
  --memory 512Mi \
  --cpu 1 \
  --max-instances 10 \
  --set-env-vars ASPNETCORE_ENVIRONMENT=Production
```

### 方法 3: 使用 Cloud Build（自動化 CI/CD）

#### 步驟 1: 連接 Git 儲存庫

1. 前往 [GCP Console > Cloud Build > Triggers](https://console.cloud.google.com/cloud-build/triggers)
2. 點擊 "Connect Repository"
3. 選擇 GitHub/Bitbucket/Cloud Source Repositories
4. 授權並選擇你的儲存庫

#### 步驟 2: 建立觸發器

1. 點擊 "Create Trigger"
2. 設定觸發條件（例如：推送到 main 分支）
3. 選擇 "Cloud Build configuration file"
4. 指定 `cloudbuild.yaml`
5. 點擊 "Create"

#### 步驟 3: 更新 cloudbuild.yaml

已經為你準備好 `cloudbuild.yaml`，只需確認區域設定：

```yaml
- '--region'
- 'asia-east1'  # 可改為其他區域：us-central1, europe-west1 等
```

#### 步驟 4: 推送程式碼觸發部署

```bash
git add .
git commit -m "Deploy to Cloud Run"
git push origin main
```

## ⚙️ 配置說明

### 環境變數

在部署時可以設定環境變數：

```bash
gcloud run deploy taipei-sports-api \
  --set-env-vars "ASPNETCORE_ENVIRONMENT=Production,ConnectionStrings__DefaultConnection=YOUR_CONNECTION_STRING"
```

或使用 `.env.yaml` 檔案：

```yaml
# env.yaml
ASPNETCORE_ENVIRONMENT: "Production"
ConnectionStrings__DefaultConnection: "Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=Showcase@2025!;"
```

然後部署：

```bash
gcloud run deploy taipei-sports-api \
  --env-vars-file=env.yaml \
  --image gcr.io/$PROJECT_ID/taipei-sports-api:latest
```

### 資源配置

根據需求調整資源：

```bash
--memory 512Mi       # 記憶體: 128Mi, 256Mi, 512Mi, 1Gi, 2Gi, 4Gi, 8Gi
--cpu 1              # CPU: 0.08, 0.17, 1, 2, 4, 6, 8
--max-instances 10   # 最大實例數
--min-instances 0    # 最小實例數（0 表示縮放到零）
--timeout 300        # 請求超時（秒）
```

### 區域選擇

建議的 Cloud Run 區域：

- **亞洲**:
  - `asia-east1` (台灣)
  - `asia-northeast1` (東京)
  - `asia-southeast1` (新加坡)

- **美國**:
  - `us-central1` (愛荷華)
  - `us-west1` (奧勒岡)

- **歐洲**:
  - `europe-west1` (比利時)
  - `europe-west4` (荷蘭)

### 自訂網域

1. 前往 Cloud Run 服務詳情頁
2. 點擊 "Manage Custom Domains"
3. 新增你的網域
4. 更新 DNS 記錄

## 🔒 安全性設定

### 啟用認證

如果不希望 API 公開存取：

```bash
gcloud run deploy taipei-sports-api \
  --no-allow-unauthenticated
```

然後使用 IAM 管理存取權限：

```bash
gcloud run services add-iam-policy-binding taipei-sports-api \
  --member="user:example@gmail.com" \
  --role="roles/run.invoker"
```

### Secret Manager

建議使用 Secret Manager 存儲敏感資訊：

1. **建立 Secret**:

```bash
echo -n "Showcase@2025!" | gcloud secrets create db-password --data-file=-
```

2. **授予 Cloud Run 存取權限**:

```bash
gcloud secrets add-iam-policy-binding db-password \
  --member="serviceAccount:PROJECT_NUMBER-compute@developer.gserviceaccount.com" \
  --role="roles/secretmanager.secretAccessor"
```

3. **在部署時引用 Secret**:

```bash
gcloud run deploy taipei-sports-api \
  --set-secrets="DB_PASSWORD=db-password:latest"
```

## 📊 監控和日誌

### 查看日誌

```bash
# 即時日誌
gcloud run services logs tail taipei-sports-api

# 查看最近的日誌
gcloud run services logs read taipei-sports-api --limit 50
```

### Cloud Console 監控

前往 [Cloud Run Console](https://console.cloud.google.com/run) 查看：
- 請求數量
- 延遲
- 錯誤率
- 實例數量
- CPU 和記憶體使用率

## 🧪 測試部署

部署完成後，你會獲得一個 URL，例如：

```
https://taipei-sports-api-xxxxx-xx.a.run.app
```

### 測試 API

```bash
# 取得服務 URL
export SERVICE_URL=$(gcloud run services describe taipei-sports-api --platform managed --region asia-east1 --format 'value(status.url)')

# 測試健康狀態
curl $SERVICE_URL/api/parking-ids

# 測試 Swagger UI
curl $SERVICE_URL/swagger
```

### 使用 PowerShell 測試

```powershell
$SERVICE_URL = gcloud run services describe taipei-sports-api --platform managed --region asia-east1 --format 'value(status.url)'

# 測試 API
Invoke-RestMethod -Uri "$SERVICE_URL/api/parking-ids" -Method Get

# 在瀏覽器開啟 Swagger
Start-Process "$SERVICE_URL/swagger"
```

## 💰 成本估算

Cloud Run 定價（以台灣區域為例）：

- **免費額度**（每月）:
  - 2 百萬次請求
  - 360,000 GB-秒
  - 180,000 vCPU-秒

- **付費（超過免費額度）**:
  - 請求: $0.40 / 百萬次
  - 記憶體: $0.00000250 / GB-秒
  - CPU: $0.00002400 / vCPU-秒

**範例估算**：
- 10 萬次請求/月
- 平均 100ms 回應時間
- 512Mi 記憶體，1 vCPU

估計成本: **免費** （在免費額度內）

## 🔄 更新部署

### 快速更新

```bash
# 重新建置並推送
docker build -t gcr.io/$PROJECT_ID/taipei-sports-api:latest .
docker push gcr.io/$PROJECT_ID/taipei-sports-api:latest

# 更新服務
gcloud run deploy taipei-sports-api \
  --image gcr.io/$PROJECT_ID/taipei-sports-api:latest
```

### 使用腳本更新

只需再次執行部署腳本：

```powershell
.\deploy-cloudrun.ps1
```

### 回滾到先前版本

```bash
# 列出所有修訂版本
gcloud run revisions list --service taipei-sports-api

# 切換到特定版本
gcloud run services update-traffic taipei-sports-api \
  --to-revisions REVISION_NAME=100
```

## 🐛 故障排除

### 問題 1: 建置失敗

**錯誤**: `docker build` 失敗

**解決方案**:
- 確保 Docker Desktop 正在運行
- 檢查 Dockerfile 語法
- 查看建置日誌

### 問題 2: 無法連接資料庫

**錯誤**: Connection timeout 或認證失敗

**解決方案**:
1. 檢查 MySQL 伺服器是否允許外部連接
2. 確認連接字串正確
3. 檢查防火牆規則
4. 考慮使用 Cloud SQL Proxy

### 問題 3: 記憶體不足

**錯誤**: Out of memory

**解決方案**:
```bash
gcloud run services update taipei-sports-api --memory 1Gi
```

### 問題 4: 請求超時

**錯誤**: Request timeout

**解決方案**:
```bash
gcloud run services update taipei-sports-api --timeout 300
```

### 問題 5: 權限錯誤

**錯誤**: Permission denied

**解決方案**:
```bash
# 確保已登入
gcloud auth login

# 確認有適當的 IAM 角色
gcloud projects get-iam-policy $PROJECT_ID
```

## 📚 相關資源

- [Cloud Run 官方文檔](https://cloud.google.com/run/docs)
- [Cloud Run 定價](https://cloud.google.com/run/pricing)
- [Cloud Run 最佳實踐](https://cloud.google.com/run/docs/best-practices)
- [ASP.NET Core on Cloud Run](https://cloud.google.com/dotnet/docs/getting-started/run)

## ✅ 檢查清單

部署前確認：

- [ ] Docker Desktop 正在運行
- [ ] 已安裝並登入 gcloud CLI
- [ ] 已建立 GCP 專案並啟用計費
- [ ] 已更新 `deploy-cloudrun.ps1` 或 `deploy-cloudrun.sh` 中的專案 ID
- [ ] 資料庫連接字串正確
- [ ] 已測試本地 Docker 映像

部署後驗證：

- [ ] 服務已成功部署
- [ ] API 端點可以訪問
- [ ] Swagger UI 可以開啟
- [ ] 資料庫連接正常
- [ ] 日誌沒有錯誤

## 🎉 完成！

現在你的 API 已經部署到 Cloud Run，可以透過提供的 URL 存取。

記得定期檢查日誌和監控指標，確保服務運行正常。

