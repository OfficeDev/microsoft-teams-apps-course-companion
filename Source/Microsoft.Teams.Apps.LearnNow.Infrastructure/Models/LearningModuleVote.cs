// <copyright file="LearningModuleVote.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;

    /// <summary>
    /// Handles learning module vote details.
    /// </summary>
    public partial class LearningModuleVote
    {
        /// <summary>
        /// Gets or sets vote id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets module id.
        /// </summary>
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Gets or sets user Azure Active Directory id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets date on which vote is created.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }
    }
}
