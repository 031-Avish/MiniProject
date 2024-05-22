using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }
        public string Name { get; set; }
        public int age { get; set; }
    }
}
