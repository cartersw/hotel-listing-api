using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services
{
    public class HotelService(HotelListingDbContext context) : IHotelService
    {
        public async Task<IEnumerable<GetHotelDto>> GetHotelsAsync()
        {
            return await context.Hotels.Select(h => new GetHotelDto(
                h.Id,
                h.Name,
                h.Address,
                h.Rating,
                h.Country!.Name
            )).ToListAsync();
        }
        public async Task<GetHotelDetailsDto?> GetHotelAsync(int id)
        {
            return await context.Hotels
            .Where(h => h.Id == id)
            .Select(h => new GetHotelDetailsDto(
                h.Id,
                h.Name,
                h.Address,
                h.Rating,
                h.CountryId
            )).FirstOrDefaultAsync();
        }

        public async Task UpdateHotelAsync(int id, UpdateHotelDto hotelDto)
        {
            var hotel = await context.Hotels.FindAsync(id);

            if (hotel == null)
            {
                throw new KeyNotFoundException($"Hotel with id {id} was not found.");
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
                    throw new KeyNotFoundException($"Hotel with id {id} was not found.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<GetHotelDetailsDto> CreateHotelAsync(CreateHotelDto hotelDto)
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
            return new GetHotelDetailsDto(
                hotel.Id,
                hotel.Name,
                hotel.Address,
                hotel.Rating,
                hotel.CountryId
            );
        }


        public async Task DeleteHotel(int id)
        {
            var hotel = await context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                throw new KeyNotFoundException($"Hotel with id {id} was not found.");
            }
            context.Hotels.Remove(hotel);
            await context.SaveChangesAsync();
        }


        public bool HotelExists(int? id)
        {
            return context.Hotels.Any(e => e.Id == id);
        }




    }
}
