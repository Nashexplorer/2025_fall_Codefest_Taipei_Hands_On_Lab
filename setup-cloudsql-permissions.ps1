# Cloud SQL 權限設置腳本

Write-Host "========================================" -ForegroundColor Green
Write-Host "  設置 Cloud SQL 連接權限" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$PROJECT_ID = "focus-copilot-475707-s0"
$CLOUD_SQL_CONNECTION_NAME = "focus-copilot-475707-s0:asia-east1:born2win-1"

# 設定專案
gcloud config set project $PROJECT_ID

# 取得專案編號
Write-Host "取得專案編號..." -ForegroundColor Yellow
$PROJECT_NUMBER = gcloud projects describe $PROJECT_ID --format="value(projectNumber)"
$SERVICE_ACCOUNT = "$PROJECT_NUMBER-compute@developer.gserviceaccount.com"

Write-Host "專案編號: $PROJECT_NUMBER" -ForegroundColor Cyan
Write-Host "服務帳號: $SERVICE_ACCOUNT" -ForegroundColor Cyan
Write-Host ""

# 授予 Cloud SQL Client 角色
Write-Host "授予 Cloud SQL Client 權限..." -ForegroundColor Yellow
gcloud projects add-iam-policy-binding $PROJECT_ID `
  --member="serviceAccount:$SERVICE_ACCOUNT" `
  --role="roles/cloudsql.client"

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  權限設置完成！" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "下一步：執行部署腳本" -ForegroundColor Yellow
    Write-Host ".\deploy-cloudrun.ps1" -ForegroundColor Cyan
} else {
    Write-Host "權限設置失敗！" -ForegroundColor Red
    exit 1
}

