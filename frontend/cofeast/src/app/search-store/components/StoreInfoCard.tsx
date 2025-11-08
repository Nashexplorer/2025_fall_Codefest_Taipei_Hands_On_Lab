/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 19:00:00
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 20:04:57
 * @FilePath: /frontend/cofeast/src/app/search-store/components/StoreInfoCard.tsx
 */
"use client";

import { SupportPoint } from "@/api/stores";
import CloseIcon from "@mui/icons-material/Close";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import PhoneIcon from "@mui/icons-material/Phone";
import { Box, Card, CardContent, IconButton, Typography } from "@mui/material";

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
        width: { xs: "90%", sm: "400px" },
        maxWidth: "90vw",
        boxShadow: "0 8px 32px rgba(0,0,0,0.15)",
        borderRadius: 3,
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
      <CardContent sx={{ p: 3, "&:last-child": { pb: 3 } }}>
        {/* 關閉按鈕 */}
        <IconButton
          onClick={onClose}
          sx={{
            position: "absolute",
            top: 8,
            right: 8,
            color: "text.secondary",
          }}
        >
          <CloseIcon />
        </IconButton>

        {/* 據點名稱 */}
        <Typography
          variant="h6"
          component="h2"
          sx={{
            fontWeight: 600,
            mb: 1,
            pr: 4,
            color: "primary.main",
          }}
        >
          {store.orgName}
        </Typography>

        {/* 據點類別 */}
        <Typography
          variant="body2"
          sx={{
            mb: 1.5,
            color: "primary.main",
            backgroundColor: "primary.light",
            display: "inline-block",
            px: 1.5,
            py: 0.5,
            borderRadius: 1,
          }}
        >
          {store.orgGroupName}
        </Typography>

        {/* 行政區 */}
        <Typography
          variant="body2"
          color="text.secondary"
          sx={{ mb: 1.5, fontWeight: 500 }}
        >
          📍 {store.division}
        </Typography>

        {/* 地址 */}
        <Box sx={{ display: "flex", alignItems: "flex-start", mb: 1.5 }}>
          <LocationOnIcon
            sx={{
              fontSize: 20,
              color: "action.active",
              mr: 1,
              mt: 0.3,
            }}
          />
          <Typography variant="body2" color="text.secondary">
            {store.address}
          </Typography>
        </Box>

        {/* 電話 */}
        {store.phone && (
          <Box sx={{ display: "flex", alignItems: "center", mb: 1.5 }}>
            <PhoneIcon
              sx={{
                fontSize: 20,
                color: "action.active",
                mr: 1,
              }}
            />
            <Typography
              variant="body2"
              color="text.secondary"
              component="a"
              href={`tel:${store.phone}`}
              sx={{
                textDecoration: "none",
                color: "text.secondary",
                "&:hover": {
                  color: "primary.main",
                  textDecoration: "underline",
                },
              }}
            >
              {store.phone}
            </Typography>
          </Box>
        )}
      </CardContent>
    </Card>
  );
};

export default StoreInfoCard;
