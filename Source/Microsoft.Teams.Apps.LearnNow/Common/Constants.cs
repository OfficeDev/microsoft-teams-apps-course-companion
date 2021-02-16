// <copyright file="Constants.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Common
{
    /// <summary>
    /// Class that holds application constants that are used in multiple files.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Constant value for policy name.
        /// </summary>
        public const string SharedAccessPolicyName = "policy";

        /// <summary>
        /// Authorization scheme.
        /// </summary>
        public const string BearerAuthorizationScheme = "Bearer";

        /// <summary>
        /// Root container name on Azure Blob storage from which file will be uploaded or downloaded.
        /// </summary>
        public const string BaseContainerName = "resource-attachments";

        /// <summary>
        /// Per page resource/learningModule count for lazy loading rendered on discover tab.
        /// </summary>
        public const int LazyLoadPerPagePostCount = 10;

        /// <summary>
        /// Default value for channel activity to send notifications.
        /// </summary>
        public const string TeamsBotFrameworkChannelId = "msteams";

        /// <summary>
        /// Value of oidClaim to get user claim.
        /// </summary>
        public const string OidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// Max description length of learning module.
        /// </summary>
        public const int LearningModuleCardDescriptionLength = 148;

        /// <summary>
        /// Max description length of resource.
        /// </summary>
        public const int ResourceCardDescriptionLength = 191;

        /// <summary>
        /// Task module hieght.
        /// </summary>
        public const int TaskModuleHeight = 600;

        /// <summary>
        /// Task module width.
        /// </summary>
        public const int TaskModuleWidth = 600;

        /// <summary>
        /// Constant value for resource entity type.
        /// </summary>
        public const string ResourceEntityType = "RESOURCE";

        /// <summary>
        /// Constant value for learning module entity type.
        /// </summary>
        public const string LearningModuleEntityType = "LEARNINGMODULE";
    }
}