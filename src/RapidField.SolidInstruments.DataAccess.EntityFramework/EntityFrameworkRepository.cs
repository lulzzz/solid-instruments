﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RapidField.SolidInstruments.Core.ArgumentValidation;
using RapidField.SolidInstruments.Core.Concurrency;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace RapidField.SolidInstruments.DataAccess.EntityFramework
{
    /// <summary>
    /// Performs data access operations against an Entity Framework entity type using a single transaction.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity.
    /// </typeparam>
    /// <typeparam name="TContext">
    /// The type of the database session for the repository.
    /// </typeparam>
    public class EntityFrameworkRepository<TEntity, TContext> : DataAccessRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkRepository{TEntity, TContext}" /> class.
        /// </summary>
        /// <param name="context">
        /// The database session for the repository.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context" /> is <see langword="null" />.
        /// </exception>
        public EntityFrameworkRepository(TContext context)
            : base()
        {
            Context = context.RejectIf().IsNull(nameof(context));
            Set = Context.Set<TEntity>();
        }

        /// <summary>
        /// Adds the specified entity to the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="entity">
        /// The entity to add.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected override void Add(TEntity entity, ConcurrencyControlToken controlToken) => Set.Add(entity);

        /// <summary>
        /// Adds the specified entities to the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="entities">
        /// The entities to add.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected override void AddRange(IEnumerable<TEntity> entities, ConcurrencyControlToken controlToken) => Set.AddRange(entities);

        /// <summary>
        /// Returns all entities from the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        /// <returns>
        /// All entities within the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </returns>
        protected override IQueryable<TEntity> All(ConcurrencyControlToken controlToken) => Set.AsQueryable();

        /// <summary>
        /// Determines whether or not any entities matching the specified predicate exist in the current
        /// <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="predicate">
        /// An expression to test each entity for a condition.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if any entities matching the specified predicate exist in the current
        /// <see cref="EntityFrameworkRepository{TEntity, TContext}" />, otherwise <see langword="false" />.
        /// </returns>
        protected override Boolean AnyWhere(Expression<Func<TEntity, Boolean>> predicate, ConcurrencyControlToken controlToken) => Set.AsNoTracking().Any(predicate);

        /// <summary>
        /// Determines whether or not the specified entity exists in the current
        /// <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="entity">
        /// The entity to evaluate.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the specified entity exists in the current
        /// <see cref="EntityFrameworkRepository{TEntity, TContext}" />, otherwise <see langword="false" />.
        /// </returns>
        protected override Boolean Contains(TEntity entity, ConcurrencyControlToken controlToken)
        {
            var entityType = Context.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey();
            var keyValues = new Object[primaryKey.Properties.Count];

            for (var i = 0; i < keyValues.Length; i++)
            {
                keyValues[i] = primaryKey.Properties[i].GetGetter().GetClrValue(entity);
            }

            var trackedEntity = Set.Find(keyValues);

            if (trackedEntity is null)
            {
                return false;
            }

            var trackedEntityEntry = Context.Entry(trackedEntity);

            if (trackedEntityEntry.State == EntityState.Unchanged)
            {
                // Permit consuming code to invoke operations that implicitly attach the entity.
                trackedEntityEntry.State = EntityState.Detached;
            }

            return true;
        }

        /// <summary>
        /// Returns the number of entities in the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        /// <returns>
        /// The number of entities in the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </returns>
        protected override Int64 Count(ConcurrencyControlToken controlToken) => Set.AsNoTracking().Count();

        /// <summary>
        /// Returns the number of entities matching the specified predicate in the current
        /// <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="predicate">
        /// An expression to test each entity for a condition.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        /// <returns>
        /// The number of entities matching the specified predicate in the current
        /// <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </returns>
        protected override Int64 CountWhere(Expression<Func<TEntity, Boolean>> predicate, ConcurrencyControlToken controlToken) => Set.AsNoTracking().Count(predicate);

        /// <summary>
        /// Releases all resources consumed by the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not managed resources should be released.
        /// </param>
        protected override void Dispose(Boolean disposing) => base.Dispose(disposing);

        /// <summary>
        /// Returns all entities matching the specified predicate from the current
        /// <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="predicate">
        /// An expression to test each entity for a condition.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        /// <returns>
        /// All entities matching the specified predicate within the current
        /// <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </returns>
        protected override IQueryable<TEntity> FindWhere(Expression<Func<TEntity, Boolean>> predicate, ConcurrencyControlToken controlToken) => Set.Where(predicate);

        /// <summary>
        /// Removes the specified entity from the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="entity">
        /// The entity to remove.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected override void Remove(TEntity entity, ConcurrencyControlToken controlToken) => Set.Remove(entity);

        /// <summary>
        /// Removes the specified entities from the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="entities">
        /// The entities to remove.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected override void RemoveRange(IEnumerable<TEntity> entities, ConcurrencyControlToken controlToken) => Set.RemoveRange(entities);

        /// <summary>
        /// Updates the specified entity in the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="entity">
        /// The entity to update.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected override void Update(TEntity entity, ConcurrencyControlToken controlToken) => Set.Update(entity);

        /// <summary>
        /// Updates the specified entities in the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        /// <param name="entities">
        /// The entities to update.
        /// </param>
        /// <param name="controlToken">
        /// A token that ensures thread safety for the operation.
        /// </param>
        protected override void UpdateRange(IEnumerable<TEntity> entities, ConcurrencyControlToken controlToken) => Set.UpdateRange(entities);

        /// <summary>
        /// Represents the database session for the current <see cref="EntityFrameworkRepository{TEntity, TContext}" />.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TContext Context;

        /// <summary>
        /// Represents an object that is used to perform queries against entities in the database.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbSet<TEntity> Set;
    }
}