using System;
using System.Collections.Generic;

namespace OwinFriendlyExceptions
{
    public interface IExceptionProvider
    {
        Exception GetException(IDictionary<string, object> environment);
    }
}