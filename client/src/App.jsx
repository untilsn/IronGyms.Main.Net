import { BrowserRouter } from "react-router-dom";
import { Toaster } from "react-hot-toast";
import { PrimeReactProvider } from "primereact/api";
import { useEffect } from "react";

import AppRoutes from "./router/AppRouter";
import { authApi } from "./api/authApi";
import { profileApi } from "./api/profileApi";
import { useAuthStore } from "./store/useAuthStore";

let hasBootstrapped = false;
export default function App() {
  useEffect(() => {
    if (hasBootstrapped) return;
    hasBootstrapped = true;

    async function bootstrap() {
      try {
        const { accessToken } = await authApi.refresh();
        useAuthStore.getState().setAccessToken(accessToken);

        const user = await profileApi.getMe();
        useAuthStore.getState().setAuth({ accessToken, user });
      } catch {
        useAuthStore.getState().clearAuth();
      }
    }

    bootstrap();
  }, []);

  const isChecking = useAuthStore((state) => state.isChecking);

  if (isChecking) {
    return (
      <div className="flex h-screen items-center justify-center">
        <span className="text-sm text-gray-500">Đang tải...</span>
      </div>
    );
  }
  return (
    <PrimeReactProvider>
      <BrowserRouter>
        <AppRoutes />
        <Toaster
          position="top-right"
          toastOptions={{
            duration: 3000,
            style: { fontFamily: "var(--font-sans)" },
          }}
        />
      </BrowserRouter>
    </PrimeReactProvider>
  );
}
