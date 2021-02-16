// <copyright file="LearningModuleCardViewModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    /// <summary>
    /// Model to handle learning module card related details.
    /// </summary>
    public class LearningModuleCardViewModel : LearningModuleDetail
    {
        /// <summary>
        /// Gets or sets title label.
        /// </summary>
        public string TitleLabel { get; set; }

        /// <summary>
        /// Gets or sets grade label.
        /// </summary>
        public string GradeLabel { get; set; }

        /// <summary>
        /// Gets or sets subject label.
        /// </summary>
        public string SubjectLabel { get; set; }

        /// <summary>
        /// Gets or sets description label.
        /// </summary>
        public string DescriptionLabel { get; set; }

        /// <summary>
        /// Gets or sets grade name.
        /// </summary>
        public string GradeName { get; set; }

        /// <summary>
        /// Gets or sets subject name.
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// Gets or sets tags.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets tag label.
        /// </summary>
        public string TagLabel { get; set; }

        /// <summary>
        /// Gets or sets task module related data object.
        /// </summary>
        public AdaptiveSubmitActionData TaskModuleData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tag should be visible.
        /// </summary>
        public bool IsTagVisible { get; set; }

        /// <summary>
        /// Gets or sets view detail label.
        /// </summary>
        public string ViewDetailLabel { get; set; }

        /// <summary>
        /// Gets or sets value indicating number of resource associated with learning module.
        /// </summary>
        public int ResourceCount { get; set; }
    }
}