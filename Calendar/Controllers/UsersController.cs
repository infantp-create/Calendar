
using Calendar.Dto.Users;
using Calendar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Calendar.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly JwtService _jwtService;

    public UsersController(UsersService usersService, JwtService jwtService)
    {
        _usersService = usersService;
        _jwtService = jwtService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _usersService.GetUsers();
        return Ok(users);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        var result = await _usersService.Register(dto);
        if (!result.Success) return BadRequest(new { result.Error });

        // 🔑 Auto-generate JWT after register
        var token = _jwtService.GenerateToken(result.User!.Id, result.User.UserName, result.User.Email);

        return Ok(new
        {
            token,
            user = result.User
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var result = await _usersService.Login(dto, _jwtService);
        if (!result.Success) return BadRequest(new { result.Error });

        return Ok(new
        {
            token = result.Token,
            user = result.User
        });
    }
    
    [HttpGet("me")]
    [Authorize] // ✅ protect with JWT
    public IActionResult Me()
    {
        var userId = User.FindFirst("sub")?.Value;
        var userName = User.Identity?.Name;
        var email = User.FindFirst("email")?.Value;

        return Ok(new { userId, userName, email });
    }


    [HttpPut("{id}"), Authorize]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
    {
        var result = await _usersService.UpdateUser(id, dto);
        if (!result.Success)
            return NotFound(new { error = result.Error });

        return Ok(result.User);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _usersService.DeleteUser(id);
        if (!result.Success)
            return NotFound(new { error = result.Error });

        return NoContent();
    }
}

