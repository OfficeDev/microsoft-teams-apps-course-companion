// <copyright file="TabConfiguration.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;

    /// <summary>
    /// A class which represents tab configuration entity model, this class is used to store the mapping of teams tab with learning module.
    /// </summary>
    public partial class TabConfiguration
    {
        /// <summary>
        /// Gets or sets id of tab configuration entity model.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets group id of team where tab is configured.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets team id of tab configuration entity model.
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// Gets or sets channel id of tab configuration entity model.
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or sets learning module id of tab configuration entity model.
        /// </summary>
        public Guid LearningModuleId { get; set; }

        /// <summary>
        /// Gets or sets tab configuration created on date.
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets tab configuration updated on date.
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets tab configuration created by.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets tab configuration updated by.
        /// </summary>
        public Guid UpdatedBy { get; set; }
    }
}