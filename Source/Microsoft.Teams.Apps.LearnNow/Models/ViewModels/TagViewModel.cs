// <copyright file="TagViewModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model to handle tag details.
    /// </summary>
    public class TagViewModel
    {
        /// <summary>
        /// Gets or sets id of tag entity model.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name of tag entity model.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets display name of user who updated the tag.
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets tag updated on date.
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }
    }
}