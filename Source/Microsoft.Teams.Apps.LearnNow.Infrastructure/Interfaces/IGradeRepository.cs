// <copyright file="IGradeRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories;

    /// <summary>
    /// Interface for handling operations related to Grade entity collection.
    /// </summary>
    public interface IGradeRepository : IRepository<Grade>
    {
        /// <summary>
        /// Gets repository for handling operations on Grade entity.
        /// </summary>
        /// <param name="gradesCollection"> List of grades that needs to be deleted.</param>
        void DeleteGrades(IEnumerable<Grade> gradesCollection);
    }
}