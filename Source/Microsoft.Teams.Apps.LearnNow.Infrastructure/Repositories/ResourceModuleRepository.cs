// <copyright file="ResourceModuleRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with Learning module entity collection.
    /// </summary>
    public class ResourceModuleRepository : BaseRepository<ResourceModuleMapping>, IResourceModuleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceModuleRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public ResourceModuleRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Handles filtering of modules based on provided grade and subject id.
        /// </summary>
        /// <param name="gradeId">Grade id being used to filter entities from database.</param>
        /// <param name="subjectId">Subject id that is being used to filter entities from database.</param>
        /// <returns>Returns collection of filtered resource module mapping using expression.</returns>
        public async Task<IEnumerable<LearningModule>> FindModulesForGradeAndSubjectAsync(Guid gradeId, Guid subjectId)
        {
            var resourceModule = await this.context.Set<ResourceModuleMapping>()
                .Include(x => x.LearningModule)
                .Include(x => x.LearningModule).ThenInclude(p => p.Subject)
                .Include(x => x.LearningModule).ThenInclude(p => p.Grade)
                .AsQueryable()
                .Where(resourceModule => resourceModule.LearningModule.GradeId == gradeId && resourceModule.LearningModule.SubjectId == subjectId).ToListAsync().ConfigureAwait(false);

            return resourceModule.Select(resourceModule => resourceModule.LearningModule).Distinct();
        }

        /// <summary>
        /// Handles filtering of resources associated with given learning module id.
        /// </summary>
        /// <param name="learningmoduleId">Learning module id that is being used to filter resources from database.</param>
        /// <returns>Returns collection of resource associated with given learning module id.</returns>
        public async Task<IEnumerable<Resource>> FindResourcesForModuleAsync(Guid learningmoduleId)
        {
            var resourceModule = await this.context.Set<ResourceModuleMapping>()
                .Include(x => x.Resource)
                .Include(x => x.Resource).ThenInclude(p => p.Subject)
                .Include(x => x.Resource).ThenInclude(p => p.Grade)
                .Include(x => x.Resource).ThenInclude(x => x.ResourceTag).ThenInclude(p => p.Tag)
                .AsQueryable()
                .Where(resourceModule => resourceModule.LearningModuleId == learningmoduleId)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync()
                .ConfigureAwait(false);

            return resourceModule.Select(resourceModule => resourceModule.Resource).Distinct();
        }

        /// <summary>
        ///  Method to track Resource Module entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="resourceModuleCollection"> Vote entity that needs to be deleted.</param>
        public void DeleteResourceModuleMappings(IEnumerable<ResourceModuleMapping> resourceModuleCollection)
        {
            this.context.ResourceModuleMapping.RemoveRange(resourceModuleCollection);
        }

        /// <summary>
        ///  Method to track Resource Module entities to be added on context save changes call.
        /// </summary>
        /// <param name="resourceModuleCollection"> resourceModuleCollection collection entity that needs to be added.</param>
        public void AddResourceModuleMappings(IEnumerable<ResourceModuleMapping> resourceModuleCollection)
        {
            this.context.ResourceModuleMapping.AddRange(resourceModuleCollection);
        }
    }
}