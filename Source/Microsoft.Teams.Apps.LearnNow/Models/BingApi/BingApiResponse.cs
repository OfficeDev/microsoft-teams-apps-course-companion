// <copyright file="BingApiResponse.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models.BingApiRequestModel
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Used to deserialize Bing search result.
    /// </summary>
    public class BingApiResponse
    {
        /// <summary>
        /// Gets or sets Images.
        /// </summary>
        [JsonProperty("value")]
        public IEnumerable<Images> Images { get; set; }
    }
}
