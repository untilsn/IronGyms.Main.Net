// components/ui/PasswordToggleButton.jsx
import { Eye, EyeOff } from "lucide-react";

export default function PasswordToggleButton({ visible, onToggle }) {
  return (
    <button
      type="button"
      onClick={onToggle}
      tabIndex={-1}
      className="absolute top-1/2 right-3 -translate-y-1/2 text-irongyms-text/40 hover:text-irongyms-text/80"
    >
      {visible ? <EyeOff size={16} /> : <Eye size={16} />}
    </button>
  );
}
