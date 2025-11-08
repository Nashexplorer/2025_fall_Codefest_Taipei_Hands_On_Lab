/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 19:00:00
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 05:43:10
 * @FilePath: /frontend/cofeast/src/app/search-store/components/StoreInfoCard.tsx
 */
"use client";

import { SupportPoint } from "@/api/stores";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import { Box, Card, Typography } from "@mui/material";
import Image from "next/image";

interface StoreInfoCardProps {
  store: SupportPoint;
  onClose: () => void;
}

const StoreInfoCard = ({ store, onClose }: StoreInfoCardProps) => {
  return (
    <Card
      sx={{
        position: "absolute",
        bottom: 30,
        left: "50%",
        transform: "translateX(-50%)",
        width: { xs: "90%", sm: "600px" },
        maxWidth: "90vw",
        boxShadow: "0 4px 20px rgba(0,0,0,0.1)",
        borderRadius: 3,
        zIndex: 1000,
        animation: "slideUp 0.3s ease-out",
        overflow: "hidden",
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
      {/* 頂部橫幅圖片 */}
      <Box
        sx={{
          position: "relative",
          width: "100%",
          height: 150,
          background: "linear-gradient(135deg, #5AB9C1 0%, #7BC9D1 100%)",
        }}
      >
        <Image
          src="/images/banner.png"
          alt="共饗光"
          fill
          style={{ objectFit: "cover" }}
          priority
        />
      </Box>

      {/* 內容區域 */}
      <Box sx={{ p: 3 }}>
        {/* 據點名稱 */}
        <Typography
          variant="h6"
          component="h2"
          sx={{
            fontWeight: 600,
            mb: 2,
            color: "#333",
            fontSize: "1.1rem",
          }}
        >
          {store.orgName}
        </Typography>

        {/* 地址 */}
        <Box
          sx={{
            display: "flex",
            alignItems: "flex-start",
            mb: 2,
            pb: 2,
            borderBottom: "1px solid #f0f0f0",
          }}
        >
          <LocationOnIcon
            sx={{
              fontSize: 22,
              color: "#666",
              mr: 1,
              mt: 0.2,
            }}
          />
          <Typography
            variant="body2"
            sx={{
              color: "#666",
              fontSize: "0.95rem",
              lineHeight: 1.6,
            }}
          >
            {store.address}
          </Typography>
        </Box>

        {/* 查看詳細資訊 */}
        <Box
          onClick={onClose}
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            cursor: "pointer",
            "&:hover": {
              "& .detail-text": {
                color: "primary.main",
              },
            },
          }}
        >
          <Typography
            className="detail-text"
            variant="body2"
            sx={{
              color: "#999",
              fontSize: "0.9rem",
              transition: "color 0.2s",
            }}
          >
            查看詳細資訊
          </Typography>
          <ChevronRightIcon
            sx={{
              fontSize: 24,
              color: "#999",
            }}
          />
        </Box>
      </Box>
    </Card>
  );
};

export default StoreInfoCard;
