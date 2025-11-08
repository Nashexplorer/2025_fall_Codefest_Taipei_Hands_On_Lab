# GCP Cloud Run 部署腳本 (PowerShell)

Write-Host "========================================" -ForegroundColor Green
Write-Host "  部署 共餐API 到 GCP Cloud Run" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# 設定變數
$PROJECT_ID = "focus-copilot-475707-s0"
$SERVICE_NAME = "gongcan-api"
$REGION = "asia-east1"
$IMAGE_NAME = "gcr.io/$PROJECT_ID/$SERVICE_NAME"

# Cloud SQL 配置（如果是 Cloud SQL，請設定此變數）
# 格式: PROJECT_ID:REGION:INSTANCE_NAME
# 範例: focus-copilot-475707-s0:asia-east1:mysql-instance
$CLOUD_SQL_CONNECTION_NAME = "focus-copilot-475707-s0:asia-east1:born2win-1"  # 如果有 Cloud SQL，請填入連接名稱

# 檢查是否已登入 GCP
Write-Host "檢查 GCP 登入狀態..." -ForegroundColor Yellow
$activeAccount = gcloud auth list --filter=status:ACTIVE --format="value(account)" 2>$null
if ([string]::IsNullOrEmpty($activeAccount)) {
    Write-Host "請先登入 GCP: gcloud auth login" -ForegroundColor Red
    exit 1
}

# 設定專案
Write-Host "設定 GCP 專案..." -ForegroundColor Yellow
gcloud config set project $PROJECT_ID

# 啟用必要的 API
Write-Host "啟用必要的 GCP API..." -ForegroundColor Yellow
gcloud services enable cloudbuild.googleapis.com
gcloud services enable run.googleapis.com
gcloud services enable containerregistry.googleapis.com

# 如果使用 Cloud SQL，需要啟用 Cloud SQL Admin API
if (-not [string]::IsNullOrEmpty($CLOUD_SQL_CONNECTION_NAME)) {
    Write-Host "啟用 Cloud SQL Admin API..." -ForegroundColor Yellow
    gcloud services enable sqladmin.googleapis.com
}

# 建置 Docker 映像
Write-Host "建置 Docker 映像..." -ForegroundColor Yellow
docker build -t "${IMAGE_NAME}:latest" .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker 建置失敗！" -ForegroundColor Red
    exit 1
}

# 推送映像到 Google Container Registry
Write-Host "推送映像到 GCR..." -ForegroundColor Yellow
docker push "${IMAGE_NAME}:latest"

if ($LASTEXITCODE -ne 0) {
    Write-Host "推送映像失敗！" -ForegroundColor Red
    exit 1
}

# 準備環境變數
$envVars = @()
$envVars += "ASPNETCORE_ENVIRONMENT=Production"

# 設定資料庫連接字串環境變數（需要正確轉義特殊字元）
$DB_CONNECTION_STRING = "Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=Showcase@2025!;"
$envVars += "ConnectionStrings__DefaultConnection=$DB_CONNECTION_STRING"

# 如果使用 Cloud SQL，添加 Cloud SQL 連接器配置
if (-not [string]::IsNullOrEmpty($CLOUD_SQL_CONNECTION_NAME)) {
    Write-Host "配置 Cloud SQL 連接器..." -ForegroundColor Yellow
    $envVars += "CloudSQL__ConnectionName=$CLOUD_SQL_CONNECTION_NAME"
    Write-Host "使用 Cloud SQL 連接名稱: $CLOUD_SQL_CONNECTION_NAME" -ForegroundColor Cyan
} else {
    Write-Host "使用標準 MySQL 連接（請確保防火牆規則已配置）" -ForegroundColor Yellow
}

# 構建部署命令字串
$envVarsString = $envVars -join ","

# 部署到 Cloud Run
Write-Host "部署到 Cloud Run..." -ForegroundColor Yellow

if (-not [string]::IsNullOrEmpty($CLOUD_SQL_CONNECTION_NAME)) {
    # 使用 Cloud SQL 連接器
    gcloud run deploy $SERVICE_NAME `
        --image "${IMAGE_NAME}:latest" `
        --platform managed `
        --region $REGION `
        --allow-unauthenticated `
        --port 8080 `
        --memory 512Mi `
        --cpu 1 `
        --max-instances 10 `
        --add-cloudsql-instances $CLOUD_SQL_CONNECTION_NAME `
        --set-env-vars "$envVarsString"
} else {
    # 標準 MySQL 連接
    gcloud run deploy $SERVICE_NAME `
        --image "${IMAGE_NAME}:latest" `
        --platform managed `
        --region $REGION `
        --allow-unauthenticated `
        --port 8080 `
        --memory 512Mi `
        --cpu 1 `
        --max-instances 10 `
        --set-env-vars "$envVarsString"
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  部署成功！" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "取得服務 URL..." -ForegroundColor Yellow
    $SERVICE_URL = gcloud run services describe $SERVICE_NAME --platform managed --region $REGION --format 'value(status.url)'
    Write-Host "服務 URL: $SERVICE_URL" -ForegroundColor Green
    Write-Host "Swagger UI: $SERVICE_URL/swagger" -ForegroundColor Green
    Write-Host ""
    Write-Host "注意：如果資料庫連接失敗，請檢查：" -ForegroundColor Yellow
    Write-Host "1. Cloud SQL 連接名稱是否正確（$CLOUD_SQL_CONNECTION_NAME）" -ForegroundColor Yellow
    Write-Host "2. 防火牆規則是否允許 Cloud Run 連接" -ForegroundColor Yellow
    Write-Host "3. 查看日誌: gcloud run services logs tail $SERVICE_NAME --region $REGION" -ForegroundColor Yellow
} else {
    Write-Host "部署失敗！" -ForegroundColor Red
    exit 1
}

