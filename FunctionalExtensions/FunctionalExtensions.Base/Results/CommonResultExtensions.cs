using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base.Results
{
    /// <summary>
    /// Extensions methods for public creation of DataResult and Result
    /// </summary>
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Reversed natural transformation from Result to DataResult.
        /// R[] -> (bool -> DR[T]) -> DR[T]
        /// </summary>
        /// <param name="target">Original result</param>
        /// <param name="func">Function to possibly construct next result in sequence</param>
        /// <returns></returns>
        public static DataResult<T> Bind<T>(this Result target, Func<bool, DataResult<T>> func) =>
            target.IsSuccess
            ? func(target.IsSuccess)
            : target.MapSingle(_ => new DataResult<T>(false, target.ErrorMessage, target.ErrorType));


        /// <summary>
        /// Reversed natural transformation from Result to DataResult.
        /// R[] -> (bool -> DR[T]) -> DR[T]
        /// </summary>
        /// <param name="target">Original result</param>
        /// <param name="func">Function to possibly construct next result in sequence</param>
        /// <returns></returns>
        public static async Task<DataResult<T>> Bind<T>(this Task<Result> target, Func<bool, Task<DataResult<T>>> func) =>
            (await target) switch
            {
                Result { IsSuccess: true } result => await func(result.IsSuccess),
                Result { IsSuccess: false } result => result.MapSingle(_ => new DataResult<T>(false, _.ErrorMessage, _.ErrorType))
            };

        /// <summary>
        /// Natural transformation from DataResult to Result. 
        /// DR[T] -> (DR[T] -> R[]) -> R[]
        /// </summary>
        /// <param name="target">Original result</param>
        /// <param name="func">Function to possibly construct next result in sequence</param>
        /// <returns></returns>
        public static Result Bind<T>(this DataResult<T> target, Func<DataResult<T>, Result> func) =>
            target.IsSuccess
            ? func(target)
            : target.MapSingle(_ => new Result(false, _.ErrorMessage, _.ErrorType));


        /// <summary>
        /// Natural transformation from DataResult to Result.
        /// R[] -> (bool -> DR[T]) -> DR[T]
        /// </summary>
        /// <param name="target">Original result</param>
        /// <param name="func">Function to possibly construct next result in sequence</param>
        /// <returns></returns>
        public static async Task<Result> Bind<T>(this Task<DataResult<T>> target, Func<DataResult<T>, Task<Result>> func) =>
            (await target) switch
            {
                DataResult<T> { IsSuccess: true } result => await func(result),
                DataResult<T> { IsSuccess: false } result => result.MapSingle(_ => new Result(false, _.ErrorMessage, _.ErrorType))
            };
    }
}
