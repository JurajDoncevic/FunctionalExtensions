using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base.Results;

public static class ResultExtensions
{
    #region MAP
    public static Result<R> Map<T, R>(this Result<T> result, Func<T, R> func)
        => result.IsSuccess
            ? func(result.Data)
            : new Result<R>(default, result.IsSuccess, result.Message, result.ResultType, result.Exception);

    public static async Task<Result<R>> Map<T, R>(this Task<Result<T>> result, Func<T, R> func) =>
        await result.Map(r => r.IsSuccess
            ? new Result<R>(func(r.Data), r.IsSuccess, r.Message, r.ResultType, r.Exception)
            : new Result<R>(default, r.IsSuccess, r.Message, r.ResultType, r.Exception));
    #endregion

    #region BIND
    public static Result Bind(this Result result, Func<Result, Result> func)
        => result.IsSuccess
            ? func(result)
            : result;

    public static async Task<Result> Bind(this Task<Result> result, Func<Result, Task<Result>> func)
    {
        var awaitedResult = await result;
        return awaitedResult
            ? await func(awaitedResult)
            : awaitedResult;
    }

    public static Result<R> Bind<T, R>(this Result<T> result, Func<T, Result<R>> func)
        => result.IsSuccess
            ? func(result.Data)
            : new Result<R>(default, result.IsSuccess, result.Message, result.ResultType, result.Exception);

    public static async Task<Result<R>> Bind<T, R>(this Task<Result<T>> result, Func<T, Task<Result<R>>> func)
    {
        var awaitedResult = await result;
        return awaitedResult
            ? await func(awaitedResult.Data)
            : new Result<R>(default, awaitedResult.IsSuccess, awaitedResult.Message, awaitedResult.ResultType, awaitedResult.Exception);
    }
    #endregion

    #region KLEISLI FISH OPERATOR

    public static Func<T1, Result> Fish<T1>(this Func<T1, Result> before, Func<Result, Result> after)
        => _1 => before(_1) switch
        {
            Result { IsSuccess: true } r => after(r),
            Result r => r
        };

    public static Func<T1, Task<Result>> Fish<T1>(this Func<T1, Task<Result>> before, Func<Result, Task<Result>> after)
        => async _1 => await before(_1) switch
        {
            Result { IsSuccess: true } r => await after(r),
            Result r => r
        };

    public static Func<Result> Fish(this Func<Result> before, Func<Result, Result> after)
        => () => before() switch
        {
            Result { IsSuccess: true } r => after(r),
            Result r => r
        };

    public static Func<Task<Result>> Fish(this Func<Task<Result>> before, Func<Result, Task<Result>> after)
        => async () => await before() switch
        {
            Result { IsSuccess: true } r => await after(r),
            Result r => r
        };

    public static Func<T1, Result<R2>> Fish<T1, R1, R2>(this Func<T1, Result<R1>> before, Func<R1, Result<R2>> after)
        => _1 => before(_1) switch
        {
            Result<R1> { IsSuccess: true, HasData: true } r => after(r.Data),
            Result<R1> r => new Result<R2>(default, r.IsSuccess, r.Message, r.ResultType, r.Exception)
        };

    public static Func<T1, Task<Result<R2>>> Fish<T1, R1, R2>(this Func<T1, Task<Result<R1>>> before, Func<R1, Task<Result<R2>>> after)
        => async _1 => await before(_1) switch
        {
            Result<R1> { IsSuccess: true, HasData: true } r => await after(r.Data),
            Result<R1> r => new Result<R2>(default, r.IsSuccess, r.Message, r.ResultType, r.Exception)
        };

    public static Func<Result<R2>> Fish<R1, R2>(this Func<Result<R1>> before, Func<R1, Result<R2>> after)
        => () => before() switch
        {
            Result<R1> { IsSuccess: true, HasData: true } r => after(r.Data),
            Result<R1> r => new Result<R2>(default, r.IsSuccess, r.Message, r.ResultType, r.Exception)
        };

    public static Func<Task<Result<R2>>> Fish<R1, R2>(this Func<Task<Result<R1>>> before, Func<R1, Task<Result<R2>>> after)
        => async () => await before() switch
        {
            Result<R1> { IsSuccess: true, HasData: true } r => await after(r.Data),
            Result<R1> r => new Result<R2>(default, r.IsSuccess, r.Message, r.ResultType, r.Exception)
        };
    #endregion

    #region TRY-CATCH

    /// <summary>
    /// Extension method to generate a Result over an operation using TryCatch
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static Result AsResult(Func<bool> operation)
        => Try.TryCatch(
            () => operation(),
            (ex) => ex
            ).ToResult();

