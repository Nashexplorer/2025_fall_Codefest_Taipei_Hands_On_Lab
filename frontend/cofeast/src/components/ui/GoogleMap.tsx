/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 16:45:00
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 16:45:00
 * @FilePath: /frontend/cofeast/src/components/ui/GoogleMap.tsx
 */
"use client";

import { GoogleMap, LoadScript, Marker } from "@react-google-maps/api";
import { useCallback, useState } from "react";

// 地圖容器樣式
const containerStyle = {
  width: "100%",
  height: "500px",
};

// 預設中心位置（台北 101）
const defaultCenter = {
  lat: 25.033,
  lng: 121.5654,
};

interface GoogleMapComponentProps {
  center?: google.maps.LatLngLiteral;
  zoom?: number;
  markers?: google.maps.LatLngLiteral[];
  onMapClick?: (event: google.maps.MapMouseEvent) => void;
}

const GoogleMapComponent = ({
  center = defaultCenter,
  zoom = 15,
  markers = [],
  onMapClick,
}: GoogleMapComponentProps) => {
  const [map, setMap] = useState<google.maps.Map | null>(null);

  const onLoad = useCallback((map: google.maps.Map) => {
    setMap(map);
  }, []);

  const onUnmount = useCallback(() => {
    setMap(null);
  }, []);

  const apiKey = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;

  if (!apiKey) {
    return (
      <div
        style={{
          ...containerStyle,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          backgroundColor: "#f0f0f0",
        }}
      >
        <p style={{ color: "#d45251" }}>
          請設定 NEXT_PUBLIC_GOOGLE_MAPS_API_KEY 環境變數
        </p>
      </div>
    );
  }

  return (
    <LoadScript googleMapsApiKey={apiKey}>
      <GoogleMap
        mapContainerStyle={containerStyle}
        center={center}
        zoom={zoom}
        onLoad={onLoad}
        onUnmount={onUnmount}
        onClick={onMapClick}
        options={{
          streetViewControl: false,
          mapTypeControl: true,
          fullscreenControl: true,
          zoomControl: true,
        }}
      >
        {/* 顯示標記點 */}
        {markers.map((position, index) => (
          <Marker key={index} position={position} />
        ))}
      </GoogleMap>
    </LoadScript>
  );
};

export default GoogleMapComponent;
