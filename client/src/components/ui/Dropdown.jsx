import { useEffect, useId, useRef, useState } from "react";
import { cn } from "@/utils/cn";

// trigger: ({ props, open }) => <button {...props}>...</button>
// children: nội dung panel, hoặc ({ close }) => nội dung (để item tự đóng menu).
export default function Dropdown({
  trigger,
  children,
  align = "right",
  className,
}) {
  const [open, setOpen] = useState(false);
  const rootRef = useRef(null);
  const panelId = useId();

  useEffect(() => {
    if (!open) return;

    const onPointerDown = (e) => {
      if (!rootRef.current?.contains(e.target)) setOpen(false);
    };
    const onKeyDown = (e) => {
      if (e.key === "Escape") setOpen(false);
    };

    document.addEventListener("pointerdown", onPointerDown);
    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.removeEventListener("pointerdown", onPointerDown);
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [open]);

  const close = () => setOpen(false);

  const triggerProps = {
    onClick: () => setOpen((v) => !v),
    "aria-haspopup": "menu",
    "aria-expanded": open,
    "aria-controls": panelId,
  };

  return (
    <div ref={rootRef} className="relative">
      {trigger({ props: triggerProps, open })}

      {open && (
        <div
          id={panelId}
          role="menu"
          className={cn(
            "absolute z-50 mt-2 rounded-box border border-base-300 bg-base-200 p-2 shadow-xl",
            align === "right" ? "right-0" : "left-0",
            className,
          )}
        >
          {typeof children === "function" ? children({ close }) : children}
        </div>
      )}
    </div>
  );
}
