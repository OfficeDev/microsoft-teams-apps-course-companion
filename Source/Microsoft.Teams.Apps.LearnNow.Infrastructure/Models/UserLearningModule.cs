// <copyright file="UserLearningModule.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;

    /// <summary>
    /// A class which represents User learning module entity model.
    /// </summary>
    public partial class UserLearningModule
    {
        /// <summary>
        /// Gets or sets id of user learning module entity model.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets user learning module user id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets user learning module resource id.
        /// </summary>
        public Guid LearningModuleId { get; set; }

        /// <summary>
        /// Gets or sets user learning module created on date.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets user learning module details.
        /// </summary>
        public virtual LearningModule LearningModule { get; set; }
    }
}