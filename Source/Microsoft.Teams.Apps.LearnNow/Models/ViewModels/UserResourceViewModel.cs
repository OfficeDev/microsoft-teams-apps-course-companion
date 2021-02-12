// <copyright file="UserResourceViewModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Teams.Apps.LearnNow.Helpers.CustomValidations;

    /// <summary>
    /// Model to handle user resource details.
    /// </summary>
    public class UserResourceViewModel
    {
        /// <summary>
        /// Gets or sets user resource resource id.
        /// </summary>
        [Required]
        [GuidValidation]
        public Guid ResourceId { get; set; }
    }
}