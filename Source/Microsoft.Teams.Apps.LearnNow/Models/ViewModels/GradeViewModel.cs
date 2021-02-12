// <copyright file="GradeViewModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model to handle grade details.
    /// </summary>
    public class GradeViewModel
    {
        /// <summary>
        /// Gets or sets id of grade.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name of grade.
        /// </summary>
        [Required]
        [MaxLength(25)]
        public string GradeName { get; set; }

        /// <summary>
        /// Gets or sets display name of user who updated the grade.
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets grade updated on date.
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }
    }
}