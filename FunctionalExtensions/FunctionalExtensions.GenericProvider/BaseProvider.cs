using FunctionalExtensions.Base.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public async Task<DataResult<IEnumerable<TEntity>>> FetchAll() =>
            await TryAsync(
                async () => DataResult<IEnumerable<TEntity>>.OnSuccess(
                            await _dbContext.Set<TEntity>()
                                            .ToListAsync()),
                (ex) => DataResult<IEnumerable<TEntity>>.OnException(ex)
                );

        public async Task<DataResult<IEnumerable<TEntity>>> FetchAllIncluding(params Expression<Func<TEntity, object>>[] includeExpressions) =>
            await TryAsync(
                async () => DataResult<IEnumerable<TEntity>>.OnSuccess(
                                await _dbContext.Include(includeExpressions)
                                .ToListAsync()),
                (ex) => DataResult<IEnumerable<TEntity>>.OnException(ex)
                );


        public async Task<DataResult<TEntity>> Fetch(TKey id) =>
            await TryAsync(
                async () => DataResult<TEntity>.OnSuccess(
                                await _dbContext.FindAsync<TEntity>(id)),
                (ex) => DataResult<TEntity>.OnException(ex)
                );


        public async Task<DataResult<TEntity>> FetchIncluding(TKey id, params Expression<Func<TEntity, object>>[] includeExpressions) =>
            await TryAsync(
                async () => DataResult<TEntity>.OnSuccess(
                                await _dbContext.Include(includeExpressions)
                                                .SingleOrDefaultAsync<TEntity>(_ => _.Id.Equals(id))),
                (ex) => DataResult<TEntity>.OnException(ex)
                );


        public async Task<Result> Insert(TEntity entity) =>
            await TryAsync(
                async () => (await _dbContext.AddAsync<TEntity>(entity)).State switch
                {
                    EntityState.Added => await _dbContext.SaveChangesAsync() > 0
                                         ? Result.OnSuccess()
                                         : Result.OnFail("Insert failed on save"),
                    _ => Result.OnFail("Insert failed")
                },
                (ex) => Result.OnException(ex)
                );

        public async Task<Result> Update(TEntity entity) =>
            await TryAsync(
                async () => (_dbContext.Update<TEntity>(entity)).State switch
                {
                    EntityState.Modified => await _dbContext.SaveChangesAsync() > 0
                                            ? Result.OnSuccess()
                                            : Result.OnFail("Update failed on save"),
                    _ => Result.OnFail("Update failed")
                },
                (ex) => Result.OnException(ex)
                );

        public async Task<Result> Delete(TKey id) =>
            await TryAsync(
                async () => (_dbContext.Remove<TEntity>(await _dbContext.FindAsync<TEntity>(id))).State switch
                {
                    EntityState.Deleted => await _dbContext.SaveChangesAsync() > 0
                                           ? Result.OnSuccess()
                                           : Result.OnFail("Delete failed on save"),
                    _ => Result.OnFail("Delete failed")
                },
                (ex) => Result.OnException(ex)
                );

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
