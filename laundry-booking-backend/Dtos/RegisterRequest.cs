namespace laundry_booking_backend.Dtos;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Pin { get; set; } = null!; // Must be 4 digits
}
