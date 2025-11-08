/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 16:45:00
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 05:45:41
 * @FilePath: /frontend/cofeast/src/app/search-store/components/GoogleMapWithSearch.tsx
 */
"use client";

import { getAllSupportPoints, SupportPoint } from "@/api/stores";
import { GoogleMap, LoadScript, Marker } from "@react-google-maps/api";
import { useEffect, useState } from "react";
import StoreInfoCard from "../../../components/ui/StoreInfoCard";

const containerStyle = {
  width: "100%",
  height: "100vh",
};

const defaultCenter = {
  lat: 25.033,
  lng: 121.5654,
};

const libraries: "places"[] = ["places"];

const GoogleMapWithSearch = () => {
  const [center, setCenter] = useState(defaultCenter);
  const [stores, setStores] = useState<SupportPoint[]>([]);
  const [selectedStore, setSelectedStore] = useState<SupportPoint | null>(null);

  // 載入據點資料
  useEffect(() => {
    const loadStores = async () => {
      const storesData = await getAllSupportPoints();
      console.log("storesData", storesData);
      setStores(storesData.data);
    };
    loadStores();
  }, []);

  const onMapClick = () => {
    // 點擊地圖空白處時關閉卡片
    setSelectedStore(null);
  };

  const onMarkerClick = (store: SupportPoint) => {
    setSelectedStore(store);
    // 將地圖中心移動到選中的據點
    setCenter({ lat: store.lat, lng: store.lon });
  };

  const apiKey = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;

  if (!apiKey) {
    return;
  }

  return (
    <LoadScript googleMapsApiKey={apiKey} libraries={libraries}>
      <div style={{ position: "relative" }}>
        <GoogleMap
          mapContainerStyle={containerStyle}
          center={center}
          zoom={15}
          onClick={onMapClick}
          options={{
            streetViewControl: false,
            mapTypeControl: true,
            fullscreenControl: true,
            zoomControl: true,
          }}
        >
          {/* 顯示所有據點的標記 */}
          {stores.map((store) => (
            <Marker
              key={store.id}
              position={{ lat: store.lat, lng: store.lon }}
              onClick={() => onMarkerClick(store)}
              icon={{
                url:
                  selectedStore?.id === store.id
                    ? "http://maps.google.com/mapfiles/ms/icons/blue-dot.png"
                    : "http://maps.google.com/mapfiles/ms/icons/red-dot.png",
              }}
            />
          ))}
        </GoogleMap>

        {/* 顯示據點資訊卡片 */}
        {selectedStore && (
          <StoreInfoCard
            store={selectedStore}
            onClose={() => setSelectedStore(null)}
          />
        )}
      </div>
    </LoadScript>
  );
};

export default GoogleMapWithSearch;
