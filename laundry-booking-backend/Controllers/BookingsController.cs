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

    [HttpGet("available/{date}")]
    [AllowAnonymous]
    public async Task<ActionResult<AvailableSlotsResponse>> GetAvailableSlots(string date)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd" });
        }

        var bookedSlots = await _bookingService.GetAvailableTimeSlots(parsedDate);
        var bookedSlotNumbers = bookedSlots.Select(s => s.TimeSlotNumber).ToList();

        var availableSlots = new List<int> { 1, 2, 3 }
            .Where(slotNum => !bookedSlotNumbers.Contains(slotNum))
            .ToList();

        return Ok(new AvailableSlotsResponse
        {
            Date = parsedDate.Date,
            AvailableSlots = availableSlots,
            BookedSlots = bookedSlots
        });
    }
}

public class AvailableSlotsResponse
{
    public DateTime Date { get; set; }
    public List<int> AvailableSlots { get; set; } = new();
    public List<BookedTimeSlotDto> BookedSlots { get; set; } = new();
}
