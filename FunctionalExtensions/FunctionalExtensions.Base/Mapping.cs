using FunctionalExtensions.Base.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base
{
    public static class Mapping
    {
        /// <summary>
        /// Map single object: T->(T->R)->R
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Mapping function</param>
        /// <returns>Mapped object</returns>
        public static R Map<T, R>(this T target, Func<T, R> func) =>
            func(target);

        /// <summary>
        /// Map IEnumerable by index: E[T]->(idx->T->R)->E[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="func"></param>
        /// <returns>Mapped enumerable</returns>
        public static IEnumerable<R> Mapi<T, R>(this IEnumerable<T> target, Func<long, T, R> func)
        {
            long idx = 0;
            foreach (var t in target)
            {
                yield return func(idx, t);
                idx++;
            }
        }

        /// <summary>
        /// Map IEnumerable E[T]->(T->R)->E[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="func"></param>
        /// <returns>Mapped enumerable</returns>
        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> target, Func<T, R> func)
        {
            foreach (T t in target)
                yield return func(t);
        }

        /// <summary>
        /// Map on DataResult: D[T]->(T->R)->D[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="dataResult"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static DataResult<R> Map<T, R> (this DataResult<T> dataResult, Func<T, R> func) =>
                dataResult.HasData
                ? new DataResult<R>(dataResult.IsSuccess, dataResult.ErrorMessage, dataResult.ErrorType, func(dataResult.Data))
                : new DataResult<R>(dataResult.IsSuccess, dataResult.ErrorMessage, dataResult.ErrorType);

        /// <summary>
        /// Async Map on DataResult: D[T]->(T->R)->D[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="dataResult"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<DataResult<R>> MapAsync<T, R>(this Task<DataResult<T>> dataResult, Func<T, R> func)=>
            await dataResult.Map(result =>
                result.HasData && result.IsSuccess // disable passing default data on fail
                    ? new DataResult<R>(result.IsSuccess, result.ErrorMessage, result.ErrorType, func(result.Data))
                    : new DataResult<R>(result.IsSuccess, result.ErrorMessage, result.ErrorType));

        /// <summary>
        /// Map operation for a Task: Ta[T]->(T->R)->Ta[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="task"></param>
        /// <param name="operation"></param>
        /// <returns>Mapped Task</returns>
        public async static Task<R> Map<T, R>(this Task<T> task, Func<T, R> operation)
            => operation(await task);

    }
}
