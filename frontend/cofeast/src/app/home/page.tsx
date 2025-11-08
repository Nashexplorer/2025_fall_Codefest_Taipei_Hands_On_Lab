/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 16:20:13
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 04:59:05
 * @FilePath: /frontend/cofeast/src/app/home/page.tsx
 */
"use client";
import { Wrapper } from "@/components/layout/layoutStyle";
import theme from "@/theme";
import { Box, CardMedia, Typography } from "@mui/material";
import { ThemeProvider } from "@mui/material/styles";
import ActivityCard from "./components/ActivityCard";
import HorizontalCard from "./components/horizontalCard";

/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 16:20:13
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 20:54:35
 * @FilePath: /frontend/cofeast/src/app/home/page.tsx
 */
const HomePage = () => {
  return (
    <ThemeProvider theme={theme}>
      <Wrapper>
        <CardMedia
          component="img"
          image="/images/banner.png"
          sx={{
            width: "100%",
            margin: "16px 0px",
            borderRadius: "12px",
          }}
        />
        <Typography variant="h2" color="text.primary" mt={6}>
          共饗食光 CoFeast
        </Typography>
        <Box
          sx={{
            display: "flex",
            gap: 1.5,
            marginTop: 3,
            marginBottom: 5,
            flexWrap: "wrap",
          }}
        >
          <ActivityCard
            link="/built-event"
            title="我要發起共餐"
            imageUrl="/images/temp.jpg"
          />
          <ActivityCard
            link="/search-participate"
            title="我要響應共餐"
            imageUrl="/images/temp.jpg"
          />
        </Box>
        <Typography variant="h2" color="text.primary" my={3}>
          愛心餐食補給站
        </Typography>
        <HorizontalCard
          link="/search-store"
          title="查看愛心據點"
          imageUrl="/images/temp2.jpg"
        />
      </Wrapper>
    </ThemeProvider>
  );
};

export default HomePage;
