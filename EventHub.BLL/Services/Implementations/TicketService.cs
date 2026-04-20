using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.BLL.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Ticket> BookTicketAsync(string eventId, string participantId)
        {
            var eventObj = await _unitOfWork.Events.GetByIdAsync(eventId);
            if (eventObj == null) throw new Exception("Event not found");

            var purchasedCount = await _unitOfWork.Events.GetSoldTicketsCountAsync(eventId);
            if (purchasedCount >= eventObj.TotalTickets)
            {
                throw new Exception("Event is sold out");
            }

            var ticket = new Ticket
            {
                EventId = eventId,
                ParticipantId = participantId,
                PurchasedAt = DateTime.UtcNow,
                QrCode = Guid.NewGuid().ToString() // Simple QR for example
            };

            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return ticket;
        }

        public async Task CancelTicketAsync(string ticketId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket != null)
            {
                _unitOfWork.Tickets.Remove(ticket);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<Ticket?> GetTicketByIdAsync(string ticketId)
        {
            return await _unitOfWork.Tickets.GetWithDetailsAsync(ticketId);
        }

        public async Task<Ticket?> GetTicketByQrCodeAsync(string qrCode)
        {
            return await _unitOfWork.Tickets.GetByQrCodeAsync(qrCode);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByEventAsync(string eventId)
        {
            return await _unitOfWork.Tickets.GetByEventAsync(eventId);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByParticipantAsync(string participantId)
        {
            return await _unitOfWork.Tickets.GetByParticipantAsync(participantId);
        }

        public async Task<bool> HasParticipantPurchasedAsync(string participantId, string eventId)
        {
            return await _unitOfWork.Tickets.HasParticipantPurchasedAsync(participantId, eventId);
        }
    }
}
