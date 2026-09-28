import { Loader2 } from "lucide-react";
import { cn } from "@/utils/cn";

const variantClasses = {
  primary: "bg-primary text-primary-content hover:opacity-90",
  outline: "border border-white/15 text-base-content hover:border-primary",
  ghost: "bg-transparent text-base-content hover:bg-base-300",
};

const sizeClasses = {
  sm: "px-4 py-2 text-xs",
  md: "px-5 py-2.5 text-sm",
};

// Button thuần Tailwind. Truyền `as={Link} to="/login"` để render thành link
// mà vẫn giữ nguyên style; khi là <button> thì tự có type="button" và loading/disabled.
export default function Button({
  as: Component = "button",
  children,
  loading = false,
  variant = "primary",
  size = "md",
  fullWidth = true,
  className,
  disabled,
  ...props
}) {
  const isButton = Component === "button";

  return (
    <Component
      {...(isButton && { type: "button", disabled: disabled || loading })}
      className={cn(
        "inline-flex cursor-pointer items-center justify-center gap-2 rounded-lg font-medium transition-colors",
        "disabled:pointer-events-none disabled:opacity-60",
        variantClasses[variant],
        sizeClasses[size],
        fullWidth && "w-full",
        className,
      )}
      {...props}
    >
      {loading && <Loader2 size={16} className="animate-spin" />}
      {children}
    </Component>
  );
}
