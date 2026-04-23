using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Interfaces
{
    public interface ITicketService
    {
        Task<Ticket?> GetTicketByIdAsync(string ticketId);
        Task<Ticket?> GetTicketByQrCodeAsync(string qrCode);
        Task<IEnumerable<Ticket>> GetTicketsByParticipantAsync(string participantId);
        Task<IEnumerable<Ticket>> GetTicketsByEventAsync(string eventId);
        Task<bool> HasParticipantPurchasedAsync(string participantId, string eventId);

        Task<Ticket> BookTicketAsync(string eventId, string participantId);
    }
}
