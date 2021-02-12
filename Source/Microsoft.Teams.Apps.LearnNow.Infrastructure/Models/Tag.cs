// <copyright file="Tag.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class which represents Tag entity model.
    /// </summary>
    public partial class Tag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        public Tag()
        {
            this.ResourceTag = new HashSet<ResourceTag>();
            this.LearningModuleTag = new HashSet<LearningModuleTag>();
        }

        /// <summary>
        /// Gets or sets id of tag entity model.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name of tag entity model.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who created the tag.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who updated the tag.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets created on UTC date time of tag entity model.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets updated on UTC date time of tag entity model.
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets resource tag entity.
        /// </summary>
#pragma warning disable CA2227 // Set is required to allow assigning result from storage.
        public virtual ICollection<ResourceTag> ResourceTag { get; set; }
#pragma warning restore CA2227 // Set is required to allow assigning result from storage.

        /// <summary>
        /// Gets or sets learning module tag entity.
        /// </summary>
#pragma warning disable CA2227 // Set is required to allow assigning result from storage.
        public virtual ICollection<LearningModuleTag> LearningModuleTag { get; set; }
#pragma warning restore CA2227 // Set is required to allow assigning result from storage.
    }
}
