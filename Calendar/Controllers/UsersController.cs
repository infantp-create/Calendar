
using Calendar.Dto.Users;
using Calendar.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _service;

    public UsersController(UsersService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _service.GetUsers();
        return Ok(users);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var result = await _service.Register(dto);
        if (!result.Success)
            return Conflict(new { error = result.Error });

        return CreatedAtAction(nameof(GetUsers), new { id = result.User!.Id }, result.User);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await _service.Login(dto);
        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(result.User);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
    {
        var result = await _service.UpdateUser(id, dto);
        if (!result.Success)
            return NotFound(new { error = result.Error });

        return Ok(result.User);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _service.DeleteUser(id);
        if (!result.Success)
            return NotFound(new { error = result.Error });

        return NoContent();
    }
}

