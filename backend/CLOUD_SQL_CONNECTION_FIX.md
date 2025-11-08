# Cloud SQL 連接問題解決指南

## 🔍 問題診斷

你遇到的錯誤：`MySqlConnector.MySqlException: Connect Timeout expired`

這表示 Cloud Run 無法連接到你的 MySQL 資料庫。

## 🎯 解決方案

根據你的 MySQL 部署方式，有兩種解決方案：

### 方案 1: MySQL 是 Cloud SQL（推薦）

如果你的 MySQL 是 GCP Cloud SQL，需要使用 Cloud SQL 連接器。

#### 步驟 1: 取得 Cloud SQL 連接名稱

```powershell
# 列出所有 Cloud SQL 實例
gcloud sql instances list

# 取得連接名稱格式
# PROJECT_ID:REGION:INSTANCE_NAME
# 例如: focus-copilot-475707-s0:asia-east1:mysql-instance
```

#### 步驟 2: 更新部署腳本

編輯 `deploy-cloudrun.ps1`，設定 Cloud SQL 連接名稱：

```powershell
# 找到這一行（約第 15 行）
$CLOUD_SQL_CONNECTION_NAME = ""  # 將此處改為你的連接名稱

# 例如：
$CLOUD_SQL_CONNECTION_NAME = "focus-copilot-475707-s0:asia-east1:mysql-instance"
```

#### 步驟 3: 授權 Cloud Run 服務帳號

```powershell
# 取得 Cloud Run 服務帳號
$PROJECT_NUMBER = gcloud projects describe $PROJECT_ID --format="value(projectNumber)"
$SERVICE_ACCOUNT = "$PROJECT_NUMBER-compute@developer.gserviceaccount.com"

# 授予 Cloud SQL Client 角色
gcloud projects add-iam-policy-binding $PROJECT_ID `
  --member="serviceAccount:$SERVICE_ACCOUNT" `
  --role="roles/cloudsql.client"
```

#### 步驟 4: 重新部署

```powershell
.\deploy-cloudrun.ps1
```

### 方案 2: MySQL 是 Compute Engine VM

如果你的 MySQL 在 Compute Engine VM 上，需要配置防火牆規則。

#### 步驟 1: 取得 Cloud Run 的 IP 範圍

Cloud Run 的來源 IP 範圍可以從以下取得：

```powershell
# Cloud Run 服務會使用 VPC 連接器或公共 IP
# 需要允許來自 Cloud Run 的連接
```

#### 步驟 2: 在 MySQL VM 上配置防火牆

```powershell
# 建立防火牆規則允許 Cloud Run 連接
gcloud compute firewall-rules create allow-cloud-run-mysql `
  --allow tcp:3306 `
  --source-ranges 0.0.0.0/0 `
  --description "Allow Cloud Run to connect to MySQL" `
  --target-tags mysql-server

# 或使用更安全的 IP 範圍（推薦）
# 請查詢 Cloud Run 的 IP 範圍，通常類似：
# gcloud compute firewall-rules create allow-cloud-run-mysql `
#   --allow tcp:3306 `
#   --source-ranges 34.0.0.0/8 `
#   --description "Allow Cloud Run to connect to MySQL"
```

#### 步驟 3: 確認 MySQL 允許遠端連接

在 MySQL VM 上檢查 MySQL 配置：

```bash
# SSH 到 MySQL VM
gcloud compute ssh YOUR_VM_NAME --zone=YOUR_ZONE

# 檢查 MySQL 配置
sudo nano /etc/mysql/mysql.conf.d/mysqld.cnf

# 確認 bind-address 設定為：
# bind-address = 0.0.0.0   # 允許所有 IP 連接
# 或
# bind-address = 34.81.245.32  # 只允許特定 IP

