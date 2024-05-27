using FlightBookingSystemAPI.Models.DTOs.PassengerDTO;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;

namespace FlightBookingSystemAPI.Models.DTOs.BookingDTO
{
    public class BookingReturnDTO
    {
        public int BookingId { get; set; }
        public string BookingStatus { get; set; }
        public DateTime BookingDate { get; set; }
        public string PaymentStatus { get; set; }
        public float TotalPrice { get; set; }
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public ScheduleBookingDTO FlightDetails { get; set; }
        public List<PassengerReturnDTO> Passengers { get; set; }
    }
}
