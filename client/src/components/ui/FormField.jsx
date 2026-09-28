import { useId } from "react";
import { cn } from "@/utils/cn";

// Không có màu viền ở đây, màu viền nằm trong nhóm trạng thái bên dưới
// để hai nhóm không bao giờ cùng xuất hiện (tránh xung đột vì chưa có tailwind-merge).
const baseClass = cn(
  "w-full rounded-lg border bg-base-100 px-4 py-2.5 text-sm text-base-content",
  "placeholder:text-base-content/30 outline-none transition-colors focus:ring-2",
);

const normalClass = cn(
  "border-base-content/15 hover:border-base-content/30",
  "focus:border-base-content/50 focus:ring-base-content/10",
);

// Lỗi: không có hover đổi màu, để viền đỏ luôn giữ nguyên khi rê chuột.
const errorClass = "border-error focus:border-error focus:ring-error/20";

// Dùng chung cho mọi input dạng label + input + error trong toàn app
// (login, register, profile...). Truyền children để chèn icon (vd nút mắt).
export default function FormField({
  label,
  error,
  children,
  register,
  className,
  ...inputProps
}) {
  const id = useId();
  const errorId = `${id}-error`;

  return (
    <div>
      <label htmlFor={id} className="mb-1.5 block text-sm text-base-content/80">
        {label}
      </label>

      <div className="relative">
        <input
          id={id}
          aria-invalid={Boolean(error)}
          aria-describedby={error ? errorId : undefined}
          {...register}
          {...inputProps}
          className={cn(
            baseClass,
            error ? errorClass : normalClass,
            children && "pr-10", // chừa chỗ cho icon bên phải
            className,
          )}
        />
        {children}
      </div>

      {error && (
        <p id={errorId} role="alert" className="mt-1 text-xs text-error">
          {error}
        </p>
      )}
    </div>
  );
}
