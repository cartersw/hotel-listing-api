using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Booking;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Domain;
using HotelListing.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Application.Services
{
    public class BookingService(HotelListingDbContext context, IUserService userService) : IBookingService
    {
        public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsHotelAsync(int hotelId)
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

        public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsUserAsync(int hotelId)
        {

            var hotel = await context.Hotels.FindAsync(hotelId);

            if (hotel == null)
            {
                return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, "Hotel with Id " + hotelId + " was not found"));
            }

            var userId = userService.UserId;

            var bookings = await context.Bookings
                .Where(b => b.HotelId == hotelId && b.UserId == userId)
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

            var userId = userService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Must be logged in to create a booking"));
            }

            var nights = createBookingDto.CheckOut.DayNumber - createBookingDto.CheckIn.DayNumber;

            if(nights <= 0)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Check-out must be after check-in"));
            }


            var hotel = await context.Hotels.FindAsync(hotelId);

            if (hotel == null)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, "Hotel with Id " + hotelId + " was not found"));
            }

            var overlaps = await context.Bookings.AnyAsync(b => 
                b.HotelId == hotelId
                && b.Status != BookingStatus.Cancelled
                && createBookingDto.CheckIn < b.CheckOut && createBookingDto.CheckOut > b.CheckIn
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

        

        public async Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto)
        {
            var userId = userService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Must be logged in to create a booking"));
            }

            var nights = updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber;

            if (nights <= 0)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Check-out must be after check-in"));
            }


            var overlaps = await context.Bookings.AnyAsync(b => 
                b.HotelId == hotelId
                && b.Status != BookingStatus.Cancelled
                && updateBookingDto.CheckIn < b.CheckOut && updateBookingDto.CheckOut > b.CheckIn
                && b.UserId == userId
                && b.Id != bookingId);

            if (overlaps)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Booking overlaps with another booking by the same user"));
            }

            var booking = await context.Bookings.FirstOrDefaultAsync(b => 
                b.Id == bookingId
                && b.HotelId == hotelId
                && b.UserId == userId);

            if(booking == null)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, "Booking does not exist"));
            }

            if(booking.Status == BookingStatus.Cancelled)
            {
                return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, "Cancelled bookings can not be modified"));
            }

            var perNight = booking.Hotel!.NightlyRate;

            booking.CheckIn = updateBookingDto.CheckIn;
            booking.CheckOut = updateBookingDto.CheckOut;
            booking.Guests = updateBookingDto.Guests;
            booking.TotalPrice = perNight * (updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber);
            booking.UpdatedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var updated = new GetBookingDto(
                booking.Id,
                booking.HotelId,
                booking.Hotel!.Name,
                booking.CheckIn,
                booking.CheckOut,
                booking.Guests,
                booking.TotalPrice,
                booking.Status.ToString(),
                booking.CreatedAtUtc,
                booking.UpdatedAtUtc
                );

            return Result<GetBookingDto>.Success(updated);
            
        }

        public async Task<Result> CancelBookingAsync(int hotelId, int bookingId)
        {
            var userId = userService.UserId;


            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "Must be logged in to create a booking"));
            }

            var booking = await context.Bookings.FirstOrDefaultAsync(b =>
                b.Id == bookingId
                && b.HotelId == hotelId
                && b.UserId == userId);

            if (booking == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Booking does not exist"));
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result.Failure(new Error(ErrorCodes.Conflict, "Cancelled bookings can not be modified"));
            }

            booking.Status = BookingStatus.Cancelled;

            booking.UpdatedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Result.Success();

        }

        public async Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId)
        {
            var userId = userService.UserId;

            var isHotelAdminUser = await context.HotelAdmins
                .AnyAsync(q => q.UserId == userId && q.HotelId == hotelId);

            if (!isHotelAdminUser)
            {
                return Result.Failure(new Error(ErrorCodes.Forbid, "Administrator privileges required for this request"));
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "Must be logged in to create a booking"));
            }

            var booking = await context.Bookings.FirstOrDefaultAsync(b =>
                b.Id == bookingId
                && b.HotelId == hotelId);

            if (booking == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Booking does not exist"));
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result.Failure(new Error(ErrorCodes.Conflict, "Cancelled bookings can not be modified"));
            }

            booking.Status = BookingStatus.Cancelled;

            booking.UpdatedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId)
        {
            var userId = userService.UserId;

            var isHotelAdminUser = await context.HotelAdmins
                .AnyAsync(q => q.UserId == userId && q.HotelId == hotelId);

            if (!isHotelAdminUser)
            {
                return Result.Failure(new Error(ErrorCodes.Forbid, "Administrator privileges required for this request"));
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "Must be logged in to create a booking"));
            }

            var booking = await context.Bookings.FirstOrDefaultAsync(b =>
                b.Id == bookingId
                && b.HotelId == hotelId);

            if (booking == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Booking does not exist"));
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return Result.Failure(new Error(ErrorCodes.Conflict, "Cancelled bookings can not be modified"));
            }
            
            if (booking.Status == BookingStatus.Confirmed)
            {
                return Result.Failure(new Error(ErrorCodes.Conflict, "Booking is already confirmed"));
            }

            booking.Status = BookingStatus.Confirmed;

            booking.UpdatedAtUtc = DateTime.UtcNow;
            
            await context.SaveChangesAsync();

            return Result.Success();
        }

        
    }

    

}
