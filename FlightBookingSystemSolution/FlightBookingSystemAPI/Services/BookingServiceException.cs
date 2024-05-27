using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Services
{
    [Serializable]
    internal class BookingServiceException : Exception
    {
        public BookingServiceException()
        {
        }

        public BookingServiceException(string? message) : base(message)
        {
        }

        public BookingServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BookingServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}