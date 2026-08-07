using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Booking;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers
{
    [Route("api/hotels/{hotelId:int}/bookings")]
    [ApiController]
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


    }
}
