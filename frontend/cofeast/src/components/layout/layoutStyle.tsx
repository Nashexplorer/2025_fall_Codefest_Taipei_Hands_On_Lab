/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 17:27:03
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 17:28:55
 * @FilePath: /frontend/cofeast/src/components/layout/layoutStyle.tsx
 */
"use client";

import theme from "@/theme";
import { Box } from "@mui/material";
import { styled } from "@mui/material/styles";

export const Wrapper = styled(Box)(() => {
  return {
    padding: "1px 16px 16px",
    backgroundColor: theme.palette.grey[50],
    minHeight: "100vh",
  };
});
