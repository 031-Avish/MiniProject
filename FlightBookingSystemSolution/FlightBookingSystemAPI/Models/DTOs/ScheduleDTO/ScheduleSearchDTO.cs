namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleSearchDTO
    {
        public string StartCity { get; set; }
        public string EndCity { get; set; }
        public DateTime Date { get; set; }
    }
}
