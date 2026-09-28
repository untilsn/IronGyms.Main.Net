using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IronGyms.Api.Configuration;
using IronGyms.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace IronGyms.Api.Services.Payments;

public interface IPayPalClient
{
    // referenceId: gắn kèm Id của MemberMembership để đối chiếu nếu cần tra soát trên PayPal Dashboard.
    Task<(string OrderId, string ApproveUrl)> CreateOrderAsync(decimal amount, string referenceId);

    // Ném ApiException nếu capture thất bại hoặc PayPal báo trạng thái khác COMPLETED.
    Task CaptureOrderAsync(string paypalOrderId);
}

public class PayPalClient : IPayPalClient
{
    private readonly HttpClient _http;
    private readonly PayPalOptions _options;
    private readonly ILogger<PayPalClient> _logger;

    public PayPalClient(HttpClient http, IOptions<PayPalOptions> options, ILogger<PayPalClient> logger)
    {
        _options = options.Value;
        _http = http;
        _http.BaseAddress = new Uri(_options.BaseUrl);
        _logger = logger;
    }

    public async Task<(string OrderId, string ApproveUrl)> CreateOrderAsync(decimal amount, string referenceId)
    {
        var token = await GetAccessTokenAsync();

        var body = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    reference_id = referenceId,
                    amount = new
                    {
                        currency_code = "USD",
                        value = amount.ToString("F2", CultureInfo.InvariantCulture)
                    }
                }
            },
            application_context = new
            {
                return_url = _options.ReturnUrl,
                cancel_url = _options.CancelUrl
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v2/checkout/orders");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(body);

        using var response = await _http.SendAsync(request);
        var rawBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Log ra lý do THẬT PayPal trả về - message ném lên client cố tình giữ chung chung,
            // chi tiết lỗi (sai amount, sai currency, thiếu field...) chỉ hiện trong log server.
            _logger.LogError(
                "PayPal CreateOrder thất bại. Status: {Status}. Body: {Body}",
                response.StatusCode, rawBody);
            throw ApiException.BadGateway("Tạo đơn thanh toán PayPal thất bại");
        }

        var json = JsonDocument.Parse(rawBody).RootElement;
        var orderId = json.GetProperty("id").GetString()!;
        var approveUrl = json.GetProperty("links")
            .EnumerateArray()
            .First(l => l.GetProperty("rel").GetString() == "approve")
            .GetProperty("href").GetString()!;

        return (orderId, approveUrl);
    }

    public async Task CaptureOrderAsync(string paypalOrderId)
    {
        var token = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/v2/checkout/orders/{paypalOrderId}/capture");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _http.SendAsync(request);
        var rawBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "PayPal CaptureOrder thất bại. Status: {Status}. Body: {Body}",
                response.StatusCode, rawBody);
            throw ApiException.BadGateway("Xác nhận thanh toán PayPal thất bại");
        }

        var json = JsonDocument.Parse(rawBody).RootElement;
        var status = json.GetProperty("status").GetString();

        if (status != "COMPLETED")
            throw ApiException.BadRequest($"Giao dịch PayPal chưa hoàn tất (trạng thái: {status})");
    }

    // Access token của PayPal sống ~9 tiếng, nhưng để đơn giản (đúng scope hiện tại) mình xin token
    // mới mỗi lần gọi thay vì cache lại - có thể tối ưu bằng IMemoryCache sau nếu traffic cao lên.
    private async Task<string> GetAccessTokenAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
        var authBytes = System.Text.Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials"
        });

        using var response = await _http.SendAsync(request);
        var rawBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "PayPal lấy access token thất bại. Status: {Status}. Body: {Body}",
                response.StatusCode, rawBody);
            throw ApiException.BadGateway("Không lấy được access token từ PayPal");
        }

        var json = JsonDocument.Parse(rawBody).RootElement;
        return json.GetProperty("access_token").GetString()!;
    }
}