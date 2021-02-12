// <copyright file="SubjectViewModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model to handle subject details.
    /// </summary>
    public class SubjectViewModel
    {
        /// <summary>
        /// Gets or sets id of subject.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name of subject.
        /// </summary>
        [Required]
        [MaxLength(25)]
        public string SubjectName { get; set; }

        /// <summary>
        /// Gets or sets display name of user who updated the subject.
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets subject updated on date.
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }
    }
}