using FlightBookingSystemAPI.Models.DTOs.PassengerDTO;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.BookingDTO
{
    public class BookingReturnDTO
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Booking status is required")]
        public string BookingStatus { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Payment status is required")]
        public string PaymentStatus { get; set; }

        [Required(ErrorMessage = "Total price is required")]
        public float TotalPrice { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Schedule ID is required")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Flight details are required")]
        public ScheduleBookingDTO FlightDetails { get; set; }

        [Required(ErrorMessage = "Passengers information is required")]
        public List<PassengerReturnDTO> Passengers { get; set; }
    }
}
