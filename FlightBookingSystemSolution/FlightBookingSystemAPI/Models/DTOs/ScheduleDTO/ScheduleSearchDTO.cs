using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleSearchDTO
    {
        [Required(ErrorMessage = "Start city is required")]
        public string StartCity { get; set; }

        [Required(ErrorMessage = "End city is required")]
        public string EndCity { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateOnly Date { get; set; }
    }
}
