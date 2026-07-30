using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Contracts;
using HotelListing.Api.Services;
using HotelListing.Api.Results;

[Route("api/[controller]")]
[ApiController]
public class CountryController(ICountryService countryService) : ControllerBase
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
    public async Task<IActionResult> PutCountry(int countryId, UpdateCountryDto updateDto)
    { 
        var result = await countryService.UpdateCountryAsync(countryId, updateDto);

        return ToActionResult(result);
    }

    // POST: api/Country
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
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
    public async Task<ActionResult> DeleteCountry(int? countryId) 
    { 
    
        var result = await countryService.DeleteCountryAsync(countryId);

        return ToActionResult(result);
    }

    private ActionResult<T> ToActionResult<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : MapErrorsToResponse(result.Errors);

    private ActionResult ToActionResult(Result result) =>
        result.IsSuccess ? NoContent() : MapErrorsToResponse(result.Errors);

    private ActionResult MapErrorsToResponse(Error[] errors)
    {
        if (errors == null || errors.Length == 0) return Problem();

        var e = errors[0];

        return e.Code switch
        {
            "NotFound" => NotFound(e.Description),
            "BadRequest" => BadRequest(e.Description),
            "Validation" => BadRequest(e.Description),
            "Conflict" => Conflict(e.Description),
            _ => Problem(detail: string.Join(": ", errors.Select(x => x.Description)), title: e.Code)
        };
    }
    
}
