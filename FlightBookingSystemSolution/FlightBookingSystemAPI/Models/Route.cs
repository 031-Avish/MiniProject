using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models
{
    public class Route
    {
        [Key]
        public int RouteId { get; set; }
        public string StartCity { get; set; }
        public string EndCity { get; set; } 
        public float Distance { get; set; }
    }
}
