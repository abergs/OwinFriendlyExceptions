using System;
using System.Net;
using NUnit.Framework;
using OwinFriendlyExceptions;

namespace OwinFriendlyExceptionsTests
{
    [TestFixture]
    public class Tests
    {

        [Test]
        public void LambdaMatcherMatches()
        {
            var builder = TransformsCollectionBuilder.Begin()
                .Map(ex => ex.Message == "ok")
                .To(HttpStatusCode.OK, "Success", (ex) => ex.Message);

            var coll = builder.Done();

            var testException1 = new Exception("fail");
            var testException2 = new Exception("ok");

            Assert.IsNull(coll.FindTransform(testException1));
            Assert.IsNotNull(coll.FindTransform(testException2));
        }

        [Test]
        public void LambdaMatcherMatchesDerived()
        {
            var builder = TransformsCollectionBuilder.Begin()
                .Map(ex => ex.GetType().IsSubclassOf(typeof(Exception)))
                .To(HttpStatusCode.OK, "Success", (ex) => ex.Message);

            var coll = builder.Done();

            var testException = new ArgumentException("ok");

            Assert.IsNotNull(coll.FindTransform(testException));
        }

        [Test]
        public void ContentTypeDefaultsToTextPlain()
        {
            var builder = TransformsCollectionBuilder.Begin()
                .Map(ex => ex.GetType().IsSubclassOf(typeof(Exception)))
                .To(HttpStatusCode.OK, "Success", (ex) => ex.Message);

            var coll = builder.Done();

            var testException = new ArgumentException("ok");

            var transform = coll.FindTransform(testException);
            Assert.IsNotNull(transform);
            Assert.AreEqual("text/plain", transform.ContentType);
        }

        [Test]
        public void ContentTypeCanBeOverridden()
        {
            var json = "application/json";

            var builder = TransformsCollectionBuilder.Begin()
                .Map(ex => ex.GetType().IsSubclassOf(typeof(Exception)))
                .To(HttpStatusCode.OK, "Success", (ex) => ex.Message, json);

            var coll = builder.Done();

            var testException = new ArgumentException("ok");

            var transform = coll.FindTransform(testException);
            Assert.IsNotNull(transform);
            Assert.AreEqual(json, transform.ContentType);
        }
    }
}
