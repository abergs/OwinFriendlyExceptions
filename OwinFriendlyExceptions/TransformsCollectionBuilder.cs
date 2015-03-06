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
            private readonly TransformsCollectionBuilder _transformsCollectionBuilderBuilder;
            private Func<T, string> _contentGenerator;

            private string _reasonPhras;
            private HttpStatusCode _statusCode;

            public Transform(TransformsCollectionBuilder transformsCollectionBuilderBuilder)
            {
                _transformsCollectionBuilderBuilder = transformsCollectionBuilderBuilder;
            }

            public string GetContent(Exception ex2)
            {
                var ex = (T) ex2;
                return _contentGenerator(ex);
            }

            public bool CanHandle<T2>(T2 ex)
            {
                return ex.GetType() == typeof (T);
            }

            public HttpStatusCode StatusCode { get; private set; }
            public string ReasonPhrase { get; private set; }

            public ITransformsMap To(HttpStatusCode statusCode, string reasonPhrase, Func<T, string> contentGenerator)
            {
                StatusCode = statusCode;
                ReasonPhrase = reasonPhrase;
                _contentGenerator = contentGenerator;
                _transformsCollectionBuilderBuilder._transforms.Add(this);
                return _transformsCollectionBuilderBuilder;
            }
        }
    }
}