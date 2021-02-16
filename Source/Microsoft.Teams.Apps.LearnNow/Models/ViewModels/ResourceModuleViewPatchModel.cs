// <copyright file="ResourceModuleViewPatchModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Class contains resource learning module details.
    /// </summary>
    public class ResourceModuleViewPatchModel
    {
        /// <summary>
        /// Gets or sets LearningModule.
        /// </summary>
        public LearningModuleViewModel LearningModule { get; set; }

        /// <summary>
        /// Gets or sets learning module resource list.
        /// </summary>
        public IEnumerable<ResourceViewModel> Resources { get; set; }
    }
}