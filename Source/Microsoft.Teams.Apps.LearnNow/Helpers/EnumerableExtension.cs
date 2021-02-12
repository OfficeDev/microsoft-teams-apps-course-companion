// <copyright file="EnumerableExtension.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class lists the extension methods for enumerable collections.
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// Indicates whether a collection is null or it has length equal to 0.
        /// </summary>
        /// <typeparam name="T">The type of objects in collection.</typeparam>
        /// <param name="enumerable">The collection of a specified type.</param>
        /// <returns>Returns true if a collection is null or it has length equal to 0. Else returns false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }
}