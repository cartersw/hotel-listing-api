using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Booking;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Enums;
using HotelListing.Api.Common.Extensions;
using HotelListing.Api.Common.Models.Filtering;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Application.Services
{
    public class BookingService(HotelListingDbContext context, IUserService userService) : IBookingService
    {
        public async Task<Result<PagedResult<GetBookingDto>>> GetBookingsHotelAsync(int hotelId, 
            PaginationParameters paginationParameters,
            BookingFilterParameters filters)
        {

            var hotel = await context.Hotels.FindAsync(hotelId);

            if (hotel == null)
            {
                return Result<PagedResult<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, "Hotel with Id " + hotelId + " was not found"));
            }

            var query = ApplyFilters(hotelId, filters);


            var bookings = await query
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
                .ToPagedResultAsync(paginationParameters);

            return Result<PagedResult<GetBookingDto>>.Success(bookings);
        }


        public async Task<Result<PagedResult<GetBookingDto>>> GetBookingsUserAsync(int hotelId, 
            PaginationParameters paginationParameters,
            BookingFilterParameters filters)
        {

            var hotel = await context.Hotels.FindAsync(hotelId);

            if (hotel == null)
            {
                return Result<PagedResult<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, "Hotel with Id " + hotelId + " was not found"));
            }

            var userId = userService.UserId;

            var query = ApplyFilters(hotelId, filters);

            var bookings = await query
                .Where(b =>  b.UserId == userId)
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
                .ToPagedResultAsync(paginationParameters);

            return Result<PagedResult<GetBookingDto>>.Success(bookings);
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

        private IQueryable<Booking> ApplyFilters(int hotelId, BookingFilterParameters filters)
        {
            var query = context.Bookings.Where(b => b.HotelId == hotelId);

            if (filters.Status.HasValue)
            {
                query = query.Where(b => b.Status == filters.Status.Value);
            }
            if (filters.CheckInFrom.HasValue)
            {
                query = query.Where(b => b.CheckIn >= filters.CheckInFrom.Value);
            }
            if (filters.CheckInTo.HasValue)
            {
                query = query.Where(b => b.CheckIn <= filters.CheckInTo.Value);
            }
            if (filters.MinPrice.HasValue)
            {
                query = query.Where(b => b.TotalPrice >= filters.MinPrice.Value);
            }
            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(b => b.TotalPrice <= filters.MaxPrice.Value);
            }
            if (filters.MinGuests.HasValue)
            {
                query = query.Where(b => b.Guests >= filters.MinGuests.Value);
            }
            if (filters.MaxGuests.HasValue)
            {
                query = query.Where(b => b.Guests <= filters.MaxGuests.Value);
            }

            query = filters.SortBy?.ToLower() switch
            {
                "checkin" => filters.SortDescending ?
                    query.OrderByDescending(b => b.CheckIn) : query.OrderBy(b => b.CheckIn),
                "checkout" => filters.SortDescending ?
                    query.OrderByDescending(b => b.CheckOut) : query.OrderBy(b => b.CheckOut),
                "price" => filters.SortDescending ?
                    query.OrderByDescending(b => b.TotalPrice) : query.OrderBy(b => b.TotalPrice),
                "created" => filters.SortDescending ?
                    query.OrderByDescending(b => b.CreatedAtUtc) : query.OrderBy(b => b.CreatedAtUtc),
                _ => query.OrderBy(b => b.CheckIn)

            };

            return query;


        }


    }

    

}
