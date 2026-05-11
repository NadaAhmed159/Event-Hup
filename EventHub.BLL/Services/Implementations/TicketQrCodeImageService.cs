using EventHub.BLL.Configuration;
using EventHub.BLL.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QRCoder;

namespace EventHub.BLL.Services.Implementations
{
    public sealed class TicketQrCodeImageService : ITicketQrCodeImageService
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly TicketQrCodeStorageOptions _options;

        public TicketQrCodeImageService(IHostEnvironment hostEnvironment, IOptions<TicketQrCodeStorageOptions> options)
        {
            _hostEnvironment = hostEnvironment;
            _options = options.Value;
        }

        public string SavePngForQrToken(string qrCodeToken, string qrPayloadToEncode)
        {
            if (string.IsNullOrWhiteSpace(qrCodeToken))
                throw new ArgumentException("QR code token is required.", nameof(qrCodeToken));
            if (string.IsNullOrWhiteSpace(qrPayloadToEncode))
                throw new ArgumentException("QR payload to encode is required.", nameof(qrPayloadToEncode));

            var token = qrCodeToken.Trim();
            var payload = qrPayloadToEncode.Trim();
            var root = Path.Combine(_hostEnvironment.ContentRootPath, _options.RelativeRoot.TrimStart('/', '\\'));
            Directory.CreateDirectory(root);

            var fileName = $"{token}.png";
            var physicalPath = Path.Combine(root, fileName);

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(qrCodeData);
            var bytes = pngQrCode.GetGraphic(20);
            File.WriteAllBytes(physicalPath, bytes);

            return Path.Combine(_options.RelativeRoot, fileName).Replace('\\', '/');
        }
    }
}
