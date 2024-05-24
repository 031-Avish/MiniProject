using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions
{
    public class PassengerRepositoryException : Exception
    {
        string msg = string.Empty;
        public PassengerRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
            msg = message;
        }
        public override string Message => msg;
    }
}