import theme from "@/theme";
import styled from "@emotion/styled";
import SearchIcon from "@mui/icons-material/Search";
import { Box, InputBase } from "@mui/material";

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

const SearchBar = (): React.ReactNode => {
  return (
    <SearchBarContainer>
      <StyledInputBase
        placeholder="搜尋補給站"
        inputProps={{ "aria-label": "search" }}
      />
      <SearchButton>
        <SearchIcon />
      </SearchButton>
    </SearchBarContainer>
  );
};

export default SearchBar;
