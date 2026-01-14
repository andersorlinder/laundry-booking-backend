namespace laundry_booking_backend.Dtos;

public class BookedTimeSlotDto
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }
    public int TimeSlotNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
