namespace FlightBookingSystemAPI.Models.DTOs.PaymentDTOs
{
    public class PaymentReturnDTO
    {
        public int PaymentId { get; set; }
        public float Amount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public int BookingId { get; set; }
    }
}
