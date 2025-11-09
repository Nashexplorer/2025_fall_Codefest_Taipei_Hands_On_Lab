import ColorButton from "@/components/ui/ColorButton";
import theme from "@/theme";
import {
    Box,
    Dialog,
    DialogContent,
    Typography,
} from "@mui/material";

interface StatusDialogProps {
  dialogOpen: boolean;
  handleCloseDialog: () => void;
  dialogMessage: string;
  dialogSeverity?: "success" | "error";
}

const StatusDialog: React.FC<StatusDialogProps> = ({
  dialogOpen,
  handleCloseDialog,
  dialogMessage,
  dialogSeverity = "success",
}) => {
    return (
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
    );
}

export default StatusDialog;