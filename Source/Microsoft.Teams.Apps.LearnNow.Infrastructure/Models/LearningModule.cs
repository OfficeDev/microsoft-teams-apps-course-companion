// <copyright file="LearningModule.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Handles learning module details.
    /// </summary>
    public partial class LearningModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModule"/> class.
        /// </summary>
        public LearningModule()
        {
            this.LearningModuleTag = new HashSet<LearningModuleTag>();
            this.UserLearningModule = new HashSet<UserLearningModule>();
        }

        /// <summary>
        /// Gets or sets learning module id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets learning module title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets learning module description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets learning module subject id.
        /// </summary>
        public Guid SubjectId { get; set; }

        /// <summary>
        /// Gets or sets learning module grade id.
        /// </summary>
        public Guid GradeId { get; set; }

        /// <summary>
        /// Gets or sets learning module image URL.
        /// </summary>
#pragma warning disable CA1056 // Using string as Uri equivalent in database is string.
        public string ImageUrl { get; set; }
#pragma warning restore CA1056 // Using string as Uri equivalent in database is string.

        /// <summary>
        /// Gets or sets learning module created on date.
        /// </summary>
        public DateTimeOffset? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets learning module updates on date.
        /// </summary>
        public DateTimeOffset? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets learning module created by user's AAD object ID.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets learning module updated by user's AAD object ID.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets grade entity.
        /// </summary>
        public virtual Grade Grade { get; set; }

        /// <summary>
        /// Gets or sets subject entity.
        /// </summary>
        public virtual Subject Subject { get; set; }

        /// <summary>
        /// Gets or sets learning moudle tag entity.
        /// </summary>
#pragma warning disable CA2227 // Set is required to allow assigning result from storage.
        public virtual ICollection<LearningModuleTag> LearningModuleTag { get; set; }
#pragma warning restore CA2227 // Set is required to allow assigning result from storage.

        /// <summary>
        /// Gets or sets user user learning module entity.
        /// </summary>
#pragma warning disable CA2227 // Set is required to allow assigning result from storage.
        public virtual ICollection<UserLearningModule> UserLearningModule { get; set; }
#pragma warning restore CA2227 // Set is required to allow assigning result from storage.
    }
}