using EventHub.API.Security;
using EventHub.API.Hubs;
using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/ticket")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IHubContext<EventAvailabilityHub> _hubContext;

        public TicketController(ITicketService ticketService, IHubContext<EventAvailabilityHub> hubContext)
        {
            _ticketService = ticketService;
            _hubContext = hubContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(string id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

        [HttpGet("qrcode/{qrCode}")]
        public async Task<IActionResult> GetTicketByQrCode(string qrCode)
        {
            var ticket = await _ticketService.GetTicketByQrCodeAsync(qrCode);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

        [HttpGet("participant/{participantId}")]
        public async Task<IActionResult> GetTicketsByParticipant(string participantId)
        {
            var tickets = await _ticketService.GetTicketsByParticipantAsync(participantId);
            return Ok(tickets);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetTicketsByEvent(string eventId)
        {
            var tickets = await _ticketService.GetTicketsByEventAsync(eventId);
            return Ok(tickets);
        }

        [HttpGet("participant/{participantId}/has-purchased/{eventId}")]
        public async Task<IActionResult> HasParticipantPurchased(string participantId, string eventId)
        {
            var hasPurchased = await _ticketService.HasParticipantPurchasedAsync(participantId, eventId);
            return Ok(hasPurchased);
        }

        [HttpPost("purchase/{eventId}")]
        [Authorize(Roles = nameof(UserRole.Participant))]
        public async Task<IActionResult> PurchaseTicket(string eventId)
        {
            var participantId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(participantId))
                return Unauthorized();

            try
            {
                var purchase = await _ticketService.PurchaseTicketAsync(eventId, participantId);
                await _hubContext.Clients.Group(EventAvailabilityHub.GroupName(eventId))
                    .SendAsync("TicketAvailabilityChanged", new
                    {
                        eventId = purchase.EventId,
                        availableTickets = purchase.RemainingAvailableTickets
                    });

                return Ok(purchase);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
