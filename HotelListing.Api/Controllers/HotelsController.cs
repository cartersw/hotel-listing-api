using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly HotelListingDbContext _context;
    public HotelsController(HotelListingDbContext context)
    {
        _context = context;
    }

    // GET: api/Hotel
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetHotel()
    {

        return await _context.Hotels
            //.Include(h => h.Country)
            .ToListAsync();
    }

    // GET: api/Hotel/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetHotel(int id)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Country)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
        {
            return NotFound();
        }

        return hotel;
    }

    // PUT: api/Hotel/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int? id, Hotel hotel)
    {
        if (id != hotel.Id)
        {
            return BadRequest();
        }

        _context.Entry(hotel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HotelExists(id))
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

    // POST: api/Hotel
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
    {
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    // DELETE: api/Hotel/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int? id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool HotelExists(int? id)
    {
        return _context.Hotels.Any(e => e.Id == id);
    }
}
