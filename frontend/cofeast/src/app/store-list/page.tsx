"use client";
import { getAllSupportPoints, SupportPoint } from "@/api/stores";
import { Wrapper } from "@/components/layout/layoutStyle";
import Loading from "@/components/ui/Loading";
import StoreCard from "@/components/ui/StoreCard";
import { Box, Typography } from "@mui/material";
import { useEffect, useState } from "react";

/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-09 01:04:44
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 05:56:38
 * @FilePath: /frontend/cofeast/src/app/store-list/page.tsx
 */

const StoreListPage = () => {
  const [storeList, setStoreList] = useState<SupportPoint[]>([]);
  const [loading, setLoading] = useState(false);
  const [page] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  useEffect(() => {
    const fetchStoreList = async () => {
      setLoading(true);
      try {
        const result = await getAllSupportPoints(page, 50);
        setStoreList(result.data);
        setTotalCount(result.totalCount);
      } catch (error) {
        console.error("獲取商店列表錯誤:", error);
        setStoreList([]);
      } finally {
        setLoading(false);
      }
    };

    fetchStoreList();
  }, [page]);

  return (
    <>
      <Wrapper>
        <Box sx={{ p: 2 }}>
          {/* 標題 */}
          <Box sx={{ mb: 3 }}>
            <Typography
              variant="h5"
              component="h1"
              fontWeight="bold"
              sx={{ mb: 1 }}
            >
              饗食服務據點列表
            </Typography>
            <Typography variant="body2" color="text.secondary">
              共 {totalCount} 個據點
            </Typography>
          </Box>

          {/* 內容區域 */}
          {loading ? (
            <Loading loading={loading} />
          ) : storeList.length > 0 ? (
            <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
              {storeList.map((item) => {
                return <StoreCard key={item.id} data={item} />;
              })}
            </Box>
          ) : (
            <Box
              sx={{
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                minHeight: "200px",
              }}
            >
              <Typography variant="body1" color="text.secondary">
                目前沒有饗食服務據點資料
              </Typography>
            </Box>
          )}
        </Box>
      </Wrapper>
    </>
  );
};

export default StoreListPage;
