﻿using FunctionalExtensions.Base.Results;
using static FunctionalExtensions.Base.Try;
using static FunctionalExtensions.Base.UnitExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace FunctionalExtensions.Base
{
    public static class Timing
    {
        public const string TIMEOUT_ERROR_MESSAGE = "Timeout reached";
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
                            : DataResult<R>.OnFail<R>(TIMEOUT_ERROR_MESSAGE));

        /// <summary>
        /// Run an operation within a time limit. Use this version when using a loops that could be infinite. Cancel with ThrowIfCancellationRequested.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="operation">Limited operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Data result of operation</returns>
        public static DataResult<R> RunWithTimeout<R>(this Func<CancellationToken, R> operation, int timeoutMillis)
            => TryCatch(
                () =>
                {
                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    R result = default;
                    Task task = Task.Run(() => result = operation(token), token);
                    tokenSource.CancelAfter(timeoutMillis);

                    var tryResult = TryCatch(() => { task.Wait(); return UnitExtensions.Unit(); }, ex => ex);
                    
                    if(tryResult.IsException && ((AggregateException)tryResult.Exception).InnerExceptions.Single() is not OperationCanceledException)
                        throw ((AggregateException)tryResult.Exception).InnerExceptions.Single();

                    bool taskCancelled = task.IsCanceled;
                    return (taskCancelled, result);
                },
                _ => _
                ).ToDataResult()
                .Bind(_ => !_.taskCancelled
                            ? ResultExtensions.AsDataResult(() => _.result)
                            : DataResult<R>.OnFail<R>(TIMEOUT_ERROR_MESSAGE));

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
                        return DataResult<R>.OnFail<R>(TIMEOUT_ERROR_MESSAGE);
                    }
                }).Map(_ => _.Data);

        /// <summary>
        /// Run an async operation within a time limit. Use this version when using a loops that could be infinite. Cancel with ThrowIfCancellationRequested.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="operation">Limited async operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Data result of operation</returns>
        public static async Task<DataResult<R>> RunWithTimeout<R>(this Func<CancellationToken, Task<R>> operation, int timeoutMillis)
            => await ResultExtensions.AsDataResult(
                async () =>
                {
                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    R result = default;

                    var delayTask = Task.Delay(timeoutMillis);
                    var task = Task.Run(async () => result = await operation(token), token);
                    var completedTask = await Task.WhenAny(task, delayTask);
                    if (completedTask != delayTask)
                    {
                        if(completedTask.IsFaulted)
                        { 
                            return DataResult<R>.OnException<R>(completedTask.Exception.InnerExceptions.First());
                        }
                        return DataResult<R>.OnSuccess(result);
                    }
                    else
                    {
                        tokenSource.Cancel();
                        return DataResult<R>.OnFail<R>(TIMEOUT_ERROR_MESSAGE);
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
                        return DataResult<R>.OnFail<R>(TIMEOUT_ERROR_MESSAGE);
                    }
                }).Map(_ => _.Data);

        /// <summary>
        /// Run an operation within a time limit. Use this version when using a loops that could be infinite. Cancel with ThrowIfCancellationRequested.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="operation">Limited operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Data result of operation</returns>
        public static async Task<DataResult<R>> RunWithTimeout<R>(this Func<CancellationToken, DataResult<R>> operation, int timeoutMillis)
            => await ResultExtensions.AsDataResult(
                async () =>
                {
                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    DataResult<R> result = default;
                    var task = Task.Run(() => result = operation(token), token);
                    var delayTask = Task.Delay(timeoutMillis);
                    var completedTask = await Task.WhenAny(task, delayTask);
                    if (completedTask != delayTask)
                    {
                        if (task.IsFaulted)
                        {
                            return DataResult<R>.OnException<R>(completedTask.Exception.InnerExceptions.First());
                        }
                        return result;
                    }
                    else
                    {
                        tokenSource.Cancel();
                        return DataResult<R>.OnFail<R>(TIMEOUT_ERROR_MESSAGE);
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
                        return DataResult<R>.OnFail<R>(TIMEOUT_ERROR_MESSAGE);
                    }
                }).Map(_ => _.Data);

        /// <summary>
        /// Run an operation within a time limit. Use this version when using a loops that could be infinite. Cancel with ThrowIfCancellationRequested.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="operation">Limited async operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Data result of operation</returns>
        public static async Task<DataResult<R>> RunWithTimeout<R>(this Func<CancellationToken, Task<DataResult<R>>> operation, int timeoutMillis)
            => await ResultExtensions.AsDataResult(
                async () =>
                {
                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    DataResult<R> result = default;
                    var task = Task.Run(async () => result = await operation(token));
                    var delayTask = Task.Delay(timeoutMillis);
                    var completedTask = await Task.WhenAny(task, delayTask);
                    if (completedTask != delayTask)
                    {
                        return result;
                    }
                    else
                    {
                        tokenSource.Cancel();
                        return DataResult<R>.OnFail<R>(TIMEOUT_ERROR_MESSAGE);
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
                    : Result.OnFail(TIMEOUT_ERROR_MESSAGE);

            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operation within a time limit. Use this version when using a loops that could be infinite. Cancel with ThrowIfCancellationRequested.
        /// </summary>
        /// <param name="operation">Limited operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Result of operation</returns>
        public static Result RunWithTimeout(this Func<CancellationToken, bool> operation, int timeoutMillis)
        {
            try
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                bool result = false;
                var task = Task.Run(() => result = operation(token), token);
                bool finishedBeforeTimeout = task.Wait(timeoutMillis);
                if (finishedBeforeTimeout)
                {
                    return ResultExtensions.AsResult(() => result);
                }
                else
                {
                    tokenSource.Cancel();
                    return Result.OnFail(TIMEOUT_ERROR_MESSAGE);
                }

            }
            catch (Exception exception)
            {
                if(exception is AggregateException)
                {
                    return Result.OnException(((AggregateException)exception).InnerExceptions.First());
                }
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operation within a time limit
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
                    return Result.OnFail(TIMEOUT_ERROR_MESSAGE);
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operation within a time limit. Use this version when using a loops that could be infinite. Cancel with ThrowIfCancellationRequested.
        /// </summary>
        /// <param name="operation">Limited async operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Result of operation</returns>
        public async static Task<Result> RunWithTimeout(this Func<CancellationToken, Task<bool>> operation, int timeoutMillis)
        {
            try
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                bool result = false;
                var task = Task.Run(async () => result = await operation(token), token);
                var delayTask = Task.Delay(timeoutMillis);
                var completedTask = await Task.WhenAny(task, delayTask);
                if (completedTask != delayTask)
                {
                    if (task.IsFaulted)
                    {
                        return Result.OnException(((AggregateException)task.Exception).InnerExceptions.First());
                    }
                    return ResultExtensions.AsResult(() => result);
                }
                else
                {
                    return Result.OnFail(TIMEOUT_ERROR_MESSAGE);
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operation within a time limit
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
                    return Result.OnFail(TIMEOUT_ERROR_MESSAGE);
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operation within a time limit. Use this version when using a loops that could be infinite. Cancel with ThrowIfCancellationRequested.
        /// </summary>
        /// <param name="operation">Limited operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Result of operation</returns>
        public static async Task<Result> RunWithTimeout(this Func<CancellationToken, Result> operation, int timeoutMillis)
        {
            try
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                Result result = default;
                var task = Task.Run(() => result = operation(token), token);
                var delayTask = Task.Delay(timeoutMillis);
                var completedTask = await Task.WhenAny(task, delayTask);
                if (completedTask != delayTask)
                {
                    return result;
                }
                else
                {
                    return Result.OnFail(TIMEOUT_ERROR_MESSAGE);
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operation within a time limit
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
                    return Result.OnFail(TIMEOUT_ERROR_MESSAGE);
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

        /// <summary>
        /// Run an operation within a time limit. Use this version when using a loops that could be infinite. Cancel with ThrowIfCancellationRequested.
        /// </summary>
        /// <param name="operation">Limited async operation</param>
        /// <param name="timeoutMillis">Time limit in milliseconds</param>
        /// <returns>Result of operation</returns>
        public static async Task<Result> RunWithTimeout(this Func<CancellationToken, Task<Result>> operation, int timeoutMillis)
        {
            try
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                Result result = default;
                var task = Task.Run(async () => result = await operation(token), token);
                var delayTask = Task.Delay(timeoutMillis);
                var completedTask = await Task.WhenAny(task, delayTask);
                if (completedTask != delayTask)
                {
                    return result;
                }
                else
                {
                    return Result.OnFail(TIMEOUT_ERROR_MESSAGE);
                }
            }
            catch (Exception exception)
            {
                return Result.OnException(exception);
            }
        }

    }
}
