using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.ServiceExceptions
{
    [Serializable]
    public class AdminFlightServiceException : Exception
    {
        public AdminFlightServiceException()
        {
        }

        public AdminFlightServiceException(string? message) : base(message)
        {
        }

        public AdminFlightServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AdminFlightServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}