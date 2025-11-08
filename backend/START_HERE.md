# 🎯 從這裡開始 - Cloud Run 部署

## ✨ 恭喜！你的專案已準備好部署到 GCP Cloud Run

所有必要的檔案和配置都已經設置完成。

## 📦 已準備的內容

### ✅ Docker 配置
- `Dockerfile` - 優化的多階段建置
- `.dockerignore` - 排除不必要的檔案

### ✅ 部署工具
- `deploy-cloudrun.ps1` - Windows 自動部署腳本
- `deploy-cloudrun.sh` - Linux/macOS 自動部署腳本
- `cloudbuild.yaml` - CI/CD 配置

### ✅ 文檔（從簡到繁）
1. **本檔案** - 快速開始
2. `CHEATSHEET.md` - 常用命令速查
3. `CLOUD_RUN_QUICKSTART.md` - 5 分鐘快速部署
4. `DOCKER_TEST.md` - 本地測試指南
5. `DEPLOYMENT_GUIDE.md` - 完整部署文檔
6. `CLOUD_RUN_SUMMARY.md` - 總體概覽

### ✅ 程式碼已更新
- ✅ 支援 Cloud Run PORT 環境變數
- ✅ 生產環境 Swagger 啟用
- ✅ 優化的 HTTPS 配置

## 🚀 開始部署（3 步驟）

### 步驟 1: 安裝工具 ⏱️ 10 分鐘

#### Windows
```powershell
# 下載並安裝 Google Cloud SDK
# https://cloud.google.com/sdk/docs/install

# 驗證安裝
gcloud --version

# 下載並安裝 Docker Desktop
# https://www.docker.com/products/docker-desktop

# 驗證安裝
docker --version
```

### 步驟 2: 設定 GCP ⏱️ 2 分鐘

```powershell
# 登入 GCP
gcloud auth login

# 設定你的專案 ID（從 GCP Console 取得）
gcloud config set project YOUR-PROJECT-ID

# 啟用必要的 API
gcloud services enable cloudbuild.googleapis.com run.googleapis.com containerregistry.googleapis.com

# 配置 Docker
gcloud auth configure-docker
```

### 步驟 3: 部署！⏱️ 5-10 分鐘

#### 選項 A: 使用自動化腳本（推薦）

```powershell
# 1. 開啟 deploy-cloudrun.ps1
# 2. 修改第 14 行的專案 ID:
#    $PROJECT_ID = "your-gcp-project-id"  # 改成你的專案 ID
# 3. 儲存檔案
# 4. 執行部署

.\deploy-cloudrun.ps1
```

#### 選項 B: 使用 gcloud 命令

```powershell
gcloud run deploy taipei-sports-api `
  --source . `
  --platform managed `
  --region asia-east1 `
  --allow-unauthenticated `
  --port 8080 `
  --memory 512Mi `
  --cpu 1
```

## 🎉 部署完成！

部署成功後，你會看到：

```
✓ Deploying to Cloud Run service [taipei-sports-api] in project [YOUR-PROJECT] region [asia-east1]
✓ Deploying...
✓ Setting IAM Policy...
✓ Creating Revision...
✓ Routing traffic...
Done.
Service [taipei-sports-api] revision [taipei-sports-api-00001-xxx] has been deployed and is serving 100 percent of traffic.
Service URL: https://taipei-sports-api-xxxxx-xx.a.run.app
```

### 測試你的 API

```powershell
# 取得服務 URL
$URL = gcloud run services describe taipei-sports-api --region asia-east1 --format 'value(status.url)'

# 在瀏覽器開啟 Swagger UI
Start-Process "$URL/swagger"

