// <copyright file="ContentType.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Models
{
    /// <summary>
    /// Supported file types for upload file to blob.
    /// </summary>
    public static class ContentType
    {
        /// <summary>
        /// Office Open XML worksheet sheet format.
        /// </summary>
        public const string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// Office Open XML word document format.
        /// </summary>
        public const string Word = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        /// <summary>
        /// Office Open XML Power point presentation format.
        /// </summary>
        public const string PPT = "application/vnd.openxmlformats-officedocument.presentationml.presentation";

        /// <summary>
        /// PDF content type.
        /// </summary>
        public const string PDF = "application/pdf";
    }
}
