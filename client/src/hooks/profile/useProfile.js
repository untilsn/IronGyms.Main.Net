import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { profileApi } from "../../../api/profileApi";
import { useAuthStore } from "../../../store/useAuthStore";

export function useMyProfile() {
  return useQuery({ queryKey: ["profile", "me"], queryFn: profileApi.getMe });
}

export function useUpdateProfile() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: profileApi.updateMe,
    onSuccess: (res) => {
      queryClient.invalidateQueries({ queryKey: ["profile", "me"] });
      // Đồng bộ luôn "user" trong store - header/sidebar hiển thị tên mới ngay không cần F5.
      useAuthStore.getState().setAuth({
        accessToken: useAuthStore.getState().accessToken,
        user: res.data,
      });
    },
  });
}

export function useUpdateAvatar() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: profileApi.updateAvatar,
    onSuccess: (res) => {
      queryClient.invalidateQueries({ queryKey: ["profile", "me"] });
      useAuthStore.getState().setAuth({
        accessToken: useAuthStore.getState().accessToken,
        user: res.data,
      });
    },
  });
}

export function useChangePassword() {
  return useMutation({ mutationFn: profileApi.changePassword });
}