# 測試 API 端點
Invoke-RestMethod -Uri "$URL/api/parking-ids" -Method Get
Invoke-RestMethod -Uri "$URL/api/parking-status?page=1&pageSize=5" -Method Get
```

## 📋 檢查清單

部署前：
- [ ] 已安裝 Google Cloud SDK
- [ ] 已安裝 Docker Desktop
- [ ] 已建立 GCP 專案
- [ ] 專案已啟用計費
- [ ] 已登入 gcloud
- [ ] 已修改部署腳本中的專案 ID

部署後：
- [ ] 服務成功部署
- [ ] 可以訪問服務 URL
- [ ] Swagger UI 可以開啟
- [ ] API 端點正常回應
- [ ] 資料庫連接正常

## 🔍 接下來做什麼？

### 立即行動
1. ✅ 測試所有 API 端點
2. ✅ 查看日誌確認沒有錯誤
3. ✅ 在 Swagger UI 中試用 API

### 本週內
1. 📊 設定監控和告警
2. 🔐 考慮添加認證
3. 📈 查看使用量和成本
4. 🌐 （可選）配置自訂網域

### 本月內
1. 🔄 設定 CI/CD 流程
2. ⚡ 實作快取機制
3. 🧪 添加自動化測試
4. 📱 開發前端應用

## 📚 需要幫助？

### 快速參考
- **常用命令**: 查看 `CHEATSHEET.md`
- **本地測試**: 查看 `DOCKER_TEST.md`
- **詳細配置**: 查看 `DEPLOYMENT_GUIDE.md`

### 常見問題

**Q: 部署失敗怎麼辦？**
A: 查看錯誤訊息，通常是：
- Docker Desktop 沒有運行
- 專案 ID 設定錯誤
- 沒有啟用必要的 API

**Q: 如何查看日誌？**
A: 
```powershell
gcloud run services logs tail taipei-sports-api
```

**Q: 如何更新部署？**
A: 再次執行部署腳本即可：
```powershell
.\deploy-cloudrun.ps1
```

**Q: 成本會很高嗎？**
A: Cloud Run 有免費額度（每月 2 百萬次請求），對於測試和小型應用完全免費。

**Q: 如何刪除服務？**
A:
```powershell
gcloud run services delete taipei-sports-api --region asia-east1
```

## 🆘 遇到問題？

### 建置失敗
```powershell
# 確認 Docker Desktop 正在運行
docker version

# 在本地測試建置
docker build -t test .
```

### 資料庫連接失敗
```powershell
# 測試連接
Test-NetConnection -ComputerName 34.81.245.32 -Port 3306

# 檢查連接字串（appsettings.json）
```

### 權限問題
```powershell
# 確認已登入
gcloud auth list

# 重新登入
gcloud auth login
```

## 🎯 快速命令

```powershell
# 部署
.\deploy-cloudrun.ps1

# 查看服務
gcloud run services list

# 查看日誌
gcloud run services logs tail taipei-sports-api

# 取得 URL
gcloud run services describe taipei-sports-api --format 'value(status.url)'

# 開啟 Swagger
$URL = gcloud run services describe taipei-sports-api --format 'value(status.url)'
Start-Process "$URL/swagger"
```

## 💡 小提示

1. **本地測試**: 部署前先在本地測試 Docker 映像（參考 `DOCKER_TEST.md`）
2. **環境變數**: 敏感資訊使用 Secret Manager（參考 `DEPLOYMENT_GUIDE.md`）
3. **成本控制**: 設定 `--max-instances` 限制最大實例數
4. **監控**: 定期查看 Cloud Console 的監控指標
5. **備份**: 使用 Git 管理程式碼變更

## 🚀 現在就開始吧！

```powershell
# 1. 確認工具已安裝
gcloud --version
docker --version

# 2. 登入 GCP
gcloud auth login

# 3. 設定專案（替換 YOUR-PROJECT-ID）
gcloud config set project YOUR-PROJECT-ID

# 4. 啟用 API
gcloud services enable cloudbuild.googleapis.com run.googleapis.com containerregistry.googleapis.com

# 5. 編輯 deploy-cloudrun.ps1 設定專案 ID

# 6. 部署！
.\deploy-cloudrun.ps1

# 7. 慶祝！🎉
```

---

**準備好了嗎？開始部署你的第一個 Cloud Run 服務！** 🚀

有任何問題，請參考相關的文檔檔案。祝你部署順利！

