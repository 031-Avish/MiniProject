using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingSystemAPI.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ReachingTime { get; set; }
        [Required]
        public int AvailableSeat {  get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public int RouteId { get; set; }
        [ForeignKey("RouteId")]
        public RouteInfo RouteInfo { get; set; }
        [Required]
        public int FlightId { get; set; }
        [ForeignKey("FlightId")]
        public Flight FlightInfo { get; set; }  
    }
}
