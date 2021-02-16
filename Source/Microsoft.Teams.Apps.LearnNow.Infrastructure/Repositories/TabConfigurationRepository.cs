// <copyright file="TabConfigurationRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with TabConfiguration entity collection.
    /// </summary>
    public class TabConfigurationRepository : BaseRepository<TabConfiguration>, ITabConfigurationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabConfigurationRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public TabConfigurationRepository(LearnNowContext context)
            : base(context)
        {
        }
    }
}