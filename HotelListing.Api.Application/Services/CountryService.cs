using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Country;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Common.Extensions;
using HotelListing.Api.Common.Models.Filtering;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
namespace HotelListing.Api.Application.Services
{
    public class CountryService(HotelListingDbContext context) : ICountryService
    {
        public async Task<Result<IEnumerable<GetCountryDto>>> GetCountriesAsync(CountryFilterParameters filters)
        {

            var query = context.Countries.AsQueryable();


            if (filters.HasHotels!.Value)
            {
                query = query.Where(c => c.Hotels.Count >= 1);
            }

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(c => c.Name.Contains(filters.Search) || c.ShortName.Contains(filters.Search));
            }

            var countries = await query.Select(c => new GetCountryDto(
                c.CountryId,
                c.Name,
                c.ShortName
            )).ToListAsync();

            return Result<IEnumerable<GetCountryDto>>.Success(countries);
        }

        public async Task<Result<GetCountryDetailsDto>> GetCountryAsync(int countryId)
        {
            var country = await context.Countries
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
            
            return country != null ? Result<GetCountryDetailsDto>.Success(country) : Result<GetCountryDetailsDto>.NotFound();
        }

        public async Task<Result<PagedResult<GetHotelDetailsDto>>> GetCountryHotelsAsync(int countryId, 
            PaginationParameters paginationParameters)
        {
            var hotels = await context.Hotels
                .Where(h => h.CountryId == countryId)
                .Select(h => new GetHotelDetailsDto(
                h.Id,
                h.Name,
                h.Address,
                h.Rating,
                h.CountryId
            )).ToPagedResultAsync(paginationParameters);

            return Result<PagedResult<GetHotelDetailsDto>>.Success(hotels);
        }


        public async Task<Result> UpdateCountryAsync(int countryId, UpdateCountryDto countryDto)
        {
            if (countryId != countryDto.CountryId)
            {
                return Result.BadRequest(new Error("Validation", "Id route value does not match payload Id."));
            }

            var country = await context.Countries.FindAsync(countryId);

            if (country == null)
            {
                return Result.NotFound();
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
                    return Result.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Result.Success();
        }

        public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto)
        {

            if (await CountryExistsAsync(countryDto.Name))
            {
                return Result<GetCountryDto>.Failure(new Error("Conflict", "Country with name" + countryDto.Name));
            }

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

            return Result<GetCountryDto>.Success(createdCountryDto); 
        }


        public async Task<Result> DeleteCountryAsync(int? countryId)
        {
            var country = await context.Countries.FindAsync(countryId);
            if (country == null)
            {
                return Result.NotFound();
            }

            context.Countries.Remove(country);
            await context.SaveChangesAsync();
            return Result.Success();
        }


        public async Task<bool> CountryExistsAsync(int? countryId)
        {
            return await context.Countries.AnyAsync(e => e.CountryId == countryId);
        }

        public async Task<bool> CountryExistsAsync(string name)
        {
            return await context.Countries.AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        
    }
}
