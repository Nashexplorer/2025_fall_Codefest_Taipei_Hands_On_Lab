"use client";
import { getParticipateList, ParticipateItem } from "@/api/participate";
import { Wrapper } from "@/components/layout/layoutStyle";
import ParticipateCard from "@/components/ui/participateCard";
import { Typography } from "@mui/material";
import { useEffect, useState } from "react";
import SearchBar from "../search-store/components/SearchBar";

/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-09 01:04:44
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 02:27:36
 * @FilePath: /frontend/cofeast/src/app/participate-list/page.tsx
 */

const ParticipateListPage = () => {
  const [address, setAddress] = useState("台大體育館");
  const [participateList, setParticipateList] = useState<ParticipateItem[]>([]);
  const [loading, setLoading] = useState(false);

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
    setAddress(searchAddress.trim() || "台大體育館");
  };

  return (
    <>
      <SearchBar onSearch={handleSearch} placeholder={`搜尋地址`} />
      <Wrapper>
        <Typography variant="h3" mt={3}>
          熱門推薦
        </Typography>
        <div>
          {loading ? (
            <p>載入中...</p>
          ) : (
            participateList.map((item) => {
              return <ParticipateCard key={item.id} data={item} />;
            })
          )}
        </div>
      </Wrapper>
    </>
  );
};

export default ParticipateListPage;
