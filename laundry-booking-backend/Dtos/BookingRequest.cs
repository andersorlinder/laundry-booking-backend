namespace laundry_booking_backend.Dtos;

public class BookingRequest
{
    public DateTime BookingDate { get; set; } // Date of the booking
    public int TimeSlotNumber { get; set; } // 1, 2, or 3
}
