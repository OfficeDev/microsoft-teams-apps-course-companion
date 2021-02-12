// <copyright file="TagRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with Tag entity collection.
    /// </summary>
    public class TagRepository : BaseRepository<Tag>, ITagRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public TagRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        ///  Method to track Tag entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="tagsCollection"> List of tags that needs to be deleted.</param>
        public void DeleteTags(IEnumerable<Tag> tagsCollection)
        {
            this.context.Tag.RemoveRange(tagsCollection);
        }
    }
}
