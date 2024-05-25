using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.RepositoryException
{
    public class RouteInfoRepositoryException : Exception
    {
        string msg = string.Empty;
        public RouteInfoRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
            msg = message;
        }
        public override string Message => msg;
    }
}