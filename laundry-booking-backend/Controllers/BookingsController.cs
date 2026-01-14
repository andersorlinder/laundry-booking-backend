using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using laundry_booking_backend.Dtos;
using laundry_booking_backend.Services;

namespace laundry_booking_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }

    [HttpPost("book")]
    public async Task<ActionResult<BookedTimeSlotDto>> BookTimeSlot(BookingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        var result = await _bookingService.BookTimeSlotAsync(userId, request);

        if (result == null)
        {
            return BadRequest(new { message = "Failed to book time slot. It might already be taken or invalid." });
        }

        return CreatedAtAction(nameof(GetUserBookings), result);
    }

    [HttpGet("my-bookings")]
    public async Task<ActionResult<List<BookedTimeSlotDto>>> GetUserBookings()
    {
        var userId = GetUserId();
        var bookings = await _bookingService.GetUserBookingsAsync(userId);

        return Ok(bookings);
    }

    [HttpDelete("unbook/{slotId}")]
    public async Task<IActionResult> UnbookTimeSlot(int slotId)
    {
        var userId = GetUserId();
        var result = await _bookingService.UnbookTimeSlotAsync(userId, slotId);

        if (!result)
        {
            return NotFound(new { message = "Booking not found or doesn't belong to you" });
        }

        return NoContent();
    }

    [HttpGet("booked/{date}")]
    [AllowAnonymous]
    public async Task<ActionResult<BookedSlotsByDateResponse>> GetBookedSlots(string date, [FromQuery] int daysAhead = 1)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd" });
        }

        if (daysAhead < 1)
        {
            return BadRequest(new { message = "daysAhead must be at least 1" });
        }

        var bookedSlots = await _bookingService.GetBookedTimeSlots(parsedDate, daysAhead);

        // Group bookings by date
        var bookingsByDate = bookedSlots.GroupBy(b => b.BookingDate.Date)
            .Select(g => new DayBookings
            {
                Date = g.Key,
                BookedSlots = g.ToList()
            })
            .OrderBy(d => d.Date)
            .ToList();

        return Ok(new BookedSlotsByDateResponse
        {
            StartDate = parsedDate.Date,
            EndDate = parsedDate.Date.AddDays(daysAhead).AddDays(-1),
            DaysAhead = daysAhead,
            BookingsByDate = bookingsByDate
        });
    }
}

public class DayBookings
{
    public DateTime Date { get; set; }
    public List<BookedTimeSlotDto> BookedSlots { get; set; } = new();
}

public class BookedSlotsByDateResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysAhead { get; set; }
    public List<DayBookings> BookingsByDate { get; set; } = new();
}
