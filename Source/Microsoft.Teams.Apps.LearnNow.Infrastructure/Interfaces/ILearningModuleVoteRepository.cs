// <copyright file="ILearningModuleVoteRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// Interface for handling common operations with entity collection.
    /// </summary>
    public interface ILearningModuleVoteRepository : IRepository<LearningModuleVote>
    {
        /// <summary>
        /// Method to track learning module vote collections to be deleted on context save changes call.
        /// </summary>
        /// <param name="learningModuleVotes">List of learning module votes that needs to be deleted.</param>
        void DeleteLearningModuleVotes(IEnumerable<LearningModuleVote> learningModuleVotes);
    }
}