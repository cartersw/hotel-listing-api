using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Data.Enums;
using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace HotelListing.Api.Services
{
    public class BookingService(HotelListingDbContext context, IHttpContextAccessor httpContextAccessor) : IBookingService
    {
        public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsAsync(int hotelId)
        {

            var hotel = await context.Hotels.FindAsync(hotelId);

            if (hotel == null)
            {
                return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, "Hotel with Id " + hotelId + " was not found"));
            }


            var bookings = await context.Bookings
                .Where(b => b.HotelId == hotelId)
                .OrderBy(b => b.CheckIn)
                .Select(b => new GetBookingDto(
                    b.Id,
                    b.HotelId,
                    b.Hotel!.Name,
                    b.CheckIn,
                    b.CheckOut,
                    b.Guests,
                    b.TotalPrice,
                    b.Status.ToString(),
                    b.CreatedAtUtc,
                    b.UpdatedAtUtc))
                .ToListAsync();

            return Result<IEnumerable<GetBookingDto>>.Success(bookings);
        }

        public async Task<Result<GetBookingDto>> CreateBookingAsync(int hotelId, CreateBookingDto createBookingDto)
        {

            var userId = httpContextAccessor?
                .HttpContext?
                .User?
                .FindFirst(JwtRegisteredClaimNames.Sub)?
                .Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Must be logged in to create a booking"));
            }

            var nights = createBookingDto.CheckOut.DayNumber - createBookingDto.CheckIn.DayNumber;

            if(nights <= 0)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Check-out must be after check-in"));
            }

            if(createBookingDto.Guests <= 0)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Booking requires at least 1 guest"));
            }
            var hotel = await context.Hotels.FindAsync(hotelId);

            if (hotel == null)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, "Hotel with Id " + hotelId + " was not found"));
            }

            var overlaps = await context.Bookings.AnyAsync(
                b => b.HotelId == hotelId
                && b.Status != BookingStatus.Cancelled
                && ((createBookingDto.CheckIn < b.CheckOut && createBookingDto.CheckIn > b.CheckIn)
                || (createBookingDto.CheckOut > b.CheckIn && createBookingDto.CheckOut < b.CheckOut))
                && b.UserId == userId);

            if (overlaps)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Booking overlaps with another booking by the same user"));
            }


            var totalPrice = hotel.NightlyRate * nights;


            var booking = new Booking
            {
                HotelId = createBookingDto.HotelId,
                UserId = userId,
                CheckIn = createBookingDto.CheckIn,
                CheckOut = createBookingDto.CheckOut,
                Guests = createBookingDto.Guests,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending
            };

            context.Bookings.Add(booking);

            await context.SaveChangesAsync();

            var created = new GetBookingDto(
                booking.Id,
                hotel.Id,
                hotel.Name,
                booking.CheckIn,
                booking.CheckOut,
                booking.Guests,
                totalPrice,
                BookingStatus.Pending.ToString(),
                booking.CreatedAtUtc,
                booking.UpdatedAtUtc
                );

            return Result<GetBookingDto>.Success(created);
        }
    }

    

}
