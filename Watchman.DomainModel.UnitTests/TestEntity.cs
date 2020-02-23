using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.UnitTests
{
    public class TestEntity : Entity
    {
        public void SetUpdatedAt(DateTime dateTime)
        {
            UpdatedAt = dateTime;
        }

        public void SetCreatedAt(DateTime dateTime)
        {
            CreatedAt = dateTime;
        }
    }
}
