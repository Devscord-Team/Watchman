using System;
using System.Collections.Generic;
using System.Text;
using Watchman.DomainModel.Commons.Calculators.Statistics.Splitters;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.UnitTests
{
    public class TestEntity : Entity, ISplittable
    {
        public void SetUpdatedAt(DateTime dateTime)
        {
            UpdatedAt = dateTime;
        }

        public void SetCreatedAt(DateTime dateTime)
        {
            CreatedAt = dateTime;
            UpdatedAt = dateTime;
        }

        public DateTime GetSplittable() => this.CreatedAt;
    }
}
