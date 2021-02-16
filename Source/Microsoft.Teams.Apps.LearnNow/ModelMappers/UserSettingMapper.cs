// <copyright file="UserSettingMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A UserSetting mapper class that contains methods related to userSetting model mappings.
    /// </summary>
    public class UserSettingMapper : IUserSettingMapper
    {
        /// <summary>
        /// Gets userSetting entity model from view model.
        /// </summary>
        /// <param name="userSettingsModel">UserSetting view model object.</param>
        /// <param name="entityType">Type of entity for which user setting details needs to be created.</param>
        /// <param name="userObjectId">User Azure Active Directory object identifier.</param>
        /// <returns>Returns a userSetting entity model object.</returns>
        public UserSettings CreateMap(UserSettingsModel userSettingsModel, string entityType, Guid userObjectId)
        {
            userSettingsModel = userSettingsModel ?? throw new ArgumentNullException(nameof(userSettingsModel));
            entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

            switch (entityType.ToUpperInvariant())
            {
                case Constants.ResourceEntityType:
                    return new UserSettings
                    {
                        UserId = userObjectId,
                        ResourceCreatedByObjectIds = string.Join(";", userSettingsModel.CreatedByObjectIds),
                        ResourceGradeIds = string.Join(";", userSettingsModel.GradeIds),
                        ResourceSubjectIds = string.Join(";", userSettingsModel.SubjectIds),
                        ResourceTagIds = string.Join(";", userSettingsModel.TagIds),
                    };
                case Constants.LearningModuleEntityType:
                    return new UserSettings
                    {
                        UserId = userObjectId,
                        ModuleCreatedByObjectIds = string.Join(";", userSettingsModel.CreatedByObjectIds),
                        ModuleGradeIds = string.Join(";", userSettingsModel.GradeIds),
                        ModuleSubjectIds = string.Join(";", userSettingsModel.SubjectIds),
                        ModuleTagIds = string.Join(";", userSettingsModel.TagIds),
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets userSetting entity model from view model.
        /// </summary>
        /// <param name="userSettingsModel">UserSetting view model object.</param>
        /// <param name="existingUserSettings">UserSettings entity model object.</param>
        /// <param name="entityType">Type of entity for which user setting details needs to be updated.</param>
        /// <returns>Returns a userSetting entity model object.</returns>
        public UserSettings UpdateMap(UserSettingsModel userSettingsModel, UserSettings existingUserSettings, string entityType)
        {
            userSettingsModel = userSettingsModel ?? throw new ArgumentNullException(nameof(userSettingsModel));
            entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

            switch (entityType.ToUpperInvariant())
            {
                case Constants.ResourceEntityType:
                    existingUserSettings.ResourceGradeIds = string.Join(";", userSettingsModel.GradeIds);
                    existingUserSettings.ResourceSubjectIds = string.Join(";", userSettingsModel.SubjectIds);
                    existingUserSettings.ResourceTagIds = string.Join(";", userSettingsModel.TagIds);
                    existingUserSettings.ResourceCreatedByObjectIds = string.Join(";", userSettingsModel.CreatedByObjectIds);
                    return existingUserSettings;
                case Constants.LearningModuleEntityType:
                    existingUserSettings.ModuleGradeIds = string.Join(";", userSettingsModel.GradeIds);
                    existingUserSettings.ModuleSubjectIds = string.Join(";", userSettingsModel.SubjectIds);
                    existingUserSettings.ModuleTagIds = string.Join(";", userSettingsModel.TagIds);
                    existingUserSettings.ModuleCreatedByObjectIds = string.Join(";", userSettingsModel.CreatedByObjectIds);
                    return existingUserSettings;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets userSetting entity model from view model.
        /// </summary>
        /// <param name="userSetting">UserSetting view model object.</param>
        /// <param name="entityType">Type of entity for which user setting details needs to be updated.</param>
        /// <returns>Returns a userSetting entity model object.</returns>
        public UserSettingsModel CreateMapToViewModel(UserSettings userSetting, string entityType)
        {
            userSetting = userSetting ?? throw new ArgumentNullException(nameof(entityType));
            entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

            switch (entityType.ToUpperInvariant())
            {
                case Constants.ResourceEntityType:
                    return new UserSettingsModel
                    {
                        CreatedByObjectIds = GetListOfGuids(userSetting.ResourceCreatedByObjectIds),
                        GradeIds = GetListOfGuids(userSetting.ResourceGradeIds),
                        SubjectIds = GetListOfGuids(userSetting.ResourceSubjectIds),
                        TagIds = GetListOfGuids(userSetting.ResourceTagIds),
                    };
                case Constants.LearningModuleEntityType:
                    return new UserSettingsModel
                    {
                        CreatedByObjectIds = GetListOfGuids(userSetting.ModuleCreatedByObjectIds),
                        GradeIds = GetListOfGuids(userSetting.ModuleGradeIds),
                        SubjectIds = GetListOfGuids(userSetting.ModuleSubjectIds),
                        TagIds = GetListOfGuids(userSetting.ModuleTagIds),
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        /// Method to get <see cref="Guid"/> collection from semi-colon separated <see cref="Guid"/> string.
        /// </summary>
        /// <param name="ids">Semi-colon separated <see cref="Guid"/> string.</param>
        /// <returns>Returns <see cref="Guid"/> collection.</returns>
        private static IEnumerable<Guid> GetListOfGuids(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return new List<Guid>();
            }

            return ids.Split(';').Select(item => Guid.Parse(item));
        }
    }
}