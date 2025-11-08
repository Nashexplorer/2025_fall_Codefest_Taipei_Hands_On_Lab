import theme from "@/theme";
import { Box, Typography } from "@mui/material";

/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 22:51:53
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 22:57:53
 * @FilePath: /frontend/cofeast/src/components/ui/Announcement.tsx
 */
const Announcement = ({ content }: { content: string }) => {
  return (
    <Box
      sx={{
        backgroundColor: theme.palette.white.main,
        width: "100%",
      }}
    >
      <Box
        sx={{
          backgroundColor: "primary.light",
          width: "100%",
          padding: 2,
          borderRadius: "10px",
          marginTop: 2,
          marginBottom: 2,
        }}
      >
        <Typography variant="body" color="text.secondary">
          {content}
        </Typography>
      </Box>
    </Box>
  );
};

export default Announcement;
