// <copyright file="TabConfigurationViewModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Teams.Apps.LearnNow.Helpers.CustomValidations;

    /// <summary>
    /// A class which represents tab configuration entity model.
    /// </summary>
    public partial class TabConfigurationViewModel
    {
        /// <summary>
        /// Gets or sets id of tab configuration entity model.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets team id of tab configuration entity model.
        /// </summary>
        [Required]
        public string TeamId { get; set; }

        /// <summary>
        /// Gets or sets channel id of tab configuration entity model.
        /// </summary>
        [Required]
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or sets learning module id of tab configuration entity model.
        /// </summary>
        [Required]
        [GuidValidation]
        public Guid LearningModuleId { get; set; }
    }
}