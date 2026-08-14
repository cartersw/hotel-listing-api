using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Contracts;
using HotelListing.Api.Services;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using HotelListing.Api.Common.Constants;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CountriesController(ICountryService countryService) : ApiControllerBase
{

    // GET: api/Country
    [HttpGet]
    
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountry()
    {
        var result = await countryService.GetCountriesAsync();

        return ToActionResult(result);
    }

    // GET: api/Country/5
    [HttpGet("{countryid}")]
    public async Task<ActionResult<GetCountryDetailsDto>> GetCountry(int countryId)
    {
        var result = await countryService.GetCountryAsync(countryId);

        return ToActionResult(result);
    }

    // PUT: api/Country/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{countryId}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult> PutCountry(int countryId, UpdateCountryDto updateDto)
    { 


        var result = await countryService.UpdateCountryAsync(countryId, updateDto);

        return ToActionResult(result);
    }

    // POST: api/Country
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult<GetCountryDetailsDto>> PostCountry(CreateCountryDto createDto)
    {
        var result = await countryService.CreateCountryAsync(createDto);

        if (!result.IsSuccess)
        {
            return MapErrorsToResponse(result.Errors);
        }

        return CreatedAtAction(nameof(GetCountry), new { countryId = result.Value!.CountryId }, result.Value); 
    }

    // DELETE: api/Country/5
    [HttpDelete("{countryId}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult> DeleteCountry(int? countryId) 
    { 
    
        var result = await countryService.DeleteCountryAsync(countryId);

        return ToActionResult(result);
    }

    
}
