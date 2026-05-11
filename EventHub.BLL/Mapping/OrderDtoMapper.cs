using EventHub.BLL.Models;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Mapping
{
    public static class OrderDtoMapper
    {
        public static OrderDetailsDto ToDetailsDto(Order order) =>
            new()
            {
                Id = order.Id,
                ParticipantId = order.ParticipantId,
                EventId = order.EventId,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                Event = order.Event == null
                    ? null
                    : new OrderEventDto
                    {
                        Id = order.Event.Id,
                        Title = order.Event.Title,
                        EventDate = order.Event.EventDate,
                        Venue = order.Event.Venue,
                        Price = order.Event.Price,
                        Image = order.Event.Image
                    },
                Tickets = order.Tickets?.Select(t => new OrderTicketDto
                {
                    Id = t.Id,
                    QrCode = t.QrCode,
                    QrCodeImagePath = t.QrCodeImagePath,
                    UsedAtUtc = t.UsedAtUtc,
                    PurchasedAt = t.PurchasedAt
                }).ToList() ?? new List<OrderTicketDto>()
            };

        public static IEnumerable<OrderDetailsDto> ToDetailsDtos(IEnumerable<Order> orders) =>
            orders.Select(ToDetailsDto);
    }
}
