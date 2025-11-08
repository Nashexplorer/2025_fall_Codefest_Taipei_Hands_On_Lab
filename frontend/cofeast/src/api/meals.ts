/*
 * @Author: Nash
 * @Date: 2025-11-09
 * @FilePath: /frontend/cofeast/src/api/meals.ts
 * @Description: 共餐活動 API 請求模組（新版 payload）
 */

const API_BASE_URL = "https://gongcan-api-d4vsmusihq-de.a.run.app/";

/**
 * 共餐活動（MealEvent）資料介面
 */
export interface MealEvent {
  id: string;
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  signupDeadline?: string;
  city: string;
  district: string;
  street: string;
  number: string;
  capacity: number;
  dietType: string;
  isDineIn: boolean;
  notes?: string;
  hostUserName?: string;
  phone?: string;
  email?: string;
  createdAt?: string;
  updatedAt?: string;
}

/**
 * 建立共餐活動的請求介面（新版）
 */
export interface CreateMealRequest {
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  signupDeadline: string;
  city: string;
  district: string;
  street: string;
  number: string;
  capacity: number;
  dietType: string;
  isDineIn: boolean;
  notes?: string;
  phone?: string;
  email?: string;
}

/**
 * 建立共餐活動
 * @param payload 新增活動的內容
 */
export async function createMealEvent(
  payload: CreateMealRequest
): Promise<MealEvent | null> {
  try {
    const response = await fetch(`${API_BASE_URL}api/gongcan/meals`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("建立共餐活動失敗：", errorText);
      throw new Error(`建立活動失敗，HTTP 狀態：${response.status}`);
    }

    const data = await response.json();
    console.log("建立共餐活動成功：", data);
    return data;
  } catch (error) {
    console.error("createMealEvent 錯誤：", error);
    return null;
  }
}

/**
 * 取得所有共餐活動（可選分頁）
 */
export async function getAllMeals(
  page: number = 1,
  pageSize: number = 20
): Promise<{ totalCount: number; data: MealEvent[] }> {
  try {
    const response = await fetch(
      `${API_BASE_URL}api/gongcan/meals?page=${page}&pageSize=${pageSize}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      }
    );

    if (!response.ok) {
      const errorText = await response.text();
      console.error("取得共餐活動列表失敗：", errorText);
      throw new Error(`取得活動列表失敗，HTTP 狀態：${response.status}`);
    }

    const data = await response.json();
    console.log("取得共餐活動列表成功：", data);
    return data;
  } catch (error) {
    console.error("getAllMeals 錯誤：", error);
    return { totalCount: 0, data: [] };
  }
}

/**
 * 取得單一共餐活動
 * @param id 活動 ID
 */
export async function getMealById(id: string): Promise<MealEvent | null> {
  try {
    const response = await fetch(`${API_BASE_URL}api/gongcan/meals/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("取得共餐活動詳情失敗：", errorText);
      throw new Error(`取得活動詳情失敗，HTTP 狀態：${response.status}`);
    }

    const data = await response.json();
    console.log("取得共餐活動詳情成功：", data);
    return data;
  } catch (error) {
    console.error("getMealById 錯誤：", error);
    return null;
  }
}
