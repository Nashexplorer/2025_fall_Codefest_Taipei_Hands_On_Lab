/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-09 05:24:10
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 05:25:54
 * @FilePath: /frontend/cofeast/src/components/ui/Loading.tsx
 */
import { Backdrop, CircularProgress, Typography } from "@mui/material";

const Loading = ({ loading }: { loading: boolean }) => {
  return (
    <Backdrop
      sx={{
        color: "#fff",
        zIndex: 1500,
        backdropFilter: "blur(4px)",
        backgroundColor: "rgba(0, 0, 0, 0.3)",
        display: "flex",
        flexDirection: "column",
        gap: 2,
      }}
      open={loading}
    >
      <CircularProgress
        size={60}
        thickness={4}
        sx={{
          color: "#5AB4C5",
        }}
      />
      <Typography
        variant="h6"
        sx={{
          color: "#fff",
          fontWeight: 500,
          textShadow: "0 2px 4px rgba(0,0,0,0.3)",
        }}
      >
        搜尋中...
      </Typography>
    </Backdrop>
  );
};

export default Loading;
