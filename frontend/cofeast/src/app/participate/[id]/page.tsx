/*
 * @Author: Fangyu Kung
 * @Date: 2025-11-09 02:30:00
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-11-09 03:33:08
 * @FilePath: /frontend/cofeast/src/app/participate/[id]/page.tsx
 */
"use client";
import { postParticipate } from "@/api/participate";
import ColorButton from "@/components/ui/ColorButton";
import theme from "@/theme";
import AccessTimeIcon from "@mui/icons-material/AccessTime";
import PeopleIcon from "@mui/icons-material/People";
import PersonIcon from "@mui/icons-material/Person";
import PhoneIcon from "@mui/icons-material/Phone";
import {
  Alert,
  Box,
  CircularProgress,
  Dialog,
  DialogContent,
  Link,
  ThemeProvider,
  Typography,
} from "@mui/material";
import Image from "next/image";
import { useParams } from "next/navigation";
import React, { useEffect, useState } from "react";

interface EventData {
  city: string;
  district: string;
  street: string;
  number: string;
  title: string;
  description: string;
  hostName: string;
  address: string;
  phone: string;
  dietType: string;
  currentParticipants: number;
  capacity: number;
  startTime: string;
  endTime: string;
  isDineIn: boolean;
  notes: string;
  latitude: number;
  longitude: number;
}

