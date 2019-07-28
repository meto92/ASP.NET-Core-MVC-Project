using System;

namespace Metomarket.Services
{
    public class ServiceException : Exception
    {
        private const string DefaultMessage = "Something went wrong.";

        public ServiceException(string message = DefaultMessage)
            : base(message)
        {
        }
    }
}