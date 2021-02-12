// <copyright file="BaseRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The Base repository class contains all common methods to work with collection.
    /// </summary>
    /// <typeparam name="T">The type of objects.</typeparam>
    public abstract class BaseRepository<T> : IRepository<T>
        where T : class
    {
        /// <summary>
        /// Entity framework database context class to work with entities.
        /// </summary>
#pragma warning disable SA1401 // Context needs to be accessed by child classes.
#pragma warning disable CA1051 // Context needs to be accessed by child classes.
        protected LearnNowContext context;
#pragma warning restore CA1051 // Context needs to be accessed by child classes.
#pragma warning restore SA1401 // Context needs to be accessed by child classes.

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The Entity framework database context class to work with entities.</param>
        public BaseRepository(LearnNowContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Handles adding new entity.
        /// </summary>
        /// <param name="entity">Entity that is being saved to database.</param>
        /// <returns>Returns entity data that is saved to database.</returns>
        public virtual T Add(T entity)
        {
            return this.context
                .Add(entity)
                .Entity;
        }

        /// <summary>
        /// Handles getting entity based on entity identifier.
        /// </summary>
        /// <param name="id">Entity id using which entity is being fetched from underlying storage.</param>
        /// <returns>Returns the entity that matches given identifier.</returns>
        public virtual async Task<T> GetAsync(Guid id)
        {
            return await this.context.FindAsync<T>(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles getting all entities from database.
        /// </summary>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this.context.Set<T>()
                .AsQueryable()
                .ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handles update of entity.
        /// </summary>
        /// <param name="entity">Entity that is being updated.</param>
        /// <returns>Returns entity data that is saved to database.</returns>
        public virtual T Update(T entity)
        {
            return this.context.Update(entity)
                .Entity;
        }

        /// <summary>
        /// Handles deletion of entity.
        /// </summary>
        /// <param name="entity">Entity that is being deleted.</param>
        /// <returns>Returns entity data that is deleted from database.</returns>
        public virtual T Delete(T entity)
        {
            this.context.Attach(entity);
            return this.context.Remove(entity).Entity;
        }

        /// <summary>
        /// Handles filtering entity based on expression.
        /// </summary>
        /// <param name="predicate">Expression that is being used to filter entities from database.</param>
        /// <returns>Returns collection of filtered entities using expression.</returns>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await this.context.Set<T>()
                .AsQueryable()
                .Where(predicate).ToListAsync().ConfigureAwait(false);
        }
    }
}
