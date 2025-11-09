/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 15:16:23
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 09:05:51
 * @FilePath: /frontend/cofeast/src/app/layout.tsx
 */
import { AppRouterCacheProvider } from "@mui/material-nextjs/v15-appRouter";
// or `v1X-appRouter` if you are using Next.js v1X
import "@/app/globals.css";
import theme from "@/theme";
import { ThemeProvider } from "@mui/material/styles";
import type { Metadata } from "next";
import { Roboto } from "next/font/google";

const roboto = Roboto({
  weight: ["300", "400", "500", "700"],
  subsets: ["latin"],
  variable: "--font-roboto",
});

export const metadata: Metadata = {
  title: "共饗食光 CoFeast",
  description: "共饗食光 CoFeast 是一個愛心餐食補給站與共餐據點查詢平台",
};

export default function RootLayout(props: { children: React.ReactNode }) {
  const { children } = props;
  return (
    <html lang="en" className={roboto.variable}>
      <body>
        <AppRouterCacheProvider>
          <ThemeProvider theme={theme}>{children} </ThemeProvider>
        </AppRouterCacheProvider>
      </body>
    </html>
  );
}
