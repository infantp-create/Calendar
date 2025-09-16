using Calendar.Dto.Events;
using Calendar.Dto.Tasks;
using Calendar.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace Calendar.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventTaskController : ControllerBase
{
    private readonly EventTaskService _service;
    public EventTaskController(EventTaskService service) => _service = service;

    // ------------------ Tasks ------------------
    [HttpGet("tasks/{userId}"), Authorize] 
    
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(int userId) => Ok(await _service.GetTasksForUser(userId));
    [HttpPost("tasks"), Authorize] 
    
    public async Task<IActionResult> CreateTask([FromBody] TaskDto dto) => HandleTaskResult(await _service.CreateTask(dto));
    [HttpPut("tasks/{id}"), Authorize]
    
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto dto) => HandleTaskResult(await _service.UpdateTask(id, dto));
    [HttpDelete("tasks/{id}"), Authorize]
    
    public async Task<IActionResult> DeleteTask(int id) { var r = await _service.DeleteTask(id); return r.Success ? NoContent() : NotFound(new { error = r.Error }); }

    // ------------------ Events ------------------
    [HttpGet("events/{userId}"), Authorize]
    
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(int userId) => Ok(await _service.GetEventsForUser(userId));
    [HttpPost("events"), Authorize]
    
    public async Task<IActionResult> CreateEvent([FromBody] EventDto dto) => HandleEventResult(await _service.CreateEvent(dto));
    [HttpPut("events/{id}"), Authorize] 
    
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto dto) => HandleEventResult(await _service.UpdateEvent(id, dto));
    [HttpDelete("events/{id}"), Authorize]
    
    public async Task<IActionResult> DeleteEvent(int id,int userId) { var r = await _service.DeleteEvent(id,userId); return r.Success ? NoContent() : NotFound(new { error = r.Error }); }

    // ------------------ Helpers ------------------
    private IActionResult HandleTaskResult((bool Success, string? Error, List<TaskDto>? Tasks) result)
        => result.Success ? Created("", result.Tasks) : BadRequest(new { error = result.Error });

    private IActionResult HandleTaskResult((bool Success, string? Error, TaskDto? Task) result)
        => result.Success ? Ok(result.Task) : BadRequest(new { error = result.Error });

    private IActionResult HandleEventResult((bool Success, string? Error, List<EventDto>? Events) result)
        => result.Success ? Created("", result.Events) : BadRequest(new { error = result.Error });

    private IActionResult HandleEventResult((bool Success, string? Error, EventDto? Event) result)
        => result.Success ? Ok(result.Event) : BadRequest(new { error = result.Error });
}
