using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts
{
    public interface IBookingService
    {
        Task<Result<GetBookingDto>> CreateBookingAsync(int hotelId, CreateBookingDto createBookingDto);
        Task<Result<IEnumerable<GetBookingDto>>> GetBookingsAsync(int hotelId);
    }
}