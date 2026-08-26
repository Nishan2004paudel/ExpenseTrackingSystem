using expensetrackerserver.DTOs;
using expensetrackerserver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace expensetrackerserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var notifications = await _service.GetMyNotifications(userId);
            return Ok(notifications);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetMyUnreadNotifications()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var notifications = await _service.GetMyUnreadNotifications(userId);
            return Ok(notifications);
        }

        [HttpPatch("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.MarkAsRead(notificationId, userId);
            return Ok(new MessageResponseDto
            {
                Message = "Notification marked as read."
            });
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.MarkAllAsRead(userId);
            return Ok(new MessageResponseDto
            {
                Message = "All notifications marked as read."
            });
        }
    }
}
