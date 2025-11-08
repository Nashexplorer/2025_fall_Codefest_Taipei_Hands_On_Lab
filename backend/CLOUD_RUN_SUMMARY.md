# ☁️ GCP Cloud Run 部署總結

## ✅ 已完成的準備工作

你的專案現在已經完全準備好部署到 Google Cloud Run！

## 📁 新增的檔案

### Docker 相關
- ✅ `Dockerfile` - 多階段建置的 Docker 配置
- ✅ `.dockerignore` - Docker 忽略檔案

### 部署腳本
- ✅ `deploy-cloudrun.ps1` - Windows PowerShell 自動化部署腳本
- ✅ `deploy-cloudrun.sh` - Linux/macOS Bash 自動化部署腳本
- ✅ `cloudbuild.yaml` - Google Cloud Build CI/CD 配置

### 配置檔案
- ✅ `env.yaml.example` - 環境變數範例檔案
- ✅ 更新的 `.gitignore` - 排除敏感檔案

### 文檔
- ✅ `CLOUD_RUN_QUICKSTART.md` - 5 分鐘快速部署指南
- ✅ `DEPLOYMENT_GUIDE.md` - 完整部署和配置指南
- ✅ `DOCKER_TEST.md` - 本地 Docker 測試指南
- ✅ `CLOUD_RUN_SUMMARY.md` - 本檔案
- ✅ 更新的 `README.md` - 包含 Cloud Run 部署說明

### 程式碼更新
- ✅ `Program.cs` - 支援 Cloud Run PORT 環境變數
- ✅ `Program.cs` - 在生產環境啟用 Swagger
- ✅ `Program.cs` - 移除 HTTPS 重定向（Cloud Run 處理）

## 🚀 快速開始（3 步驟）

### 步驟 1: 安裝工具

- [ ] 安裝 [Google Cloud SDK](https://cloud.google.com/sdk/docs/install)
- [ ] 安裝 [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 步驟 2: 設定 GCP

```powershell
# 登入 GCP
gcloud auth login

# 設定專案（替換成你的專案 ID）
gcloud config set project YOUR-PROJECT-ID

# 啟用必要的 API
gcloud services enable cloudbuild.googleapis.com run.googleapis.com containerregistry.googleapis.com
```

### 步驟 3: 部署

```powershell
# 編輯 deploy-cloudrun.ps1，修改：
#   $PROJECT_ID = "your-gcp-project-id"

# 執行部署
.\deploy-cloudrun.ps1
```

## 📚 詳細指南

根據你的需求選擇合適的指南：

| 指南 | 適合對象 | 時間 |
|------|----------|------|
| [CLOUD_RUN_QUICKSTART.md](CLOUD_RUN_QUICKSTART.md) | 想快速部署的開發者 | 5 分鐘 |
| [DOCKER_TEST.md](DOCKER_TEST.md) | 想先在本地測試的開發者 | 10 分鐘 |
| [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) | 需要完整配置的開發者 | 30 分鐘 |

## 🎯 部署選項

### 選項 1: 使用自動化腳本（推薦）

**優點**: 最簡單，一鍵部署
**適合**: 初學者和快速迭代

```powershell
.\deploy-cloudrun.ps1
```

### 選項 2: 使用 gcloud 命令

**優點**: 更靈活的配置
**適合**: 需要自訂設定的開發者

```powershell
gcloud run deploy taipei-sports-api \
  --source . \
  --platform managed \
  --region asia-east1 \
  --allow-unauthenticated
```

### 選項 3: 使用 Cloud Build（CI/CD）

**優點**: 自動化 CI/CD 流程
**適合**: 團隊協作和生產環境

1. 連接 Git 儲存庫到 Cloud Build
2. 使用提供的 `cloudbuild.yaml`
3. 每次推送自動部署

## ⚙️ 重要配置

### 環境變數

你的資料庫連接字串已在 `appsettings.json` 中配置：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=Showcase@2025!;"
  }
}
```

**安全建議**: 在生產環境中，使用環境變數或 Secret Manager：

```powershell
# 使用環境變數
gcloud run services update taipei-sports-api `
  --set-env-vars "ConnectionStrings__DefaultConnection=YOUR_CONNECTION_STRING"

# 或使用 Secret Manager（更安全）
gcloud secrets create db-connection-string --data-file=-
# 輸入連接字串，然後 Ctrl+Z (Windows) 或 Ctrl+D (Linux/macOS)

gcloud run services update taipei-sports-api `
  --set-secrets="ConnectionStrings__DefaultConnection=db-connection-string:latest"
```

### 資源配置

預設配置（已在部署腳本中設定）：

- **記憶體**: 512Mi
- **CPU**: 1
- **最大實例數**: 10
- **區域**: asia-east1（台灣）
- **端口**: 8080

### 存取權限

預設設定為公開存取（`--allow-unauthenticated`）。

如需限制存取：

```powershell
gcloud run services update taipei-sports-api `
  --no-allow-unauthenticated
```

## 🧪 測試計畫

### 1. 本地測試（可選但推薦）

