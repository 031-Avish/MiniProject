using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models
{
    public class RouteInfo
    {
        [Key]
        public int RouteId { get; set; }
        public string StartCity { get; set; }
        public string EndCity { get; set; } 
        public float Distance { get; set; }
    }
}
