// <copyright file="ResourceTagRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Teams.Apps.LearnNow.Infrastructure.Models;

    /// <summary>
    /// A repository class contains all common methods to work with Tag entity collection.
    /// </summary>
    public class ResourceTagRepository : BaseRepository<ResourceTag>, IResourceTagRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTagRepository"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public ResourceTagRepository(LearnNowContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Handles getting all entities from database.
        /// </summary>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        public override async Task<IEnumerable<ResourceTag>> GetAllAsync()
        {
            return await this.context.Set<ResourceTag>()
                .Include(x => x.Tag)
                .ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///  Method to track tag entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="resourceTagsCollection"> Tag collection entity that needs to be deleted.</param>
        public void Delete(IEnumerable<ResourceTag> resourceTagsCollection)
        {
            this.context.ResourceTag.RemoveRange(resourceTagsCollection);
        }

        /// <summary>
        ///  Method to track tag entities to be deleted on context save changes call.
        /// </summary>
        /// <param name="resourceTagsCollection"> Tag collection entity that needs to be deleted.</param>
        public void Add(IEnumerable<ResourceTag> resourceTagsCollection)
        {
            this.context.ResourceTag.AddRange(resourceTagsCollection);
        }
    }
}
