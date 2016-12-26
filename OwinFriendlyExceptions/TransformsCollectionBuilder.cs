using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

        public ITransformTo<Exception> MapOthers()
        {
            return Map<Exception>();
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

            private readonly Func<Exception, bool> _matcher;

            public Transform(TransformsCollectionBuilder transformsCollectionBuilder)
                : this(transformsCollectionBuilder, (ex) => ex.GetType() == typeof(T))
            {
            }

            public Transform(TransformsCollectionBuilder transformsCollectionBuilder, Func<Exception, bool> matching)
            {
                _transformsCollectionBuilder = transformsCollectionBuilder;
                _matcher = matching;
            }

            public string GetContent(Exception ex2)
            {
                T ex = (T)ex2;
                return _contentGenerator(ex);
            }

            public bool CanHandle<T2>(T2 ex) where T2 : Exception
            {
                var result = _matcher(ex);
                if (!result) // Made possible Other Exceptions handling if delegate was defined
                    result = _matcher(new Exception());
                return result;
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