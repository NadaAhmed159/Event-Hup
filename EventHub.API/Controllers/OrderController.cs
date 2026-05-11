using EventHub.API.Security;
using EventHub.BLL.Mapping;
using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IEventService _eventService;

        public OrderController(IOrderService orderService, IEventService eventService)
        {
            _orderService = orderService;
            _eventService = eventService;
        }

        [HttpGet("{orderId}")]
        [Authorize(Roles = $"{nameof(UserRole.Participant)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> GetById(string orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
                return NotFound();

            if (EventManagementAuth.IsAdmin(User))
                return Ok(OrderDtoMapper.ToDetailsDto(order));

            var userId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (order.ParticipantId != userId)
                return Forbid();

            return Ok(OrderDtoMapper.ToDetailsDto(order));
        }

        [HttpGet("my-orders")]
        [Authorize(Roles = nameof(UserRole.Participant))]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var orders = await _orderService.GetMyOrdersAsync(userId);
            return Ok(OrderDtoMapper.ToDetailsDtos(orders));
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.EventOrganizer)}")]
        public async Task<IActionResult> GetByEvent(string eventId)
        {
            var eventItem = await _eventService.GetEventByIdAsync(eventId);
            if (eventItem == null)
                return NotFound("Event not found.");

            if (!EventManagementAuth.CanMutateEvent(User, eventItem.OrganizerId))
                return Forbid();

            var orders = await _orderService.GetByEventAsync(eventId);
            return Ok(OrderDtoMapper.ToDetailsDtos(orders));
        }
    }
}
