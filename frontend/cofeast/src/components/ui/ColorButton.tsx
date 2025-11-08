/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 16:29:55
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 16:29:59
 * @FilePath: /frontend/cofeast/src/components/ui/ColorButton.tsx
 */
import { Button } from "@mui/material";
import { styled } from "@mui/material/styles";

const ColorButton = styled(Button)(({ theme }) => ({
  color: theme.palette.white.main,
  backgroundColor: theme.palette.primary.main,
  borderRadius: "10px",
  width: "100%",
  marginTop: "16px",
  "&:hover": {
    backgroundColor: theme.palette.primary.dark,
  },
}));

export default ColorButton;
