// <copyright file="LearningModuleTag.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;

    /// <summary>
    /// Handles learning module tag details.
    /// </summary>
    public partial class LearningModuleTag
    {
        /// <summary>
        /// Gets or sets unique resource tag id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets learning module id associated with particular tag.
        /// </summary>
        public Guid LearningModuleId { get; set; }

        /// <summary>
        /// Gets or sets tag id.
        /// </summary>
        public Guid TagId { get; set; }

        /// <summary>
        /// Gets or sets corresponding learning module details linked with resource id.
        /// </summary>
        public virtual LearningModule LearningModule { get; set; }

        /// <summary>
        /// Gets or sets corresponding tag details linked with tag id.
        /// </summary>
        public virtual Tag Tag { get; set; }
    }
}