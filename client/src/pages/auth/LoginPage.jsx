import Button from "@/components/ui/Button";
import DividerWithText from "@/components/ui/DividerWithText";
import FormField from "@/components/ui/FormField";
import GoogleButton from "@/components/ui/GoogleButton";
import PasswordToggleButton from "@/components/ui/PasswordToggleButton";

import { useLogin } from "@/hooks/auth/useAuth";
import { loginSchema } from "@/schemas/authSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Dumbbell, User } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link } from "react-router-dom";

const demoAccounts = [
  {
    label: "Member",
    email: "testuser2356@gmail.com",
    password: "testuser2356",
    icon: User,
  },
  {
    label: "Trainer",
    email: "trainer@test.com",
    password: "55B;)0£2m#l",
    icon: Dumbbell,
  },
];

export default function LoginPage() {
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm({ resolver: zodResolver(loginSchema) });

  const loginMutation = useLogin();
  const onSubmit = (values) => loginMutation.mutate(values);

  const fillDemoAccount = (account) => {
    setValue("email", account.email, { shouldValidate: true });
    setValue("password", account.password, { shouldValidate: true });
  };

  return (
    <div className="w-full rounded-2xl border border-white/10 bg-base-100/90 p-8 shadow-2xl backdrop-blur-md">
      <Link to="/" className="font-display text-3xl font-bold">
        Iron<span className="text-primary">Gyms</span>
      </Link>
      <p className="mt-1 mb-6 text-sm text-neutral-content/60">
        Đăng nhập để tiếp tục hành trình tập luyện của bạn
      </p>

      <div className="mb-5 rounded-lg border border-dashed border-white/15 p-3">
        <p className="mb-2 text-xs font-medium tracking-wide text-neutral-content/40 uppercase">
          Tài khoản demo
        </p>
        <div className="flex flex-wrap gap-2">
          {demoAccounts.map((account) => (
            <button
              key={account.email}
              type="button"
              onClick={() => fillDemoAccount(account)}
              className="flex items-center gap-1.5 rounded-md border border-white/15 px-2.5 py-1 text-xs text-neutral-content/80 transition-colors hover:border-primary hover:text-primary"
            >
              <account.icon size={12} />
              {account.label}
            </button>
          ))}
        </div>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField
          label="Email"
          register={register("email")}
          error={errors.email?.message}
          placeholder="ban@email.com"
          autoFocus
        />

        <FormField
          label="Mật khẩu"
          register={register("password")}
          error={errors.password?.message}
          type={showPassword ? "text" : "password"}
          placeholder="••••••••"
        >
          <PasswordToggleButton
            visible={showPassword}
            onToggle={() => setShowPassword((v) => !v)}
          />
        </FormField>

        <Button type="submit" loading={loginMutation.isPending}>
          Đăng nhập
        </Button>
      </form>

      <DividerWithText label="hoặc" />

      <GoogleButton label="Đăng nhập với Google" />

      <p className="mt-5 text-center text-sm text-neutral-content/60">
        Chưa có tài khoản?{" "}
        <Link
          to="/register"
          className="font-medium text-primary hover:underline"
        >
          Đăng ký ngay
        </Link>
      </p>
    </div>
  );
}
