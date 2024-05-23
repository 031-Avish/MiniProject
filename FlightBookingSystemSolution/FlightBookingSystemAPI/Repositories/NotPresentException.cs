using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Repositories
{
    public class NotPresentException : Exception
    {
        string msg=string.Empty;
        public NotPresentException(string message) 
        {
            msg = message;
        }
        public override string Message => msg;
    }
}