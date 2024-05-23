using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Repositories
{ 
    public class UserRepositoryException : Exception
    {
        string msg = string.Empty; 
        public UserRepositoryException(string message, Exception innerException): base(message, innerException)
        {
            msg=message;
        }
        public override string Message => msg;
    }
}