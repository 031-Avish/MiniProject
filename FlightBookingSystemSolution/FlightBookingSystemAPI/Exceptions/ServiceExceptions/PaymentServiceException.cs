﻿using System.Runtime.Serialization;

namespace FlightBookingSystemAPI.Exceptions.ServiceExceptions
{
    [Serializable]
    public class PaymentServiceException : Exception
    {
        public PaymentServiceException()
        {
        }

        public PaymentServiceException(string? message) : base(message)
        {
        }

        public PaymentServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PaymentServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}