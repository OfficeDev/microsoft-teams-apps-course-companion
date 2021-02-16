// <copyright file="ResourceTag.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;

    /// <summary>
    /// Handles resource tag details.
    /// </summary>
    public partial class ResourceTag
    {
        /// <summary>
        /// Gets or sets unique resource tag id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets resource id associated with particular tag.
        /// </summary>
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets tag id.
        /// </summary>
        public Guid TagId { get; set; }

        /// <summary>
        /// Gets or sets corresponding resource details linked with resource id.
        /// </summary>
        public virtual Resource Resource { get; set; }

        /// <summary>
        /// Gets or sets corresponding tag details linked with tag id.
        /// </summary>
        public virtual Tag Tag { get; set; }
    }
}
