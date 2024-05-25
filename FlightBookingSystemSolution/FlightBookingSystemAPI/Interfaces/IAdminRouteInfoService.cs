using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IAdminRouteInfoService
    {
        Task<RouteInfoReturnDTO> AddRouteInfo(RouteInfoDTO routeInfoDTO);
        Task<RouteInfoReturnDTO> DeleteRouteInfo(int routeInfoId);
        Task<List<RouteInfoReturnDTO>> GetAllRouteInfos();
        Task<RouteInfoReturnDTO> GetRouteInfo(int routeInfoId);
        Task<RouteInfoReturnDTO> UpdateRouteInfo(RouteInfoReturnDTO routeInfoReturnDTO);
    }
}
