/*
 * @Author: Auto
 * @Date: 2025-01-XX
 * @LastEditors: Do not edit
 * @LastEditTime: 2025-01-XX
 * @FilePath: /frontend/cofeast/src/app/search-store-detail/[id]/page.tsx
 */
"use client";

import { getSupportStoreByIdLocalhost, SupportPoint } from "@/api/stores";
import theme from "@/theme";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import CloseIcon from "@mui/icons-material/Close";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import PhoneIcon from "@mui/icons-material/Phone";
import {
  Alert,
  AppBar,
  Box,
  CircularProgress,
  IconButton,
  Link,
  ThemeProvider,
  Toolbar,
  Typography,
} from "@mui/material";
import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";

const SearchStoreDetailPage = (): React.ReactNode => {
  const params = useParams();
  const router = useRouter();
  const storeId = params.id as string;

  const [storeData, setStoreData] = useState<SupportPoint | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchStoreData = async () => {
      try {
        setLoading(true);
        const data = await getSupportStoreByIdLocalhost(storeId);

        if (!data) {
          throw new Error("無法載入據點資料");
        }

        setStoreData(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : "載入失敗");
      } finally {
        setLoading(false);
      }
    };

    if (storeId) {
      fetchStoreData();
    }
  }, [storeId]);

  const handleBack = () => {
    router.back();
  };

  const handleClose = () => {
    router.back();
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

  if (error || !storeData) {
    return (
      <ThemeProvider theme={theme}>
        <Box
          sx={{
            padding: "16px",
            minHeight: "100vh",
          }}
        >
          <Alert severity="error">{error || "找不到據點資料"}</Alert>
        </Box>
      </ThemeProvider>
    );
  }

  return (
    <ThemeProvider theme={theme}>
      <Box
        sx={{
          minHeight: "100vh",
          backgroundColor: "#fff",
        }}
      >
        {/* Header with Teal Background */}
        <AppBar
          position="static"
          sx={{
            backgroundColor: theme.palette.primary.main,
            boxShadow: "none",
          }}
        >
          <Toolbar
            sx={{
              minHeight: "56px !important",
              padding: "8px 16px",
            }}
          >
            <IconButton
              edge="start"
              color="inherit"
              onClick={handleBack}
              sx={{ mr: 1 }}
            >
              <ArrowBackIcon />
            </IconButton>
            <Typography
              variant="h6"
              component="div"
              sx={{
                flexGrow: 1,
                textAlign: "center",
                color: "#fff",
                fontWeight: 500,
              }}
            >
              {storeData.orgName}
            </Typography>
            <IconButton
              edge="end"
              color="inherit"
              onClick={handleClose}
              sx={{ ml: 1 }}
            >
              <CloseIcon />
            </IconButton>
          </Toolbar>
        </AppBar>

        {/* Main Content */}
        <Box sx={{ padding: "16px" }}>
          {/* Page Title */}
          <Typography
            variant="h2SemiBold"
            component="h2"
            sx={{
              mb: 3,
              color: "text.primary",
              fontWeight: 700,
            }}
          >
            {storeData.orgName}
          </Typography>

          {/* Details Section */}
          <Box sx={{ mb: 3 }}>
            {/* Address */}
            <Box
              sx={{
                display: "flex",
                alignItems: "flex-start",
                mb: 2,
              }}
            >
              <LocationOnIcon
                sx={{
                  fontSize: 20,
                  color: "text.secondary",
                  mr: 1.5,
                  mt: 0.3,
                }}
              />
              <Link
                href={`https://www.google.com/maps/search/?api=1&query=${storeData.lat},${storeData.lon}`}
                target="_blank"
                rel="noopener noreferrer"
                sx={{
                  color: "text.secondary",
                  textDecoration: "underline",
                  fontSize: "0.875rem",
                  lineHeight: "1.375rem",
                  "&:hover": {
                    color: "primary.main",
                  },
                }}
              >
                {storeData.address}
              </Link>
            </Box>

            {/* Phone Number 1 */}
            {storeData.phone && (
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  mb: 2,
                }}
              >
                <PhoneIcon
                  sx={{
                    fontSize: 20,
                    color: "text.secondary",
                    mr: 1.5,
                  }}
                />
                <Link
                  href={`tel:${storeData.phone}`}
                  sx={{
                    color: "text.secondary",
                    textDecoration: "underline",
                    fontSize: "0.875rem",
                    lineHeight: "1.375rem",
                    "&:hover": {
                      color: "primary.main",
                    },
                  }}
                >
                  {storeData.phone}
                </Link>
              </Box>
            )}

            {/* Phone Number 2 / Fax - Not in API, so we'll skip for now */}
            {/* Contact Person - Not in API, so we'll skip for now */}
          </Box>

          {/* Information Box */}
          <Alert
            severity="info"
            icon={false}
            sx={{
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
                  sx={{
                    color: "#5AB4C5",
                    fontWeight: 600,
                    fontSize: "0.7rem",
                  }}
                >
                  i
                </Typography>
              </Box>
              <Typography
                variant="body"
                color="text.secondary"
                sx={{ fontSize: "0.875rem", lineHeight: "1.5rem" }}
              >
                如市民想加入愛心餐食補給站或老人共餐單位之行列,也請洽本局各區社會福利服務中心。臺北市政府社會局竭誠為您服務。
              </Typography>
            </Box>
          </Alert>
        </Box>
      </Box>
    </ThemeProvider>
  );
};

export default SearchStoreDetailPage;

