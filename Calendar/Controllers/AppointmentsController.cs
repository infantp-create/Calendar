using Calendar.Dto.Appointments;
using Calendar.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace Calendar.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentsService _service;

    public AppointmentsController(AppointmentsService service)
    {
        _service = service;
    }

    [HttpGet("{userId}"), Authorize]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments(
        int userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? type)
    {
        // Default startDate to today if not provided
        var start = startDate ?? DateTime.Today;

        var appointments = await _service.GetAppointmentsForUser(userId, start, endDate, type);
        return Ok(appointments);
    }
}


