/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-09
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 08:17:00
 * @FilePath: /frontend/cofeast/src/components/ui/StoreCard.tsx
 */
"use client";

import { SupportPoint } from "@/api/stores";
import theme from "@/theme";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import PhoneIcon from "@mui/icons-material/Phone";
import {
  Box,
  CardActionArea,
  CardContent,
  CardMedia,
  Link,
  Typography,
} from "@mui/material";
import Image from "next/image";
import { useRouter } from "next/navigation";
import EntryCard from "./EntryCard";

interface StoreCardProps {
  data: SupportPoint;
}

const StoreCard = ({ data }: StoreCardProps) => {
  const router = useRouter();

  const handleCardClick = () => {
    router.push(`/search-store-detail/${data.id}`);
  };

  return (
    <EntryCard>
      <CardActionArea onClick={handleCardClick}>
        <Box
          sx={{
            borderRadius: "8px",
            overflow: "hidden",
            margin: "0px 16px ",
          }}
        >
          <CardMedia
            sx={{
              width: "100%",
            }}
            component="img"
            image="/images/store.png"
            alt={data.orgName}
          />
        </Box>
        <CardContent>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: "8px",
              mb: 1,
            }}
          >
            <Typography
              variant="h3SemiBold"
              component="div"
              color="palette.primary.main"
            >
              {data.orgName}
            </Typography>
          </Box>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: "6px",
              mb: 2,
            }}
          >
            <Image
              src="/images/location.svg"
              alt="location icon"
              width={16}
              height={18}
            />
            <Link
              component="a"
              variant="body2"
              href={`https://www.google.com/maps?q=${data.lat},${data.lon}`}
              target="_blank"
              rel="noopener noreferrer"
              sx={{
                color: "text.secondary",
                textDecoration: "underline",
              }}
              onClick={(e) => e.stopPropagation()}
            >
              {data.address}
            </Link>
          </Box>

          {/* 電話資訊 */}
          {data.phone && (
            <Box
              sx={{
                backgroundColor: `${theme.palette.primary.light}`,
                borderRadius: "8px",
                padding: "16px",
                mb: 2,
                display: "flex",
                alignItems: "center",
                gap: "8px",
                border: `1px solid ${theme.palette.primary.main}`,
              }}
            >
              <PhoneIcon sx={{ color: "text.secondary", fontSize: 20 }} />
              <Link
                href={`tel:${data.phone}`}
                sx={{
                  color: "text.secondary",
                  textDecoration: "none",
                  "&:hover": {
                    textDecoration: "underline",
                  },
                }}
                onClick={(e) => e.stopPropagation()}
              >
                <Typography variant="body2">{data.phone}</Typography>
              </Link>
            </Box>
          )}

          {/* 查看詳情 */}
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Typography variant="body" color="text.secondary">
              查看詳情
            </Typography>
            <ChevronRightIcon sx={{ color: "text.secondary" }} />
          </Box>
        </CardContent>
      </CardActionArea>
    </EntryCard>
  );
};

export default StoreCard;
