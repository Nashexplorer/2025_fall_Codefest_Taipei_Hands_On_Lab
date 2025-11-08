/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 17:10:00
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 01:01:53
 * @FilePath: /frontend/cofeast/src/api/stores.ts
 */

// 據點資料介面（老人共餐據點）
export interface SupportPoint {
  id: number;
  importDate: string;
  orgGroupName: string;
  orgName: string;
  division: string;
  address: string;
  phone: string;
  lat: number;
  lon: number;
}

// 據點列表回應介面
export interface SupportPointListResponse {
  totalCount: number;
  page: number;
  pageSize: number;
  data: SupportPoint[];
}

const API_BASE_URL = "https://gongcan-api-d4vsmusihq-de.a.run.app/";

/**
 * 獲取所有老人共餐據點資料（分頁）
 * @param page 頁碼（預設為 1）
 * @param pageSize 每頁數量（預設為 20）
 */
export async function getAllSupportPoints(
  page: number = 1,
  pageSize: number = 20
): Promise<SupportPointListResponse> {
  try {
    const response = await fetch(
      `${API_BASE_URL}api/support-points?page=${page}&pageSize=${pageSize}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      }
    );

    if (!response.ok) {
      const errorText = await response.text();
      console.error("API 錯誤回應:", errorText);
      throw new Error(`獲取據點資料失敗: ${response.status}`);
    }

    const data = await response.json();
    console.log("API 回應資料:", data);
    return data;
  } catch (error) {
    console.error("獲取據點資料錯誤:", error);
    return {
      totalCount: 0,
      page: page,
      pageSize: pageSize,
      data: [],
    };
  }
}

// 創建用餐活動的請求介面
export interface CreateMealRequest {
  title: string;
  description: string;
  location: string;
  latitude: number;
  longitude: number;
  hostUserId: string;
  capacity: number;
  dietType: string;
  isDineIn: boolean;
  startTime: string;
  endTime: string;
  signupDeadline: string;
  tags?: string[];
  notes?: string;
  imageUrl?: string;
}

/**
 * 根據 ID 獲取單個用餐活動
 * @param id 用餐活動 ID
 */
export async function getSupportStoreById(
  id: string
): Promise<SupportPoint | null> {
  try {
    const response = await fetch(`${API_BASE_URL}api/support-points/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("獲取用餐活動詳情錯誤回應:", errorText);
      throw new Error(`獲取用餐活動詳情失敗: ${response.status}`);
    }

    const data = await response.json();
    console.log("獲取用餐活動詳情成功:", data);
    return data;
  } catch (error) {
    console.error("獲取用餐活動詳情錯誤:", error);
    return null;
  }
}

/**
 * 根據 ID 獲取單個據點詳情（使用 localhost）
 * @param id 據點 ID
 */
export async function getSupportStoreByIdLocalhost(
  id: string
): Promise<SupportPoint | null> {
  try {
    const response = await fetch(`${API_BASE_URL}api/support-points/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        accept: "*/*",
      },
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("獲取據點詳情錯誤回應:", errorText);
      throw new Error(`獲取據點詳情失敗: ${response.status}`);
    }

    const data = await response.json();
    console.log("獲取據點詳情成功:", data);
    return data;
  } catch (error) {
    console.error("獲取據點詳情錯誤:", error);
    return null;
  }
}