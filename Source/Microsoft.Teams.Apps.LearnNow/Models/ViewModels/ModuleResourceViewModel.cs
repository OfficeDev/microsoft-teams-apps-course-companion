// <copyright file="ModuleResourceViewModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Class contains learning module resource details.
    /// </summary>
    public class ModuleResourceViewModel
    {
        /// <summary>
        /// Gets or sets learning module.
        /// </summary>
        public LearningModuleViewModel LearningModule { get; set; }

        /// <summary>
        /// Gets or sets learning module associated resource collection.
        /// </summary>
        public IEnumerable<ResourceViewModel> Resources { get; set; }
    }
}