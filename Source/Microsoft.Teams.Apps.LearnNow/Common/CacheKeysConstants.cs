// <copyright file="CacheKeysConstants.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Common
{
    /// <summary>
    /// Constants to list keys used by cache layers in application.
    /// </summary>
    public static class CacheKeysConstants
    {
        /// <summary>
        /// Cache key for Team members.
        /// </summary>
        public const string TeamMember = "_Tm";

        /// <summary>
        /// Cache key for resource card payload.
        /// </summary>
        public const string ResourceCardJSONTemplate = "_RCTemplate";

        /// <summary>
        /// Cache key for learning module card payload.
        /// </summary>
        public const string LearningModuleCardJSONTemplate = "_LMCTemplate";
    }
}