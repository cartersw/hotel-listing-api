using HotelListing.Api.Common.Results;
using HotelListing.Api.DTOs.Hotel;

namespace HotelListing.Api.Contracts
{
    public interface IHotelService
    {
        Task<Result> AddHotelAdminAsync(int hotelId, AddHotelAdminDto addHotelAdminDto);
        Task<Result<GetHotelDetailsDto>> CreateHotelAsync(CreateHotelDto hotelDto);
        Task<Result> DeleteHotel(int id);
        Task<Result<GetHotelDetailsDto>> GetHotelAsync(int id);
        Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync();
        bool HotelExists(int id);
        Task<Result> UpdateHotelAsync(int id, UpdateHotelDto hotelDto);
    }
}