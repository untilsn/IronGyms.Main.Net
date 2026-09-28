import { create } from "zustand";
import { persist } from "zustand/middleware";

export const useAuthStore = create(
  persist(
    (set) => ({
      accessToken: null,
      user: null, // nguyên response của GET /profile/me - có role, fullName, avatarUrl...
      isAuthenticated: false,

      // true trong lúc App.jsx đang xác minh phiên đăng nhập lúc mới load trang (F5) -
      // ProtectedRoute/PublicOnlyRoute phải chờ cờ này về false mới được quyết định redirect,
      // nếu không sẽ redirect nhầm về /login ngay cả khi thật ra vẫn còn đăng nhập hợp lệ.
      isChecking: true,

      // Gọi sau khi có ĐỦ cả accessToken lẫn user (từ /profile/me) - dùng cho cả login/register
      // lẫn bootstrap lúc app khởi động.
      setAuth: ({ accessToken, user }) =>
        set({ accessToken, user, isAuthenticated: true, isChecking: false }),

      // Chỉ set tạm accessToken - dùng ngay sau khi login/refresh xong nhưng CHƯA có profile,
      // để axios interceptor gắn được token vào request gọi /profile/me tiếp theo.
      setAccessToken: (token) => set({ accessToken: token }),

      clearAuth: () =>
        set({
          accessToken: null,
          user: null,
          isAuthenticated: false,
          isChecking: false,
        }),
    }),
    {
      name: "irongyms-auth",
      // Chỉ persist "user" để không giật UI lúc F5 (hiện tạm tên/avatar cũ trong lúc chờ xác minh) -
      // accessToken và isAuthenticated KHÔNG persist, luôn phải xác minh lại thật qua bootstrap.
      partialize: (state) => ({ user: state.user }),
    },
  ),
);