    /// <summary>
    /// Extension method to generate a Result over an operation using TryCatch
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static async Task<Result> AsResult(Func<Task<bool>> operation)
        => await Try.TryCatch(
            () => operation(),
            (ex) => ex
            ).ToResult();

    /// <summary>
    /// Extension method to transform Try[Unit] into a Result 
    /// </summary>
    /// <param name="try">Try object</param>
    /// <returns>Result</returns>
    public static Result ToResult(this Try<Unit> @try) =>
        @try switch
        {
            Try<Unit> t when !t.IsException => Result.OnSuccess(),
            Try<Unit> t when t.IsException => Result.OnException(t.Exception),
            _ => Result.OnFailure()
        };

    /// <summary>
    /// Async extension method to transform Try[Unit] into a Result 
    /// </summary>
    /// <param name="try">Try object</param>
    /// <returns>Result object</returns>
    public static async Task<Result> ToResult(this Task<Try<Unit>> @try) =>
        await @try switch
        {
            Try<Unit> t when !t.IsException => Result.OnSuccess(),
            Try<Unit> t when t.IsException => Result.OnException(t.Exception),
            _ => Result.OnFailure()
        };

    /// <summary>
    /// Extension method to transform Try[Unit] into a Result 
    /// </summary>
    /// <param name="try">Try object</param>
    /// <typeparam name="TException">Type of expected exception from Try</typeparam>
    /// <returns>Result</returns>
    public static Result ToResult<TExpection>(this Try<Unit, TExpection> @try) where TExpection : Exception =>
        @try switch
        {
            Try<Unit> t when !t.IsException => Result.OnSuccess(),
            Try<Unit> t when t.IsException => Result.OnException(t.Exception),
            _ => Result.OnFailure()
        };

    /// <summary>
    /// Async extension method to transform Try[Unit] into a Result 
    /// </summary>
    /// <param name="try">Try object</param>
    /// <typeparam name="TException">Type of expected exception from Try</typeparam>
    /// <returns>Result object</returns>
    public static async Task<Result> ToResult<TExpection>(this Task<Try<Unit, TExpection>> @try) where TExpection : Exception =>
        await @try switch
        {
            Try<Unit> t when !t.IsException => Result.OnSuccess(),
            Try<Unit> t when t.IsException => Result.OnException(t.Exception),
            _ => Result.OnFailure()
        };

    /// <summary>
    /// Extension method to transform a Try[bool] into a Result by the encapsulated bool.
    /// Used for Try blocks that should notify an operation outcome
    /// </summary>
    /// <param name="try">Try object returning a bool</param>
    /// <returns>Result object</returns>
    public static Result ToResult(this Try<bool> @try) =>
        @try switch
        {
            Try<bool> t when !t.IsException && t.ExpectedData => Result.OnSuccess(),
            Try<bool> t when !t.IsException && !t.ExpectedData => Result.OnFailure(),
            Try<bool> t when t.IsException => Result.OnException(t.Exception),
            _ => Result.OnFailure()
        };

    /// <summary>
    /// Async extension method to transform a Try[bool] into a Result by the encapsulated bool.
    /// Used for Try blocks that should notify an operation outcome
    /// </summary>
    /// <param name="try">Try object returning a bool</param>
    /// <returns>Result object</returns>
    public static async Task<Result> ToResult(this Task<Try<bool>> @try) =>
        await @try switch
        {
            Try<bool> t when !t.IsException && t.ExpectedData => Result.OnSuccess(),
            Try<bool> t when !t.IsException && !t.ExpectedData => Result.OnFailure(),
            Try<bool> t when t.IsException => Result.OnException(t.Exception),
            _ => Result.OnFailure()
        };


    /// <summary>
    /// Extension method to transform a Try[bool] into a Result by the encapsulated bool.
    /// Used for Try blocks that should notify an operation outcome
    /// </summary>
    /// <param name="try">Try object returning a bool</param>
    /// <typeparam name="TException">Type of expected exception from Try</typeparam>
    /// <returns>Result object</returns>
    public static Result ToResult<TException>(this Try<bool, TException> @try) where TException : Exception =>
        @try switch
        {
            Try<bool> t when !t.IsException && t.ExpectedData => Result.OnSuccess(),
            Try<bool> t when !t.IsException && !t.ExpectedData => Result.OnFailure(),
            Try<bool> t when t.IsException => Result.OnException(t.Exception),
            _ => Result.OnFailure(),
        };

