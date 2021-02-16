// <copyright file="ResourceRepository.cs" company="Microsoft Corporation">
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
    /// A repository class contains all common methods to work with Resource entity collection.
    /// </summary>
    public class ResourceRepository : BaseRepository<Resource>, IResourceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public ResourceRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Handles getting entity based on entity identifier.
        /// </summary>
        /// <param name="id">Filter entities from database using id.</param>
        /// <returns>Returns the entity that matches given identifier.</returns>
        public override async Task<Resource> GetAsync(Guid id)
        {
            return await this.context.Resource
                .Include(x => x.Subject)
                .Include(x => x.Grade)
                .Include(x => x.ResourceTag).ThenInclude(p => p.Tag)
                .FirstOrDefaultAsync(resource => resource.Id == id).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles getting all entities from database.
        /// </summary>
        /// <param name="skip">Number to records to be skipped to fetch next set of records.</param>
        /// <param name="count">Number of records to be fetched from database.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        public async Task<IEnumerable<Resource>> GetResourcesAsync(int skip, int count)
        {
            return await this.context.Set<Resource>()
                .Include(x => x.Subject)
                .Include(x => x.Grade)
                .Include(x => x.ResourceTag).ThenInclude(p => p.Tag)
                .OrderByDescending(x => x.UpdatedOn)
                .Skip(skip)
                .Take(count)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handles getting all entities from database.
        /// </summary>
        /// <param name="filterModel">User Selected filter based on which learning module entity needs to be filtered.</param>
        /// <param name="skip">Number to records to be skipped to fetch next set of records.</param>
        /// <param name="count">Number of records to be fetched from database.</param>
        /// <param name="exactMatch">Represents whether resource title search should be exact match or not.</param>
        /// <returns>Returns collection of resources.</returns>
        public async Task<IEnumerable<Resource>> GetResourcesAsync(FilterModel filterModel, int skip, int count, bool exactMatch)
        {
            var subjectIds = filterModel.SubjectIds;
            var gradeIds = filterModel.GradeIds;
            var createdByObjectIds = filterModel.CreatedByObjectIds;
            var tagIds = filterModel.TagIds;

            var resourceEntities = this.context.Set<Resource>()
                .Include(x => x.Subject)
                .Include(x => x.Grade)
                .Include(x => x.ResourceTag).ThenInclude(p => p.Tag).AsQueryable();

            var query = resourceEntities;
            if (exactMatch)
            {
#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                return await query.Where(x => string.Equals(x.Title, filterModel.SearchText)).AsNoTracking().ToListAsync();
#pragma warning restore CA1307 // Specify StringComparison
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

            if (!string.IsNullOrEmpty(filterModel.SearchText))
            {
#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                query = query.Where(x => x.Title.Contains(filterModel.SearchText));
#pragma warning restore CA1307 // Specify StringComparison
            }

            if (tagIds != null && tagIds.Any())
            {
                // Resource has multiple tags associated with it, so joining resource and resource tag to filter out resources based on provided resource tag Ids.
                var withTagsQuery = from resource in query
                             join resourceTag in this.context.Set<ResourceTag>()
                             on resource.Id equals resourceTag.ResourceId into grouping
                             from resourceTag in grouping.DefaultIfEmpty()
                             select new { resource, resourceTag };
                withTagsQuery = withTagsQuery.Where(x => tagIds.Contains(x.resourceTag.TagId));
                var tagsResult = await withTagsQuery.Skip(skip).Take(count).AsNoTracking().ToListAsync().ConfigureAwait(false);
                return tagsResult.Select(x => x.resource);
            }

            return await query.OrderByDescending(x => x.UpdatedOn).Skip(skip).Take(count).AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Update the existing resource entity in DbContext.
        /// </summary>
        /// <param name="entity">Resource entity that is to be updated.</param>
        /// <returns>Return updated entity.</returns>
        public override Resource Update(Resource entity)
        {
            var localEntity = this.context.Set<Resource>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entity.Id));

            if (localEntity != null)
            {
                this.context.Entry(localEntity).State = EntityState.Detached;
            }

            this.context.Entry(entity).State = EntityState.Modified;
            return base.Update(entity);
        }

        /// <summary>
        /// Handles filtering entity based on expression.
        /// </summary>
        /// <param name="predicate">Expression that is being used to filter entities from database.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        public override async Task<IEnumerable<Resource>> FindAsync(Expression<Func<Resource, bool>> predicate)
        {
            return await this.context.Set<Resource>()
                .Include(x => x.Subject)
                .Include(x => x.Grade)
                .AsQueryable()
                .Where(predicate).AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Delete the resource from storage.
        /// </summary>
        /// <param name="entity">Resource entity.</param>
        public new void Delete(Resource entity)
        {
            this.context.Remove(entity);
        }

        /// <summary>
        /// Get resource based on page count and records to skip.
        /// </summary>
        /// <param name="filterModel">Filter model to get search data.</param>
        /// <param name="skip">Number of resources to be skipped to fetch next set of resource.</param>
        /// <param name="count">Number of resources to be fetched from database.</param>
        /// <returns>Returns collection of filtered entities.</returns>
        public async Task<IEnumerable<Resource>> GetUserResourcesAsync(UserLearningFilterModel filterModel, int skip, int count)
        {
            filterModel = filterModel ?? throw new ArgumentNullException(nameof(filterModel));

            var resourceEntities = this.context.Set<Resource>()
               .Include(x => x.Subject)
               .Include(x => x.Grade)
               .Include(x => x.ResourceTag).ThenInclude(p => p.Tag).AsQueryable();

            var query = resourceEntities;

            query = query.Where(x => x.CreatedBy == filterModel.UserObjectId);

            if (!string.IsNullOrEmpty(filterModel.SearchText))
            {
#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                query = query.Where(x => x.Title.Contains(filterModel.SearchText));
#pragma warning restore CA1307 // Specify StringComparison
            }

            return await query.OrderByDescending(x => x.UpdatedOn).Skip(skip).Take(count).AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get resource created by Azure Active Directory ids.
        /// </summary>
        /// <param name="createdByObjectIdCountToFetch">Count of created by ids to fetch.</param>
        /// <returns>Returns collection of resource created by ids.</returns>
        public async Task<IEnumerable<Guid>> GetCreatedByObjectIdsAsync(int createdByObjectIdCountToFetch)
        {
            return await this.context.Set<Resource>()
                .AsNoTracking()
                .Select(resource => resource.CreatedBy)
                .Distinct()
                .Take(createdByObjectIdCountToFetch)
                .ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get resources with votes.
        /// </summary>
        /// <param name="resources">Resource collection.</param>
        /// <returns>Returns resource detail model collection.</returns>
        public Dictionary<Guid, List<ResourceDetailModel>> GetResourcesWithVotes(IEnumerable<Resource> resources)
        {
            resources = resources ?? throw new ArgumentNullException(nameof(resources));

            var filteredResources =
                (from resource in resources
                 join resourceVote in this.context.ResourceVote
                 on resource.Id equals resourceVote.ResourceId into joinedResourceVotes
                 select new ResourceDetailModel
                 {
                     Id = resource.Id,
                     Title = resource.Title,
                     Description = resource.Description,
                     GradeId = resource.GradeId,
                     SubjectId = resource.SubjectId,
                     Subject = resource.Subject,
                     Grade = resource.Grade,
                     ImageUrl = resource.ImageUrl,
                     LinkUrl = resource.LinkUrl,
                     AttachmentUrl = resource.AttachmentUrl,
                     ResourceType = resource.ResourceType,
                     CreatedBy = resource.CreatedBy,
                     UpdatedBy = resource.UpdatedBy,
                     CreatedOn = resource.CreatedOn,
                     UpdatedOn = resource.UpdatedOn,
                     ResourceTag = resource.ResourceTag,
                     Votes = joinedResourceVotes,
                 })
                .ToList()
                .GroupBy(resource => resource.Id)
                .ToDictionary(resource => resource.Key, resource => resource.ToList());

            return filteredResources;
        }
    }
}