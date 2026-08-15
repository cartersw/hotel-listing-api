using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Extensions;
using HotelListing.Api.Common.Models.Filtering;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Application.Services
{
    public class HotelService(HotelListingDbContext context, UserManager<ApplicationUser> userManager) : IHotelService
    {
        public async Task<Result<PagedResult<GetHotelDto>>> GetHotelsAsync(PaginationParameters paginationParameters,
            HotelFilterParameters filters)
        {
            var query = context.Hotels.AsQueryable();

            if (filters.CountryId.HasValue)
            {
                query = query.Where(q => q.CountryId == filters.CountryId);
            }
            if (filters.MinRating.HasValue)
            {
                query = query.Where(h => h.Rating >= filters.MinRating.Value);
            }
            if (filters.MinPrice.HasValue)
            {
                query = query.Where(h => h.NightlyRate >= filters.MinPrice.Value);
            }
            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(h => h.NightlyRate <= filters.MaxPrice.Value);
            }
            if (!string.IsNullOrWhiteSpace(filters.Location))
            {
                query = query.Where(h => h.Address.Contains(filters.Location));
            }
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(h => h.Name.Contains(filters.Search) || h.Address.Contains(filters.Search));
            }

            query = filters.SortBy?.ToLower() switch
            {
                "name" => filters.SortDescending ?
                    query.OrderByDescending(h => h.Name) : query.OrderBy(h => h.Name),

                "rating" => filters.SortDescending ?
                    query.OrderByDescending(h => h.Rating) : query.OrderBy(h => h.Rating),

                "price" => filters.SortDescending ?
                    query.OrderByDescending(h => h.NightlyRate) : query.OrderBy(h => h.NightlyRate),

                _ => query.OrderBy(h => h.Name)
            };


            var hotels = await query.Select(h => new GetHotelDto(
                h.Id,
                h.Name,
                h.Address,
                h.Rating,
                h.Country!.Name
            )).ToPagedResultAsync(paginationParameters);

            return Result<PagedResult<GetHotelDto>>.Success(hotels);
        }
        public async Task<Result<GetHotelDetailsDto>> GetHotelAsync(int id)
        {
            var hotel = await context.Hotels
            .Where(h => h.Id == id)
            .Select(h => new GetHotelDetailsDto(
                h.Id,
                h.Name,
                h.Address,
                h.Rating,
                h.CountryId
            )).FirstOrDefaultAsync();

            return hotel != null ? Result<GetHotelDetailsDto>.Success(hotel) : Result<GetHotelDetailsDto>.NotFound();
        }

        public async Task<Result> UpdateHotelAsync(int id, UpdateHotelDto hotelDto)
        {

            if(id != hotelDto.Id)
            {
                return Result.BadRequest(new Error("Validation", "Id route value does not match payload id"));
            }

            var hotel = await context.Hotels.FindAsync(id);

            if (hotel == null)
            {
                return Result.NotFound();
            }

            var countryExists = await context.Countries.AnyAsync(c => c.CountryId == hotelDto.CountryId);

            if (!countryExists)
            {
                return Result.NotFound();
            }

            hotel.Name = hotelDto.Name;
            hotel.Address = hotelDto.Address;
            hotel.Rating = hotelDto.Rating;
            hotel.CountryId = hotelDto.CountryId;

            context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    Result.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Result.Success();
        }

        public async Task<Result<GetHotelDetailsDto>> CreateHotelAsync(CreateHotelDto hotelDto)
        {

            var countryExists = await context.Countries.AnyAsync(c => c.CountryId == hotelDto.CountryId);

            if (!countryExists)
            {
                return Result<GetHotelDetailsDto>.NotFound();
            }

            var hotel = new Hotel
            {
                Name = hotelDto.Name,
                Address = hotelDto.Address,
                Rating = hotelDto.Rating,
                CountryId = hotelDto.CountryId
            };

            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();
            var resultHotelDto = new GetHotelDetailsDto(
                hotel.Id,
                hotel.Name,
                hotel.Address,
                hotel.Rating,
                hotel.CountryId
            );

            return Result<GetHotelDetailsDto>.Success(resultHotelDto);

        }


        public async Task<Result> DeleteHotel(int id)
        {
            var hotel = await context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return Result.NotFound();
            }
            context.Hotels.Remove(hotel);
            await context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AddHotelAdminAsync(int hotelId, AddHotelAdminDto addHotelAdminDto)
        {
            if (!HotelExists(hotelId))
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Hotel does not exist"));
            }

            var user = await userManager.FindByIdAsync(addHotelAdminDto.UserId);

            if(user == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "User does not exist"));
            }

            var isAdmin = await context.HotelAdmins.AnyAsync(q => q.UserId == addHotelAdminDto.UserId && q.HotelId == hotelId);

            if (isAdmin)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "User is already an admin for this hotel"));
            }

            var hotelAdmin = new HotelAdmin
            {
                HotelId = hotelId,
                UserId = addHotelAdminDto.UserId
            };

            context.HotelAdmins.Add(hotelAdmin);
            await context.SaveChangesAsync();

            return Result.Success();
        }


        public bool HotelExists(int id)
        {
            return context.Hotels.Any(e => e.Id == id);
        }

        public bool HotelExists(string name)
        {
            return context.Hotels.Any(e => e.Name.ToLower().Trim() == name.ToLower().Trim()); ;
        }

        
    }
}
