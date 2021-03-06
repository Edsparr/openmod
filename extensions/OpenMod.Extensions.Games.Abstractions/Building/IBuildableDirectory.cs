﻿using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    [Service]
    public interface IBuildableDirectory
    {
        Task<IReadOnlyCollection<IBuildableAsset>> GetBuildableAssetsAsync();

        Task<IReadOnlyCollection<IBuildable>> GetBuildablesAsync();
    }
}