using Microsoft.EntityFrameworkCore;
using laundry_booking_backend.Data;
using laundry_booking_backend.Dtos;
using laundry_booking_backend.Models;

namespace laundry_booking_backend.Services;

public interface IBookingService
{
    Task<BookedTimeSlotDto?> BookTimeSlotAsync(int userId, BookingRequest request);
    Task<bool> UnbookTimeSlotAsync(int userId, int slotId);
    Task<List<BookedTimeSlotDto>> GetUserBookingsAsync(int userId);
    Task<List<BookedTimeSlotDto>> GetBookedTimeSlots(DateTime date, int daysAhead = 1);
}

public class BookingService : IBookingService
{
    private readonly LaundryDbContext _context;

    public BookingService(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<BookedTimeSlotDto?> BookTimeSlotAsync(int userId, BookingRequest request)
    {
        // Validate time slot number (must be 1, 2, or 3)
        if (request.TimeSlotNumber < 1 || request.TimeSlotNumber > 3)
        {
            return null; // Invalid time slot number
        }

        // Ensure booking date is in the future
        var bookingDateOnly = DateOnly.FromDateTime(request.BookingDate);
        var todayOnly = DateOnly.FromDateTime(DateTime.UtcNow);
        if (bookingDateOnly < todayOnly)
        {
            return null; // Cannot book for past dates
        }

        var requestDateUtc = DateTime.SpecifyKind(request.BookingDate.Date, DateTimeKind.Utc);

        // Check if user already booked this slot
        var existingBooking = await _context.BookedTimeSlots
            .FirstOrDefaultAsync(b => b.UserId == userId &&
                                      b.BookingDate.Date == requestDateUtc &&
                                      b.TimeSlotNumber == request.TimeSlotNumber);

        if (existingBooking != null)
        {
            return null; // User already booked this slot
        }

        // Check if slot is already taken by another user
        var slotTaken = await _context.BookedTimeSlots
            .FirstOrDefaultAsync(b => b.BookingDate.Date == requestDateUtc &&
                                      b.TimeSlotNumber == request.TimeSlotNumber);

        if (slotTaken != null)
        {
            return null; // Slot is already booked
        }

        var booking = new BookedTimeSlot
        {
            UserId = userId,
            BookingDate = requestDateUtc,
            TimeSlotNumber = request.TimeSlotNumber,
            CreatedAt = DateTime.UtcNow
        };

        _context.BookedTimeSlots.Add(booking);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);

        return new BookedTimeSlotDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            ApartmentNumber = user!.ApartmentNumber,
            BookingDate = booking.BookingDate,
            TimeSlotNumber = booking.TimeSlotNumber,
            CreatedAt = booking.CreatedAt
        };
    }

    public async Task<bool> UnbookTimeSlotAsync(int userId, int slotId)
    {
        var booking = await _context.BookedTimeSlots
            .FirstOrDefaultAsync(b => b.Id == slotId && b.UserId == userId);

        if (booking == null)
        {
            return false; // Booking not found or doesn't belong to user
        }

        // Prevent unbooking past dates
        var bookingDateOnly = DateOnly.FromDateTime(booking.BookingDate);
        var todayOnly = DateOnly.FromDateTime(DateTime.UtcNow);
        if (bookingDateOnly < todayOnly)
        {
            return false; // Cannot unbook past dates
        }

        _context.BookedTimeSlots.Remove(booking);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<BookedTimeSlotDto>> GetUserBookingsAsync(int userId)
    {
        return await _context.BookedTimeSlots
            .Include(b => b.User)
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.TimeSlotNumber)
            .Select(b => new BookedTimeSlotDto
            {
                Id = b.Id,
                UserId = b.UserId,
                ApartmentNumber = b.User.ApartmentNumber,
                BookingDate = b.BookingDate,
                TimeSlotNumber = b.TimeSlotNumber,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<BookedTimeSlotDto>> GetBookedTimeSlots(DateTime date, int daysAhead = 1)
    {
        var startDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var endDate = startDate.AddDays(daysAhead);

        var bookedSlots = await _context.BookedTimeSlots
            .Include(b => b.User)
            .Where(b => b.BookingDate.Date >= startDate && b.BookingDate.Date < endDate)
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.TimeSlotNumber)
            .Select(b => new BookedTimeSlotDto
            {
                Id = b.Id,
                UserId = b.UserId,
                ApartmentNumber = b.User.ApartmentNumber,
                BookingDate = b.BookingDate,
                TimeSlotNumber = b.TimeSlotNumber,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return bookedSlots;
    }
}
