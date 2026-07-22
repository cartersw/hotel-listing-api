using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(HotelListingDbContext context) : ControllerBase
{

    // GET: api/Country
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> GetCountry()
    {
        var countries = await context.Countries.ToListAsync();
        return countries;
    }

    // GET: api/Country/5
    [HttpGet("{countryid}")]
    public async Task<ActionResult<Country>> GetCountry(int countryid)
    {
        var country = await context.Countries
            .Include(c => c.Hotels)
            .FirstOrDefaultAsync(c => c.CountryId == countryid);

        if (country == null)
        {
            return NotFound();
        }

        return country;
    }

    // PUT: api/Country/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{countryid}")]
    public async Task<IActionResult> PutCountry(int? countryid, Country country)
    {
        if (countryid != country.CountryId)
        {
            return BadRequest();
        }

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
    public async Task<ActionResult<Country>> PostCountry(Country country)
    {
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
