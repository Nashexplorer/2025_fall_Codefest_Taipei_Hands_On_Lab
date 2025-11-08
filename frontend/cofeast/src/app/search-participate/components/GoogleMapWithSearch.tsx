/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 16:45:00
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 04:48:33
 * @FilePath: /frontend/cofeast/src/app/search-participate/components/GoogleMapWithSearch.tsx
 */
"use client";

import { ParticipateItem } from "@/api/participate";
import ParticipateCard from "@/components/ui/participateCard";
import { Box } from "@mui/material";
import { GoogleMap, LoadScript, Marker } from "@react-google-maps/api";
import { useEffect, useState } from "react";

const containerStyle = {
  width: "100%",
  height: "100vh",
};

const defaultCenter = {
  lat: 25.033,
  lng: 121.5654,
};

const libraries: "places"[] = ["places"];

interface GoogleMapWithSearchProps {
  participateList: ParticipateItem[];
}

const GoogleMapWithSearch = ({ participateList }: GoogleMapWithSearchProps) => {
  const [center, setCenter] = useState(defaultCenter);
  const [selectedStore, setSelectedStore] = useState<ParticipateItem | null>(
    null
  );

  useEffect(() => {
    if (participateList.length > 0) {
      setCenter({
        lat: participateList[0].latitude,
        lng: participateList[0].longitude,
      });
    }
  }, [participateList]);

  const onMapClick = () => {
    setSelectedStore(null);
  };

  const onMarkerClick = (store: ParticipateItem) => {
    setSelectedStore(store);
    setCenter({ lat: store.latitude, lng: store.longitude });
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
          {participateList.map((item) => (
            <Marker
              key={item.id}
              position={{ lat: item.latitude, lng: item.longitude }}
              onClick={() => onMarkerClick(item)}
              icon={{
                url:
                  selectedStore?.id === item.id
                    ? "http://maps.google.com/mapfiles/ms/icons/blue-dot.png"
                    : "http://maps.google.com/mapfiles/ms/icons/red-dot.png",
              }}
            />
          ))}
        </GoogleMap>

        {selectedStore && (
          <Box
            sx={{
              position: "absolute",
              bottom: "160px",
              left: "50%",
              transform: "translateX(-50%)",
              width: { xs: "90%", sm: "400px" },
              maxWidth: "90vw",
              zIndex: 1000,
              animation: "slideUp 0.3s ease-out",
              "@keyframes slideUp": {
                from: {
                  transform: "translateX(-50%) translateY(100%)",
                  opacity: 0,
                },
                to: {
                  transform: "translateX(-50%) translateY(0)",
                  opacity: 1,
                },
              },
            }}
          >
            <ParticipateCard data={selectedStore} />
          </Box>
        )}
      </div>
    </LoadScript>
  );
};

export default GoogleMapWithSearch;
