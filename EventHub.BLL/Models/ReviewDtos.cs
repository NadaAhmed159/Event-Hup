using System.ComponentModel.DataAnnotations;

namespace EventHub.BLL.Models
{
    public sealed class CreateReviewRequest
    {
        [Required]
        public string EventId { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
