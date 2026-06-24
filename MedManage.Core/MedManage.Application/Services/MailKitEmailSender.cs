using System.Net.Http.Json;
using System.Text.Json.Serialization;
using MailKit.Net.Smtp;
using MailKit.Security;
using MedManage.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace MedManage.Application.Services;

public class MailKitEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MailKitEmailSender> _logger;
    private readonly HttpClient _httpClient;

    public MailKitEmailSender(
        IConfiguration configuration,
        ILogger<MailKitEmailSender> logger,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var clientId = _configuration["GOOGLE_CLIENT_ID"];
        var clientSecret = _configuration["GOOGLE_CLIENT_SECRET"];
        var refreshToken = _configuration["GOOGLE_REFRESH_TOKEN"];
        var fromEmail = _configuration["GOOGLE_EMAIL"];
        var fromName = _configuration["SMTP_FROM_NAME"] ?? "MedManage";

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            _logger.LogInformation("[EMAIL LOG] To: {To}, Subject: {Subject}, Body: {Body}", to, subject, body);
            return;
        }

        var accessToken = await GetGoogleAccessTokenAsync(clientId, clientSecret, refreshToken);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress(string.Empty, to));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Plain) { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(new SaslMechanismOAuth2(fromEmail, accessToken));
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Email sent successfully to {To}", to);
    }

    private async Task<string> GetGoogleAccessTokenAsync(
        string clientId, string clientSecret, string refreshToken)
    {
        var requestBody = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["refresh_token"] = refreshToken,
            ["grant_type"] = "refresh_token"
        };

        var response = await _httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(requestBody));

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
        return result?.AccessToken
            ?? throw new InvalidOperationException("Failed to get Google access token");
    }

    private class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
