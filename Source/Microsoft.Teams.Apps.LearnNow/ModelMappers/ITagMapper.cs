// <copyright file="ITagMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// Interface for handling operations related to tag model mappings.
    /// </summary>
    public interface ITagMapper
    {
        /// <summary>
        /// Gets tag entity model from view model.
        /// </summary>
        /// <param name="tagViewModel">Tag view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a tag entity model object.</returns>
        public Tag MapToDTO(
            TagViewModel tagViewModel,
            Guid userAadObjectId);

        /// <summary>
        /// Gets tag view model from entity model.
        /// </summary>
        /// <param name="tags">Collection of tag entity model objects.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns collection of tag view model objects.</returns>
        public IEnumerable<TagViewModel> MapToViewModel(
            IEnumerable<Tag> tags,
            Dictionary<Guid, string> idToNameMap);
    }
}