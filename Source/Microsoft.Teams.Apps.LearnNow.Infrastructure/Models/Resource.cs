// <copyright file="Resource.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class which represents Resource entity model.
    /// </summary>
    public partial class Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        public Resource()
        {
            this.ResourceTag = new HashSet<ResourceTag>();
            this.UserResource = new HashSet<UserResource>();
        }

        /// <summary>
        /// Gets or sets resource id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets resource title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets resource description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets resource subject id.
        /// </summary>
        public Guid SubjectId { get; set; }

        /// <summary>
        /// Gets or sets resource grade id.
        /// </summary>
        public Guid GradeId { get; set; }

        /// <summary>
        /// Gets or sets resource image URL.
        /// </summary>
#pragma warning disable CA1056 // Using string as Uri equivalent in database is string.
        public string ImageUrl { get; set; }
#pragma warning restore CA1056 // Using string as Uri equivalent in database is string.

        /// <summary>
        /// Gets or sets resource link URL.
        /// </summary>
#pragma warning disable CA1056 // Using string as Uri equivalent in database is string.
        public string LinkUrl { get; set; }
#pragma warning restore CA1056 // Using string as Uri equivalent in database is string.

        /// <summary>
        /// Gets or sets resource attachment URL.
        /// </summary>
#pragma warning disable CA1056 // Using string as Uri equivalent in database is string.
        public string AttachmentUrl { get; set; }
#pragma warning restore CA1056 // Using string as Uri equivalent in database is string.

        /// <summary>
        /// Gets or sets resource created on date.
        /// </summary>
        public DateTimeOffset? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets resource updates on date.
        /// </summary>
        public DateTimeOffset? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who created the resource.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who updated the resource.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets resource type.
        /// </summary>
        public int ResourceType { get; set; }

        /// <summary>
        /// Gets or sets resource grade details.
        /// </summary>
        public Grade Grade { get; set; }

        /// <summary>
        /// Gets or sets resource subject details.
        /// </summary>
        public Subject Subject { get; set; }

        /// <summary>
        /// Gets or sets resource tag entity.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public virtual ICollection<ResourceTag> ResourceTag { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Gets or sets User Resource entity.
        /// </summary>
 #pragma warning disable CA2227 // Collection properties should be read only
        public virtual ICollection<UserResource> UserResource { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
