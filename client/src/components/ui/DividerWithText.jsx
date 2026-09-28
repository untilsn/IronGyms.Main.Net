// Tự vẽ gạch ngang bằng div thuần thay vì PrimeReact Divider -
// đảm bảo chắc chắn hiển thị ở mọi chế độ, không phụ thuộc CSS theme của thư viện.
export default function DividerWithText({ label }) {
  return (
    <div className="my-5 flex items-center gap-3">
      <div className="h-px flex-1 bg-white/10" />
      <span className="text-xs whitespace-nowrap text-white/30">{label}</span>
      <div className="h-px flex-1 bg-white/10" />
    </div>
  );
}
