namespace FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO
{
    public class RouteInfoReturnDTO
    {
        public int RouteId { get; set; }
        public string StartCity { get; set; }
        public string EndCity { get; set; }
        public float Distance { get; set; }
    }
}
