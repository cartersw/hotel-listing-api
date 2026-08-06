using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services
{
    public class BookingService(HotelListingDbContext context) : IBookingService
    {
        public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsAsync(int hotelId)
        {

            var hotel = await context.Hotels.FindAsync(hotelId);

            if (hotel == null)
            {
                return Result<IEnumerable<GetBookingDto>>.NotFound();
            }


            var bookings = await context.Bookings
                .Where(b => b.HotelId == hotelId)
                .Select(b => new GetBookingDto(
                    b.Id,
                    b.HotelId,
                    hotel.Name,
                    b.CheckIn,
                    b.Checkout,
                    b.Guests,
                    b.TotalPrice,
                    b.Status.ToString(),
                    b.CreatedAtUtc,
                    b.UpdatedAtUtc))
                .ToListAsync();

            return Result<IEnumerable<GetBookingDto>>.Success(bookings);
        }
    }
}
