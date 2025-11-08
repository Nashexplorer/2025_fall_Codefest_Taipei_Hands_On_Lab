/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 18:12:53
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 04:51:47
 * @FilePath: /frontend/cofeast/src/app/search-participate/page.tsx
 */
"use client";

import { getParticipateList, ParticipateItem } from "@/api/participate";
import GoogleMapWithSearch from "@/app/search-participate/components/GoogleMapWithSearch";
import SearchBar from "@/app/search-store/components/SearchBar";
import {
  Backdrop,
  Box,
  Button,
  CircularProgress,
  Typography,
} from "@mui/material";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

const SearchParticipate = (): React.ReactNode => {
  const [address, setAddress] = useState("台大體育館");
  const [participateList, setParticipateList] = useState<ParticipateItem[]>([]);
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  useEffect(() => {
    const fetchParticipateList = async () => {
      const searchAddress = address.trim() || "台大體育館";

      setLoading(true);
      try {
        const result = await getParticipateList(searchAddress);
        setParticipateList(result.data);
      } catch (error) {
        console.error("獲取參與列表錯誤:", error);
        setParticipateList([]);
      } finally {
        setLoading(false);
      }
    };

    fetchParticipateList();
  }, [address]);

  const handleSearch = (searchAddress: string) => {
    console.log("searchAddress", searchAddress);
    setAddress(searchAddress.trim() || "台大體育館");
  };

  const handleViewList = () => {
    router.push("/participate-list");
  };

  return (
    <>
      <SearchBar onSearch={handleSearch} placeholder="搜尋地址" />
      <div>
        <GoogleMapWithSearch participateList={participateList} />
      </div>

      {/* Loading Backdrop */}
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

      <Box
        sx={{
          position: "fixed",
          bottom: 0,
          left: 0,
          right: 0,
          background: "white",
          zIndex: 1000,
          borderTopRightRadius: "10px",
          borderTopLeftRadius: "10px",
          boxShadow: "0 -2px 10px rgba(0,0,0,0.1)",
        }}
      >
        <Box
          sx={{
            padding: "16px 24px",
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <Typography
            variant="h6"
            sx={{
              fontWeight: 600,
              fontSize: "20px",
              flex: 1,
            }}
          >
            饗食服務據點
          </Typography>

          <Box
            sx={{
              backgroundColor: "white",
              borderRadius: "999px",
              padding: "2px 8px",
              marginX: 2,
              border: "1px solid #5AB4C5",
              display: "flex",
              alignItems: "center",
              gap: 1,
              minWidth: "80px",
              justifyContent: "center",
            }}
          >
            {loading ? (
              <>
                <CircularProgress
                  size={16}
                  thickness={4}
                  sx={{ color: "#5AB4C5" }}
                />
                <Typography variant="body" color="#5AB4C5">
                  搜尋中
                </Typography>
              </>
            ) : (
              <Typography variant="body" color="#5AB4C5">
                {participateList.length}筆結果
              </Typography>
            )}
          </Box>

          <Button
            onClick={handleViewList}
            disabled={loading || participateList.length === 0}
            sx={{
              textTransform: "none",
              fontSize: "14px",
              color: "#5AB4C5",
              height: "32px",
              "&:disabled": {
                color: "#ccc",
              },
            }}
          >
            展開列表
          </Button>
        </Box>
      </Box>
    </>
  );
};

export default SearchParticipate;
