using System;

namespace OwinFriendlyExceptions
{
    public interface ITransformsCollection
    {
        ITransform FindTransform(Exception exception);
    }
}