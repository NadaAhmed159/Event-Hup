using EventHub.Domain.Enums;

namespace EventHub.BLL.Models
{
    public class EventCreateDto
    {
        /// <summary>When the caller is an admin, assigns the event to this organizer. Ignored for event-organizer callers.</summary>
        public string? OrganizerId { get; set; }

        public string CategoryId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }
    }

    public class EventUpdateDto
    {
        public string CategoryId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }
    }

    public class EventOrganizerSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class CategorySummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class EventAttachmentResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public sealed class EventReviewAuthorDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public sealed class EventReviewResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public EventReviewAuthorDto? Author { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EventResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrganizerId { get; set; } = string.Empty;
        public EventOrganizerSummaryDto? Organizer { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public CategorySummaryDto? Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Image { get; set; }
        public EventStatus Status { get; set; }
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IReadOnlyList<EventAttachmentResponseDto> Attachments { get; set; } = Array.Empty<EventAttachmentResponseDto>();
        public IReadOnlyList<EventReviewResponseDto> Reviews { get; set; } = Array.Empty<EventReviewResponseDto>();
    }
}
