using System;

namespace OwinFriendlyExceptions
{
    public interface ITransformsMap
    {
        ITransformTo<T> Map<T>() where T : Exception;
        ITransformTo<Exception> Map(Func<Exception, bool> matcher);
        ITransformsCollection Done();
    }
}