import { ParticipateItem } from "@/api/participate";
import theme from "@/theme";
import AccessTimeIcon from "@mui/icons-material/AccessTime";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import PeopleIcon from "@mui/icons-material/People";
import {
  Box,
  CardActionArea,
  CardContent,
  Link,
  Typography,
} from "@mui/material";
import Image from "next/image";
import EntryCard from "./EntryCard";

/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-09 01:10:20
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 02:15:40
 * @FilePath: /frontend/cofeast/src/components/ui/participateCard.tsx
 */

interface ParticipateCardProps {
  data: ParticipateItem;
}

const ParticipateCard = ({ data }: ParticipateCardProps) => {
  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleString("zh-TW", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    });
  };

  const formatTimeRange = () => {
    const start = formatTime(data.startTime);
    const end = new Date(data.endTime).toLocaleTimeString("zh-TW", {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    });
    return `${start} ~ ${end}`;
  };

  const formatDistance = (distance: number) => {
    return `${distance.toFixed(1)}km`;
  };

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
              src="/images/mealIcon.svg"
              alt="meal icon"
              width={17}
              height={17}
            />
            <Typography
              variant="h3SemiBold"
              component="div"
              color="palette.primary.main"
            >
              {data.title}
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
              href={`https://www.google.com/maps?q=${data.latitude},${data.longitude}`}
              target="_blank"
              rel="noopener noreferrer"
              sx={{
                color: "text.secondary",
                textDecoration: "underline",
              }}
              onClick={(e) => e.stopPropagation()}
            >
              {data.fullAddress}
            </Link>
            <Typography variant="body2" color="text.secondary">
              {formatDistance(data.distance)}
            </Typography>
          </Box>

          <Box
            sx={{
              backgroundColor: `${theme.palette.primary.light}`,
              borderRadius: "8px",
              padding: "16px",
              mb: 2,
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              border: `1px solid ${theme.palette.primary.main}`,
            }}
          >
            <Box sx={{ display: "flex", alignItems: "center", gap: "8px" }}>
              <AccessTimeIcon sx={{ color: "text.secondary", fontSize: 20 }} />
              <Typography variant="body2" color="text.secondary">
                {formatTimeRange()}
              </Typography>
            </Box>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                gap: "8px",
                borderLeft: "1px solid #E0E0E0",
                paddingLeft: "16px",
              }}
            >
              <PeopleIcon sx={{ color: "text.secondary", fontSize: 20 }} />
              <Typography variant="body2" color="text.secondary">
                {data.currentParticipants} / {data.capacity}
              </Typography>
            </Box>
          </Box>
          <Link href={`/participate/${data.id}`}>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
              }}
            >
              <Typography variant="body" color="text.secondary">
                響應這場共餐
              </Typography>
              <ChevronRightIcon sx={{ color: "text.secondary" }} />
            </Box>
          </Link>
        </CardContent>
      </CardActionArea>
    </EntryCard>
  );
};

export default ParticipateCard;
