// <copyright file="ResourceVoteRepository.cs" company="Microsoft Corporation">
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
    public class ResourceVoteRepository : BaseRepository<ResourceVote>, IResourceVoteRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVoteRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public ResourceVoteRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        ///  Method to track resource vote collections to be deleted on context save changes call.
        /// </summary>
        /// <param name="resourceVotes"> Resource vote collections that needs to be deleted.</param>
        public void DeleteResourceVotes(IEnumerable<ResourceVote> resourceVotes)
        {
            this.context.ResourceVote.RemoveRange(resourceVotes);
        }
    }
}
