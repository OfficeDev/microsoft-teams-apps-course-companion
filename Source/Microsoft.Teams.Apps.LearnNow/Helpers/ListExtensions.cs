// <copyright file="ListExtensions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Helpers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class providing extension methods for List type.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// MS Graph batch limit is 20
        /// https://docs.microsoft.com/en-us/graph/known-issues#json-batching.
        /// </summary>
        private const int BatchSplitCount = 20;

        /// <summary>
        /// This method is to split list into given batch size.
        /// </summary>
        /// <typeparam name="T">T type.</typeparam>
        /// <param name="source">Source list to split.</param>
        /// <returns>A <see cref="IEnumerable{TResult}"/> representing the sub-lists by specified size.</returns>
        public static IEnumerable<List<T>> SplitList<T>(this List<T> source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));

            for (int i = 0; i < source.Count; i += BatchSplitCount)
            {
                yield return source.GetRange(i, Math.Min(BatchSplitCount, source.Count - i));
            }
        }
    }
}
