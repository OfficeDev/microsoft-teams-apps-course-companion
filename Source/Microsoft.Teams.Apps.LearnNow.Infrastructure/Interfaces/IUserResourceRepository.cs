// <copyright file="IUserResourceRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// Interface for handling common operations related with user resource entity collection.
    /// </summary>
    public interface IUserResourceRepository : IRepository<UserResource>
    {
        /// <summary>
        /// Get resource based on page count and records to skip.
        /// </summary>
        /// <param name="filterModel">Filter model to get search data.</param>
        /// <param name="skip">Number of resources to be skipped to fetch next set of resource.</param>
        /// <param name="count">Number of resources to be fetched from database.</param>
        /// <returns>Returns collection of filtered entities.</returns>
        Task<IEnumerable<Resource>> GetUserSavedResourcesAsync(UserLearningFilterModel filterModel, int skip, int count);

        /// <summary>
        ///  Method to track User Resource collections to be deleted on context save changes call.
        /// </summary>
        /// <param name="userResources"> User Resource collections that needs to be deleted.</param>
        void DeleteUserResources(IEnumerable<UserResource> userResources);
    }
}