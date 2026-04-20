namespace EventHub.BLL.Configuration
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; set; } = "EventHub";
        public string Audience { get; set; } = "EventHub";
        public string Key { get; set; } = string.Empty;
        public int ExpiresMinutes { get; set; } = 60;
    }
}
