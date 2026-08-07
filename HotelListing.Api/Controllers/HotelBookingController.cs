using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers
{
    [Route("api/hotels/{hotelId:int}/bookings")]
    [ApiController]
    [Authorize]
    public class HotelBookingController(IBookingService bookingService) : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBookingDto>>> GetBookings([FromRoute]int hotelId)
        {
            var result = await bookingService.GetBookingsAsync(hotelId);

            return ToActionResult(result);
        }

        
        [HttpPost]
        public async Task<ActionResult<GetBookingDto>> CreateBooking([FromRoute] int hotelId, [FromBody] CreateBookingDto createBookingDto)
        {
            var result = await bookingService.CreateBookingAsync(hotelId, createBookingDto);

            return ToActionResult(result);
        }

        [HttpPut("{bookingId:int}")]
        public async Task<ActionResult<GetBookingDto>> UpdateBooking(
            [FromRoute] int hotelId, 
            [FromRoute] int bookingId, 
            [FromBody] UpdateBookingDto 
            updateBookingDto)
        {
            var result = await bookingService.UpdateBookingAsync(hotelId, bookingId, updateBookingDto);

            return ToActionResult(result);
        }

        [HttpPut("{bookingId:int}/cancel")]
        public async Task<IActionResult> CancelBooking(
            [FromRoute] int hotelId,
            [FromRoute] int bookingId)
        {
            var result = await bookingService.CancelBookingAsync(hotelId, bookingId);

            return ToActionResult(result);
        }

        [HttpPut("{bookingId:int}/admin/cancel")]
        [Authorize(Roles = "Hotel Admin, Administrator")]
        public async Task<IActionResult> AdminCancelBooking(
            [FromRoute] int hotelId,
            [FromRoute] int bookingId)
        {
            var result = await bookingService.AdminCancelBookingAsync(hotelId, bookingId);

            return ToActionResult(result);
        }

        [HttpPut("{bookingId:int}/admin/confirm")]
        [Authorize(Roles = "Hotel Admin, Administrator")]
        public async Task<IActionResult> AdminConfirmBooking(
            [FromRoute] int hotelId,
            [FromRoute] int bookingId)
        {
            var result = await bookingService.AdminConfirmBookingAsync(hotelId, bookingId);

            return ToActionResult(result);
        }


    }
}
