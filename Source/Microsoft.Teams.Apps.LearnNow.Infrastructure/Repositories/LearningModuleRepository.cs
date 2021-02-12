// <copyright file="LearningModuleRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with Learning module entity collection.
    /// </summary>
    public class LearningModuleRepository : BaseRepository<LearningModule>, ILearningModuleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModuleRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public LearningModuleRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Handles getting learning module entities from database.
        /// </summary>
        /// <param name="skip">Number to records to be skipped to fetch next set of records.</param>
        /// <param name="count">Number of records to be fetched from database.</param>
        /// <returns>Returns collection of learning module entities.</returns>
        public async Task<IEnumerable<LearningModule>> GetLearningModulesAsync(int skip, int count)
        {
            return await this.context.Set<LearningModule>()
                .Include(x => x.Subject)
                .Include(x => x.Grade)
                .Include(x => x.LearningModuleTag).ThenInclude(p => p.Tag)
                .OrderByDescending(x => x.UpdatedOn)
                .AsNoTracking()
                .Skip(skip)
                .Take(count)
                .ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handles getting learning module entities from database.
        /// </summary>
        /// <param name="filterModel">User Selected filter based on which learning module entity needs to be filtered.</param>
        /// <param name="count">Number of records to be fetched from database.</param>
        /// <param name="skip">Number to records to be skipped to fetch next set of records.</param>
        /// <param name="exactMatch">Represents whether learning module title search should be exact match or not.</param>
        /// <param name="excludeEmptyModules">Represents whether filter should exclude learning modules which has resources associated with it.</param>
        /// <returns>Returns collection of learning module entities.</returns>
        public async Task<IEnumerable<LearningModule>> GetLearningModulesAsync(FilterModel filterModel, int count, int skip, bool exactMatch, bool excludeEmptyModules)
        {
            var subjectIds = filterModel.SubjectIds;
            var gradeIds = filterModel.GradeIds;
            var createdByObjectIds = filterModel.CreatedByObjectIds;
            var tagIds = filterModel.TagIds;

            var learningModuleEntities = this.context.Set<LearningModule>()
                .Include(x => x.Subject)
                .Include(x => x.Grade)
                .Include(x => x.LearningModuleTag).ThenInclude(p => p.Tag).AsQueryable();

            var query = learningModuleEntities;

            if (exactMatch)
            {
#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                return await query.Where(x => string.Equals(x.Title, filterModel.SearchText)).AsNoTracking().ToListAsync().ConfigureAwait(false);
#pragma warning restore CA1307 // Specify StringComparison
            }

            if (excludeEmptyModules)
            {
                var moduleMappingQuery = from learningModule in learningModuleEntities
                                         join resourceModuleMapping in this.context.Set<ResourceModuleMapping>()
                                         on learningModule.Id equals resourceModuleMapping.LearningModuleId
                                         select new { learningModule, resourceModuleMapping };

                if (subjectIds.Any())
                {
                    moduleMappingQuery = moduleMappingQuery.Where(x => subjectIds.Contains(x.learningModule.SubjectId));
                }

                if (gradeIds.Any())
                {
                    moduleMappingQuery = moduleMappingQuery.Where(x => gradeIds.Contains(x.learningModule.GradeId));
                }

                return await moduleMappingQuery.Select(x => x.learningModule).ToListAsync().ConfigureAwait(false);
            }

            if (subjectIds != null && subjectIds.Any())
            {
                query = query.Where(x => subjectIds.Contains(x.SubjectId));
            }

            if (gradeIds != null && gradeIds.Any())
            {
                query = query.Where(x => gradeIds.Contains(x.GradeId));
            }

            if (createdByObjectIds != null && createdByObjectIds.Any())
            {
                query = query.Where(x => createdByObjectIds.Contains(x.CreatedBy));
            }

            if (tagIds != null && tagIds.Any())
            {
                // Learning module has multiple tags associated with it, so joining learning module and learning module tag to filter out learning modules based on provided learning module tag Ids.
                var withTagsQuery = from learningModule in learningModuleEntities
                                     join learningModuleTag in this.context.Set<LearningModuleTag>()
                                     on learningModule.Id equals learningModuleTag.LearningModuleId into grouping
                                     from moduleTag in grouping.DefaultIfEmpty()
                                     select new { learningModule, moduleTag };
                withTagsQuery = withTagsQuery.Where(x => tagIds.Contains(x.moduleTag.TagId));
                var tagsResult = await withTagsQuery.Skip(skip).Take(count).AsNoTracking().ToListAsync().ConfigureAwait(false);
                return tagsResult.Select(x => x.learningModule);
            }

            if (!string.IsNullOrEmpty(filterModel.SearchText))
            {
#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                query = query.Where(x => x.Title.Contains(filterModel.SearchText));
#pragma warning restore CA1307 // Specify StringComparison
            }

            return await query.OrderByDescending(x => x.UpdatedOn).Skip(skip).Take(count).AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handles filtering entity based on expression.
        /// </summary>
        /// <param name="predicate">Expression that is being used to filter entities from database.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        public override async Task<IEnumerable<LearningModule>> FindAsync(Expression<Func<LearningModule, bool>> predicate)
        {
            return await this.context.Set<LearningModule>()
                .Include(x => x.Subject)
                .Include(x => x.Grade)
                .Include(x => x.LearningModuleTag).ThenInclude(p => p.Tag)
                .AsQueryable()
                .Where(predicate).AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handles getting entity based on entity identifier.
        /// </summary>
        /// <param name="id">Filter entities from database using id.</param>
        /// <returns>Returns the entity that matches given identifier.</returns>
        public override async Task<LearningModule> GetAsync(Guid id)
        {
            return await this.context.LearningModule
                .Include(x => x.Subject)
                .Include(x => x.Grade)
                .Include(x => x.LearningModuleTag).ThenInclude(p => p.Tag)
                .FirstOrDefaultAsync(resource => resource.Id == id).ConfigureAwait(false);
        }

        /// <summary>
        /// Update the existing resource entity into database.
        /// </summary>
        /// <param name="entity">Resource entity that is to be updated.</param>
        /// <returns>Return updated entity.</returns>
        public override LearningModule Update(LearningModule entity)
        {
            var localentity = this.context.Set<LearningModule>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entity.Id));
            if (localentity != null)
            {
                this.context.Entry(localentity).State = EntityState.Detached;
            }

            this.context.Entry(entity).State = EntityState.Modified;
            return base.Update(entity);
        }

        /// <summary>
        /// Handles getting learning module entities from database.
        /// </summary>
        /// <param name="filterModel">Filter model to search learning module.</param>
        /// <param name="count">Number of records to be fetched from database.</param>
        /// <param name="skip">Number to records to be skipped to fetch next set of records.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        public async Task<IEnumerable<LearningModule>> GetUserModulesAsync(UserLearningFilterModel filterModel, int count, int skip)
        {
            filterModel = filterModel ?? throw new ArgumentNullException(nameof(filterModel));

            var learningModuleEntities = this.context.Set<LearningModule>()
               .Include(x => x.Subject)
               .Include(x => x.Grade)
               .Include(x => x.LearningModuleTag).ThenInclude(p => p.Tag).AsQueryable();

            var query = learningModuleEntities;

            query = query.Where(x => x.CreatedBy == filterModel.UserObjectId);

            if (!string.IsNullOrEmpty(filterModel.SearchText))
            {
#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                query = query.Where(x => x.Title.Contains(filterModel.SearchText));
#pragma warning restore CA1307 // Specify StringComparison
            }

            return await query.OrderByDescending(x => x.UpdatedOn).Skip(skip).Take(count).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Get learning module created by object Ids.
        /// </summary>
        /// <param name="createdByObjectIdCountToFetch">Count of created by object Ids to fetch.</param>
        /// <returns>Returns collection of learning module created by object Id's.</returns>
        public async Task<IEnumerable<Guid>> GetCreatedByObjectIdsAsync(int createdByObjectIdCountToFetch)
        {
            return await this.context.Set<LearningModule>()
                .AsNoTracking()
                .Select(resource => resource.CreatedBy)
                .Distinct()
                .Take(createdByObjectIdCountToFetch)
                .ToListAsync();
        }

        /// <summary>
        /// Gets learning modules with votes and resources models.
        /// </summary>
        /// <param name="learningModules">Learning module entity object collection.</param>
        /// <returns>Returns a collection of learning module detail models.</returns>
        public Dictionary<Guid, List<LearningModuleDetailModel>> GetModulesWithVotesAndResources(IEnumerable<LearningModule> learningModules)
        {
            learningModules = learningModules ?? throw new ArgumentNullException(nameof(learningModules));

            var filteredLearningModules =
                (from learningModule in learningModules
                 join learningModuleVote in this.context.LearningModuleVote
                 on learningModule.Id equals learningModuleVote.ModuleId into joinedLearningModuleAndVote
                 join resourceModuleMapping in this.context.ResourceModuleMapping
                 on learningModule.Id equals resourceModuleMapping.LearningModuleId into joinedResourceModule
                 select new LearningModuleDetailModel
                 {
                     Id = learningModule.Id,
                     Title = learningModule.Title,
                     Description = learningModule.Description,
                     GradeId = learningModule.GradeId,
                     SubjectId = learningModule.SubjectId,
                     Subject = learningModule.Subject,
                     Grade = learningModule.Grade,
                     ImageUrl = learningModule.ImageUrl,
                     CreatedBy = learningModule.CreatedBy,
                     UpdatedBy = learningModule.UpdatedBy,
                     CreatedOn = learningModule.CreatedOn,
                     UpdatedOn = learningModule.UpdatedOn,
                     LearningModuleTag = learningModule.LearningModuleTag,
                     Votes = joinedLearningModuleAndVote,
                     ResourceModuleMappings = joinedResourceModule,
                 })
                .ToList()
                .GroupBy(module => module.Id)
                .ToDictionary(module => module.Key, module => module.ToList());

            return filteredLearningModules;
        }
    }
}