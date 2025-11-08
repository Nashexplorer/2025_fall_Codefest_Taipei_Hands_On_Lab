/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 18:14:35
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 01:51:33
 * @FilePath: /frontend/cofeast/src/app/search-store/components/SearchBar.tsx
 */
"use client";
import theme from "@/theme";
import styled from "@emotion/styled";
import SearchIcon from "@mui/icons-material/Search";
import { Box, InputBase } from "@mui/material";
import { useState } from "react";

const SearchBarContainer = styled(Box)`
  display: flex;
  gap: 12px;
  align-items: center;
  width: 100%;
  background-color: ${theme.palette.white.main};
  padding: 16px;
`;

const StyledInputBase = styled(InputBase)`
  flex: 1;
  background-color: ${theme.palette.grey[100]};
  border-radius: 10px;
  padding: 12px 20px;

  input {
    padding: 0;
    font-size: 1rem;
    color: ${theme.palette.text.primary};

    &::placeholder {
      color: ${theme.palette.grey[400]};
      opacity: 1;
    }
  }
`;

const SearchButton = styled.button`
  width: 48px;
  height: 48px;
  border-radius: 10px;
  background-color: ${theme.palette.primary.main};
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  flex-shrink: 0;
  transition: background-color 0.2s ease;

  &:hover {
    opacity: 0.9;
  }

  svg {
    color: white;
    font-size: 28px;
  }
`;

interface SearchBarProps {
  onSearch?: (address: string) => void;
  placeholder?: string;
}

const SearchBar = ({
  onSearch,
  placeholder = "搜尋補給站",
}: SearchBarProps): React.ReactNode => {
  const [inputValue, setInputValue] = useState("");

  const handleSearch = () => {
    if (onSearch) {
      onSearch(inputValue);
    }
  };

  const handleKeyPress = (event: React.KeyboardEvent) => {
    if (event.key === "Enter") {
      handleSearch();
    }
  };

  return (
    <SearchBarContainer>
      <StyledInputBase
        placeholder={placeholder}
        inputProps={{ "aria-label": "search" }}
        value={inputValue}
        onChange={(e) => setInputValue(e.target.value)}
        onKeyDown={handleKeyPress}
      />
      <SearchButton onClick={handleSearch}>
        <SearchIcon />
      </SearchButton>
    </SearchBarContainer>
  );
};

export default SearchBar;