const ParticipatePage = (): React.ReactNode => {
  const params = useParams();
  const eventId = params.id;

  const [eventData, setEventData] = useState<EventData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [participating, setParticipating] = useState(false);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogMessage, setDialogMessage] = useState("");
  const [dialogSeverity, setDialogSeverity] = useState<"success" | "error">(
    "success"
  );

  useEffect(() => {
    const fetchEventData = async () => {
      try {
        setLoading(true);
        const response = await fetch(
          `https://gongcan-api-d4vsmusihq-de.a.run.app/api/gongcan/meals/${eventId}`
        );

        if (!response.ok) {
          throw new Error("無法載入活動資料");
        }

        const data = await response.json();

        setEventData({
          title: data.title || "共餐活動",
          description: data.description || "",
          hostName: data.hostName || data.host_name || "主辦人",
          address: data.address || data.fullAddress || data.full_address || "",
          phone: data.phone || data.contact_phone || "0912******",
          dietType: data.dietType || data.diet_type || "葷",
          currentParticipants:
            data.currentParticipants || data.current_participants || 0,
          capacity: data.capacity || data.max_participants || 0,
          startTime: data.startTime || data.start_time || "",
          endTime: data.endTime || data.end_time || "",
          isDineIn: data.isDineIn ?? data.is_dine_in ?? true,
          notes: data.notes || data.remarks || "",
          latitude: data.latitude || 25.033,
          longitude: data.longitude || 121.565,
          city: data.city || "",
          district: data.district || "",
          street: data.street || "",
          number: data.number || "",
        });
      } catch (err) {
        setError(err instanceof Error ? err.message : "載入失敗");
      } finally {
        setLoading(false);
      }
    };

    if (eventId) {
      fetchEventData();
    }
  }, [eventId]);

  const handleParticipate = async () => {
    if (!eventId || typeof eventId !== "string") {
      setDialogMessage("無效的活動 ID");
      setDialogSeverity("error");
      setDialogOpen(true);
      return;
    }

    try {
      setParticipating(true);
      await postParticipate(eventId, 1);

      setDialogMessage("響應成功！請至email收信");
      setDialogSeverity("success");
      setDialogOpen(true);

      setTimeout(() => {
        window.location.reload();
      }, 2000);
    } catch (err) {
      setDialogMessage(
        err instanceof Error ? err.message : "響應共餐失敗，請稍後再試"
      );
      setDialogSeverity("error");
      setDialogOpen(true);
    } finally {
      setParticipating(false);
    }
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
  };

  const formatTime = (dateString: string) => {
    if (!dateString) return "";
    const date = new Date(dateString);
    return date.toLocaleString("zh-TW", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    });
  };

  const formatTimeOnly = (dateString: string) => {
    if (!dateString) return "";
    const date = new Date(dateString);
    return date.toLocaleTimeString("zh-TW", {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    });
  };

  const formatPhone = (phone: string) => {
    if (!phone) return "";
    if (phone.length > 4) {
      return phone.slice(0, -4) + "****";
    }
    return phone;
  };

  if (loading) {
    return (
      <ThemeProvider theme={theme}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            minHeight: "100vh",
          }}
        >
          <CircularProgress />
        </Box>
      </ThemeProvider>
    );
  }

  if (error || !eventData) {
    return (
      <ThemeProvider theme={theme}>
        <Box
          sx={{
            padding: "16px",
            minHeight: "100vh",
          }}
        >
          <Alert severity="error">{error || "找不到活動資料"}</Alert>
        </Box>
      </ThemeProvider>
    );
  }

  return (
    <ThemeProvider theme={theme}>
      <Box
        sx={{
          padding: "16px",
          minHeight: "100vh",
          backgroundColor: "#fff",
        }}
      >
        <Box sx={{ mb: 2 }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: "8px",
              mb: 1,
            }}
          >
            <Image
              src="/images/mealIcon.svg"
              alt="meal icon"
              width={24}
              height={24}
            />
            <Typography
              variant="h2SemiBold"
              component="h1"
              color="primary.main"
            >
              {eventData.title}
            </Typography>
          </Box>
          {eventData.description && (
            <Typography variant="body" color="text.secondary" sx={{ mt: 1 }}>
              {eventData.description}
            </Typography>
          )}
        </Box>

        <Box
          sx={{
            height: "1px",
            backgroundColor: "#E0E0E0",
            my: 3,
          }}
        />

        <Box sx={{ mb: 3 }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: "8px",
              mb: 2,
            }}
          >
            <PersonIcon sx={{ color: "text.secondary", fontSize: 20 }} />
            <Typography variant="h3" color="text.primary">
              {eventData.hostName}
            </Typography>
          </Box>

          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: "8px",
              mb: 2,
            }}
          >
            <Image
              src="/images/location.svg"
              alt="location icon"
              width={16}
              height={18}
            />
            <Link
              variant="body"
              rel="noopener noreferrer"
              sx={{
                color: "text.secondary",
                textDecoration: "underline",
              }}
            >
              {eventData.city} {eventData.district} {eventData.street}
            </Link>
          </Box>

          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: "8px",
            }}
          >
            <PhoneIcon sx={{ color: "text.secondary", fontSize: 20 }} />
            <Typography variant="body" color="text.secondary">
              {formatPhone(eventData.phone)}
            </Typography>
          </Box>
        </Box>

        <Alert
          severity="info"
          icon={false}
          sx={{
            mb: 3,
            backgroundColor: "#EDF8FA",
            border: "1px solid #5AB4C5",
            borderRadius: "8px",
            "& .MuiAlert-message": {
              width: "100%",
            },
          }}
        >
          <Box sx={{ display: "flex", alignItems: "flex-start", gap: 1 }}>
            <Box
              sx={{
                width: "20px",
                height: "20px",
                borderRadius: "50%",
                border: "2px solid #5AB4C5",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                flexShrink: 0,
                mt: 0.2,
              }}
            >
              <Typography
                variant="caption"
                sx={{ color: "#5AB4C5", fontWeight: 600, fontSize: "0.7rem" }}
              >
                i
              </Typography>
            </Box>
            <Typography variant="body" color="text.secondary">
              完成響應後，您將透過台北通綁定之電子信箱獲得共餐聯絡資訊。
            </Typography>
          </Box>
        </Alert>

        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: "1fr 1fr",
            gap: 2,
            mb: 3,
          }}
        >
          <Box
            sx={{
              backgroundColor: "#EDF8FA",
              borderRadius: "8px",
              padding: "16px",
              display: "flex",
              flexDirection: "column",
              gap: 1,
            }}
          >
            <Typography variant="body" color="primary.main">
              飲食型態
            </Typography>
            <Box sx={{ display: "flex", alignItems: "center", gap: "8px" }}>
              <Image
                src="/images/mealIcon.svg"
                alt="meal type icon"
                width={20}
                height={20}
              />
              <Typography variant="h3SemiBold" color="text.primary">
                {eventData.dietType}
              </Typography>
            </Box>
          </Box>

          {/* 共餐活動人數卡片 */}
          <Box
            sx={{
              backgroundColor: "#EDF8FA",
              borderRadius: "8px",
              padding: "16px",
              display: "flex",
              flexDirection: "column",
              gap: 1,
            }}
          >
            <Typography variant="body" color="primary.main">
              共餐活動人數
            </Typography>
            <Box sx={{ display: "flex", alignItems: "center", gap: "8px" }}>
              <PeopleIcon sx={{ color: "text.primary", fontSize: 20 }} />
              <Typography variant="h3SemiBold" color="text.primary">
                {eventData.currentParticipants} / {eventData.capacity}
              </Typography>
            </Box>
          </Box>
        </Box>

        <Box
          sx={{
            backgroundColor: "#EDF8FA",
            borderRadius: "8px",
            padding: "16px",
            mb: 2,
          }}
        >
          <Typography
            variant="body"
            color="primary.main"
            sx={{ mb: 1, display: "block" }}
          >
            共餐時間
          </Typography>
          <Box sx={{ display: "flex", alignItems: "center", gap: "8px" }}>
            <AccessTimeIcon sx={{ color: "text.primary", fontSize: 20 }} />
            <Typography variant="h3SemiBold" color="text.primary">
              {formatTime(eventData.startTime)} ~{" "}
              {formatTimeOnly(eventData.endTime)}
            </Typography>
          </Box>
        </Box>

        {!eventData.isDineIn && (
          <Typography
            variant="body"
            sx={{ color: "#D45251", mb: 3, display: "block" }}
          >
            *此共餐活動僅開供外帶
          </Typography>
        )}

        {eventData.notes && (
          <Box
            sx={{
              backgroundColor: "#F1F3F4",
              borderRadius: "8px",
              padding: "16px",
              mb: 4,
            }}
          >
            <Typography
              variant="bodySemiBold"
              color="text.primary"
              sx={{ mb: 1, display: "block" }}
            >
              備註
            </Typography>
            <Typography variant="body" color="text.secondary">
              {eventData.notes}
            </Typography>
          </Box>
        )}

        <Box sx={{ mb: 4 }}>
          <ColorButton onClick={handleParticipate} disabled={participating}>
            {participating ? "處理中..." : "響應共餐"}
          </ColorButton>
        </Box>

        <Dialog
          open={dialogOpen}
          onClose={handleCloseDialog}
          maxWidth="xs"
          fullWidth
          PaperProps={{
            sx: {
              borderRadius: "16px",
              padding: "24px",
              backgroundColor: "#fff",
            },
          }}
          sx={{
            "& .MuiBackdrop-root": {
              backgroundColor: "rgba(0, 0, 0, 0.6)",
            },
          }}
        >
          <DialogContent sx={{ padding: 0 }}>
            <Box
              sx={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                gap: 2,
              }}
            >
              <Box
                sx={{
                  width: "56px",
                  height: "56px",
                  borderRadius: "50%",
                  backgroundColor:
                    dialogSeverity === "success" ? "#EDF8FA" : "#FEF3F2",
                  border: `2px solid ${
                    dialogSeverity === "success"
                      ? theme.palette.primary.main
                      : theme.palette.error.main
                  }`,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <Typography
                  sx={{
                    color:
                      dialogSeverity === "success"
                        ? theme.palette.primary.main
                        : theme.palette.error.main,
                    fontWeight: 700,
                    fontSize: "2rem",
                  }}
                >
                  {dialogSeverity === "success" ? "✓" : "!"}
                </Typography>
              </Box>

              <Typography
                variant="h3SemiBold"
                color="text.primary"
                sx={{ textAlign: "center" }}
              >
                {dialogMessage}
              </Typography>

              <ColorButton
                onClick={handleCloseDialog}
                sx={{ width: "100%", mt: 1 }}
              >
                確定
              </ColorButton>
            </Box>
          </DialogContent>
        </Dialog>
      </Box>
    </ThemeProvider>
  );
};

export default ParticipatePage;
