// <copyright file="LearningModuleDetail.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// Model to handle learning module details.
    /// </summary>
    public class LearningModuleDetail : LearningModule
    {
        /// <summary>
        /// Gets or sets vote count
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether resource is liked by user.
        /// </summary>
        public bool IsLikedByUser { get; set; }

        /// <summary>
        /// Gets or sets display name of user who created the resource.
        /// </summary>
        public string UserDisplayName { get; set; }
    }
}
