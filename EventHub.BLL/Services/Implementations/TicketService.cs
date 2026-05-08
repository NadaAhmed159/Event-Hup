using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.BLL.Models;
using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.Data.SqlClient;
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

        public async Task<TicketPurchaseResult> PurchaseTicketAsync(string eventId, string participantId)
        {
            const int maxRetries = 3;
            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var alreadyBooked = await _unitOfWork.Tickets.HasParticipantPurchasedAsync(participantId, eventId);
                    if (alreadyBooked)
                        throw new InvalidOperationException("You already have a ticket for this event.");

                    var eventObj = await _unitOfWork.Events.GetByIdAsync(eventId);
                    if (eventObj == null)
                        throw new KeyNotFoundException("Event not found.");

                    if (eventObj.Status != EventStatus.Approved)
                        throw new InvalidOperationException("Only approved events can be booked.");

                    if (eventObj.EventDate <= DateTime.UtcNow)
                        throw new InvalidOperationException("You can only book upcoming events.");

                    if (eventObj.AvailableTickets < 1)
                        throw new InvalidOperationException("No tickets are available for this event.");

                    eventObj.AvailableTickets -= 1;

                    var order = new Order
                    {
                        ParticipantId = participantId,
                        EventId = eventObj.Id,
                        TotalPrice = eventObj.Price
                    };

                    var ticket = new Ticket
                    {
                        EventId = eventId,
                        ParticipantId = participantId,
                        OrderId = order.Id,
                        PurchasedAt = DateTime.UtcNow,
                        QrCode = Guid.NewGuid().ToString("N")
                    };

                    await _unitOfWork.Orders.AddAsync(order);
                    await _unitOfWork.Tickets.AddAsync(ticket);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return new TicketPurchaseResult
                    {
                        OrderId = order.Id,
                        EventId = eventObj.Id,
                        ParticipantId = participantId,
                        TotalPrice = order.TotalPrice,
                        RemainingAvailableTickets = eventObj.AvailableTickets,
                        TicketId = ticket.Id,
                        TicketQrCode = ticket.QrCode
                    };
                }
                catch (DbUpdateConcurrencyException) when (attempt < maxRetries)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch (DbUpdateException ex) when (IsUniqueViolation(ex))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new InvalidOperationException("You already have a ticket for this event.");
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }

            throw new InvalidOperationException("Could not complete purchase due to concurrent bookings. Please try again.");
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sql)
                return sql.Number is 2627 or 2601;
            return ex.InnerException?.InnerException is SqlException nested &&
                   nested.Number is 2627 or 2601;
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
