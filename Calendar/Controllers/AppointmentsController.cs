using Calendar.Dto.Appointments;
using Calendar.Services;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{userId}")]
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


