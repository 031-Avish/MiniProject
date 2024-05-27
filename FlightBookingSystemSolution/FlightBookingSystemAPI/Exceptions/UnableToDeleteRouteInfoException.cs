using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions
{
    [Serializable]
    internal class UnableToDeleteRouteInfoException : Exception
    {
        public UnableToDeleteRouteInfoException()
        {
        }

        public UnableToDeleteRouteInfoException(string? message) : base(message)
        {
        }

        public UnableToDeleteRouteInfoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToDeleteRouteInfoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}