```powershell
# 建置並測試 Docker 映像
docker build -t taipei-sports-api:test .
docker run -p 8080:8080 taipei-sports-api:test

# 訪問 http://localhost:8080/swagger
```

詳見: [DOCKER_TEST.md](DOCKER_TEST.md)

### 2. 部署到 Cloud Run

```powershell
.\deploy-cloudrun.ps1
```

### 3. 驗證部署

```powershell
# 取得服務 URL
$URL = gcloud run services describe taipei-sports-api --region asia-east1 --format 'value(status.url)'

# 測試 API
Invoke-RestMethod -Uri "$URL/api/parking-ids"

# 開啟 Swagger
Start-Process "$URL/swagger"
```

## 📊 部署後檢查清單

- [ ] 服務成功部署
- [ ] 可以訪問服務 URL
- [ ] Swagger UI 正常載入
- [ ] API 端點正常回應
- [ ] 資料庫連接正常
- [ ] 沒有錯誤日誌
- [ ] 回應時間正常（< 1 秒）

## 🔍 監控和維護

### 查看日誌

```powershell
# 即時日誌
gcloud run services logs tail taipei-sports-api --region asia-east1

# 最近的日誌
gcloud run services logs read taipei-sports-api --limit 50 --region asia-east1
```

### 查看服務狀態

```powershell
gcloud run services describe taipei-sports-api --region asia-east1
```

### 查看指標

前往 [Cloud Console](https://console.cloud.google.com/run):
- 請求數量
- 延遲
- 錯誤率
- 實例數量
- CPU 和記憶體使用率

## 💰 成本預估

Cloud Run 的定價模式：

### 免費額度（每月）
- ✅ 2 百萬次請求
- ✅ 360,000 GB-秒
- ✅ 180,000 vCPU-秒
- ✅ 1 GB 網路輸出

### 你的配置成本
- 記憶體: 512Mi
- CPU: 1 vCPU
- 估計流量: 低到中等

**預估**: 對於大多數測試和小型應用，**完全免費**（在免費額度內）

## 🔄 更新部署

### 快速更新

```powershell
# 只需再次執行部署腳本
.\deploy-cloudrun.ps1
```

### 查看修訂版本

```powershell
# 列出所有版本
gcloud run revisions list --service taipei-sports-api --region asia-east1

# 回滾到特定版本
gcloud run services update-traffic taipei-sports-api `
  --to-revisions REVISION_NAME=100 `
  --region asia-east1
```

## 🛠️ 常用命令

```powershell
# 查看服務列表
gcloud run services list

# 查看服務詳情
gcloud run services describe taipei-sports-api --region asia-east1

# 更新記憶體
gcloud run services update taipei-sports-api --memory 1Gi --region asia-east1

# 更新環境變數
gcloud run services update taipei-sports-api `
  --set-env-vars "KEY=VALUE" `
  --region asia-east1

# 刪除服務
gcloud run services delete taipei-sports-api --region asia-east1
```

## 🎓 學習資源

### 官方文檔
- [Cloud Run 快速入門](https://cloud.google.com/run/docs/quickstarts)
- [Cloud Run 最佳實踐](https://cloud.google.com/run/docs/best-practices)
- [ASP.NET Core on Google Cloud](https://cloud.google.com/dotnet/docs/getting-started/run)

### 範例和教學
- [Cloud Run 範例](https://github.com/GoogleCloudPlatform/cloud-run-samples)
- [.NET on Google Cloud](https://github.com/GoogleCloudPlatform/dotnet-docs-samples)

### 社群
- [Stack Overflow - google-cloud-run](https://stackoverflow.com/questions/tagged/google-cloud-run)
- [Google Cloud Community](https://www.googlecloudcommunity.com/)

## 🚨 故障排除

### 常見問題

| 問題 | 解決方案 |
|------|----------|
| 建置失敗 | 檢查 Docker Desktop 是否運行 |
| 資料庫連不上 | 檢查連接字串和防火牆設定 |
| 記憶體不足 | 增加記憶體: `--memory 1Gi` |
| 請求超時 | 增加超時: `--timeout 300` |
| 權限錯誤 | 確認已登入: `gcloud auth login` |

詳細故障排除請參閱 [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)

## ✨ 下一步建議

### 短期（本週）
1. ✅ 完成本地測試
2. ✅ 部署到 Cloud Run
3. ✅ 驗證所有 API 端點
4. ✅ 設定監控和告警

### 中期（本月）
1. 🔐 實作認證授權
2. 📊 添加 Application Insights
3. ⚡ 實作快取機制
4. 🔄 設定 CI/CD 流程

### 長期（本季）
1. 🌐 配置自訂網域
2. 📈 效能優化
3. 🧪 添加單元測試和整合測試
4. 📱 開發前端應用

## 🎉 準備就緒！

你已經擁有所有需要的工具和文檔來部署你的 API 到 Google Cloud Run。

**現在就開始吧！**

```powershell
# 1. 編輯 deploy-cloudrun.ps1 中的專案 ID
# 2. 執行部署
.\deploy-cloudrun.ps1

# 3. 享受你的 Cloud API！ 🚀
```

有任何問題，請參考相關的指南文檔。祝你部署順利！

