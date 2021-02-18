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
    public abstract class BaseProvider<TKey, TEntity, TContext>
            where TEntity : BaseModel<TKey>
            where TContext : DbContext
    {
        protected TContext _dbContext;

        public BaseProvider(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DataResult<List<TEntity>>> FetchAll() =>
            (await TryCatchAsync(
                async () => await _dbContext.Set<TEntity>()
                                            .ToListAsync(),
                (ex) => ex
                )).ToDataResult();

        public async Task<DataResult<List<TEntity>>> FetchAllIncluding(params Expression<Func<TEntity, object>>[] includeExpressions) =>
            (await TryCatchAsync(
                async () => await _dbContext.Include(includeExpressions)
                                            .ToListAsync<TEntity>(),
                (ex) => ex
                )).ToDataResult();


        public async Task<DataResult<TEntity>> Fetch(TKey id) =>
            (await TryCatchAsync(
                async () => await _dbContext.FindAsync<TEntity>(id),
                (ex) => ex
                )).ToDataResult();


        public async Task<DataResult<TEntity>> FetchIncluding(TKey id, params Expression<Func<TEntity, object>>[] includeExpressions) =>
            (await TryCatchAsync(
                async () => await _dbContext.Include(includeExpressions)
                                            .SingleOrDefaultAsync<TEntity>(_ => _.Id.Equals(id)),
                (ex) => ex
                )).ToDataResult();


        public async Task<Result> Insert(TEntity entity) =>
            await TryCatchAsync(
                async () => (await _dbContext.AddAsync<TEntity>(entity)).State == EntityState.Added
                                && await _dbContext.SaveChangesAsync() > 0,
                (ex) => ex
                ).ToResultAsync();

        public async Task<Result> Update(TEntity entity) =>
            await TryCatchAsync(
                async () => _dbContext.Update<TEntity>(entity).State == EntityState.Modified
                                && await _dbContext.SaveChangesAsync() > 0,
                (ex) => ex
                ).ToResultAsync();

        public async Task<Result> Delete(TKey id) =>
            await TryCatchAsync(
                async () => _dbContext.Remove<TEntity>(await _dbContext.FindAsync<TEntity>(id)).State == EntityState.Deleted
                                && await _dbContext.SaveChangesAsync() > 0,
                (ex) => ex
                ).ToResultAsync();

    }

    internal static class ProviderHelpers
    {
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
