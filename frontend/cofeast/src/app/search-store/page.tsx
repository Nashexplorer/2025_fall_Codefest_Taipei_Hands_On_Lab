/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 18:12:53
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 00:53:23
 * @FilePath: /frontend/cofeast/src/app/search-store/page.tsx
 */
"use client";

import { getAllSupportPoints } from "@/api/stores";
import GoogleMapWithSearch from "@/app/search-store/components/GoogleMapWithSearch";
import SearchBar from "@/app/search-store/components/SearchBar";
import { useEffect } from "react";

const SearchStorePage = (): React.ReactNode => {
  useEffect(() => {
    const fetchStores = async () => {
      const stores = await getAllSupportPoints();
      console.log("stores", stores);
    };
    fetchStores();
  }, []);

  return (
    <>
      <SearchBar />
      <div>
        <GoogleMapWithSearch />
      </div>
    </>
  );
};

export default SearchStorePage;
