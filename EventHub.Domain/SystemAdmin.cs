namespace EventHub.Domain
{
    /// <summary>
    /// The single built-in administrator. No other user may have the Admin role; this email is not available for registration.
    /// </summary>
    public static class SystemAdmin
    {
        public const string UserId = "admin-user-1";

        public const string Email = "admin@eventhub.com";

        /// <summary>BCrypt hash for the initial password <c>Admin@123</c> (work factor 11).</summary>
        public const string SeededPasswordHash =
            "$2a$11$NG3AMxDEku9vTcnr9ykkcOURESi73r1D/ub7Kefe6uY6n3hGgKoNm";
    }
}
