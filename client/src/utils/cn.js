// utils/cn.js
import clsx from "clsx";

// Wrapper mỏng qua clsx - đủ dùng để gộp className kèm điều kiện
// (vd cn("base", isError && "border-red-500")). Nếu sau này cần merge
// các class Tailwind trùng nhóm (vd "p-2" ghi đè "p-4"), cân nhắc thêm
// tailwind-merge, còn hiện tại clsx là đủ.
export function cn(...inputs) {
  return clsx(...inputs);
}
