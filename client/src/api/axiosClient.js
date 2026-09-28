import axios from "axios";
import { useAuthStore } from "@/store/useAuthStore";

const baseURL = import.meta.env.VITE_API_URL || "http://localhost:5000/api";

const axiosClient = axios.create({
  baseURL,
  // Bắt buộc để browser gửi kèm httpOnly cookie chứa refresh token.
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

// Gắn access token vào mọi request.
axiosClient.interceptors.request.use((config) => {
  const accessToken = useAuthStore.getState().accessToken;
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

// Các endpoint public này KHÔNG bao giờ nên kích hoạt auto-refresh -
// 401 ở đây là lỗi nghiệp vụ thật (sai mật khẩu, email đã tồn tại, refresh
// token hết hạn thật...), không phải do access token hết hạn giữa chừng.
// Thiếu bước lọc này sẽ khiến lỗi login/register sai bị "nuốt" và thay
// bằng lỗi của chính request refresh, gây hiểu nhầm toast sai nội dung.
const PUBLIC_AUTH_PATHS = ["/auth/login", "/auth/register", "/auth/refresh"];

function isPublicAuthCall(url = "") {
  return PUBLIC_AUTH_PATHS.some((path) => url.includes(path));
}

// Gom các request bị 401 trong lúc đang refresh vào 1 hàng đợi, tránh gọi
// /auth/refresh nhiều lần cùng lúc nếu nhiều request song song đều hết hạn
// token cùng 1 thời điểm.
let isRefreshing = false;
let pendingQueue = [];

function resolveQueue(error, token) {
  pendingQueue.forEach(({ resolve, reject }) => {
    if (error) reject(error);
    else resolve(token);
  });
  pendingQueue = [];
}

axiosClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Không có response (mất mạng, CORS chặn, server sập) thì không có gì
    // để retry - trả lỗi ngay, đừng cố refresh.
    if (!error.response) {
      return Promise.reject(error);
    }

    const isUnauthorized = error.response.status === 401;
    const alreadyRetried = originalRequest._retry;
    const isPublicCall = isPublicAuthCall(originalRequest?.url);

    // Chỉ can thiệp khi: đúng là 401, chưa retry lần nào, và KHÔNG phải
    // gọi tới các endpoint public (login/register/refresh tự thân).
    if (!isUnauthorized || alreadyRetried || isPublicCall) {
      return Promise.reject(error);
    }

    // Nếu đang có 1 lượt refresh khác chạy rồi thì xếp hàng chờ, dùng
    // chung kết quả của lượt đó thay vì tự gọi refresh thêm lần nữa.
    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        pendingQueue.push({ resolve, reject });
      }).then((newAccessToken) => {
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return axiosClient(originalRequest);
      });
    }

    originalRequest._retry = true;
    isRefreshing = true;

    try {
      const { data } = await axiosClient.post("/auth/refresh");
      const newAccessToken = data.accessToken;

      useAuthStore.getState().setAccessToken(newAccessToken);
      resolveQueue(null, newAccessToken);

      originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
      return axiosClient(originalRequest);
    } catch (refreshError) {
      resolveQueue(refreshError, null);
      useAuthStore.getState().clearAuth();

      if (window.location.pathname !== "/login") {
        window.location.href = "/login";
      }
      return Promise.reject(refreshError);
    } finally {
      isRefreshing = false;
    }
  },
);

export default axiosClient;
