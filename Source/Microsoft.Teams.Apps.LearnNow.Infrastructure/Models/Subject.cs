// <copyright file="Subject.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class which represents Subject entity model.
    /// </summary>
    public partial class Subject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subject"/> class.
        /// </summary>
        public Subject()
        {
            this.Resource = new HashSet<Resource>();
            this.LearningModule = new HashSet<LearningModule>();
        }

        /// <summary>
        /// Gets or sets id of subject.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name of subject.
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who created the subject.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who updated the subject.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets created on UTC date time.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets updated on UTC date time.
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets resource entity.
        /// </summary>
#pragma warning disable CA2227 // Set is required to allow assigning result from storage.
        public virtual ICollection<Resource> Resource { get; set; }
#pragma warning restore CA2227 // Set is required to allow assigning result from storage.

        /// <summary>
        /// Gets or sets collection of learning module entity.
        /// </summary>
#pragma warning disable CA2227 // Set is required to allow assigning result from storage.
        public virtual ICollection<LearningModule> LearningModule { get; set; }
#pragma warning restore CA2227 // Set is required to allow assigning result from storage.
    }
}