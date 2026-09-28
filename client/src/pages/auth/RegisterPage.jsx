import Button from "@/components/ui/Button";
import DividerWithText from "@/components/ui/DividerWithText";
import FormField from "@/components/ui/FormField";
import GoogleButton from "@/components/ui/GoogleButton";
import PasswordToggleButton from "@/components/ui/PasswordToggleButton";
import { useRegister } from "@/hooks/auth/useAuth";
import { registerSchema } from "@/schemas/authSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link } from "react-router-dom";

export default function RegisterPage() {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({ resolver: zodResolver(registerSchema) });

  const registerMutation = useRegister();

  // Backend chỉ cần fullName/email/password - confirmPassword chỉ để validate
  // phía client, không gửi lên server.
  const onSubmit = ({ confirmPassword, ...payload }) =>
    registerMutation.mutate(payload);

  return (
    <div className="w-full rounded-2xl border border-white/10 bg-base-100/90 p-8 shadow-2xl backdrop-blur-md">
      <Link to="/" className="font-display text-3xl font-bold">
        Iron<span className="text-primary">Gyms</span>
      </Link>
      <p className="mt-1 mb-6 text-sm text-neutral-content/60">
        Tạo tài khoản để bắt đầu hành trình tập luyện của bạn
      </p>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField
          label="Họ và tên"
          register={register("fullName")}
          error={errors.fullName?.message}
          placeholder="Nguyễn Văn A"
          autoFocus
        />

        <FormField
          label="Email"
          register={register("email")}
          error={errors.email?.message}
          placeholder="ban@email.com"
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

        <FormField
          label="Xác nhận mật khẩu"
          register={register("confirmPassword")}
          error={errors.confirmPassword?.message}
          type={showConfirmPassword ? "text" : "password"}
          placeholder="••••••••"
        >
          <PasswordToggleButton
            visible={showConfirmPassword}
            onToggle={() => setShowConfirmPassword((v) => !v)}
          />
        </FormField>

        <Button type="submit" loading={registerMutation.isPending}>
          Đăng ký
        </Button>
      </form>

      <DividerWithText label="hoặc" />

      <GoogleButton label="Đăng ký với Google" />

      <p className="mt-5 text-center text-sm text-neutral-content/60">
        Đã có tài khoản?{" "}
        <Link to="/login" className="font-medium text-primary hover:underline">
          Đăng nhập ngay
        </Link>
      </p>
    </div>
  );
}