    /// <summary>
    /// Async extension method to transform a Try[bool] into a Result by the encapsulated bool.
    /// Used for Try blocks that should notify an operation outcome
    /// </summary>
    /// <param name="try">Try object returning a bool</param>
    /// <typeparam name="TException">Type of expected exception from Try</typeparam>
    /// <returns>Result object</returns>
    public static async Task<Result> ToResult<TException>(this Task<Try<bool, TException>> @try) where TException : Exception =>
        await @try switch
        {
            Try<bool> t when !t.IsException && t.ExpectedData => Result.OnSuccess(),
            Try<bool> t when !t.IsException && !t.ExpectedData => Result.OnFailure(),
            Try<bool> t when t.IsException => Result.OnException(t.Exception),
            _ => Result.OnFailure(),
        };

    /// <summary>
    /// Extension method to generate a Result over an operation using TryCatch
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static Result<TResult> AsResult<TResult>(Func<TResult> operation)
        => Try.TryCatch(
            () => operation(),
            (ex) => ex
            ).ToResult();

    /// <summary>
    /// Extension method to generate a Result over an operation using TryCatch
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static async Task<Result<TResult>> AsResult<TResult>(Func<Task<TResult>> operation)
        => await Try.TryCatch(
            () => operation(),
            (ex) => ex
            ).ToResult();

    /// <summary>
    /// Extension method to transform a Try into a Result
    /// </summary>
    /// <typeparam name="TResult">Expected return data type</typeparam>
    /// <param name="try">Try object</param>
    /// <returns>Result object</returns>
    public static Result<TResult> ToResult<TResult>(this Try<TResult> @try) =>
        @try switch
        {
            Try<TResult> t when !t.IsException && !t.IsData => Result<TResult>.OnFailure<TResult>(),
            Try<TResult> t when !t.IsException && t.IsData => Result<TResult>.OnSuccess(t.ExpectedData),
            Try<TResult> t when t.IsException => Result<TResult>.OnException<TResult>(t.Exception),
            _ => Result<TResult>.OnFailure<TResult>()
        };

    /// <summary>
    /// Async extension method to transform a Try into a Result
    /// </summary>
    /// <typeparam name="TResult">Expected return data type</typeparam>
    /// <param name="try">Try object task</param>
    /// <returns>Result object</returns>
    public static async Task<Result<TResult>> ToResult<TResult>(this Task<Try<TResult>> @try) =>
        await @try switch
        {
            Try<TResult> t when !t.IsException && !t.IsData => Result<TResult>.OnFailure<TResult>(),
            Try<TResult> t when !t.IsException && t.IsData => Result<TResult>.OnSuccess(t.ExpectedData),
            Try<TResult> t when t.IsException => Result<TResult>.OnException<TResult>(t.Exception),
            _ => Result<TResult>.OnFailure<TResult>()
        };

    /// <summary>
    /// Extension method to transform a Try into a Result
    /// </summary>
    /// <typeparam name="TResult">Expected return data type</typeparam>
    /// <typeparam name="TException">Type of expected exception</typeparam>
    /// <param name="try">Try object</param>
    /// <returns>Result object</returns>
    public static Result<TResult> ToResult<TResult, TException>(this Try<TResult, TException> @try) where TException : Exception =>
        @try switch
        {
            Try<TResult> t when !t.IsException && !t.IsData => Result<TResult>.OnFailure<TResult>(),
            Try<TResult> t when !t.IsException && t.IsData => Result<TResult>.OnSuccess(t.ExpectedData),
            Try<TResult> t when t.IsException => Result<TResult>.OnException<TResult>(t.Exception),
            _ => Result<TResult>.OnFailure<TResult>()
        };

    /// <summary>
    /// Async extension method to transform a Try into a Result
    /// </summary>
    /// <typeparam name="TResult">Expected return data type</typeparam>
    /// <typeparam name="TException">Type of expected exception</typeparam>
    /// <param name="try">Try object task</param>
    /// <returns>Result object</returns>
    public static async Task<Result<TResult>> ToResult<TResult, TException>(this Task<Try<TResult, TException>> @try) where TException : Exception =>
        await @try switch
        {
            Try<TResult> t when !t.IsException && !t.IsData => Result<TResult>.OnFailure<TResult>(),
            Try<TResult> t when !t.IsException && t.IsData => Result<TResult>.OnSuccess(t.ExpectedData),
            Try<TResult> t when t.IsException => Result<TResult>.OnException<TResult>(t.Exception),
            _ => Result<TResult>.OnFailure<TResult>()
        };

    #endregion

}
