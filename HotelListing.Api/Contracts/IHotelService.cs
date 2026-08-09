using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts
{
    public interface IHotelService
    {
        Task<Result> AddHotelAdminAsync(int hotelId, string userId);
        Task<Result<GetHotelDetailsDto>> CreateHotelAsync(CreateHotelDto hotelDto);
        Task<Result> DeleteHotel(int id);
        Task<Result<GetHotelDetailsDto>> GetHotelAsync(int id);
        Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync();
        bool HotelExists(int id);
        Task<Result> UpdateHotelAsync(int id, UpdateHotelDto hotelDto);
    }
}