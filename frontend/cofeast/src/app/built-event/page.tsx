"use client";
import Announcement from "@/components/ui/Announcement";
import StatusDialog from "@/components/ui/StatusDialog";
import { SelectedDropdown } from "@/components/ui/SelectedDropdown";
import { taipeiDistricts } from "@/data";
import theme from "@/theme";
import {
  Box,
  Button,
  Alert,
  Radio,
  RadioGroup,
  FormControlLabel,
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
import { createMealEvent } from "@/api/meals";
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';
import { useRouter } from "next/navigation";

dayjs.extend(utc);
dayjs.extend(timezone);

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
  const router = useRouter();
  const [selectedDistrict, setSelectedDistrict] = useState("All");
  const [street, setStreet] = useState("");
  const [startDateTime, setStartDateTime] = useState<Dayjs | null>(null);
  const [endDateTime, setEndDateTime] = useState<Dayjs | null>(null);
  const [address, setAddress] = useState("");
  const [foodType, setFoodType] = useState("");
  const [dineIn, setDineIn] = useState("");
  const [title, setTitle] = useState("");
  const [summary, setSummary] = useState("");
  const [peopleCount, setPeopleCount] = useState("");
  const [notes, setNotes] = useState("");

  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogMessage, setDialogMessage] = useState("");
  const [dialogSeverity, setDialogSeverity] = useState<"success" | "error">("success");

  const handleDistrictChange = (
    event:
      | React.ChangeEvent<HTMLInputElement>
      | (Event & { target: { value: unknown; name: string } })
  ) => {
    setSelectedDistrict(event.target.value as string);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    if (dialogSeverity === "success") {
      router.push("/home");
    }
  };

  const handleSubmit = async () => {
    try {
      // 必填欄位驗證
      if (
        !title ||
        !startDateTime ||
        !endDateTime ||
        selectedDistrict === "All" ||
        !address ||
        !peopleCount ||
        !foodType ||
        !dineIn
      ) {
        setDialogMessage("請填寫所有必填欄位！");
        setDialogSeverity("error");
        setDialogOpen(true);
        return;
      }

      const numberMatch = address.match(/\d+號/);
      const number = numberMatch ? numberMatch[0] : "";

      const payload = {
        title: title,
        description: summary,
        capacity: parseInt(peopleCount),
        dietType: foodType,
        isDineIn: dineIn === "dinein",
        startTime: startDateTime?.tz("Asia/Taipei").format("YYYY-MM-DDTHH:mm:ss"),
        endTime: endDateTime?.tz("Asia/Taipei").format("YYYY-MM-DDTHH:mm:ss"),
        signupDeadline: dayjs("2025-11-14T23:59:59").tz("Asia/Taipei").format("YYYY-MM-DDTHH:mm:ss"),
        notes: notes,
        city: "臺北市",
        district: selectedDistrict,
        street: street,
        number: number
      };

      const result = await createMealEvent(payload);

      if (result) {
        setDialogMessage("共餐活動建立成功！");
        setDialogSeverity("success");
        setDialogOpen(true);
        // 清空表單
        setTitle("");
        setSummary("");
        setStartDateTime(null);
        setEndDateTime(null);
        setSelectedDistrict("All");
        setStreet("");
        setAddress("");
        setPeopleCount("");
        setFoodType("");
        setDineIn("");
        setNotes("");
      } else {
        setDialogMessage("建立失敗，請稍後再試");
        setDialogSeverity("error");
        setDialogOpen(true);
      }
    } catch (error) {
      console.error("建立共餐活動失敗：", error);
      setDialogMessage("建立失敗，請稍後再試");
      setDialogSeverity("error");
      setDialogOpen(true);
    }
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
            value={title}
            onChange={(e) => setTitle(e.target.value)}
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
            value={summary}
            onChange={(e) => setSummary(e.target.value)}
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
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}>
            <Typography variant="h3" color="text.primary">
              共餐活動人數
            </Typography>
            <Typography variant="h3" color="primary.main" ml={0.5}>
              *
            </Typography>
          </Box>
        </FormControl>
        <FormControl fullWidth>
          <Box flex={1} mt={3} mb={2}>
            <StyledInputBase
              placeholder="請輸入人數"
              inputProps={{ "aria-label": "text" }}
              value={peopleCount}
              onChange={(e) => setPeopleCount(e.target.value)}
            />
          </Box>
        </FormControl>
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}>
            <Typography variant="h3" color="text.primary">
              飲食型態
            </Typography>
            <Typography variant="h3" color="primary.main" ml={0.5}>
              *
            </Typography>
          </Box>
        </FormControl>
        <FormControl fullWidth>
          <RadioGroup row value={foodType} onChange={(e) => setFoodType(e.target.value)}>
            <FormControlLabel value="meat" control={<Radio />} label="葷" />
            <FormControlLabel value="vegan" control={<Radio />} label="素" />
          </RadioGroup>
        </FormControl>
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}>
            <Typography variant="h3" color="text.primary">
              是否開放內用
            </Typography>
            <Typography variant="h3" color="primary.main" ml={0.5}>
              *
            </Typography>
          </Box>
        </FormControl>
        <FormControl fullWidth>
          <RadioGroup value={dineIn} onChange={(e) => setDineIn(e.target.value)}>
            <FormControlLabel value="takeout" control={<Radio />} label="否，僅供外帶" />
            <FormControlLabel value="dinein" control={<Radio />} label="是，參與人可以在上述「共餐地點」享用餐點" />
          </RadioGroup>
        </FormControl>
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}>
            <Typography variant="h3" color="text.primary">
              活動備註
            </Typography>
            <Typography variant="h3" color="primary.main" ml={0.5}>
              *
            </Typography>
          </Box>
        </FormControl>
        <FormControl fullWidth>
          <Box flex={1} mt={3} mb={2}>
            <StyledInputBase
              placeholder="請輸入備註"
              inputProps={{ "aria-label": "text" }}
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
            />
          </Box>
        </FormControl>
        <FormControl fullWidth>
          <Box display="flex" alignItems="center" mt={3} mb={2}></Box>
          <Alert variant="outlined" severity="info" sx={{ bgcolor: '#090909ff', color: '#fff', borderRadius: 2 }}>
            <Typography variant="body2">
              您的所有隱私資訊將受到保護，僅在報名確認後供聯繫使用，不會公開顯示。
            </Typography>
          </Alert>
        </FormControl>
        <FormControl fullWidth sx={{ mt: 4, mb: 6 }}>
          <Button
            variant="contained"
            color="primary"
            size="large"
            sx={{
              borderRadius: "12px",
              py: 1.5,
              fontSize: "1.1rem",
              fontWeight: "bold",
              textTransform: "none",
            }}
            onClick={handleSubmit}
          >
            送出表單
          </Button>
        </FormControl>
        <StatusDialog
          dialogOpen={dialogOpen}
          handleCloseDialog={handleCloseDialog}
          dialogMessage={dialogMessage}
          dialogSeverity={dialogSeverity}
        />
      </div>
    </ThemeProvider>
  );
};

export default BuiltEventPage;
