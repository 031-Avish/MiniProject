﻿using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.ServiceExceptions
{
    [Serializable]
    public class ScheduleServiceException : Exception
    {
        public ScheduleServiceException()
        {
        }

        public ScheduleServiceException(string? message) : base(message)
        {
        }

        public ScheduleServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ScheduleServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}