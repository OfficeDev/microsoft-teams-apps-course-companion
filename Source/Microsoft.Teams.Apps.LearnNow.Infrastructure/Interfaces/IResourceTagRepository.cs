// <copyright file="IResourceTagRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// Interface for handling operations related to ResourceTag entity collection.
    /// </summary>
    public interface IResourceTagRepository : IRepository<ResourceTag>
    {
        /// <summary>
        /// Gets repository for handling operations on Subject entity.
        /// </summary>
        /// <param name="resourceTagsCollection"> List of resource tags that needs to be deleted.</param>
        void Delete(IEnumerable<ResourceTag> resourceTagsCollection);

        /// <summary>
        /// Gets repository for handling operations on Tag entity.
        /// </summary>
        /// <param name="resourceTagsCollection"> List of resource tags that needs to be added.</param>
        void Add(IEnumerable<ResourceTag> resourceTagsCollection);
    }
}