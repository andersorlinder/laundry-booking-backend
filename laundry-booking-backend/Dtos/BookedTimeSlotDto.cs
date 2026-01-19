namespace laundry_booking_backend.Dtos;

public class BookedTimeSlotDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ApartmentNumber { get; set; } = null!;
    public DateTime BookingDate { get; set; }
    public int TimeSlotNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
