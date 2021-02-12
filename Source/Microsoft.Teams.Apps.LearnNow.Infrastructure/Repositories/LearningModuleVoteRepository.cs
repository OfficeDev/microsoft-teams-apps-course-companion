// <copyright file="LearningModuleVoteRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with Vote entity collection.
    /// </summary>
    public class LearningModuleVoteRepository : BaseRepository<LearningModuleVote>, ILearningModuleVoteRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModuleVoteRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public LearningModuleVoteRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets repository for handling delete operations on LearningModuleVote entity.
        /// </summary>
        /// <param name="learningModuleVotes">List of learning module votes that needs to be deleted.</param>
        public void DeleteLearningModuleVotes(IEnumerable<LearningModuleVote> learningModuleVotes)
        {
            this.context.LearningModuleVote.RemoveRange(learningModuleVotes);
        }
    }
}