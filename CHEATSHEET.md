# GCP Cloud Run 快速參考

## 🚀 一行命令部署

```powershell
# 編輯 deploy-cloudrun.ps1 中的 PROJECT_ID，然後執行：
.\deploy-cloudrun.ps1
```

## 📋 常用命令

### 部署和更新

```powershell
# 部署服務
gcloud run deploy taipei-sports-api --source . --region asia-east1

# 更新記憶體
gcloud run services update taipei-sports-api --memory 1Gi

# 更新 CPU
gcloud run services update taipei-sports-api --cpu 2

# 設定環境變數
gcloud run services update taipei-sports-api --set-env-vars "KEY=VALUE"

# 更新最大實例數
gcloud run services update taipei-sports-api --max-instances 20
```

### 查看和監控

```powershell
# 查看服務列表
gcloud run services list

# 查看服務詳情
gcloud run services describe taipei-sports-api

# 取得服務 URL
gcloud run services describe taipei-sports-api --format 'value(status.url)'

# 查看即時日誌
gcloud run services logs tail taipei-sports-api

# 查看最近日誌
gcloud run services logs read taipei-sports-api --limit 50
```

### 版本管理

```powershell
# 列出所有修訂版本
gcloud run revisions list --service taipei-sports-api

# 回滾到指定版本
gcloud run services update-traffic taipei-sports-api --to-revisions REVISION_NAME=100

# 流量分割（金絲雀部署）
gcloud run services update-traffic taipei-sports-api --to-revisions latest=50,previous=50
```

### Docker 本地測試

```powershell
# 建置映像
docker build -t taipei-sports-api:test .

# 執行容器
docker run -p 8080:8080 taipei-sports-api:test

# 查看日誌
docker logs -f <container-id>

# 停止容器
docker stop <container-id>

# 清理
docker system prune -a
```

### 管理

```powershell
# 刪除服務
gcloud run services delete taipei-sports-api

# 查看配額
gcloud run services quotas list

# 查看計費
gcloud billing accounts list
```

## 🔧 常用配置

### 資源大小

```powershell
--memory 128Mi|256Mi|512Mi|1Gi|2Gi|4Gi|8Gi
--cpu 0.08|0.17|1|2|4|6|8
--max-instances 1-1000
--min-instances 0-1000
--timeout 1-3600 (秒)
```

### 區域

```powershell
--region asia-east1      # 台灣
--region asia-northeast1 # 東京
--region asia-southeast1 # 新加坡
--region us-central1     # 美國中部
--region europe-west1    # 歐洲西部
```

### 環境變數

```powershell
# 單個變數
--set-env-vars "KEY=VALUE"

# 多個變數
--set-env-vars "KEY1=VALUE1,KEY2=VALUE2"

# 從檔案讀取
--env-vars-file=env.yaml

# 使用 Secret
--set-secrets="KEY=secret-name:version"
```

## 🧪 測試命令

```powershell
# PowerShell
$URL = gcloud run services describe taipei-sports-api --format 'value(status.url)'
Invoke-RestMethod -Uri "$URL/api/parking-ids"
Start-Process "$URL/swagger"

# 負載測試（簡單）
1..100 | ForEach-Object { Invoke-RestMethod -Uri "$URL/api/parking-ids" }
```

## 🔐 安全性

```powershell
# 禁用公開存取
--no-allow-unauthenticated

# 授權特定使用者
gcloud run services add-iam-policy-binding taipei-sports-api \
  --member="user:email@example.com" \
  --role="roles/run.invoker"

# 建立 Secret
echo -n "password" | gcloud secrets create db-password --data-file=-

# 授予 Secret 存取權限
gcloud secrets add-iam-policy-binding db-password \
  --member="serviceAccount:PROJECT_NUMBER-compute@developer.gserviceaccount.com" \
  --role="roles/secretmanager.secretAccessor"
```

## 💰 成本控制

```powershell
# 設定最小實例為 0（縮放到零）
--min-instances 0

# 限制最大實例數
--max-instances 5

# 設定並發請求數
--concurrency 80

# 查看成本
# 前往: https://console.cloud.google.com/billing
```

## 📚 快速連結

| 資源 | 連結 |
|------|------|
| Cloud Console | https://console.cloud.google.com/run |
| 日誌 | https://console.cloud.google.com/logs |
| 計費 | https://console.cloud.google.com/billing |
| Secret Manager | https://console.cloud.google.com/security/secret-manager |
| Cloud Build | https://console.cloud.google.com/cloud-build |

## 🆘 緊急修復

```powershell
# 服務無回應 - 重新部署
.\deploy-cloudrun.ps1

# 回滾到上一個版本
gcloud run revisions list --service taipei-sports-api
gcloud run services update-traffic taipei-sports-api --to-revisions PREVIOUS_REVISION=100

# 快速擴容
gcloud run services update taipei-sports-api --max-instances 50 --cpu 2 --memory 2Gi

# 查看詳細錯誤
gcloud run services logs read taipei-sports-api --limit 200

# 停止接收流量（緊急維護）
gcloud run services update-traffic taipei-sports-api --to-revisions OLD_REVISION=100
```

## 📖 文檔參考

- 快速開始: [CLOUD_RUN_QUICKSTART.md](CLOUD_RUN_QUICKSTART.md)
- 完整指南: [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)
- Docker 測試: [DOCKER_TEST.md](DOCKER_TEST.md)
- 總結: [CLOUD_RUN_SUMMARY.md](CLOUD_RUN_SUMMARY.md)

