using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(HotelListingDbContext context) : ControllerBase
{

    // GET: api/Country
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountry()
    {
        var countries = await context.Countries.Select(c => new GetCountryDto(
            c.CountryId,
            c.Name,
            c.ShortName
        )).ToListAsync();

        return countries;
    }

    // GET: api/Country/5
    [HttpGet("{countryid}")]
    public async Task<ActionResult<GetCountryDetailsDto>> GetCountry(int countryid)
    {
        var country = await context.Countries
            .Where(c => c.CountryId == countryid)
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

        if (country == null)
        {
            return NotFound();
        }

        return country;
    }

    // PUT: api/Country/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{countryid}")]
    public async Task<IActionResult> PutCountry(int? countryid, UpdateCountryDto countryDto)
    {
        if (countryid != countryDto.CountryId)
        {
            return BadRequest();
        }

        var country = await context.Countries.FindAsync(countryid);

        if(country == null)
        {
            return NotFound();
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
            if (! await CountryExistsAsync(countryid))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Country
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto countryDto)
    {
        var country = new Country
        {
            Name = countryDto.Name,
            ShortName = countryDto.ShortName
        };
    
        context.Countries.Add(country);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetCountry", new { countryid = country.CountryId }, country);
    }

    // DELETE: api/Country/5
    [HttpDelete("{countryid}")]
    public async Task<IActionResult> DeleteCountry(int? countryid)
    {
        var country = await context.Countries.FindAsync(countryid);
        if (country == null)
        {
            return NotFound();
        }

        context.Countries.Remove(country);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> CountryExistsAsync(int? countryid)
    {
        return await context.Countries.AnyAsync(e => e.CountryId == countryid);
    }
}
