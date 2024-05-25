using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.RepositoryException
{
    public class BookingRepositoryException : Exception
    {
        string msg = string.Empty;
        public BookingRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
            msg = message;
        }
        public override string Message => msg;
    }
}