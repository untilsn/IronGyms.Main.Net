import AuthLayout from "@/layouts/auth/AuthLayout";
import ClientLayout from "@/layouts/client/ClientLayout";
import LoginPage from "@/pages/auth/LoginPage";
import RegisterPage from "@/pages/auth/RegisterPage";
import AboutPage from "@/pages/client/about/AboutPage";
import ContactPage from "@/pages/client/contact/ContactPage";
import HomePage from "@/pages/client/home/HomePage";
import ProgramsPage from "@/pages/client/programs/ProgramsPage";
import TrainersPage from "@/pages/client/trainers/TrainersPage";
import PricingPage from "@/pages/client/pricing/PricingPage";
import { Route } from "react-router-dom";
import PublicOnlyRoute from "./PublicOnlyRoute";

function ClientRoutes() {
  return (
    <Route>
      <Route element={<PublicOnlyRoute area="client" />}>
        <Route element={<AuthLayout />}>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Route>
      </Route>

      {/* <Route path="payment/success" element={<PayPalReturnPage />} /> */}

      <Route element={<ClientLayout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/about" element={<AboutPage />} />
        <Route path="/programs" element={<ProgramsPage />} />
        <Route path="/trainers" element={<TrainersPage />} />
        <Route path="/pricing" element={<PricingPage />} />
        <Route path="/contact" element={<ContactPage />} />
        {/* <Route index element={<MembershipPlansPage />} />
        <Route path="membership-plans" element={<MembershipPlansPage />} /> */}

        {/* Profile dùng chung cho cả Member lẫn Trainer - cả 2 đều đăng nhập ở khu client */}
        {/* <Route element={<ProtectedRoute allowedRoles={['Member', 'Trainer']} />}>
          <Route path="profile" element={<ProfilePage />} />
          </Route> */}

        {/* Các trang dưới đây chỉ dành riêng cho Member (mua gói, check-in) */}
        {/* <Route element={<ProtectedRoute allowedRoles={['Member']} />}>
          <Route path="memberships" element={<MyMembershipsPage />} />
          <Route path="checkin" element={<MyCheckInQrPage />} />
          <Route path="checkin/history" element={<MyCheckInHistoryPage />} />
          </Route> */}
      </Route>
    </Route>
  );
}

export default ClientRoutes;
