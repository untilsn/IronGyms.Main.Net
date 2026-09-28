// Nút redirect sang endpoint challenge Google của backend ASP.NET Core -
// dùng thẻ <a> thường (không cần thư viện OAuth phía frontend), backend
// tự xử lý challenge rồi redirect ngược về app kèm cookie.
const GOOGLE_LOGIN_URL = `${import.meta.env.VITE_API_URL}/auth/google`;

export default function GoogleButton({ label = "Tiếp tục với Google" }) {
  return (
    <a
      href={GOOGLE_LOGIN_URL}
      className="flex w-full items-center justify-center gap-2 rounded-lg border border-white/15 py-2.5 text-sm transition-colors hover:border-neutral-content/50"
    >
      <svg width="18" height="18" viewBox="0 0 48 48" aria-hidden="true">
        <path
          fill="#FFC107"
          d="M43.6 20.5H42V20H24v8h11.3c-1.6 4.6-6 8-11.3 8-6.6 0-12-5.4-12-12s5.4-12 12-12c3.1 0 5.8 1.1 8 3l6-6C34.5 5.1 29.5 3 24 3 12.4 3 3 12.4 3 24s9.4 21 21 21 21-9.4 21-21c0-1.2-.1-2.4-.4-3.5z"
        />
        <path
          fill="#FF3D00"
          d="M6.3 14.7l6.6 4.8C14.7 15.9 19 13 24 13c3.1 0 5.8 1.1 8 3l6-6C34.5 6.1 29.5 4 24 4 16 4 9.2 8.5 6.3 14.7z"
        />
        <path
          fill="#4CAF50"
          d="M24 44c5.4 0 10.3-2.1 14-5.5l-6.5-5.5c-2.1 1.5-4.7 2.4-7.5 2.4-5.3 0-9.7-3.4-11.3-8l-6.6 5.1C9.2 39.5 16 44 24 44z"
        />
        <path
          fill="#1976D2"
          d="M43.6 20.5H42V20H24v8h11.3c-.8 2.2-2.2 4.1-4 5.5l6.5 5.5C41.4 35.9 44 30.4 44 24c0-1.2-.1-2.4-.4-3.5z"
        />
      </svg>
      {label}
    </a>
  );
}
