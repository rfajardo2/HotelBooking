using HotelBooking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Aquí podrías validar el usuario en la base de datos
        if (request.Username == "admin" && request.Password == "123")
        {
            var token = _jwtService.GenerateToken(request.Username, "Admin");
            return Ok(new { Token = token });
        }

        return Unauthorized("❌ Usuario o contraseña incorrectos.");
    }
}

// DTO para la autenticación
public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
