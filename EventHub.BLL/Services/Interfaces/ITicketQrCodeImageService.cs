namespace EventHub.BLL.Services.Interfaces
{
    public interface ITicketQrCodeImageService
    {
        /// <summary>
        /// Writes <c>{qrCodeToken}.png</c> under the configured folder, encoding <paramref name="qrPayloadToEncode"/> (typically a verify URL) in the QR image.
        /// </summary>
        string SavePngForQrToken(string qrCodeToken, string qrPayloadToEncode);
    }
}
