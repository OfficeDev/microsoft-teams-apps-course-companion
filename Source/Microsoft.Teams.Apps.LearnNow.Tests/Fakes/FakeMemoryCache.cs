// <copyright file="FakeMemoryCache.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Tests.Fakes
{
    using System;

    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Fake memory cache for test project.
    /// </summary>
    public class FakeMemoryCache : IMemoryCache
    {
        /// <inheritdoc/>
        public ICacheEntry CreateEntry(object key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetValue(object key, out object value)
        {
            value = true;
            return true;
        }
    }
}
