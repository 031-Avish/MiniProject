using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingSystemAPI.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ReachingTime { get; set; }  
        public int AvailableSeat {  get; set; } 
        public float Price { get; set; }
        public int RouteId { get; set; }
        [ForeignKey("RouteId")]
        public RouteInfo RouteInfo { get; set; }
        public int FlightId { get; set; }
        [ForeignKey("FlightId")]
        public Flight FlightInfo { get; set; }  
    }
}
