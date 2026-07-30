using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services
{
    public class HotelService(HotelListingDbContext context) : IHotelService
    {
        public async Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync()
        {
            var hotels = await context.Hotels.Select(h => new GetHotelDto(
                h.Id,
                h.Name,
                h.Address,
                h.Rating,
                h.Country!.Name
            )).ToListAsync();

            return Result<IEnumerable<GetHotelDto>>.Success(hotels);
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


        public bool HotelExists(int? id)
        {
            return context.Hotels.Any(e => e.Id == id);
        }

        public bool HotelExists(string name)
        {
            return context.Hotels.Any(e => e.Name.ToLower().Trim() == name.ToLower().Trim()); ;
        }




    }
}
