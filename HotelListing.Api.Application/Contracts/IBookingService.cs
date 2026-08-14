using HotelListing.Api.Application.DTOs.Booking;
using HotelListing.Api.Common.Results;

namespace HotelListing.Api.Application.Contracts
{
    public interface IBookingService
    {
        Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId);
        Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId);
        Task<Result> CancelBookingAsync(int hotelId, int bookingId);
        Task<Result<GetBookingDto>> CreateBookingAsync(int hotelId, CreateBookingDto createBookingDto);
        Task<Result<IEnumerable<GetBookingDto>>> GetBookingsHotelAsync(int hotelId);
        Task<Result<IEnumerable<GetBookingDto>>> GetBookingsUserAsync(int hotelId);
        Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
    }
}