using System;

namespace EventHub.BLL.Configuration
{
    public class TicketQrCodeStorageOptions
    {
        public const string SectionName = "TicketQrCodes";

        /// <summary>Folder under the application content root where ticket QR PNGs are stored.</summary>
        public string RelativeRoot { get; set; } = "uploads/ticket-qrcodes";

        /// <summary>Public base URL of this API (no trailing slash), used inside QR images, e.g. https://localhost:7131</summary>
        public string PublicApiBaseUrl { get; set; } = string.Empty;

        public string BuildTicketVerifyUrl(string qrCodeToken)
        {
            var token = qrCodeToken?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("QR code token is required.", nameof(qrCodeToken));

            var baseUrl = PublicApiBaseUrl?.Trim().TrimEnd('/') ?? string.Empty;
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TicketQrCodes:PublicApiBaseUrl must be configured (public API root, no trailing slash).");

            return $"{baseUrl}/api/ticket/verify/{Uri.EscapeDataString(token)}";
        }
    }
}
