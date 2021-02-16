// <copyright file="Grade.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class which represents Grade entity model.
    /// </summary>
    public partial class Grade
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Grade"/> class.
        /// </summary>
        public Grade()
        {
            this.LearningModule = new HashSet<LearningModule>();
            this.Resource = new HashSet<Resource>();
        }

        /// <summary>
        /// Gets or sets id of grade entity model.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name of grade entity model.
        /// </summary>
        public string GradeName { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who created the grade.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who updated the grade.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets created on UTC date time of grade entity model.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets updated on UTC date time of grade entity model.
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets learning modules of grade entity model.
        /// </summary>
#pragma warning disable CA2227 // Set is required to allow assigning result from storage.
        public virtual ICollection<LearningModule> LearningModule { get; set; }
#pragma warning restore CA2227 // Set is required to allow assigning result from storage.

        /// <summary>
        /// Gets or sets resources of grade entity model.
        /// </summary>
#pragma warning disable CA2227 // Set is required to allow assigning result from storage.
        public virtual ICollection<Resource> Resource { get; set; }
#pragma warning restore CA2227 // Set is required to allow assigning result from storage.
    }
}
