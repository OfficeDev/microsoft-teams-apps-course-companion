// <copyright file="IResourceModuleRepository.cs" company="Microsoft Corporation">
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
    public interface IResourceModuleRepository : IRepository<ResourceModuleMapping>
    {
        /// <summary>
        /// Handles filtering entity based on expression.
        /// </summary>
        /// <param name="gradeId">Grade id being used to filter entities from database.</param>
        /// <param name="subjectId">Subject id that is being used to filter entities from database.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        Task<IEnumerable<LearningModule>> FindModulesForGradeAndSubjectAsync(Guid gradeId, Guid subjectId);

        /// <summary>
        /// Handles filtering of resources associated with given learning module id.
        /// </summary>
        /// <param name="learningmoduleId">Learning module id that is being used to filter resources from database.</param>
        /// <returns>Returns collection of resource associated with given learning module id.</returns>
        Task<IEnumerable<Resource>> FindResourcesForModuleAsync(Guid learningmoduleId);

        /// <summary>
        /// Delete repository for handling operations on resource module entity.
        /// </summary>
        /// <param name="resourceModuleCollection"> Collection of resource modules that needs to be deleted.</param>
        void DeleteResourceModuleMappings(IEnumerable<ResourceModuleMapping> resourceModuleCollection);

        /// <summary>
        ///  Method to add collection of resource module mapping entity.
        /// </summary>
        /// <param name="resourceModuleCollection"> Collection of resource modules that needs to be added.</param>
        void AddResourceModuleMappings(IEnumerable<ResourceModuleMapping> resourceModuleCollection);
    }
}