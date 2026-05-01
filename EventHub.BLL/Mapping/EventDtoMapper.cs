using EventHub.BLL.Models;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Mapping
{
    public static class EventDtoMapper
    {
        public static EventResponseDto ToResponseDto(Event e)
        {
            return new EventResponseDto
            {
                Id = e.Id,
                OrganizerId = e.OrganizerId,
                Organizer = e.Organizer == null
                    ? null
                    : new EventOrganizerSummaryDto
                    {
                        Id = e.Organizer.Id,
                        FirstName = e.Organizer.FirstName,
                        LastName = e.Organizer.LastName
                    },
                CategoryId = e.CategoryId,
                Category = e.Category == null
                    ? null
                    : new CategorySummaryDto { Id = e.Category.Id, Name = e.Category.Name },
                Title = e.Title,
                Description = e.Description,
                Venue = e.Venue,
                EventDate = e.EventDate,
                Image = e.Image,
                Status = e.Status,
                Price = e.Price,
                TotalTickets = e.TotalTickets,
                AvailableTickets = e.AvailableTickets,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Attachments = e.Attachments == null
                    ? Array.Empty<EventAttachmentResponseDto>()
                    : e.Attachments.Select(a => new EventAttachmentResponseDto
                    {
                        Id = a.Id,
                        EventId = a.EventId,
                        FilePath = a.FilePath,
                        CreatedAt = a.CreatedAt
                    }).ToList()
            };
        }

        public static IEnumerable<EventResponseDto> ToResponseDtos(IEnumerable<Event> events) =>
            events.Select(ToResponseDto);

        public static EventAttachmentResponseDto ToAttachmentResponseDto(EventAttachment a) =>
            new()
            {
                Id = a.Id,
                EventId = a.EventId,
                FilePath = a.FilePath,
                CreatedAt = a.CreatedAt
            };

        public static IEnumerable<EventAttachmentResponseDto> ToAttachmentResponseDtos(IEnumerable<EventAttachment> items) =>
            items.Select(ToAttachmentResponseDto);
    }
}
