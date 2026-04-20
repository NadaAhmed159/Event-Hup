using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/ticket")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
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

        [HttpPost("book")]
        public async Task<IActionResult> BookTicket([FromQuery] string eventId, [FromQuery] string participantId)
        {
            try
            {
                var ticket = await _ticketService.BookTicketAsync(eventId, participantId);
                return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
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

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelTicket(string id)
        {
            try
            {
                await _ticketService.CancelTicketAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
