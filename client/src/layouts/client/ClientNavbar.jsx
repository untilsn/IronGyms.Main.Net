import { useState } from "react";
import { Link, NavLink } from "react-router-dom";
import {
  CalendarDays,
  ChevronDown,
  CreditCard,
  LayoutDashboard,
  LogOut,
  Menu as MenuIcon,
  QrCode,
  Settings,
  User,
  Wallet,
} from "lucide-react";

import { useAuthStore } from "@/store/useAuthStore";
import { useLogout } from "@/hooks/auth/useAuth";
import { cn } from "@/utils/cn";
import Avatar from "@/components/ui/Avatar";
import Button from "@/components/ui/Button";
import Drawer from "@/components/ui/Drawer";
import Dropdown from "@/components/ui/Dropdown";

const marketingLinks = [
  { label: "Trang chủ", to: "/" },
  { label: "Giới thiệu", to: "/about" },
  { label: "Chương trình", to: "/programs" },
  { label: "Huấn luyện viên", to: "/trainers" },
  { label: "Bảng giá", to: "/pricing" },
  { label: "Liên hệ", to: "/contact" },
];

const memberMenuLinks = [
  { label: "Tổng quan", to: "/dashboard", icon: LayoutDashboard },
  { label: "Gói tập", to: "/dashboard/membership", icon: CreditCard },
  { label: "Lịch tập", to: "/dashboard/schedule", icon: CalendarDays },
  { label: "Check-in", to: "/dashboard/checkin", icon: QrCode },
  { label: "Thanh toán", to: "/dashboard/payments", icon: Wallet },
];

const accountLinks = [
  { label: "Hồ sơ của tôi", to: "/dashboard/profile", icon: User },
  { label: "Cài đặt tài khoản", to: "/dashboard/settings", icon: Settings },
];

/* ------------------------- Class dùng chung ------------------------- */

const navLinkBase =
  "font-display font-medium tracking-wide uppercase outline-none transition-colors focus-visible:ring-2 focus-visible:ring-primary/40";

const desktopLinkClass = ({ isActive }) =>
  cn(
    navLinkBase,
    "rounded-md border-x-2 px-3.5 py-2 text-xs",
    isActive
      ? "border-primary/60 text-primary"
      : "border-transparent text-base-content/60 hover:text-base-content",
  );

const mobileLinkClass = ({ isActive }) =>
  cn(
    navLinkBase,
    "rounded-md border-l-2 px-3 py-3 text-sm",
    isActive
      ? "border-primary bg-primary/10 text-primary"
      : "border-transparent text-base-content/70 hover:bg-base-300 hover:text-base-content",
  );

const menuItemClass = ({ isActive }) =>
  cn(
    "flex items-center gap-3 rounded-field px-3 py-2 text-sm outline-none transition-colors focus-visible:bg-base-300",
    isActive
      ? "bg-primary/10 text-primary"
      : "text-base-content hover:bg-base-300",
  );

/* --------------------------- Thành phần con -------------------------- */

function Logo({ onClick }) {
  return (
    <Link
      to="/"
      onClick={onClick}
      className="font-display text-xl font-bold tracking-tight uppercase"
    >
      Iron<span className="text-primary">Gyms</span>
    </Link>
  );
}

function AuthButtons({ vertical = false, onNavigate }) {
  return (
    <div className={cn("flex gap-2", vertical ? "flex-col" : "items-center")}>
      <Button
        as={Link}
        to="/login"
        onClick={onNavigate}
        variant="ghost"
        size="sm"
        fullWidth={vertical}
        className="font-display tracking-wide uppercase"
      >
        Đăng nhập
      </Button>
      <Button
        as={Link}
        to="/register"
        onClick={onNavigate}
        size="sm"
        fullWidth={vertical}
        className={cn(
          "font-display tracking-wide uppercase",
          !vertical && "hidden sm:inline-flex",
        )}
      >
        Đăng ký
      </Button>
    </div>
  );
}

