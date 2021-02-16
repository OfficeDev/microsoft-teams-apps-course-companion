// <copyright file="ResourceModuleMapping.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;

    /// <summary>
    /// Handles resource learning module mapping details.
    /// </summary>
    public partial class ResourceModuleMapping
    {
        /// <summary>
        /// Gets or sets resource id associated with particular learning module.
        /// </summary>
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets learning module id associated with particular resource.
        /// </summary>
        public Guid LearningModuleId { get; set; }

        /// <summary>
        /// Gets or sets resource module mapping created on date.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets learning module entity.
        /// </summary>
        public virtual LearningModule LearningModule { get; set; }

        /// <summary>
        /// Gets or sets resource entity.
        /// </summary>
        public virtual Resource Resource { get; set; }
    }
}
