namespace laundry_booking_backend.Dtos;

public class RegisterRequest
{
    public string Forename { get; set; } = null!;
    public string Pin { get; set; } = null!; // Must be 4 digits
    public string ApartmentNumber { get; set; } = null!; // Must be exactly 6 characters
}
