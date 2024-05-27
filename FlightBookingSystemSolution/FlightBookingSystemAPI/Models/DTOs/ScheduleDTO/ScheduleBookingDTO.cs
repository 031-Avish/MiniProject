namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleBookingDTO
    {
        public DateTime DepartureTime { get; set; }
        public DateTime ReachingTime { get; set; }
        public int RouteId { get; set; }
        public int FlightId { get; set; }
    }
}
