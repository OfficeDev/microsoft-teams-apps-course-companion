// <copyright file="BingSearchServiceSettings.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models.Configuration
{
    /// <summary>
    ///  A class that represents settings related to Bing search API.
    /// </summary>
    public class BingSearchServiceSettings
    {
        /// <summary>
        /// Gets or Sets Bing search subscription key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or Sets Bing search API end point.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or Sets Bing search API SafeSearch.
        /// </summary>
        public string SafeSearch { get; set; }
    }
}
