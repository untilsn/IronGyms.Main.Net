// utils/toast.js
import toast from "react-hot-toast";

// Style chung khớp theme IronGyms (nền tối, chữ sáng, viền cam khi success,
// viền đỏ khi error) - style riêng theo loại, còn cấu hình chung (font, radius,
// duration) đặt ở <Toaster toastOptions> trong App.jsx.
const baseStyle = {
  background: "#1c1b1b", // --color-irongyms-surface
  color: "#e5e2e1", // --color-irongyms-text
  fontSize: "0.875rem",
  padding: "12px 16px",
  borderRadius: "10px",
  border: "1px solid rgba(255,255,255,0.08)",
};

export const notify = {
  success: (message) =>
    toast.success(message, {
      style: { ...baseStyle, borderColor: "#e86c31" }, // --color-irongyms-primary
      // iconTheme: { primary: "#e86c31", secondary: "#1c1b1b" },
    }),

  error: (message) =>
    toast.error(message, {
      style: { ...baseStyle, borderColor: "#fc7c7c" }, // --color-irongyms-error
      // iconTheme: { primary: "#fc7c7c", secondary: "#1c1b1b" },
    }),

  // Dùng khi backend trả lỗi có message cụ thể (400/401/409...),
  // fallback về message mặc định nếu backend không trả message.
  apiError: (error, fallback = "Có lỗi xảy ra, vui lòng thử lại") => {
    const message = error?.response?.data?.message || fallback;
    notify.error(message);
  },
};
