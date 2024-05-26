namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleReturnDTO
    {
        public int ScheduleId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ReachingTime { get; set; }
        public int AvailableSeat { get; set; }
        public float Price { get; set; }
        public int RouteId { get; set; }
        public int FlightId { get; set; }
    }
}