# 重啟 MySQL
sudo systemctl restart mysql
```

#### 步驟 4: 更新 MySQL 使用者權限

```sql
-- 在 MySQL 中執行
CREATE USER IF NOT EXISTS 'demo_user'@'%' IDENTIFIED BY 'Showcase@2025!';
GRANT ALL PRIVILEGES ON taipeipass_db.* TO 'demo_user'@'%';
FLUSH PRIVILEGES;
```

#### 步驟 5: 重新部署

```powershell
.\deploy-cloudrun.ps1
```

## 🔧 進階配置

### 使用 VPC 連接器（最佳實踐）

如果 MySQL 在 VPC 網路上，建議使用 VPC 連接器：

```powershell
# 建立 VPC 連接器
gcloud compute networks vpc-access connectors create cloud-run-connector `
  --region=$REGION `
  --network=default `
  --range=10.8.0.0/28

# 部署時使用 VPC 連接器
gcloud run deploy $SERVICE_NAME `
  --vpc-connector=cloud-run-connector `
  --vpc-egress=all `
  --image=$IMAGE_NAME
```

### 使用 Secret Manager（安全）

```powershell
# 建立 Secret
echo -n "Showcase@2025!" | gcloud secrets create db-password --data-file=-

# 授予 Cloud Run 存取權限
$PROJECT_NUMBER = gcloud projects describe $PROJECT_ID --format="value(projectNumber)"
$SERVICE_ACCOUNT = "$PROJECT_NUMBER-compute@developer.gserviceaccount.com"

gcloud secrets add-iam-policy-binding db-password `
  --member="serviceAccount:$SERVICE_ACCOUNT" `
  --role="roles/secretmanager.secretAccessor"

# 部署時使用 Secret
gcloud run deploy $SERVICE_NAME `
  --set-secrets="DB_PASSWORD=db-password:latest" `
  --update-env-vars="ConnectionStrings__DefaultConnection=Server=34.81.245.32;Port=3306;Database=taipeipass_db;User=demo_user;Password=$$DB_PASSWORD;"
```

## 🐛 故障排除

### 檢查連接

```powershell
# 查看 Cloud Run 服務日誌
gcloud run services logs tail taipei-sports-api --region asia-east1

# 查看詳細錯誤
gcloud run services logs read taipei-sports-api --limit 50 --region asia-east1
```

### 測試資料庫連接

從 Cloud Run 服務內部測試（需要 SSH 到容器）：

```powershell
# 建立測試容器
gcloud run deploy test-mysql-connection `
  --image=mysql:8.0 `
  --command=mysql `
  --args="-h,34.81.245.32,-u,demo_user,-pShowcase@2025!,-e,SHOW DATABASES;" `
  --region=asia-east1 `
  --allow-unauthenticated
```

### 常見錯誤及解決方案

| 錯誤 | 原因 | 解決方案 |
|------|------|----------|
| `Connect Timeout expired` | 防火牆阻擋 | 配置防火牆規則 |
| `Access denied` | 使用者權限錯誤 | 檢查 MySQL 使用者權限 |
| `Unknown MySQL server host` | 主機名稱錯誤 | 確認 IP 或連接名稱正確 |
| `Too many connections` | 連接數限制 | 增加 MySQL max_connections |

## ✅ 驗證清單

部署後確認：

- [ ] Cloud SQL 連接名稱正確（如果使用 Cloud SQL）
- [ ] Cloud Run 服務帳號有 Cloud SQL Client 角色
- [ ] 防火牆規則允許連接（如果使用 VM）
- [ ] MySQL 允許遠端連接
- [ ] MySQL 使用者有正確權限
- [ ] 連接字串格式正確
- [ ] 日誌中沒有連接錯誤

## 📚 相關資源

- [Cloud SQL 連接器](https://cloud.google.com/sql/docs/mysql/connect-run)
- [Cloud Run 連接 VPC](https://cloud.google.com/run/docs/configuring/vpc)
- [MySQL 遠端連接配置](https://dev.mysql.com/doc/refman/8.0/en/connecting.html)

## 🆘 仍無法連接？

如果以上方法都無法解決，請提供：

1. MySQL 部署類型（Cloud SQL 或 Compute Engine）
2. `gcloud sql instances list` 的輸出（如果是 Cloud SQL）
3. Cloud Run 日誌中的完整錯誤訊息
4. MySQL 所在區域和網路配置

我可以根據你的具體情況提供更精確的解決方案！

