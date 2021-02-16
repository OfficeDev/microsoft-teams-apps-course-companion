// <copyright file="IResourceRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// Interface for handling operations related to Resource entity collection.
    /// </summary>
    public interface IResourceRepository : IRepository<Resource>
    {
        /// <summary>
        /// Get resource based on page count and records to skip.
        /// </summary>
        /// <param name="skip">Number of resources to be skipped to fetch next set of resource.</param>
        /// <param name="count">Number of resources to be fetched from database.</param>
        /// <returns>Returns collection of filtered entities.</returns>
        Task<IEnumerable<Resource>> GetResourcesAsync(int skip, int count);

        /// <summary>
        /// Get resource based on page count and records to skip.
        /// </summary>
        /// <param name="filterModel">Filter model to get search data.</param>
        /// <param name="skip">Number of resources to be skipped to fetch next set of resource.</param>
        /// <param name="count">Number of resources to be fetched from database.</param>
        /// <param name="exactMatch">Represents whether resource title search should be exact match or not.</param>
        /// <returns>Returns collection of filtered entities.</returns>
        Task<IEnumerable<Resource>> GetResourcesAsync(FilterModel filterModel, int skip, int count, bool exactMatch);

        /// <summary>
        /// Gets repository for handling operations on Subject entity.
        /// </summary>
        /// <param name="resourceEntity"> Resource that needs to be deleted.</param>
        new void Delete(Resource resourceEntity);

        /// <summary>
        /// Get resource based on page count and records to skip.
        /// </summary>
        /// <param name="filterModel">Filter model to get search data.</param>
        /// <param name="skip">Number of resources to be skipped to fetch next set of resource.</param>
        /// <param name="count">Number of resources to be fetched from database.</param>
        /// <returns>Returns collection of filtered entities.</returns>
        Task<IEnumerable<Resource>> GetUserResourcesAsync(UserLearningFilterModel filterModel, int skip, int count);

        /// <summary>
        /// Get resource created by object Ids.
        /// </summary>
        /// <param name="createdByObjectIdCountToFetch">Count of created by object Ids to fetch.</param>
        /// <returns>Returns collection of resource created by object Ids.</returns>
        Task<IEnumerable<Guid>> GetCreatedByObjectIdsAsync(int createdByObjectIdCountToFetch);

        /// <summary>
        /// Get resources with votes.
        /// </summary>
        /// <param name="resources">Resource collection.</param>
        /// <returns>Returns resource detail model collection.</returns>
        Dictionary<Guid, List<ResourceDetailModel>> GetResourcesWithVotes(IEnumerable<Resource> resources);
    }
}