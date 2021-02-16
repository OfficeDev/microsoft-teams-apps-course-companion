// <copyright file="IResourceVoteRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// Interface for handling operations related to ResourceVote entity collection.
    /// </summary>
    public interface IResourceVoteRepository : IRepository<ResourceVote>
    {
        /// <summary>
        ///  Method to track resource vote collections to be deleted on context save changes call.
        /// </summary>
        /// <param name="resourceVotes"> Resource vote collections that needs to be deleted.</param>
        void DeleteResourceVotes(IEnumerable<ResourceVote> resourceVotes);
    }
}