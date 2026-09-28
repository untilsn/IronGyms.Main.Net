import { useState } from "react";
import { cn } from "@/utils/cn";

export default function Avatar({ src, name, online = false, className }) {
  const [failedSrc, setFailedSrc] = useState(null);
  const initial = name?.charAt(0)?.toUpperCase() ?? "?";
  const showImage = Boolean(src) && failedSrc !== src;

  return (
    <span className="relative flex shrink-0">
      <span
        className={cn(
          "flex size-8 items-center justify-center overflow-hidden rounded-full bg-primary/20 text-sm font-semibold text-primary",
          className,
        )}
      >
        {showImage ? (
          <img
            src={src}
            alt={name ?? "Avatar"}
            className="size-full object-cover"
            onError={() => setFailedSrc(src)}
          />
        ) : (
          initial
        )}
      </span>

      {online && (
        <span className="absolute -right-0.5 -bottom-0.5 size-2.5 rounded-full border-2 border-base-100 bg-success" />
      )}
    </span>
  );
}
