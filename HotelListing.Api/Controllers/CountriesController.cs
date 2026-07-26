using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Contracts;
using HotelListing.Api.Services;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(ICountriesService countriesService) : ControllerBase
{

    // GET: api/Country
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountry()
    {
        var resultDto = await countriesService.GetCountriesAsync();

        return Ok(resultDto);
    }

    // GET: api/Country/5
    [HttpGet("{countryid}")]
    public async Task<ActionResult<GetCountryDetailsDto>> GetCountry(int countryId)
    {
        var resultDto = await countriesService.GetCountryAsync(countryId);

        if (resultDto == null)
        {
            return NotFound();
        }

        return Ok(resultDto);
    }

    // PUT: api/Country/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{countryId}")]
    public async Task<IActionResult> PutCountry(int? countryId, UpdateCountryDto updateDto)
    {
        if (countryId != updateDto.CountryId)
        {
            return BadRequest();
        }

        await countriesService.UpdateCountryAsync(countryId, updateDto);

        return NoContent();
    }

    // POST: api/Country
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GetCountryDetailsDto>> PostCountry(CreateCountryDto createDto)
    {
        var resultDto = await countriesService.CreateCountryAsync(createDto);

        return CreatedAtAction(nameof(GetCountry), new { countryId = resultDto.CountryId }, resultDto); 
    }

    // DELETE: api/Country/5
    [HttpDelete("{countryId}")]
    public async Task<IActionResult> DeleteCountry(int? countryId)
    {

        await countriesService.DeleteCountryAsync(countryId);

        return NoContent();
    }

    
}
