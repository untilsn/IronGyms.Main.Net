import { Route, Routes } from "react-router-dom";
import ClientRoutes from "./ClientRoutes";
import AdminRoutes from "./AdminRoutes";
import NotFoundPage from "@/pages/client/common/NotFoundPage";

export default function AppRoutes() {
  return (
    <Routes>
      {ClientRoutes()}
      {AdminRoutes()}
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
