namespace EventHub.BLL.Configuration
{
    public class AttachmentStorageOptions
    {
        public const string SectionName = "Attachments";

        /// <summary>Folder name under the application content root (e.g. "uploads/events").</summary>
        public string RelativeRoot { get; set; } = "uploads/events";
    }
}
