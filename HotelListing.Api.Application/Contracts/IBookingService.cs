using HotelListing.Api.Application.DTOs.Booking;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;

namespace HotelListing.Api.Application.Contracts
{
    public interface IBookingService
    {
        Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId);
        Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId);
        Task<Result> CancelBookingAsync(int hotelId, int bookingId);
        Task<Result<GetBookingDto>> CreateBookingAsync(int hotelId, CreateBookingDto createBookingDto);
        Task<Result<PagedResult<GetBookingDto>>> GetBookingsHotelAsync(int hotelId, PaginationParameters paginationParameters);
        Task<Result<PagedResult<GetBookingDto>>> GetBookingsUserAsync(int hotelId, PaginationParameters paginationParameters);
        Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
    }
}