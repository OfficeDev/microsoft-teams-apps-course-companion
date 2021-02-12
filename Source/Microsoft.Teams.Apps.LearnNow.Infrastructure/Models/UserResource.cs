// <copyright file="UserResource.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;

    /// <summary>
    /// A class which represents user resource entity model.
    /// </summary>
    public partial class UserResource
    {
        /// <summary>
        /// Gets or sets user resource id of  user resource model.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets user resource user id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets user resource resource id.
        /// </summary>
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets user resource created on date.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets user resource details.
        /// </summary>
        public virtual Resource Resource { get; set; }
    }
}
