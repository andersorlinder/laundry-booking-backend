namespace laundry_booking_backend.Models;

public class User
{
    public int Id { get; set; }
    public string Forename { get; set; } = null!;
    public string PasswordPin { get; set; } = null!; // Stored as hashed value
    public string ApartmentNumber { get; set; } = null!; // Exactly 6 characters
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<BookedTimeSlot> BookedTimeSlots { get; set; } = new List<BookedTimeSlot>();
}
