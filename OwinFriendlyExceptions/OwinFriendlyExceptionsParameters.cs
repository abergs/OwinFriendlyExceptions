using System;
using System.Collections.Generic;

namespace OwinFriendlyExceptions
{
    public class OwinFriendlyExceptionsParameters
    {
        public OwinFriendlyExceptionsParameters()
        {
            SwallowedExceptionsProviders = new List<IExceptionProvider>();
        }

        public IEnumerable<IExceptionProvider> SwallowedExceptionsProviders { get; set; }
    }

    public interface IExceptionProvider
    {
        Exception GetException(IDictionary<string, object> environment);
    }
}