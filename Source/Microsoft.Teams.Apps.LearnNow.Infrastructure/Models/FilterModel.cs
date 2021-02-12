// <copyright file="FilterModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class handles User filter setting.
    /// </summary>
    public class FilterModel
    {
        /// <summary>
        /// Gets or sets subject Ids which will be used to filter resources or learning module.
        /// </summary>
        public IEnumerable<Guid> SubjectIds { get; set; }

        /// <summary>
        /// Gets or sets grade Ids which will be used to filter resources or learning module.
        /// </summary>
        public IEnumerable<Guid> GradeIds { get; set; }

        /// <summary>
        /// Gets or sets tag Ids which will be used to filter resources or learning module.
        /// </summary>
        public IEnumerable<Guid> TagIds { get; set; }

        /// <summary>
        /// Gets or sets created by AAD Ids which will be used to filter resources or learning module.
        /// </summary>
        public IEnumerable<Guid> CreatedByObjectIds { get; set; }

        /// <summary>
        /// Gets or sets search text for filter.
        /// </summary>
        public string SearchText { get; set; }
    }
}