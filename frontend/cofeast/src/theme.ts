/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 15:44:24
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 20:46:21
 * @FilePath: /frontend/cofeast/src/theme.ts
 */
"use client";
import { createTheme } from "@mui/material/styles";

// 擴展 Material-UI 的類型定義
declare module "@mui/material/styles" {
  interface Palette {
    gradient: {
      main: string;
    };
    white: {
      main: string;
    };
  }
  interface PaletteOptions {
    gradient?: {
      main: string;
    };
    white?: {
      main: string;
    };
  }
  interface TypeText {
    third: string;
  }
  interface TypographyVariants {
    h1SemiBold: React.CSSProperties;
    h2SemiBold: React.CSSProperties;
    h3SemiBold: React.CSSProperties;
    body: React.CSSProperties;
    bodySemiBold: React.CSSProperties;
    captionSemiBold: React.CSSProperties;
  }
  interface TypographyVariantsOptions {
    h1SemiBold?: React.CSSProperties;
    h2SemiBold?: React.CSSProperties;
    h3SemiBold?: React.CSSProperties;
    body?: React.CSSProperties;
    bodySemiBold?: React.CSSProperties;
    captionSemiBold?: React.CSSProperties;
  }
}

declare module "@mui/material/Typography" {
  interface TypographyPropsVariantOverrides {
    h1SemiBold: true;
    h2SemiBold: true;
    h3SemiBold: true;
    body: true;
    bodySemiBold: true;
    captionSemiBold: true;
  }
}

export const theme = createTheme({
  palette: {
    primary: {
      main: "#5AB4C5",
      light: "#EDF8FA",
      dark: "#468D9B",
    },
    secondary: {
      light: "#FDF8ED",
      main: "#F5BA4B",
      dark: "#E7A43C",
    },
    grey: {
      50: "#F1F3F4", //page-background-color
      100: "#E3E7E9", //btn-background-color
      200: "#738995", //card-background-color
    },
    text: {
      primary: "#30383D",
      secondary: "#475259",
      third: "#91A0A8",
    },
    gradient: {
      main: "#44B6C7", // 使用純色代替漸變，漸變效果請使用 sx prop
    },
    success: {
      main: "#76A732",
    },
    error: {
      main: "#D45251",
    },
    warning: {
      main: "#FD853A",
    },
    white: {
      main: "#FFFFFF",
    },
  },
  typography: {
    fontFamily: '"PingFang TC", "Helvetica", "Arial", sans-serif',
    h1: {
      fontSize: "2.25rem",
      lineHeight: "3rem",
      fontWeight: 500,
    },
    h1SemiBold: {
      fontSize: "2.25rem",
      lineHeight: "3rem",
      fontWeight: 600,
    },
    h2: {
      fontSize: "1.5rem",
      lineHeight: "2rem",
      fontWeight: 500,
    },
    h2SemiBold: {
      fontSize: "1.5rem",
      lineHeight: "2rem",
      fontWeight: 600,
    },
    h3: {
      fontSize: "1rem",
      lineHeight: "1.375rem",
      fontWeight: 500,
    },
    h3SemiBold: {
      fontSize: "1rem",
      lineHeight: "1.375rem",
      fontWeight: 600,
    },
    body: {
      fontSize: "0.875rem",
      lineHeight: "1.375rem",
      fontWeight: 400,
    },
    bodySemiBold: {
      fontSize: "0.875rem",
      lineHeight: "1.375rem",
      fontWeight: 600,
    },
    caption: {
      fontSize: "0.75rem",
      lineHeight: "1.25rem",
      fontWeight: 500,
    },
    captionSemiBold: {
      fontSize: "0.75rem",
      lineHeight: "1.25rem",
      fontWeight: 600,
    },
  },
});

export default theme;
