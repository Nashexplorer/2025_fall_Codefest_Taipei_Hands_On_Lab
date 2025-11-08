#!/bin/bash

# GCP Cloud Run 部署腳本

# 顏色定義
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  部署 TaipeiSportsApi 到 GCP Cloud Run${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""

# 設定變數
PROJECT_ID="your-gcp-project-id"
SERVICE_NAME="taipei-sports-api"
REGION="asia-east1"
IMAGE_NAME="gcr.io/${PROJECT_ID}/${SERVICE_NAME}"

# 檢查是否已登入 GCP
echo -e "${YELLOW}檢查 GCP 登入狀態...${NC}"
if ! gcloud auth list --filter=status:ACTIVE --format="value(account)" | grep -q .; then
    echo -e "${RED}請先登入 GCP: gcloud auth login${NC}"
    exit 1
fi

# 設定專案
echo -e "${YELLOW}設定 GCP 專案...${NC}"
gcloud config set project ${PROJECT_ID}

# 啟用必要的 API
echo -e "${YELLOW}啟用必要的 GCP API...${NC}"
gcloud services enable cloudbuild.googleapis.com
gcloud services enable run.googleapis.com
gcloud services enable containerregistry.googleapis.com

# 建置 Docker 映像
echo -e "${YELLOW}建置 Docker 映像...${NC}"
docker build -t ${IMAGE_NAME}:latest .

if [ $? -ne 0 ]; then
    echo -e "${RED}Docker 建置失敗！${NC}"
    exit 1
fi

# 推送映像到 Google Container Registry
echo -e "${YELLOW}推送映像到 GCR...${NC}"
docker push ${IMAGE_NAME}:latest

if [ $? -ne 0 ]; then
    echo -e "${RED}推送映像失敗！${NC}"
    exit 1
fi

# 部署到 Cloud Run
echo -e "${YELLOW}部署到 Cloud Run...${NC}"
gcloud run deploy ${SERVICE_NAME} \
  --image ${IMAGE_NAME}:latest \
  --platform managed \
  --region ${REGION} \
  --allow-unauthenticated \
  --port 8080 \
  --memory 512Mi \
  --cpu 1 \
  --max-instances 10 \
  --set-env-vars ASPNETCORE_ENVIRONMENT=Production

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}========================================${NC}"
    echo -e "${GREEN}  部署成功！${NC}"
    echo -e "${GREEN}========================================${NC}"
    echo ""
    echo -e "${YELLOW}取得服務 URL...${NC}"
    SERVICE_URL=$(gcloud run services describe ${SERVICE_NAME} --platform managed --region ${REGION} --format 'value(status.url)')
    echo -e "${GREEN}服務 URL: ${SERVICE_URL}${NC}"
    echo -e "${GREEN}Swagger UI: ${SERVICE_URL}/swagger${NC}"
else
    echo -e "${RED}部署失敗！${NC}"
    exit 1
fi

