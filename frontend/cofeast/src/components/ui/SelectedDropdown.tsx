/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 23:39:05
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 23:49:19
 * @FilePath: /frontend/cofeast/src/components/ui/SelectedDropdown.tsx
 */
import { Select, SelectProps } from "@mui/material";
import { styled } from "@mui/material/styles";

export const SelectedDropdown = styled(Select)<SelectProps>(() => {
  return {
    borderRadius: "10px",
    "& .MuiOutlinedInput-notchedOutline": {
      borderRadius: "10px",
    },
  };
});
