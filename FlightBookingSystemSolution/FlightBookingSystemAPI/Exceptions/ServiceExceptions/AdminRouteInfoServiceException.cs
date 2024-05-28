using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.ServiceExceptions
{
    [Serializable]
    internal class AdminRouteInfoServiceException : Exception
    {
        public AdminRouteInfoServiceException()
        {
        }

        public AdminRouteInfoServiceException(string? message) : base(message)
        {
        }

        public AdminRouteInfoServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AdminRouteInfoServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}