function UserMenu({ user }) {
  const logoutMutation = useLogout();

  return (
    <Dropdown
      className="w-64"
      trigger={({ props, open }) => (
        <button
          type="button"
          {...props}
          aria-label="Mở menu người dùng"
          className={cn(
            "flex cursor-pointer items-center gap-2 rounded-full px-2 py-1.5 outline-none transition-colors hover:bg-base-200 focus-visible:ring-2 focus-visible:ring-primary/50 md:px-3 md:py-2",
            open && "bg-base-200",
          )}
        >
          <Avatar src={user?.avatarUrl} name={user?.fullName} online />

          <span className="hidden flex-col items-start leading-tight md:flex">
            <span className="text-sm font-medium text-base-content">
              {user?.fullName ?? "Người dùng"}
            </span>
            <span className="text-[11px] text-base-content/50">
              Đang hoạt động
            </span>
          </span>

          <ChevronDown
            size={14}
            className={cn(
              "hidden text-base-content/50 transition-transform md:block",
              open && "rotate-180",
            )}
          />
        </button>
      )}
    >
      {({ close }) => (
        <>
          <p className="truncate px-3 pt-1 pb-2 text-xs text-base-content/50">
            {user?.email}
          </p>

          {memberMenuLinks.map(({ label, to, icon: Icon }) => (
            <NavLink
              key={to}
              to={to}
              end
              role="menuitem"
              onClick={close}
              className={menuItemClass}
            >
              <Icon size={16} />
              {label}
            </NavLink>
          ))}

          <div role="separator" className="my-1 h-px bg-base-content/10" />

          {accountLinks.map(({ label, to, icon: Icon }) => (
            <NavLink
              key={to}
              to={to}
              role="menuitem"
              onClick={close}
              className={menuItemClass}
            >
              <Icon size={16} />
              {label}
            </NavLink>
          ))}

          <div role="separator" className="my-1 h-px bg-base-content/10" />

          <button
            type="button"
            role="menuitem"
            onClick={() => logoutMutation.mutate()}
            disabled={logoutMutation.isPending}
            className="flex w-full cursor-pointer items-center gap-3 rounded-field px-3 py-2 text-sm text-error outline-none transition-colors hover:bg-error/10 focus-visible:bg-error/10 disabled:opacity-60"
          >
            <LogOut size={16} />
            {logoutMutation.isPending ? "Đang đăng xuất..." : "Đăng xuất"}
          </button>
        </>
      )}
    </Dropdown>
  );
}

/* ------------------------------ Navbar ------------------------------ */

export default function ClientNavbar() {
  const [mobileOpen, setMobileOpen] = useState(false);

  const user = useAuthStore((s) => s.user);
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const isChecking = useAuthStore((s) => s.isChecking);

  const closeMobile = () => setMobileOpen(false);

  // Đang xác minh phiên lúc F5 -> hiện khung chờ, tránh nháy nút "Đăng nhập" rồi mới đổi.
  let authArea;
  if (isChecking) {
    authArea = (
      <div className="size-8 animate-pulse rounded-full bg-base-300" />
    );
  } else if (isAuthenticated) {
    authArea = <UserMenu user={user} />;
  } else {
    authArea = <AuthButtons />;
  }

  return (
    <>
      <header className="sticky top-0 z-40 border-b border-base-300 bg-base-100/90 backdrop-blur-xl">
        <div className="wrapper flex h-16 items-center justify-between gap-4">
          <div className="flex items-center gap-1">
            <button
              type="button"
              onClick={() => setMobileOpen(true)}
              aria-label="Mở menu điều hướng"
              className="rounded-full p-2 text-base-content transition-colors hover:bg-base-200 md:hidden"
            >
              <MenuIcon size={20} />
            </button>
            <Logo />
          </div>

          <nav
            aria-label="Điều hướng chính"
            className="hidden items-center gap-1 md:flex"
          >
            {marketingLinks.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                end
                className={desktopLinkClass}
              >
                {link.label}
              </NavLink>
            ))}
          </nav>

          <div className="flex items-center gap-2">{authArea}</div>
        </div>
      </header>

      {/* Đặt ngoài <header> vì Drawer dùng portal; xem chú thích trong Drawer.jsx */}
      <Drawer
        open={mobileOpen}
        onClose={closeMobile}
        header={<Logo onClick={closeMobile} />}
      >
        <nav
          aria-label="Điều hướng chính (di động)"
          className="flex flex-col gap-1"
        >
          {marketingLinks.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              end
              onClick={closeMobile}
              className={mobileLinkClass}
            >
              {link.label}
            </NavLink>
          ))}
        </nav>

        {!isChecking && !isAuthenticated && (
          <div className="mt-6 border-t border-base-300 pt-4">
            <AuthButtons vertical onNavigate={closeMobile} />
          </div>
        )}
      </Drawer>
    </>
  );
}
