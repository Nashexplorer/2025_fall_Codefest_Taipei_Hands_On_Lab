/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-09
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 05:55:42
 * @FilePath: /frontend/cofeast/src/components/ui/StoreCard.tsx
 */
import { SupportPoint } from "@/api/stores";
import theme from "@/theme";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import PhoneIcon from "@mui/icons-material/Phone";
import {
  Box,
  CardActionArea,
  CardContent,
  Link,
  Typography,
} from "@mui/material";
import Image from "next/image";
import EntryCard from "./EntryCard";

interface StoreCardProps {
  data: SupportPoint;
}

const StoreCard = ({ data }: StoreCardProps) => {
  return (
    <EntryCard>
      <CardActionArea onClick={() => console.log("click", data)}>
        <CardContent>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: "8px",
              mb: 1,
            }}
          >
            <Image
              src="/images/store.png"
              alt="store icon"
              width={24}
              height={24}
            />
            <Typography
              variant="h3SemiBold"
              component="div"
              color="palette.primary.main"
            >
              {data.orgName}
            </Typography>
          </Box>

          {/* 組織單位 */}
          {data.orgGroupName && (
            <Typography
              variant="body2"
              color="text.secondary"
              sx={{ mb: 1, ml: 4 }}
            >
              {data.orgGroupName}
            </Typography>
          )}

          {/* 地址 */}
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
