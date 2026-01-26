using Microsoft.AspNetCore.Mvc;
using laundry_booking_backend.Dtos;
using laundry_booking_backend.Services;

namespace laundry_booking_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_authService.ValidatePin(request.Pin))
        {
            return BadRequest(new { message = "PIN must be exactly 4 digits" });
        }

        if (!_authService.ValidateApartmentNumber(request.ApartmentNumber))
        {
            return BadRequest(new { message = "Apartment number must be exactly 6 uppercase alphanumeric characters" });
        }

        var result = await _authService.RegisterAsync(request);

        if (result == null)
        {
            return BadRequest(new { message = "Registration failed. Apartment number might already be registered." });
        }

        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid apartment number or PIN" });
        }

        return Ok(result);
    }
}
