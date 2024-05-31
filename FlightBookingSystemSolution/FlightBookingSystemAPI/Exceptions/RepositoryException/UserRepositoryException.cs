using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.RepositoryException
{
    public class UserServiceException : Exception
    {
        string msg = string.Empty;
        public UserServiceException(string message, Exception innerException) : base(message, innerException)
        {
            msg = message;
        }
        public override string Message => msg;
    }
}