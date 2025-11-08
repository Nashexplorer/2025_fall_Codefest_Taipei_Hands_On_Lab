import EntryCard from "@/components/ui/EntryCard";
import ArrowForwardIosIcon from "@mui/icons-material/ArrowForwardIos";
import { Box, Link, Typography } from "@mui/material";
import Image from "next/image";
const ActivityCard = ({
  title,
  imageUrl,
  link,
}: {
  title: string;
  imageUrl: string;
  link: string;
}) => {
  return (
    <Box
      sx={{
        width: { xs: "calc(50% - 6px)", sm: "calc(50% - 6px)" },
      }}
    >
      <Link href={link} sx={{ textDecoration: "none" }}>
        <EntryCard
          sx={{
            textAlign: "center",
            padding: "12px",
            boxSizing: "border-box",
            margin: "0px",
          }}
        >
          <Box
            sx={{
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
            }}
          >
            <Box
              sx={{
                position: "relative",
                width: "100%",
                aspectRatio: "4/3",
                height: "200px",
              }}
            >
              <Image
                src={imageUrl}
                fill
                style={{ objectFit: "cover" }}
                alt={title}
              />
            </Box>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                width: "100%",
                mt: 1,
              }}
            >
              <Typography
                variant="bodySemiBold"
                color="text.primary"
                textAlign="left"
              >
                {title}
              </Typography>
              <ArrowForwardIosIcon />
            </Box>
          </Box>
        </EntryCard>
      </Link>
    </Box>
  );
};

export default ActivityCard;
