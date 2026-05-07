using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.BLL.Models;
using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHub.BLL.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketPurchaseResult> PurchaseTicketsAsync(string eventId, string participantId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            const int maxRetries = 3;
            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var eventObj = await _unitOfWork.Events.GetByIdAsync(eventId);
                    if (eventObj == null)
                        throw new KeyNotFoundException("Event not found.");

                    if (eventObj.Status != EventStatus.Approved)
                        throw new InvalidOperationException("Only approved events can be booked.");

                    if (eventObj.EventDate <= DateTime.UtcNow)
                        throw new InvalidOperationException("You can only book upcoming events.");

                    if (eventObj.AvailableTickets < quantity)
                        throw new InvalidOperationException($"Only {eventObj.AvailableTickets} tickets are available.");

                    eventObj.AvailableTickets -= quantity;

                    var order = new Order
                    {
                        ParticipantId = participantId,
                        EventId = eventObj.Id,
                        Quantity = quantity,
                        TotalPrice = eventObj.Price * quantity
                    };

                    var tickets = Enumerable.Range(0, quantity).Select(_ => new Ticket
                    {
                        EventId = eventId,
                        ParticipantId = participantId,
                        OrderId = order.Id,
                        PurchasedAt = DateTime.UtcNow,
                        QrCode = Guid.NewGuid().ToString("N")
                    }).ToList();

                    await _unitOfWork.Orders.AddAsync(order);
                    await _unitOfWork.Tickets.AddRangeAsync(tickets);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return new TicketPurchaseResult
                    {
                        OrderId = order.Id,
                        EventId = eventObj.Id,
                        ParticipantId = participantId,
                        Quantity = quantity,
                        TotalPrice = order.TotalPrice,
                        RemainingAvailableTickets = eventObj.AvailableTickets,
                        TicketIds = tickets.Select(t => t.Id).ToList(),
                        TicketQrCodes = tickets.Select(t => t.QrCode).ToList()
                    };
                }
                catch (DbUpdateConcurrencyException) when (attempt < maxRetries)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }

            throw new InvalidOperationException("Could not complete purchase due to concurrent bookings. Please try again.");
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
