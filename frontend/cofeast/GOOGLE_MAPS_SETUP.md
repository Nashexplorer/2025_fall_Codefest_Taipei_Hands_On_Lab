# Google Maps API 整合說明

## 步驟 1: 修復 npm 權限問題並安裝套件

首先，修復 npm cache 權限問題：

```bash
sudo chown -R 501:20 "/Users/fangyukung/.npm"
```

然後安裝 Google Maps 套件：

```bash
cd /Users/fangyukung/2025_fall_Codefest_Taipei_Hands_On_Lab/frontend/cofeast
npm install @react-google-maps/api
```

## 步驟 2: 取得 Google Maps API Key

1. 前往 [Google Cloud Console](https://console.cloud.google.com/)
2. 創建新專案或選擇現有專案
3. 啟用以下 API：
   - Maps JavaScript API
   - Places API（如果要使用地點搜尋功能）
4. 前往「憑證」頁面創建 API 金鑰
5. （建議）設定 API 金鑰限制：
   - 應用程式限制：HTTP 參照網址（網站）
   - 網站限制：加入你的網域
   - API 限制：限制為 Maps JavaScript API 和 Places API

## 步驟 3: 設定環境變數

在專案根目錄創建 `.env.local` 檔案：

```bash
cp .env.local.example .env.local
```

編輯 `.env.local`，填入你的 API Key：

```env
NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=your_actual_api_key_here
```

**注意：** `.env.local` 檔案已經在 `.gitignore` 中，不會被提交到版本控制。

## 步驟 4: 使用 Google Maps 組件

### 基本地圖使用

```typescript
import GoogleMapComponent from "@/components/ui/GoogleMap";

export default function Page() {
  return (
    <GoogleMapComponent
      center={{ lat: 25.033, lng: 121.5654 }}
      zoom={15}
      markers={[
        { lat: 25.033, lng: 121.5654 },
        { lat: 25.034, lng: 121.5664 },
      ]}
    />
  );
}
```

### 帶搜尋功能的地圖

```typescript
import GoogleMapWithSearch from "@/components/ui/GoogleMapWithSearch";

export default function Page() {
  const handlePlaceSelected = (place: google.maps.places.PlaceResult) => {
    console.log("選擇的地點:", place.name);
    console.log("地址:", place.formatted_address);
  };

  return <GoogleMapWithSearch onPlaceSelected={handlePlaceSelected} />;
}
```

## 組件 Props 說明

### GoogleMapComponent

| Prop       | 類型                                         | 預設值    | 說明            |
| ---------- | -------------------------------------------- | --------- | --------------- |
| center     | `{ lat: number, lng: number }`               | 台北 101  | 地圖中心座標    |
| zoom       | `number`                                     | 15        | 縮放等級 (1-20) |
| markers    | `Array<{ lat: number, lng: number }>`        | `[]`      | 標記點陣列      |
| onMapClick | `(event: google.maps.MapMouseEvent) => void` | undefined | 地圖點擊事件    |

### GoogleMapWithSearch

| Prop            | 類型                                              | 預設值    | 說明         |
| --------------- | ------------------------------------------------- | --------- | ------------ |
| onPlaceSelected | `(place: google.maps.places.PlaceResult) => void` | undefined | 地點選擇回調 |

## 常見問題

### Q: 地圖顯示 "請設定環境變數" 錯誤

A: 確認 `.env.local` 檔案存在且包含正確的 API Key，然後重新啟動開發伺服器。

### Q: 地圖顯示灰色區域

A: 檢查以下項目：

1. API Key 是否正確
2. Maps JavaScript API 是否已啟用
3. 是否有設定過嚴格的 API Key 限制
4. 瀏覽器控制台是否有錯誤訊息

### Q: 搜尋功能無法使用

A: 確認 Places API 已在 Google Cloud Console 中啟用。

## 進階自訂

你可以修改組件來添加更多功能：

- 自訂地圖樣式
- 添加路線規劃
- 顯示交通狀況
- 添加 Info Window
- 使用不同的標記圖示

參考 [Google Maps JavaScript API 文件](https://developers.google.com/maps/documentation/javascript)
