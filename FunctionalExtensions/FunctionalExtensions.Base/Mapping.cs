using FunctionalExtensions.Base.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base
{
    /// <summary>
    /// Mapping extensions for systems types
    /// </summary>
    public static class Mapping
    {
        /// <summary>
        /// Map single object without looking inside: T->(T->R)->R
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Mapping function</param>
        /// <returns>Mapped object</returns>
        [Obsolete("MapSingle is obsolete, please use the Identity functor")]
        public static R MapSingle<T, R>(this T target, Func<T, R> func) =>
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
