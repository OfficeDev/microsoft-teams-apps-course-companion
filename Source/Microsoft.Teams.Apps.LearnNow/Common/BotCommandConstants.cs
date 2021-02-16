// <copyright file="BotCommandConstants.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Common
{
    /// <summary>
    /// Class that holds bot command constants that are used in multiple files.
    /// </summary>
    public static class BotCommandConstants
    {
        /// <summary>
        /// ME resource CommandId.
        /// </summary>
        public const string ResourceCommandId = "resource";

        /// <summary>
        /// ME learning module CommandId.
        /// </summary>
        public const string LearningModuleCommandId = "learningmodule";

        /// <summary>
        /// Adaptive action type to feth learning module card.
        /// </summary>
        public const string ViewLearningModule = "LEARNINGMODULEDETAIL";

        /// <summary>
        /// Adaptive action type to feth resource card.
        /// </summary>
        public const string ViewResource = "RESOURCEDETAIL";
    }
}