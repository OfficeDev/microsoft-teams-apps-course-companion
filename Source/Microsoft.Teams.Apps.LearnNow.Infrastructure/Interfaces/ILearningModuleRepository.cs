// <copyright file="ILearningModuleRepository.cs" company="Microsoft Corporation">
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
    /// Interface for handling common operations with entity collection.
    /// </summary>
    public interface ILearningModuleRepository : IRepository<LearningModule>
    {
        /// <summary>
        /// Handles getting learning module entities from database.
        /// </summary>
        /// <param name="skip">Number to records to be skipped to fetch next set of records.</param>
        /// <param name="count">Number of records to be fetched from database.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        Task<IEnumerable<LearningModule>> GetLearningModulesAsync(int skip, int count);

        /// <summary>
        /// Handles getting learning module entities from database.
        /// </summary>
        /// <param name="filterModel">Filter model to search learning module.</param>
        /// <param name="count">Number of records to be fetched from database.</param>
        /// <param name="skip">Number to records to be skipped to fetch next set of records.</param>
        /// <param name="exactMatch">Represents whether learning module title search should be exact match or not.</param>
        /// <param name="excludeEmptyModules">Represents whether filter should exclude learning modules which has resources associated with it.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        Task<IEnumerable<LearningModule>> GetLearningModulesAsync(FilterModel filterModel, int count, int skip, bool exactMatch, bool excludeEmptyModules);

        /// <summary>
        /// Handles getting learning module entities from database.
        /// </summary>
        /// <param name="filterModel">Filter model to search learning module.</param>
        /// <param name="count">Number of records to be fetched from database.</param>
        /// <param name="skip">Number to records to be skipped to fetch next set of records.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        Task<IEnumerable<LearningModule>> GetUserModulesAsync(UserLearningFilterModel filterModel, int count, int skip);

        /// <summary>
        /// Get learning module created by object Ids.
        /// </summary>
        /// <param name="createdByObjectIdCountToFetch">Count of created by object Ids to fetch.</param>
        /// <returns>Returns collection of learning module created by object Ids.</returns>
        Task<IEnumerable<Guid>> GetCreatedByObjectIdsAsync(int createdByObjectIdCountToFetch);

        /// <summary>
        /// Gets learning modules with votes and resources.
        /// </summary>
        /// <param name="learningModules">Learning module entity object collection.</param>
        /// <returns>Returns a collection of learning module detail models.</returns>
        Dictionary<Guid, List<LearningModuleDetailModel>> GetModulesWithVotesAndResources(IEnumerable<LearningModule> learningModules);
    }
}