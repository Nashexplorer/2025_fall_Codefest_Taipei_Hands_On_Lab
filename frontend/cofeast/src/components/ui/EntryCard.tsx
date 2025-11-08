/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 20:19:56
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 20:21:24
 * @FilePath: /frontend/cofeast/src/components/ui/EntryCard.tsx
 */
import Card from "@mui/material/Card";
import { styled } from "@mui/material/styles";

const EntryCard = styled(Card)(() => {
  return {
    maxWidth: "100%",
    width: "100%",
    borderRadius: "16px",
    marginTop: "16px",
    padding: "16px 0px",
    boxShadow: "0px 4px 12px 0px rgba(0, 0, 0, 0.04)",
  };
});

export default EntryCard;
