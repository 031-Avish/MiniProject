using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.ServiceExceptions
{
    [Serializable]
    public  class BookingServiceException : Exception
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