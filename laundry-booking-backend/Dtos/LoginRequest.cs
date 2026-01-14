namespace laundry_booking_backend.Dtos;

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Pin { get; set; } = null!;
}
