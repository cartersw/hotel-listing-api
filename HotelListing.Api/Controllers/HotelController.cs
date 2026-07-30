using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Contracts;
using HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelController : ApiControllerBase
{
    private readonly IHotelService _hotelService;
    public HotelController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    // GET: api/Hotel
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotels()
    {
        var result = await _hotelService.GetHotelsAsync();


        return ToActionResult(result);

    }

    // GET: api/Hotel/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetHotelDetailsDto>> GetHotel(int id)
    {
        var result = await _hotelService.GetHotelAsync(id);


        return ToActionResult(result);
    }

    // PUT: api/Hotel/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        var result = await _hotelService.UpdateHotelAsync(id, hotelDto);

        return ToActionResult(result);
    }

    // POST: api/Hotel
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GetHotelDetailsDto>> PostHotel(CreateHotelDto hotelDto)
    {

        var result = await _hotelService.CreateHotelAsync(hotelDto);

        if (!result.IsSuccess)
        {
            return MapErrorsToResponse(result.Errors);
        }


        return CreatedAtAction("GetHotel", new { id = result.Value!.Id }, result.Value);
    }

    // DELETE: api/Hotel/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var result = await _hotelService.DeleteHotel(id);

        return ToActionResult(result);
    }

}
