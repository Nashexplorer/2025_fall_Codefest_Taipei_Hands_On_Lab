/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 15:16:23
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 20:29:53
 * @FilePath: /frontend/cofeast/next.config.ts
 */
import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: "https://gongcan-api-d4vsmusihq-de.a.run.app/api/:path*",
      },
    ];
  },
};

export default nextConfig;
