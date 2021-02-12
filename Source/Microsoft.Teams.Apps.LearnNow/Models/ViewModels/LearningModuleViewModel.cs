// <copyright file="LearningModuleViewModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Teams.Apps.LearnNow.Helpers.CustomValidations;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// Model to handle learning module details.
    /// </summary>
    public class LearningModuleViewModel
    {
        /// <summary>
        /// Gets or sets learning module id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets learning module title
        /// </summary>
        [Required]
        [MaxLength(75)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets learning module description.
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets learning module subject id.
        /// </summary>
        [GuidValidation]
        public Guid SubjectId { get; set; }

        /// <summary>
        /// Gets or sets learning module grade id.
        /// </summary>
        [GuidValidation]
        public Guid GradeId { get; set; }

        /// <summary>
        /// Gets or sets learning module image URL.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets learning module created on date.
        /// </summary>
        public DateTimeOffset? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets learning module updates on date.
        /// </summary>
        public DateTimeOffset? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who created the learning module.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of user who updated the learning module.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets learning module grade details.
        /// </summary>
        public Grade Grade { get; set; }

        /// <summary>
        /// Gets or sets learning module subject details.
        /// </summary>
        public Subject Subject { get; set; }

        /// <summary>
        /// Gets or sets vote count
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether learning module is liked by user.
        /// </summary>
        public bool IsLikedByUser { get; set; }

        /// <summary>
        /// Gets or sets display name of user who created the learning module.
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets learning module tag details.
        /// </summary>
        public IEnumerable<LearningModuleTag> LearningModuleTag { get; set; }

        /// <summary>
        /// Gets or sets associated resource count.
        /// </summary>
        public int ResourceCount { get; set; }
    }
}