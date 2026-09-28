import axiosClient from "./axiosClient";

export const profileApi = {
  getMe: () => axiosClient.get("/profile/me").then((res) => res.data),
  updateMe: (payload) =>
    axiosClient.put("/profile/me", payload).then((res) => res.data),
  updateAvatar: (file) => {
    const formData = new FormData();
    formData.append("file", file);
    return axiosClient
      .put("/profile/me/avatar", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      })
      .then((res) => res.data);
  },
  changePassword: (payload) =>
    axiosClient
      .put("/profile/me/change-password", payload)
      .then((res) => res.data),
};
