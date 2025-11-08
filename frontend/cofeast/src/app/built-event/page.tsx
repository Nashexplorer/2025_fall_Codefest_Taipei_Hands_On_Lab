"use client";
import Announcement from "@/components/ui/Announcement";
import { SelectedDropdown } from "@/components/ui/SelectedDropdown";
import { taipeiDistricts } from "@/data";
import theme from "@/theme";
import {
  Box,
  FormControl,
  InputBase,
  MenuItem,
  styled,
  ThemeProvider,
  Typography,
} from "@mui/material";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import dayjs, { Dayjs } from "dayjs";
import React, { useState } from "react";

/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-08 22:17:30
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-08 23:11:28
 * @FilePath: /frontend/cofeast/src/app/built-event/page.tsx
 */

const StyledInputBase = styled(InputBase)`
  flex: 1;
  background-color: ${theme.palette.grey[50]};
  border-radius: 10px;
  padding: 12px 20px;
  width: 100%;

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

const StyledDateTimePicker = styled(DateTimePicker)`
  width: 100%;
  .MuiOutlinedInput-root {
    background-color: ${theme.palette.grey[50]};
    border-radius: 10px;

    fieldset {
      border: none;
    }

    &:hover fieldset {
      border: none;
    }

    &.Mui-focused fieldset {
      border: none;
    }
  }

  .MuiOutlinedInput-input {
    padding: 12px 20px;
    font-size: 1rem;
    color: ${theme.palette.text.primary};
  }
`;

const BuiltEventPage = (): React.ReactNode => {
  const [selectedDistrict, setSelectedDistrict] = useState("All");
  const [street, setStreet] = useState("");
  const [startDateTime, setStartDateTime] = useState<Dayjs | null>(null);
  const [endDateTime, setEndDateTime] = useState<Dayjs | null>(null);
  const [address, setAddress] = useState("");

  const handleDistrictChange = (
    event:
      | React.ChangeEvent<HTMLInputElement>
      | (Event & { target: { value: unknown; name: string } })
  ) => {
    setSelectedDistrict(event.target.value as string);
  };

  return (
    <ThemeProvider theme={theme}>
      <div style={{ padding: "1px 16px 16px", height: "100vh" }}>
        <Announcement
          content="分享一頓飯，也分享彼此的故事。
          填寫資訊、建立你的共餐活動，邀請城市裡的人一起上桌。"
        />
        <Typography variant="h3" color="text.primary" mt={3} mb={2}>
          發起人姓名
        </Typography>
        <StyledInputBase
          placeholder="請輸入名稱"
          inputProps={{ "aria-label": "text" }}
          value={"趙式隆"}
          disabled
        />
        <Typography variant="h3" color="text.primary" mt={3} mb={2}>
          發起人電話
        </Typography>
        <StyledInputBase
          inputProps={{ "aria-label": "phone" }}
          value={"0912345678"}
          disabled
        />
        <Typography variant="h3" color="text.primary" mt={3} mb={2}>
          發起人電子信箱
        </Typography>
        <StyledInputBase
          inputProps={{ "aria-label": "email" }}
          value={"codeFeast1109@example.com"}
          disabled
        />
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}>
            <Typography variant="h3" color="text.primary">
              共餐活動名稱
            </Typography>
            <Typography variant="h3" color="primary.main" ml={0.5}>
              *
            </Typography>
          </Box>
          <StyledInputBase
            placeholder="請輸入名稱"
            inputProps={{ "aria-label": "search" }}
          />
          <Box mt={1}>
            <Typography color="#91A0A" className="text-center" variant="body2">
              為你的共餐活動取一個響亮又有溫度的名字吧！
            </Typography>
          </Box>
        </FormControl>
        <FormControl fullWidth>
          <Typography variant="h3" color="text.primary" mt={3} mb={2}>
            概述
          </Typography>
          <StyledInputBase
            placeholder="請輸入概述"
            inputProps={{ "aria-label": "text" }}
            value=""
          />
        </FormControl>
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}>
            <Typography variant="h3" color="text.primary">
              共餐活動時間
            </Typography>
            <Typography variant="h3" color="primary.main" ml={0.5}>
              *
            </Typography>
          </Box>
          <StyledInputBase
            placeholder="請輸入概述"
            inputProps={{ "aria-label": "text" }}
            value=""
          />
        </FormControl>
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}>
            <Typography variant="h3" color="text.primary">
              共餐時間
            </Typography>
            <Typography variant="h3" color="primary.main" ml={0.5}>
              *
            </Typography>
          </Box>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <Box
              display="flex"
              gap={2}
              flexDirection={{ xs: "column", sm: "row" }}
            >
              <Box flex={1}>
                <Typography variant="body2" color="text.secondary" mb={1}>
                  開始時間
                </Typography>
                <StyledDateTimePicker
                  value={startDateTime}
                  onChange={(newValue) => setStartDateTime(newValue)}
                  format="YYYY-MM-DD HH:mm"
                  ampm={false}
                  minDateTime={dayjs()}
                  slotProps={{
                    textField: {
                      placeholder: "選擇開始時間",
                    },
                  }}
                />
              </Box>
              <Box flex={1}>
                <Typography variant="body2" color="text.secondary" mb={1}>
                  結束時間
                </Typography>
                <StyledDateTimePicker
                  value={endDateTime}
                  onChange={(newValue) => setEndDateTime(newValue)}
                  format="YYYY-MM-DD HH:mm"
                  ampm={false}
                  minDateTime={startDateTime || dayjs()}
                  slotProps={{
                    textField: {
                      placeholder: "選擇結束時間",
                    },
                  }}
                />
              </Box>
            </Box>
          </LocalizationProvider>
        </FormControl>
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}>
            <Typography variant="h3" color="text.primary">
              共餐地點
            </Typography>
            <Typography variant="h3" color="primary.main" ml={0.5}>
              *
            </Typography>
          </Box>
          <SelectedDropdown
            id="district-select"
            value={selectedDistrict}
            defaultValue={"All"}
            displayEmpty
            onChange={handleDistrictChange}
            renderValue={(selected) => {
              if (selected === "All") {
                return (
                  <span style={{ color: theme.palette.grey[400] }}>
                    選擇行政區
                  </span>
                );
              }
              return selected as string;
            }}
          >
            <MenuItem value="All">全部</MenuItem>
            {taipeiDistricts.map((district) => (
              <MenuItem key={district} value={district}>
                {district}
              </MenuItem>
            ))}
          </SelectedDropdown>
        </FormControl>
        <FormControl fullWidth>
          <Box flex={1} mt={3}>
            <StyledInputBase
              placeholder="請輸入路段"
              inputProps={{ "aria-label": "street" }}
              value={street}
              onChange={(e) => setStreet(e.target.value)}
            />
          </Box>
        </FormControl>
        <FormControl fullWidth>
          <Box flex={1} mt={3} mb={2}>
            <StyledInputBase
              placeholder="請輸入地址"
              inputProps={{ "aria-label": "text" }}
              value={address}
              onChange={(e) => setAddress(e.target.value)}
            />
          </Box>
        </FormControl>
      </div>
    </ThemeProvider>
  );
};

export default BuiltEventPage;
