namespace laundry_booking_backend.Dtos;

public class LoginResponse
{
    public int UserId { get; set; }
    public string Forename { get; set; } = null!;
    public string ApartmentNumber { get; set; } = null!;
    public string Token { get; set; } = null!;
}
