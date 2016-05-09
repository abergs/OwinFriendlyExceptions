using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;

namespace OwinFriendlyExceptions
{
    public class TransformsCollectionBuilder : ITransformsMap, ITransformsCollection
    {
        private readonly List<ITransform> _transforms = new List<ITransform>();

        private TransformsCollectionBuilder()
        {
        }

        public ITransform FindTransform(Exception exception)
        {
            ITransform handler = _transforms.FirstOrDefault(x => x.CanHandle(exception));
            return handler;
        }

        public ITransformTo<T> Map<T>() where T : Exception
        {
            var transform = new Transform<T>(this);
            return transform;
        }

        public ITransformTo<Exception> Map(Func<Exception, bool> matching)
        {
            var transform = new Transform<Exception>(this, matching);
            return transform;
        }

        public ITransformsCollection Done()
        {
            return this;
        }

        public static ITransformsMap Begin()
        {
            return new TransformsCollectionBuilder();
        }


        private class Transform<T> : ITransformTo<T>, ITransform where T : Exception
        {
            private readonly TransformsCollectionBuilder _transformsCollectionBuilder;
            private Func<T, string> _contentGenerator;

            private Func<Exception, bool> matcher;

            public Transform(TransformsCollectionBuilder transformsCollectionBuilder)
                : this(transformsCollectionBuilder, (ex) => ex.GetType() == typeof(T))
            {
            }

            public Transform(TransformsCollectionBuilder transformsCollectionBuilder, Func<Exception, bool> matching)
            {
                this._transformsCollectionBuilder = transformsCollectionBuilder;
                this.matcher = matching;
            }

            public string GetContent(Exception ex2)
            {
                var ex = (T)ex2;
                return _contentGenerator(ex);
            }

            public bool CanHandle<T2>(T2 ex) where T2 : Exception
            {
                //return ex.GetType() == typeof(T);
                return matcher(ex);

            }

            public string ContentType { get; private set; }

            public HttpStatusCode StatusCode { get; private set; }
            public string ReasonPhrase { get; private set; }

            public ITransformsMap To(HttpStatusCode statusCode, string reasonPhrase, Func<T, string> contentGenerator, string contentType = "text/plain")
            {
                StatusCode = statusCode;
                ReasonPhrase = reasonPhrase;
                ContentType = contentType;
                _contentGenerator = contentGenerator;
                _transformsCollectionBuilder._transforms.Add(this);
                return _transformsCollectionBuilder;
            }
        }
    }
}