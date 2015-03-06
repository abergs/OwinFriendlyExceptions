using System.Collections.Generic;
using Owin;

namespace OwinFriendlyExceptions.Extensions
{
    public static class AppBuilderExtensions
    {
        public static void UseFriendlyExceptions(this IAppBuilder app, ITransformsCollection transforms,
            IEnumerable<IExceptionProvider> swallowedExceptionsProviders = null)
        {
            var options = new OwinFriendlyExceptionsParameters
            {
                SwallowedExceptionsProviders = swallowedExceptionsProviders ?? new List<IExceptionProvider>()
            };

            app.Use<OwinFriendlyExceptionsMiddleware>(transforms, options);
        }
    }
}