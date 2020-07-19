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
            EventStore.Subscribe<TestEvent>(x => isWorking = true);
            new TestEvent().Publish().Wait();
            Assert.That(isWorking, Is.True);
        }
    }

    public class TestEvent : Event
    {
        public override string Name => nameof(TestEvent);
    }
}
