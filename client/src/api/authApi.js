import axiosClient from "./axiosClient";

export const authApi = {
  login: (payload) =>
    axiosClient.post("/auth/login", payload).then((res) => res.data),
  register: (payload) =>
    axiosClient.post("/auth/register", payload).then((res) => res.data),
  refresh: () => axiosClient.post("/auth/refresh").then((res) => res.data),
  logout: () => axiosClient.post("/auth/logout").then((res) => res.data),
};
