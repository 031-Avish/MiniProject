﻿using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Repositories
{
    [Serializable]
    internal class UserDetailRepositoryException : Exception
    {
        public UserDetailRepositoryException()
        {
        }

        public UserDetailRepositoryException(string? message) : base(message)
        {
        }

        public UserDetailRepositoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserDetailRepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}