/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-09 01:22:46
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 02:09:45
 * @FilePath: /frontend/cofeast/src/api/participate.ts
 */

// 用戶位置介面
export interface UserLocation {
  latitude: number;
  longitude: number;
}

// 參與活動資料介面
export interface ParticipateItem {
  id: string;
  title: string;
  description: string;
  imageUrl: string;
  latitude: number;
  longitude: number;
  fullAddress: string;
  city: string;
  district: string;
  street: string;
  number: string;
  hostUserId: string;
  hostUserName: string;
  capacity: number;
  currentParticipants: number;
  dietType: string;
  isDineIn: boolean;
  startTime: string;
  endTime: string;
  signupDeadline: string;
  createdAt: string;
  updatedAt: string;
  status: "open" | "closed" | "cancelled" | "completed";
  notes: string;
  phone: string;
  email: string;
  distance: number;
}

// API 回應介面
export interface ParticipateListResponse {
  userAddress: string;
  userLocation: UserLocation;
  totalCount: number;
  data: ParticipateItem[];
}

const API_BASE_URL = "https://gongcan-api-d4vsmusihq-de.a.run.app/";

export const getParticipateList = async (
  address: string
): Promise<ParticipateListResponse> => {
  const encodedAddress = encodeURIComponent(address);
  const url = `${API_BASE_URL}api/gongcan/meals/by-location?address=${encodedAddress}`;
  const response = await fetch(url);

  if (!response.ok) {
    const errorText = await response.text();
    console.error("API 錯誤回應:", errorText);
    throw new Error(`API 請求失敗: ${response.status} ${response.statusText}`);
  }

  const text = await response.text();

  if (!text || text.trim() === "") {
    throw new Error("API 回應為空");
  }

  try {
    return JSON.parse(text);
  } catch (error) {
    throw new Error(`error: ${error}`);
  }
};
