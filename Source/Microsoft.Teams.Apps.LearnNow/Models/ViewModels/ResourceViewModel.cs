// <copyright file="ResourceViewModel.cs" company="Microsoft Corporation">
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
    /// Model to handle resource details.
    /// </summary>
    public class ResourceViewModel
    {
        /// <summary>
        /// Gets or sets resource id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets resource title
        /// </summary>
        [Required]
        [MaxLength(75)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets resource description.
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets resource subject id.
        /// </summary>
        [GuidValidation]
        public Guid SubjectId { get; set; }

        /// <summary>
        /// Gets or sets resource grade id.
        /// </summary>
        [GuidValidation]
        public Guid GradeId { get; set; }

        /// <summary>
        /// Gets or sets resource image URL.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets resource link URL.
        /// </summary>
        [MaxLength(400)]
        [RegularExpression("^((https)://)(www.)?" + "[a-zA-Z0-9@:%._\\+~#?&//=]{2,256}\\.[a-z]" + "{2,6}\\b([-a-zA-Z0-9@:%._\\+~#?&//=]*)")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets resource attachment URL.
        /// </summary>
        [MaxLength(500)]
        public string AttachmentUrl { get; set; }

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
        [Required]
        [RegularExpression("([1-5])")]
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
        /// Gets or sets vote count
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is liked by current user or not.
        /// </summary>
        public bool IsLikedByUser { get; set; }

        /// <summary>
        /// Gets or sets display name of user who created the resource.
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets resource tag details.
        /// </summary>
        public IEnumerable<ResourceTag> ResourceTag { get; set; }
    }
}
