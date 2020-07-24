using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Threading.Tasks;

namespace Devscord.EventStore.Tests
{
    [TestFixture]
    public class EventStoreTests 
    {
        [Test]
        public void ShouldReactOnEvents()
        {
            var isWorking = false;
            EventStore.Subscribe<ToSubscribe.TestEvent>(x => isWorking = x.Test == "ABCD");
            new ToPublish.TestEvent() { Test = "ABCD" }.Publish().Wait();
            Assert.That(isWorking, Is.True);
        }
    }
}

namespace Devscord.EventStore.Tests.ToPublish
{
    public class TestEvent : Event
    {
        public string Test { get; set; }
    }
}

namespace Devscord.EventStore.Tests.ToSubscribe
{
    public class TestEvent : Event
    {
        public string Test { get; set; }
    }
}