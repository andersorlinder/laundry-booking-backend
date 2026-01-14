namespace laundry_booking_backend.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordPin { get; set; } = null!; // Stored as hashed value
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<BookedTimeSlot> BookedTimeSlots { get; set; } = new List<BookedTimeSlot>();
}
