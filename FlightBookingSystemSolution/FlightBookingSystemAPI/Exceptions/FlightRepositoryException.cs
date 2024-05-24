using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions
{
    public class FlightRepositoryException : Exception
    {
        string msg = string.Empty;
        public FlightRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
            msg = message;
        }
        public override string Message => msg;
    }
}