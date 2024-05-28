using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO
{
    public class RouteInfoReturnDTO
    {
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Start city is required")]
        public string StartCity { get; set; }

        [Required(ErrorMessage = "End city is required")]
        public string EndCity { get; set; }

        [Required(ErrorMessage = "Distance is required")]
        public float Distance { get; set; }
    }
}
