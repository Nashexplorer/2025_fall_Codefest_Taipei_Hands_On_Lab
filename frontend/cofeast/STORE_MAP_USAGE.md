# 商店地圖功能使用說明

## 概述

商店地圖頁面 (`/map-demo`) 提供以下功能：

1. ✅ 顯示所有商店的列表和地圖標記
2. ✅ 顯示使用者當前位置
3. ✅ 計算並顯示商店與使用者的距離
4. ✅ 互動式地圖操作

## 主要功能

### 1. 商店列表顯示

左側面板顯示所有商店的詳細資訊：

- 🏪 商店名稱
- ⭐ 評分（如果有）
- 📍 地址
- 📞 電話
- 🚶 距離（當開啟定位後）

### 2. 獲取當前位置

點擊「📍 我的位置」按鈕可以：

- 獲取使用者當前的地理位置
- 在地圖上顯示藍色圓形標記
- 自動計算並顯示與各商店的距離
- 地圖自動移動到使用者位置

### 3. 互動式地圖

地圖功能：

- 紅色標記表示商店位置
- 藍色圓形標記表示使用者位置
- 點擊商店標記可顯示詳細資訊視窗
- 支援地圖縮放、平移等操作

### 4. 商店選擇

點擊左側列表中的商店卡片：

- 商店卡片會有藍色邊框高亮
- 地圖自動移動到該商店位置
- 自動放大顯示商店詳情

## 技術實現

### API 介面

商店資料來自 `/src/api/stores.ts`：

```typescript
// 獲取所有商店
getAllStores(): Promise<Store[]>

// 獲取附近商店（可選功能）
getNearbyStores(lat: number, lng: number, radius: number): Promise<Store[]>
```

### Store 資料結構

```typescript
interface Store {
  id: string;
  name: string;
  address: string;
  latitude: number;
  longitude: number;
  phone?: string;
  description?: string;
  rating?: number;
  image?: string;
}
```

### 距離計算

使用 Haversine 公式計算地球表面兩點間的實際距離：

- 小於 1 公里時，以公尺顯示
- 大於 1 公里時，以公里顯示（保留一位小數）

## 環境設定

確保已設定以下環境變數：

```bash
NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=你的_Google_Maps_API_金鑰
NEXT_PUBLIC_API_BASE_URL=後端_API_網址  # 可選，預設為 http://localhost:8000/api
```

## 使用流程

1. **首次載入**

   - 頁面會自動呼叫 API 載入所有商店資料
   - 地圖預設中心為台北 101（lat: 25.033, lng: 121.5654）

2. **開啟定位**

   - 點擊「📍 我的位置」按鈕
   - 瀏覽器會請求定位權限
   - 允許權限後，地圖會移動到您的位置
   - 商店列表會顯示與您的距離

3. **查看商店詳情**
   - 點擊列表中的商店卡片
   - 或點擊地圖上的紅色標記
   - 會顯示商店的詳細資訊

## 測試資料

如果 API 無法連接，系統會自動使用測試資料，包含台北市的 5 個模擬商店：

- 台北 101 咖啡廳
- 台北車站餐廳
- 西門町美食廣場
- 東區精緻料理
- 士林夜市小吃

## 瀏覽器支援

定位功能需要：

- HTTPS 連線（開發環境的 localhost 除外）
- 瀏覽器支援 Geolocation API
- 使用者授予定位權限

## 後續改進建議

1. **效能優化**

   - 實作商店資料分頁
   - 只載入地圖可視範圍內的商店

2. **功能增強**

   - 商店分類篩選
   - 按距離排序
   - 路線導航功能
   - 商店詳情頁面

3. **使用者體驗**
   - 加入載入骨架屏
   - 離線模式支援
   - 商店圖片展示

## 故障排除

### 無法獲取位置

- 檢查瀏覽器是否允許定位權限
- 確認網站使用 HTTPS 連線
- 檢查設備的定位服務是否開啟

### API 連接失敗

- 檢查 `NEXT_PUBLIC_API_BASE_URL` 設定
- 確認後端服務正在運行
- 查看瀏覽器控制台的錯誤訊息

### 地圖無法顯示

- 確認 `NEXT_PUBLIC_GOOGLE_MAPS_API_KEY` 已正確設定
- 檢查 API 金鑰是否有效且未超出配額
- 確認 API 金鑰已啟用 Maps JavaScript API 和 Places API

## 相關檔案

- `/src/app/map-demo/page.tsx` - 主要頁面組件
- `/src/api/stores.ts` - API 介面和測試資料
- `/src/components/ui/GoogleMap.tsx` - 基本地圖組件
- `/src/components/ui/GoogleMapWithSearch.tsx` - 帶搜尋功能的地圖組件
