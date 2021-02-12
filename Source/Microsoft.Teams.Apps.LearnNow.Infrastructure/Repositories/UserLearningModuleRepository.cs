// <copyright file="UserLearningModuleRepository.cs" company="Microsoft Corporation">
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
    /// A repository class contains all common methods to work with TeamPreference entity collection.
    /// </summary>
    public class UserLearningModuleRepository : BaseRepository<UserLearningModule>, IUserLearningModuleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserLearningModuleRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public UserLearningModuleRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Get module based on page count, records to skip and provided filters.
        /// </summary>
        /// <param name="filterModel">Filter model to get search data.</param>
        /// <param name="skip">Number of modules to be skipped to fetch next set of module.</param>
        /// <param name="count">Number of modules to be fetched from database.</param>
        /// <returns>Returns collection of filtered entities.</returns>
        public async Task<IEnumerable<LearningModule>> GetUserSavedModulesAsync(UserLearningFilterModel filterModel, int skip, int count)
        {
            filterModel = filterModel ?? throw new ArgumentNullException(nameof(filterModel));

            var userLearningModuleEntities = this.context.Set<UserLearningModule>()
               .Include(x => x.LearningModule).ThenInclude(x => x.Subject)
               .Include(x => x.LearningModule).ThenInclude(x => x.Grade)
               .Include(x => x.LearningModule).ThenInclude(x => x.LearningModuleTag).ThenInclude(p => p.Tag);

            var query = userLearningModuleEntities.Where(x => x.UserId == filterModel.UserObjectId);

            if (!string.IsNullOrEmpty(filterModel.SearchText))
            {
#pragma warning disable CA1307 // Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.// Ignoring StringComparison as EF handles the string comparison while building SQL query from LINQ expression. In case of explicit StringComparison addition, then it fails the SQL query execution with error.
                query = query.Where(x => x.LearningModule.Title.Contains(filterModel.SearchText));
#pragma warning restore CA1307 // Specify StringComparison
            }

            var learningModules = await query.OrderByDescending(x => x.LearningModule.UpdatedOn).Skip(skip).Take(count).AsNoTracking().ToListAsync().ConfigureAwait(false);
            return learningModules.Select(x => x.LearningModule);
        }

        /// <summary>
        ///  Method to track User Module entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="userLearningModules"> User Learning Module collections that needs to be deleted.</param>
        public void DeleteUserModules(IEnumerable<UserLearningModule> userLearningModules)
        {
            this.context.UserLearningModule.RemoveRange(userLearningModules);
        }
    }
}