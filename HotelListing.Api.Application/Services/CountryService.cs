using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Country;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Extensions;
using HotelListing.Api.Common.Models.Cache;
using HotelListing.Api.Common.Models.Filtering;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.Metrics;
namespace HotelListing.Api.Application.Services
{
    public class CountryService(HotelListingDbContext context, IMemoryCache cache) : ICountryService
    {
        public async Task<Result<IEnumerable<GetCountryDto>>> GetCountriesAsync(CountryFilterParameters filters)
        {
            var cacheSearchQuery = string.Empty;

            var query = context.Countries.AsQueryable();


            if (filters.HasHotels.HasValue)
            {
                if (filters.HasHotels.Value)
                {
                    query = query.Where(c => c.Hotels.Count >= 1);
                }
                else
                {
                    query = query.Where(c => c.Hotels.Count == 0);
                }

                cacheSearchQuery += filters.HasHotels.Value.ToString();
            }

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(c => c.Name.Contains(filters.Search) || c.ShortName.Contains(filters.Search));
                cacheSearchQuery += filters.Search.Trim().ToLowerInvariant();
            }

            var cacheKey = CacheKeyNames.GetCountriesAsyncKey + cacheSearchQuery;

            if(!cache.TryGetValue(cacheKey, out IEnumerable<GetCountryDto>? countries))
            {
                countries = await query
                    .AsNoTracking()
                    .Select(c => new GetCountryDto(
                    c.CountryId,
                    c.Name,
                    c.ShortName
                    )).ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

                cache.Set(cacheKey, countries, cacheOptions);
            }

            countries ??= [];
            return Result<IEnumerable<GetCountryDto>>.Success(countries);
        }

        public async Task<Result<GetCountryDetailsDto>> GetCountryAsync(int countryId)
        {
            var cacheKey = CacheKeyNames.GetCountryAsyncKey + countryId;

            if(!cache.TryGetValue(cacheKey, out GetCountryDetailsDto? country))
            {
                country = await context.Countries
                    .AsNoTracking()
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

                if(country != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    cache.Set(cacheKey, country, cacheOptions);
                }
            }
            
            return country != null ? Result<GetCountryDetailsDto>.Success(country) : Result<GetCountryDetailsDto>.NotFound();
        }

        public async Task<Result<PagedResult<GetHotelDetailsDto>>> GetCountryHotelsAsync(int countryId, 
            PaginationParameters paginationParameters)
        {
            var hotels = await context.Hotels
                .AsNoTracking()
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

        public async Task<Result> PatchCountryAsync(int countryId, JsonPatchDocument<UpdateCountryDto> patchDoc)
        {
            var country = await context.Countries.FindAsync(countryId);
            if (country == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, "Country with Id" + countryId + "was not found"));
            }

            var countryDto = new UpdateCountryDto
            {
                CountryId = country.CountryId,
                Name = country.Name,
                ShortName = country.ShortName
            };

            patchDoc.ApplyTo(countryDto);

            if (countryDto.CountryId != countryId)
            {
                return Result.Failure(new Error(ErrorCodes.Validation, "Cannot modify Id field"));
            }

            var duplicateExists = await context.Countries
                .AnyAsync(c => c.Name.ToLower().Trim() == countryDto.Name.ToLower().Trim()
                && c.CountryId != countryId);

            if (duplicateExists)
            {
                return Result.Failure(new Error(ErrorCodes.Conflict, "Country with name" + countryDto.Name + "already exists"));
            }

            country.Name = countryDto.Name;
            country.ShortName = countryDto.ShortName;

            await context.SaveChangesAsync();

            return Result.Success();
        }


        public async Task<bool> CountryExistsAsync(int? countryId)
        {
            return await context.Countries
                .AsNoTracking()
                .AnyAsync(e => e.CountryId == countryId);
        }

        public async Task<bool> CountryExistsAsync(string name)
        {
            return await context.Countries
                .AsNoTracking()
                .AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        
    }
}
