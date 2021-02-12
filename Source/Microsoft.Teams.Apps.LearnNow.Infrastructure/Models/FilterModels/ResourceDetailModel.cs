// <copyright file="ResourceDetailModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A class which represents Resource detail model.
    /// </summary>
    public class ResourceDetailModel : Resource
    {
        /// <summary>
        /// Gets or sets resource votes.
        /// </summary>
        public IEnumerable<ResourceVote> Votes { get; set; }
    }
}