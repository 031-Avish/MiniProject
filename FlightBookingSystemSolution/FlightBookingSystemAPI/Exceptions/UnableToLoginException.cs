using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions
{
    [Serializable]
    internal class UnableToLoginException : Exception
    {
        public UnableToLoginException()
        {
        }

        public UnableToLoginException(string? message) : base(message)
        {
        }

        public UnableToLoginException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}