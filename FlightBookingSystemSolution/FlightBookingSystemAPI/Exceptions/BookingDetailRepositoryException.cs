using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions
{
    public class BookingDetailRepositoryException : Exception
    {
        string msg = string.Empty;
        public BookingDetailRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
            msg = message;
        }
        public override string Message => msg;
    }
}