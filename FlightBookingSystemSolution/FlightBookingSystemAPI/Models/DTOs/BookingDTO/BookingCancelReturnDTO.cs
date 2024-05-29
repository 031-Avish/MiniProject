namespace FlightBookingSystemAPI.Models.DTOs.BookingDTO
{
    public class BookingCancelReturnDTO
    {
        public int BookingId { get; set; }
        public float RefundAmount { get; set; }
        public string Message { get; set; }
    }
}
