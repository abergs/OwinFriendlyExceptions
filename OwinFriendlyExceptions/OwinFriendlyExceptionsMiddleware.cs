using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OwinFriendlyExceptions
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class OwinFriendlyExceptionsMiddleware
    {
        private readonly AppFunc _next;
        private readonly OwinFriendlyExceptionsParameters _parameters;
        private readonly ITransformsCollection _transformsCollection;

        public OwinFriendlyExceptionsMiddleware(AppFunc next, ITransformsCollection transformsCollection)
            : this(next, transformsCollection, DefaultProperties())
        {
        }

        public OwinFriendlyExceptionsMiddleware(AppFunc next, ITransformsCollection transformsCollection,
            OwinFriendlyExceptionsParameters parameters)
        {
            _next = next;
            _transformsCollection = transformsCollection;
            _parameters = parameters;
        }

        private static OwinFriendlyExceptionsParameters DefaultProperties()
        {
            return new OwinFriendlyExceptionsParameters();
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);

            Exception exception = null;
            ITransform transformer = null;

            try
            {
                await _next.Invoke(environment);
                exception = GetSwallowedException(context);

                if (exception != null)
                {
                    transformer = _transformsCollection.FindTransform(exception);
                }
            }
            catch (Exception catchedException)
            {
                exception = catchedException;

                // check if we can transform it, otherwise we should throw it
                transformer = _transformsCollection.FindTransform(exception);
                if (transformer == null)
                {
                    throw;
                }
            }

            if (transformer != null)
            {
                TransformException(context, transformer, exception);
            }
        }

        private void TransformException(OwinContext context, ITransform transform, Exception ex)
        {
            string content = transform.GetContent(ex);

            context.Response.ContentType = transform.ContentType;
            context.Response.StatusCode = (int) transform.StatusCode;
            context.Response.ReasonPhrase = transform.ReasonPhrase;
	    context.Response.ContentLength = Encoding.UTF8.GetByteCount(content);
            context.Response.Write(content);
        }

        private Exception GetSwallowedException(IOwinContext context)
        {
            Exception swallowedException =
                _parameters.SwallowedExceptionsProviders.Select(provider => provider.GetException(context.Environment))
                    .FirstOrDefault(e => e != null);
          
            return swallowedException;
        }
    }
}
