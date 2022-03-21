using FunctionalExtensions.Base.Results;
using static FunctionalExtensions.Base.Try;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base
{
    public static class Timing
    {
        /// <summary>
        /// Run an operation within a time limit
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="operation">Limited operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Data result of operation</returns>
        public static DataResult<R> RunWithTimeout<R>(this Func<R> operation, int timeoutMillis)
            => TryCatch(
                () =>
                {
                    R result = default;
                    bool finishedBeforeTimeout = Task.Run(() => result = operation()).Wait(timeoutMillis);
                    return (finishedBeforeTimeout, result);
                },
                _ => _
                ).ToDataResult()
                .Bind(_ => _.finishedBeforeTimeout
                            ? ResultExtensions.AsDataResult(() => _.result)
                            : DataResult<R>.OnFail<R>("Timeout reached"));

        /// <summary>
        /// Run an async operation within a time limit 
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="operation">Limited async operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Data result of operation</returns>
        public static async Task<DataResult<R>> RunWithTimeout<R>(this Func<Task<R>> operation, int timeoutMillis)
            => await ResultExtensions.AsDataResult(
                async () =>
                {
                    R result = default;
                    var task = async () => result = await operation();
                    var delayTask = Task.Delay(timeoutMillis);
                    var completedTask = await Task.WhenAny(task.Invoke(), delayTask);
                    if (completedTask != delayTask)
                    {
                        return DataResult<R>.OnSuccess(result);
                    }
                    else
                    {
                        return DataResult<R>.OnFail<R>("Timeout reached");
                    }
                }).Map(_ => _.Data);

        /// <summary>
        /// Run an operation within a time limit
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="operation">Limited operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Data result of operation</returns>
        public static async Task<DataResult<R>> RunWithTimeout<R>(this Func<DataResult<R>> operation, int timeoutMillis)
            => await ResultExtensions.AsDataResult(
                async () =>
                {
                    DataResult<R> result = default;
                    var task = async () => result = await Task.Run(() => operation());
                    var delayTask = Task.Delay(timeoutMillis);
                    var completedTask = await Task.WhenAny(task.Invoke(), delayTask);
                    if (completedTask != delayTask)
                    {
                        return result;
                    }
                    else
                    {
                        return DataResult<R>.OnFail<R>("Timeout reached");
                    }
                }).Map(_ => _.Data);

        /// <summary>
        /// Run an operation within a time limit
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="operation">Limited async operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Data result of operation</returns>
        public static async Task<DataResult<R>> RunWithTimeout<R>(this Func<Task<DataResult<R>>> operation, int timeoutMillis)
            => await ResultExtensions.AsDataResult(
                async () =>
                {
                    DataResult<R> result = default;
                    var task = async () => result = await operation();
                    var delayTask = Task.Delay(timeoutMillis);
                    var completedTask = await Task.WhenAny(task.Invoke(), delayTask);
                    if (completedTask != delayTask)
                    {
                        return result;
                    }
                    else
                    {
                        return DataResult<R>.OnFail<R>("Timeout reached");
                    }
                }).Map(_ => _.Data);

        /// <summary>
        /// Run an operation within a time limit
        /// </summary>
        /// <param name="operation">Limited operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Result of operation</returns>
        public static Result RunWithTimeout(this Func<bool> operation, int timeoutMillis)
        {
            try
            {
                bool result = false;
                bool finishedBeforeTimeout = Task.Run(() => result = operation()).Wait(timeoutMillis);
                return finishedBeforeTimeout
                    ? ResultExtensions.AsResult(() => result)
                    : Result.OnFail("Timeout reached");

            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operatoin within a time limit
        /// </summary>
        /// <param name="operation">Limited async operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Result of operation</returns>
        public async static Task<Result> RunWithTimeout(this Func<Task<bool>> operation, int timeoutMillis)
        {
            try
            {
                bool result = false;
                var task = async () => result = await operation();
                var delayTask = Task.Delay(timeoutMillis);
                var completedTask = await Task.WhenAny(task.Invoke(), delayTask);
                if (completedTask != delayTask)
                {
                    return ResultExtensions.AsResult(() => result);
                }
                else
                {
                    return Result.OnFail("Timeout reached");
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operatoin within a time limit
        /// </summary>
        /// <param name="operation">Limited operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Result of operation</returns>
        public static async Task<Result> RunWithTimeout(this Func<Result> operation, int timeoutMillis)
        {
            try
            {
                Result result = default;
                var task = async () => result = await Task.Run(() => operation());
                var delayTask = Task.Delay(timeoutMillis);
                var completedTask = await Task.WhenAny(task.Invoke(), delayTask);
                if (completedTask != delayTask)
                {
                    return result;
                }
                else
                {
                    return Result.OnFail("Timeout reached");
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operatoin within a time limit
        /// </summary>
        /// <param name="operation">Limited async operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Result of operation</returns>
        public static async Task<Result> RunWithTimeout(this Func<Task<Result>> operation, int timeoutMillis)
        {
            try
            {
                Result result = default;
                var task = async () => result = await operation();
                var delayTask = Task.Delay(timeoutMillis);
                var completedTask = await Task.WhenAny(task.Invoke(), delayTask);
                if (completedTask != delayTask)
                {
                    return result;
                }
                else
                {
                    return Result.OnFail("Timeout reached");
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

    }
}
