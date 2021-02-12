// <copyright file="AdaptiveSubmitActionData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Adaptive submit card action data to create adaptive card related data.
    /// </summary>
    public class AdaptiveSubmitActionData
    {
        /// <summary>
        /// Gets or sets adaptive action type.
        /// </summary>
        [JsonProperty("AdaptiveActionType")]
        public string AdaptiveActionType { get; set; }

        /// <summary>
        /// Gets or sets post id action type.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
