// <copyright file="ITagRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// Interface for handling operations related to Tag entity collection.
    /// </summary>
    public interface ITagRepository : IRepository<Tag>
    {
        /// <summary>
        /// Gets repository for handling operations on Tag entity.
        /// </summary>
        /// <param name="tagsCollection"> List of tags that needs to be deleted.</param>
        void DeleteTags(IEnumerable<Tag> tagsCollection);
    }
}