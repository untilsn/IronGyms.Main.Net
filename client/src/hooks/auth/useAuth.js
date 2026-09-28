import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { profileApi } from "@/api/profileApi";
import { useAuthStore } from "@/store/useAuthStore";
import { authApi } from "@/api/authApi";
import { notify } from "@/utils/toast";

const ADMIN_ROLES = ["Admin", "Staff"];

async function completeAuthFlow(authResponse) {
  useAuthStore.getState().setAccessToken(authResponse.accessToken);
  const user = await profileApi.getMe();
  useAuthStore
    .getState()
    .setAuth({ accessToken: authResponse.accessToken, user });
  return user;
}

function navigateByRole(navigate, user) {
  navigate(ADMIN_ROLES.includes(user.role) ? "/admin" : "/");
}

export function useLogin() {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: async (payload) => {
      const authResponse = await authApi.login(payload);
      return completeAuthFlow(authResponse);
    },
    onSuccess: (user) => {
      notify.success(`Chào mừng trở lại, ${user.fullName || user.email}!`);
      navigateByRole(navigate, user);
    },
    onError: (error) => notify.apiError(error, "Sai email hoặc mật khẩu"),
  });
}

export function useRegister() {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: async (payload) => {
      const authResponse = await authApi.register(payload);
      return completeAuthFlow(authResponse);
    },
    onSuccess: () => {
      notify.success("Đăng ký thành công, chào mừng bạn đến với IronGyms!");
      navigate("/");
    },
    onError: (error) => notify.apiError(error, "Đăng ký thất bại"),
  });
}

export function useLogout() {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => notify.success("Đã đăng xuất"),
    onError: (error) => notify.apiError(error, "Đăng xuất thất bại"),
    onSettled: () => {
      useAuthStore.getState().clearAuth();
      navigate("/");
    },
  });
}
