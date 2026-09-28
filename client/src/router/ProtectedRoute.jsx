import { Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "@/store/useAuthStore";

// allowedRoles: mảng role được phép, vd ["Member"] hoặc ["Admin", "Staff"].
function ProtectedRoute({ allowedRoles }) {
  const { accessToken, user, isChecking } = useAuthStore();

  // Chờ App.jsx xác minh xong phiên đăng nhập (refresh + lấy profile) trước khi quyết định
  // redirect - thiếu bước này sẽ đá nhầm về /login lúc F5 dù thật ra vẫn còn đăng nhập.
  if (isChecking) return null;

  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && !allowedRoles.includes(user?.role)) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}

export default ProtectedRoute;
