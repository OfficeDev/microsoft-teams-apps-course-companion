// <copyright file="UserSettingRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with UserSetting entity collection.
    /// </summary>
    public class UserSettingRepository : BaseRepository<UserSettings>, IUserSettingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public UserSettingRepository(LearnNowContext context)
            : base(context)
        {
        }
    }
}