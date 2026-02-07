using System.Text.Json;
using AlumniApi.Options;
using Microsoft.Extensions.Options;

public class CaptchaService
{
    private readonly HttpClient _http;
    private readonly CaptchaOptions _opt;

    public CaptchaService(HttpClient http, IOptions<CaptchaOptions> opt)
    {
        _http = http;
        _opt = opt.Value;
    }

    public async Task<bool> VerifyAsync(string token, string? remoteIp = null)
    {
        var form = new Dictionary<string, string>
        {
            ["secret"] = _opt.SecretKey,
            ["response"] = token
        };

        if (!string.IsNullOrWhiteSpace(remoteIp))
            form["remoteip"] = remoteIp;

        var res = await _http.PostAsync(_opt.VerifyUrl, new FormUrlEncodedContent(form));
        if (!res.IsSuccessStatusCode) return false;

        var json = await res.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("success", out var success)) return false;

        return success.GetBoolean();
    }
}
