// <copyright file="UserResourceRepository.cs" company="Microsoft Corporation">
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
    /// A repository class contains all common methods to work with UserResource entity collection.
    /// </summary>
    public class UserResourceRepository : BaseRepository<UserResource>, IUserResourceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserResourceRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity Framework database context class to work with entities.</param>
        public UserResourceRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Get user resource based on page count and records to skip.
        /// </summary>
        /// <param name="filterModel">Filter model to get search data.</param>
        /// <param name="skip">Number of resources to be skipped to fetch next set of resource.</param>
        /// <param name="count">Number of resources to be fetched from database.</param>
        /// <returns>Returns collection of filtered resource entities.</returns>
        public async Task<IEnumerable<Resource>> GetUserSavedResourcesAsync(UserLearningFilterModel filterModel, int skip, int count)
        {
            filterModel = filterModel ?? throw new ArgumentNullException(nameof(filterModel));

            var userResourceEntities = this.context.Set<UserResource>()
               .Include(x => x.Resource).ThenInclude(x => x.Subject)
               .Include(x => x.Resource).ThenInclude(x => x.Grade)
               .Include(x => x.Resource).ThenInclude(x => x.ResourceTag).ThenInclude(p => p.Tag);

            var query = userResourceEntities.Where(x => x.UserId == filterModel.UserObjectId);

            if (!string.IsNullOrEmpty(filterModel.SearchText))
            {
#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                query = query.Where(x => x.Resource.Title.Contains(filterModel.SearchText));
#pragma warning restore CA1307 // Specify StringComparison
            }

            var resources = await query.OrderByDescending(x => x.Resource.UpdatedOn).Skip(skip).Take(count).AsNoTracking().ToListAsync().ConfigureAwait(false);
            return resources.Select(x => x.Resource);
        }

        /// <summary>
        ///  Method to track User Resource collections to be deleted on context save changes call.
        /// </summary>
        /// <param name="userResources"> User Resource collections that needs to be deleted.</param>
        public void DeleteUserResources(IEnumerable<UserResource> userResources)
        {
            this.context.UserResource.RemoveRange(userResources);
        }
    }
}