// <copyright file="SubjectMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;
    using Microsoft.Teams.Apps.LearnNow.Models;

    /// <summary>
    /// A Subject mapper class that contains methods related to model mappings.
    /// </summary>
    public class SubjectMapper : ISubjectMapper
    {
        /// <summary>
        /// Gets subject entity model from view model.
        /// </summary>
        /// <param name="subjectViewModel">Subject view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a subject entity model object.</returns>
        public Subject MapToDTO(
            SubjectViewModel subjectViewModel,
            Guid userAadObjectId)
        {
            subjectViewModel = subjectViewModel ?? throw new ArgumentNullException(nameof(subjectViewModel));

            return new Subject
            {
                SubjectName = subjectViewModel.SubjectName,
                CreatedBy = userAadObjectId,
                UpdatedBy = userAadObjectId,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
            };
        }

        /// <summary>
        /// Gets subject view model from entity model.
        /// </summary>
        /// <param name="subjects">Collection of subject entity model objects.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns collection of subject view model objects.</returns>
        public IEnumerable<SubjectViewModel> MapToViewModel(
            IEnumerable<Subject> subjects,
            Dictionary<Guid, string> idToNameMap)
        {
            subjects = subjects ?? throw new ArgumentNullException(nameof(subjects));

            return subjects.Select(subject => new SubjectViewModel
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                UpdatedOn = subject.UpdatedOn,
                UserDisplayName = idToNameMap[subject.CreatedBy],
            });
        }
    }
}
