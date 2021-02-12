// <copyright file="IUserSettingMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.ModelMappers
{
    using System;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// Interface for handling operations related to userSetting model mappings.
    /// </summary>
    public interface IUserSettingMapper
    {
        /// <summary>
        /// Gets userSetting entity model from view model.
        /// </summary>
        /// <param name="userSettingsModel">UserSetting view model object.</param>
        /// <param name="entityType">Type of entity for which user setting details needs to be created.</param>
        /// <param name="userObjectId">User Azure Active Directory object identifier.</param>
        /// <returns>Returns a userSetting entity model object.</returns>
        public UserSettings CreateMap(UserSettingsModel userSettingsModel, string entityType, Guid userObjectId);

        /// <summary>
        /// Gets userSetting entity model from view model.
        /// </summary>
        /// <param name="userSettingsModel">UserSetting view model object.</param>
        /// <param name="existingUserSettings">UserSettings entity model object.</param>
        /// <param name="entityType">Type of entity for which user setting details needs to be updated.</param>
        /// <returns>Returns a userSetting entity model object.</returns>
        public UserSettings UpdateMap(UserSettingsModel userSettingsModel, UserSettings existingUserSettings, string entityType);

        /// <summary>
        /// Gets userSetting entity model from view model.
        /// </summary>
        /// <param name="userSetting">UserSetting view model object.</param>
        /// <param name="entityType">Type of entity for which user setting details needs to be updated.</param>
        /// <returns>Returns a userSetting entity model object.</returns>
        public UserSettingsModel CreateMapToViewModel(UserSettings userSetting, string entityType);
    }
}