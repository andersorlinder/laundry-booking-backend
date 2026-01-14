namespace laundry_booking_backend.Models;

public class BookedTimeSlot
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime BookingDate { get; set; } // Date of the booking
    public int TimeSlotNumber { get; set; } // 1, 2, or 3 for the three daily slots
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
