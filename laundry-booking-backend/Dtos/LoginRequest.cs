namespace laundry_booking_backend.Dtos;

public class LoginRequest
{
    public string ApartmentNumber { get; set; } = null!;
    public string Pin { get; set; } = null!;
}
