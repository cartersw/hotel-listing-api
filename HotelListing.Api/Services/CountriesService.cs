using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
namespace HotelListing.Api.Services
{
    public class CountriesService(HotelListingDbContext context) : ICountriesService
    {
        public async Task<IEnumerable<GetCountryDto>> GetCountriesAsync()
        {
            return await context.Countries.Select(c => new GetCountryDto(
                c.CountryId,
                c.Name,
                c.ShortName
            )).ToListAsync();

        }

        public async Task<GetCountryDetailsDto?> GetCountryAsync(int countryId)
        {
            return await context.Countries
            .Where(c => c.CountryId == countryId)
            .Select(c => new GetCountryDetailsDto(
            c.CountryId,
            c.Name,
            c.ShortName,
            c.Hotels.Select(h => new GetHotelDto(
                h.Id,
                h.Name,
                h.Address,
                h.Rating,
                h.Country!.Name
                )).ToList()
            )).FirstOrDefaultAsync();
        }
        

        public async Task UpdateCountryAsync(int? countryId, UpdateCountryDto countryDto)
        {
            var country = await context.Countries.FindAsync(countryId);

            if (country == null)
            {
                throw new KeyNotFoundException($"Country with id {countryId} was not found.");
            }

            country.Name = countryDto.Name;
            country.ShortName = countryDto.ShortName;

            context.Entry(country).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExistsAsync(countryId))
                {
                    throw new KeyNotFoundException($"Country with id {countryId} was not found.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<GetCountryDto> CreateCountryAsync(CreateCountryDto countryDto)
        {
            var country = new Country
            {
                Name = countryDto.Name,
                ShortName = countryDto.ShortName
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            var createdCountryDto = new GetCountryDto(
                country.CountryId,
                country.Name,
                country.ShortName
            );

            return createdCountryDto;
        }


        public async Task DeleteCountryAsync(int? countryId)
        {
            var country = await context.Countries.FindAsync(countryId);
            if (country == null)
            {
                throw new KeyNotFoundException($"Country with id {countryId} was not found."); ;
            }

            context.Countries.Remove(country);
            await context.SaveChangesAsync();
        }


        public async Task<bool> CountryExistsAsync(int? countryId)
        {
            return await context.Countries.AnyAsync(e => e.CountryId == countryId);
        }


    }
}
