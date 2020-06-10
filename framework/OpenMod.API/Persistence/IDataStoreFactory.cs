﻿using OpenMod.API.Ioc;

namespace OpenMod.API.Persistence
{
    [Service]
    public interface IDataStoreFactory
    {
        IDataStore CreateDataStore(string id, string basePath);
    }
}