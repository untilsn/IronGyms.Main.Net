namespace IronGyms.Api.DTOs;

// Dùng cho response của POST/PUT/DELETE - để client luôn có "message" để toast,
// không phải tự hardcode message theo từng action ở phía frontend.
// GET không dùng cái này - đọc dữ liệu thì trả thẳng data, không cần message.
public class ApiResult<T>
{
    public string Message { get; set; } = null!;
    public T? Data { get; set; }
}