using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Models.Filtering;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HotelsController : ApiControllerBase
{
    private readonly IHotelService _hotelService;
    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    // GET: api/Hotel
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetHotelDto>>> GetHotels([FromQuery] PaginationParameters paginationParameters,
        [FromQuery] HotelFilterParameters hotelFilterParameters)
    {
        var result = await _hotelService.GetHotelsAsync(paginationParameters, hotelFilterParameters);


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
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        var result = await _hotelService.UpdateHotelAsync(id, hotelDto);

        return ToActionResult(result);
    }

    // POST: api/Hotel
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = RoleNames.Administrator)]
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
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var result = await _hotelService.DeleteHotel(id);

        return ToActionResult(result);
    }


    [HttpPost("{hotelId:int}/admins")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> AddHotelAdmin([FromRoute] int hotelId, AddHotelAdminDto addHotelAdminDto)
    {
        var result = await _hotelService.AddHotelAdminAsync(hotelId, addHotelAdminDto);

        return ToActionResult(result);
    }

}
