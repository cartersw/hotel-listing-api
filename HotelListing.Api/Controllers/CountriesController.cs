using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Application.DTOs.Country;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Common.Models.Filtering;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.OutputCaching;


[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("fixed")]

public class CountriesController(ICountryService countryService) : ApiControllerBase
{

    // GET: api/Country
    [HttpGet]
    [OutputCache]
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountry([FromQuery] CountryFilterParameters countryFilterParameters)
    {
        var result = await countryService.GetCountriesAsync(countryFilterParameters);

        return ToActionResult(result);
    }

    // GET: api/Country/5
    
    [HttpGet("{countryid}")]
    [OutputCache]
    public async Task<ActionResult<GetCountryDetailsDto>> GetCountry(int countryId)
    {
        var result = await countryService.GetCountryAsync(countryId);

        return ToActionResult(result);
    }

    
    [HttpGet("{countryid}/hotels")]
    [Authorize]
    public async Task<ActionResult<PagedResult<GetHotelDetailsDto>>> GetCountryHotels(int countryId, 
        [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await countryService.GetCountryHotelsAsync(countryId, paginationParameters);

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

    [HttpPatch("{countryId}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> PatchCountry(int countryId, [FromBody] JsonPatchDocument<UpdateCountryDto> patchDoc)
    {
        if(patchDoc == null)
        {
            return BadRequest("Patch document is required");
        }
        
        var result = await countryService.PatchCountryAsync(countryId, patchDoc);
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
