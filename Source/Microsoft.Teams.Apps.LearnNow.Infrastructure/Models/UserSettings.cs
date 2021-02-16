// <copyright file="UserSettings.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;

    /// <summary>
    /// A class which represents UserSettings entity model.
    /// </summary>
    public partial class UserSettings
    {
        /// <summary>
        /// Gets or sets user AAD id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets semi-colon separated grade Ids for resource filter.
        /// </summary>
        public string ResourceGradeIds { get; set; }

        /// <summary>
        /// Gets or sets semi-colon separated subject Ids for resource filter.
        /// </summary>
        public string ResourceSubjectIds { get; set; }

        /// <summary>
        /// Gets or sets semi-colon separated tag Ids for resource filter.
        /// </summary>
        public string ResourceTagIds { get; set; }

        /// <summary>
        /// Gets or sets semi-colon separated resource creted by object Ids for resource filter.
        /// </summary>
        public string ResourceCreatedByObjectIds { get; set; }

        /// <summary>
        /// Gets or sets semi-colon separated grade Ids for module filter.
        /// </summary>
        public string ModuleGradeIds { get; set; }

        /// <summary>
        /// Gets or sets semi-colon separated subject Ids for module filter.
        /// </summary>
        public string ModuleSubjectIds { get; set; }

        /// <summary>
        /// Gets or sets semi-colon separated tag Ids for module filter.
        /// </summary>
        public string ModuleTagIds { get; set; }

        /// <summary>
        /// Gets or sets semi-colon separated created by object Ids for module filter.
        /// </summary>
        public string ModuleCreatedByObjectIds { get; set; }
    }
}