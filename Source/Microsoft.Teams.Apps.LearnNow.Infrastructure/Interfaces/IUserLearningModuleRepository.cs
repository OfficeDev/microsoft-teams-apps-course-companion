// <copyright file="IUserLearningModuleRepository.cs" company="Microsoft Corporation">
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
    /// Interface for handling common operations with entity collection.
    /// </summary>
    public interface IUserLearningModuleRepository : IRepository<UserLearningModule>
    {
        /// <summary>
        /// Get module based on page count and records to skip.
        /// </summary>
        /// <param name="filterModel">Filter model to get search data.</param>
        /// <param name="skip">Number of modules to be skipped to fetch next set of module.</param>
        /// <param name="count">Number of modules to be fetched from database.</param>
        /// <returns>Returns collection of filtered entities.</returns>
        Task<IEnumerable<LearningModule>> GetUserSavedModulesAsync(UserLearningFilterModel filterModel, int skip, int count);

        /// <summary>
        ///  Method to track User Module entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="userLearningModules"> User Learning Module collections that needs to be deleted.</param>
        void DeleteUserModules(IEnumerable<UserLearningModule> userLearningModules);
    }
}