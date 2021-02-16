// <copyright file="GradeMapper.cs" company="Microsoft Corporation">
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
    /// A model class that contains methods related to grade model mappings.
    /// </summary>
    public class GradeMapper : IGradeMapper
    {
        /// <summary>
        /// Gets grade model from view model.
        /// </summary>
        /// <param name="gradeViewModel">Grade entity view model object.</param>
        /// <param name="userAadObjectId">Azure Active Directory id of current logged-in user.</param>
        /// <returns>Returns a grade entity model object.</returns>
        public Grade MapToDTO(
            GradeViewModel gradeViewModel,
            Guid userAadObjectId)
        {
            gradeViewModel = gradeViewModel ?? throw new ArgumentNullException(nameof(gradeViewModel));

            return new Grade
            {
                GradeName = gradeViewModel.GradeName,
                CreatedBy = userAadObjectId,
                UpdatedBy = userAadObjectId,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
            };
        }

        /// <summary>
        /// Gets grade view model from entity model.
        /// </summary>
        /// <param name="grades">Collection of grade entity model objects.</param>
        /// <param name="idToNameMap">User id and name key value pairs.</param>
        /// <returns>Returns collection of grade view model objects.</returns>
        public IEnumerable<GradeViewModel> MapToViewModel(
            IEnumerable<Grade> grades,
            Dictionary<Guid, string> idToNameMap)
        {
            grades = grades ?? throw new ArgumentNullException(nameof(grades));

            return grades.Select(grade => new GradeViewModel
            {
                Id = grade.Id,
                GradeName = grade.GradeName,
                UpdatedOn = grade.UpdatedOn,
                UserDisplayName = idToNameMap[grade.CreatedBy],
            });
        }
    }
}