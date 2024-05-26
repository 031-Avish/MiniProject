using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;

namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleDetailDTO
    {
        public int ScheduleId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ReachingTime { get; set; }
        public int AvailableSeat { get; set; }
        public float Price { get; set; }
        public RouteInfoReturnDTO RouteInfo { get; set; }
        public FlightReturnDTO FlightInfo { get; set; }
    }
}
