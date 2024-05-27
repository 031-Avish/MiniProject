snamespace FlightBookingSystemAPI.Models.DTOs.BookingDTO
{
	public class BookingDTO
	{
		public int ScheduleId { get; set; }
		public int UserId { get; set; }
		public List<PassengerDTO> Passengers { get; set; }
	}
}
