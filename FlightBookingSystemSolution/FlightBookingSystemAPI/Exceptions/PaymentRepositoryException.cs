using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions
{
    public class PaymentRepositoryException : Exception
    {
        string msg = string.Empty;
        public PaymentRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
            msg = message;
        }
        public override string Message => msg;
    }
}