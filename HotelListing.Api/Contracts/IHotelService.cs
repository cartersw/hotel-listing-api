using HotelListing.Api.DTOs.Hotel;

namespace HotelListing.Api.Contracts
{
    public interface IHotelService
    {
        Task<GetHotelDetailsDto> CreateHotelAsync(CreateHotelDto hotelDto);
        Task DeleteHotel(int id);
        Task<GetHotelDetailsDto?> GetHotelAsync(int hotelId);
        Task<IEnumerable<GetHotelDto>> GetHotelsAsync();
        bool HotelExists(int? id);
        Task UpdateHotelAsync(int id, UpdateHotelDto hotelDto);
    }
}