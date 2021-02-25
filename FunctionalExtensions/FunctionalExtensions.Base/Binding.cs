﻿using FunctionalExtensions.Base.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base
{
    public static class Binding
    {
        /// <summary>
        /// Bind over IEnumerable (SelectMany): E[T]->(T->E[R])->E[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> target, Func<T, IEnumerable<R>> func)
        {
            foreach (T t in target)
                foreach (R r in func(t))
                    yield return r;
        }

        /// <summary>
        /// Bind over DataResult: D[T]->(T->D[R])->D[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="dataResult">Data result</param>
        /// <param name="func">Transformation function</param>
        /// <returns></returns>
        public static DataResult<R> Bind<T, R>(this DataResult<T> dataResult, Func<T, DataResult<R>> func)
        {
            return
                dataResult.IsSuccess && dataResult.HasData
                ? func(dataResult.Data)
                : new DataResult<R>(dataResult.IsSuccess, dataResult.ErrorMessage, dataResult.ErrorType);
        }

        /// <summary>
        /// Async bind over DataResult: D[T]->(T->D[R])->D[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="dataResult">Data result</param>
        /// <param name="func">Transformation function</param>
        /// <returns></returns>
        public static async Task<DataResult<R>> BindAsync<T, R>(this Task<DataResult<T>> dataResult, Func<T, Task<DataResult<R>>> func)
        {
            var awaitedResult = await dataResult;
            return
                awaitedResult.IsSuccess && awaitedResult.HasData
                ? await func(awaitedResult.Data)
                : new DataResult<R>(awaitedResult.IsSuccess, awaitedResult.ErrorMessage, awaitedResult.ErrorType);
        }
    }
}
