// <copyright file="LearningModuleTagRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with Tag entity collection.
    /// </summary>
    public class LearningModuleTagRepository : BaseRepository<LearningModuleTag>, ILearningModuleTagRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModuleTagRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public LearningModuleTagRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Handles getting all entities from database.
        /// </summary>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        public override async Task<IEnumerable<LearningModuleTag>> GetAllAsync()
        {
            return await this.context.Set<LearningModuleTag>()
                .Include(x => x.Tag)
                .ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///  Method to track tag entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="learningModuleTagsCollection"> Tag collection entity that needs to be deleted.</param>
        public void DeleteLearningModuleTag(IEnumerable<LearningModuleTag> learningModuleTagsCollection)
        {
            this.context.LearningModuleTag.RemoveRange(learningModuleTagsCollection);
        }

        /// <summary>
        ///  Method to track tag entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="learningModuleTagsCollection"> Tag collection entity that needs to be deleted.</param>
        public void AddLearningModuleTag(IEnumerable<LearningModuleTag> learningModuleTagsCollection)
        {
            this.context.LearningModuleTag.AddRange(learningModuleTagsCollection);
        }
    }
}