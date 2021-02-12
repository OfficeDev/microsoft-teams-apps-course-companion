// <copyright file="SubjectRepository.cs" company="Microsoft Corporation">
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
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public SubjectRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        ///  Method to track Subject entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="subjectsCollection"> List of Subjects that needs to be deleted.</param>
        public void DeleteSubjects(IEnumerable<Subject> subjectsCollection)
        {
            this.context.Subject.RemoveRange(subjectsCollection);
        }
    }
}
