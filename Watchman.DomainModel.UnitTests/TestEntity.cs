using System;

using Watchman.DomainModel.Commons.Calculators.Statistics.Splitters;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.UnitTests
{
    public class TestEntity : Entity, ISplittable
    {
        public void SetUpdatedAt(DateTime dateTime)
        {
            this.UpdatedAt = dateTime;
        }

        public void SetCreatedAt(DateTime dateTime)
        {
            this.CreatedAt = dateTime;
            this.UpdatedAt = dateTime;
        }

        public DateTime GetSplittable()
        {
            return this.CreatedAt;
        }
    }
}
