using FunctionalExtensions.Base.Results;
using static FunctionalExtensions.Base.Results.ResultExtensions;
using static FunctionalExtensions.Base.Try;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FunctionalExtensions.GenericProvider
{
    /// <summary>
    /// Base asyncronous data provider for an EF DbSet-wrapped class
    /// </summary>
    /// <typeparam name="TKey">Type of key in model</typeparam>
    /// <typeparam name="TEntity">Type of entity inheriting BaseModel</typeparam>
    /// <typeparam name="TContext">Type of DbContext</typeparam>
    public abstract class BaseProvider<TKey, TEntity, TContext>
            where TEntity : BaseModel<TKey>
            where TContext : DbContext
    {
        /// <summary>
        /// EF database context
        /// </summary>
        protected TContext _dbContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">Used database context</param>
        public BaseProvider(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Fetch all entites of table
        /// </summary>
        /// <returns>DataResult of list of entity</returns>
        public async Task<DataResult<List<TEntity>>> FetchAll() =>
            await TryCatchAsync(
                async () => await _dbContext.Set<TEntity>()
                                            .ToListAsync(),
                (ex) => ex
                ).ToDataResultAsync();

        /// <summary>
        /// Fetch all entities of table including connected data from other tables. Used to get aggregates
        /// </summary>
        /// <param name="includeExpressions">Lambdas for includes</param>
        /// <returns>DataResult of list of entity</returns>
        public async Task<DataResult<List<TEntity>>> FetchAllIncluding(params Expression<Func<TEntity, object>>[] includeExpressions) =>
            await TryCatchAsync(
                async () => await _dbContext.Include(includeExpressions)
                                            .ToListAsync<TEntity>(),
                (ex) => ex
                ).ToDataResultAsync();

        /// <summary>
        /// Fetch an entity of table with given id.
        /// </summary>
        /// <param name="id">Id of entity</param>
        /// <returns>Dataresult of entity</returns>
        public async Task<DataResult<TEntity>> Fetch(TKey id) =>
            await TryCatchAsync(
                async () => await _dbContext.FindAsync<TEntity>(id),
                (ex) => ex
                ).ToDataResultAsync();

        /// <summary>
        /// Fetch and entity of table with given id, including connected data from other tables. Used to get an aggregate
        /// </summary>
        /// <param name="id">Id of entity</param>
        /// <param name="includeExpressions">Lambdas for includes</param>
        /// <returns>DataResult of entity</returns>
        public async Task<DataResult<TEntity>> FetchIncluding(TKey id, params Expression<Func<TEntity, object>>[] includeExpressions) =>
            await TryCatchAsync(
                async () => await _dbContext.Include(includeExpressions)
                                            .SingleOrDefaultAsync<TEntity>(_ => _.Id.Equals(id)),
                (ex) => ex
                ).ToDataResultAsync();

        /// <summary>
        /// Insert an new entity into the database table
        /// </summary>
        /// <param name="entity">Entity to insert</param>
        /// <returns>Result</returns>
        public async Task<Result> Insert(TEntity entity) =>
            await TryCatchAsync(
                async () => (await _dbContext.AddAsync<TEntity>(entity)).State == EntityState.Added
                                && await _dbContext.SaveChangesAsync() > 0,
                (ex) => ex
                ).ToResultAsync();

        /// <summary>
        /// Update an entity in the database. Id must be present in entity model!
        /// </summary>
        /// <param name="entity">Entity object with correct id</param>
        /// <returns>Result</returns>
        public async Task<Result> Update(TEntity entity) =>
            await TryCatchAsync(
                async () => _dbContext.Update<TEntity>(entity).State == EntityState.Modified
                                && await _dbContext.SaveChangesAsync() > 0,
                (ex) => ex
                ).ToResultAsync();

        /// <summary>
        /// Delete an entity with given id from the database
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Result</returns>
        public async Task<Result> Delete(TKey id) =>
            await TryCatchAsync(
                async () => _dbContext.Remove<TEntity>(await _dbContext.FindAsync<TEntity>(id)).State == EntityState.Deleted
                                && await _dbContext.SaveChangesAsync() > 0,
                (ex) => ex
                ).ToResultAsync();

    }

    /// <summary>
    /// Helper methods for BaseProvider
    /// </summary>
    internal static class ProviderHelpers
    {
        /// <summary>
        /// Prepare EF LINQ query with includes by given lambdas
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="ctx">Database context</param>
        /// <param name="includeExpressions">Include lambdas</param>
        /// <returns>IQueryable with prepared includes</returns>
        internal static IQueryable<TEntity> Include<TEntity>(this DbContext ctx, params Expression<Func<TEntity, object>>[] includeExpressions) where TEntity : class
        {
            DbSet<TEntity> dbSet = ctx.Set<TEntity>();

            IQueryable<TEntity> query = null;
            foreach (var includeExpression in includeExpressions)
            {
                query = dbSet.Include(includeExpression);
            }

            return query ?? dbSet;
        }
    }
}
