﻿namespace Watchman.DomainModel.Configuration
{
    public interface IMappedConfiguration
    {
        ulong ServerId { get; }
        string Name { get; }
    }
}
