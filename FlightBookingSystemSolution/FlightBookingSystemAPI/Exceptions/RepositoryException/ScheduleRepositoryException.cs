using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.RepositoryException
{
    public class ScheduleRepositoryException : Exception
    {
        string msg = string.Empty;
        public ScheduleRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
            msg = message;
        }
        public override string Message => msg;
    }
}