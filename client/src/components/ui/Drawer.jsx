import { useEffect } from "react";
import { createPortal } from "react-dom";
import { X } from "lucide-react";
import { cn } from "@/utils/cn";

export default function Drawer({ open, onClose, header, children }) {
  useEffect(() => {
    if (!open) return;

    const onKeyDown = (e) => e.key === "Escape" && onClose();
    const prevOverflow = document.body.style.overflow;

    document.addEventListener("keydown", onKeyDown);
    document.body.style.overflow = "hidden"; // khoá cuộn trang phía sau

    return () => {
      document.removeEventListener("keydown", onKeyDown);
      document.body.style.overflow = prevOverflow;
    };
  }, [open, onClose]);

  return createPortal(
    <div
      className={cn("fixed inset-0 z-50", !open && "pointer-events-none")}
      inert={!open}
    >
      {/* Lớp phủ mờ */}
      <div
        onClick={onClose}
        className={cn(
          "absolute inset-0 bg-black/60 backdrop-blur-sm transition-opacity duration-300",
          open ? "opacity-100" : "opacity-0",
        )}
      />

      {/* Panel */}
      <aside
        role="dialog"
        aria-modal="true"
        className={cn(
          "absolute inset-y-0 left-0 flex w-72 flex-col border-r border-base-300 bg-base-200 shadow-2xl transition-transform duration-300",
          open ? "translate-x-0" : "-translate-x-full",
        )}
      >
        <div className="flex h-16 shrink-0 items-center justify-between px-4">
          {header}
          <button
            type="button"
            onClick={onClose}
            aria-label="Đóng menu"
            className="rounded-full p-2 text-base-content/70 transition-colors hover:bg-base-300 hover:text-base-content"
          >
            <X size={18} />
          </button>
        </div>

        <div className="flex-1 overflow-y-auto px-4 pb-4">{children}</div>
      </aside>
    </div>,
    document.body,
  );
}
