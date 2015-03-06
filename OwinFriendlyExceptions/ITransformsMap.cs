using System;

namespace OwinFriendlyExceptions
{
    public interface ITransformsMap
    {
        ITransformTo<T> Map<T>() where T : Exception;
        ITransformsCollection Done();
    }
}