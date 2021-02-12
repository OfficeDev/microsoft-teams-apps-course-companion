// <copyright file="GradeRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with Grade entity collection.
    /// </summary>
    public class GradeRepository : BaseRepository<Grade>, IGradeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GradeRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public GradeRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        ///  Method to track Grade entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="gradesCollection"> List of Grades that needs to be deleted.</param>
        public void DeleteGrades(IEnumerable<Grade> gradesCollection)
        {
            this.context.Grade.RemoveRange(gradesCollection);
        }
    }
}
