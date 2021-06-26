#define _USE_CONSTRUCTOR_FUNCS
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static FunctionalExtensions.Base.FunctionalHelpers;

namespace FunctionalExtensions.Base.Results
{
    /// <summary>
    /// Operation result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Did the operation succeed
        /// </summary>
        public bool IsSuccess { get; private set; }
        /// <summary>
        /// Did the operation fail
        /// </summary>
        public bool IsFailure { get => !IsSuccess; }
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// Error type for failure
        /// </summary>
        public ErrorTypes ErrorType { get; private set; }

        internal Result(bool isSuccess, string errorMessage, ErrorTypes errorType)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

#if USE_CONSTRUCTOR_FUNCS
        public static Result OnException(Exception exception) =>
            new Result(false, exception.Message, ErrorType.ExceptionThrown);

        public static Result OnFail(string message) =>
            new Result(false, message, ErrorType.Failure);

        public static Result OnSuccess() =>
            new Result(true, string.Empty, ErrorType.None);
#endif
    }

    /// <summary>
    /// Extensions methods for public creation of DataResult and Result
    /// </summary>
    public static partial class ResultExtensions
    {
        #region TRY RESOLVE
        /// <summary>
        /// Extension method to transform Try[Unit] into a Result 
        /// </summary>
        /// <param name="try">Try object</param>
        /// <returns>Result</returns>
        public static Result ToResult(this Try<Unit> @try) =>
            @try switch
            {
                Try<Unit> t when !t.IsException => new Result(true, string.Empty, ErrorTypes.None),
                Try<Unit> t when t.IsException => new Result(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorTypes.Unknown)
            };

        /// <summary>
        /// Async extension method to transform Try[Unit] into a Result 
        /// </summary>
        /// <param name="try">Try object</param>
        /// <returns>Result object</returns>
        public static async Task<Result> ToResult(this Task<Try<Unit>> @try) =>
            (await @try) switch
            {
                Try<Unit> t when !t.IsException => new Result(true, string.Empty, ErrorTypes.None),
                Try<Unit> t when t.IsException => new Result(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorTypes.Unknown)
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
                Try<bool> t when !t.IsException => t.ExpectedData ? new Result(true, string.Empty, ErrorTypes.None)
                                                                  : new Result(false, "Operation failed", ErrorTypes.Failure),
                Try<bool> t when t.IsException => new Result(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorTypes.Unknown)
            };

        /// <summary>
        /// Async extension method to transform a Try[bool] into a Result by the encapsulated bool.
        /// Used for Try blocks that should notify an operation outcome
        /// </summary>
        /// <param name="try">Try object returning a bool</param>
        /// <returns>Result object</returns>
        public static async Task<Result> ToResult(this Task<Try<bool>> @try) =>
            (await @try) switch
            {
                Try<bool> t when !t.IsException => t.ExpectedData ? new Result(true, string.Empty, ErrorTypes.None)
                                                                  : new Result(false, "Operation failed", ErrorTypes.Failure),
                Try<bool> t when t.IsException => new Result(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorTypes.Unknown)
            };
        #endregion

        /// <summary>
        /// Used to pipe Result objects. 
        /// R[] -> (() -> R[]) -> R[]
        /// </summary>
        /// <param name="target">Original result</param>
        /// <param name="func">Function to possibly construct next result in sequence</param>
        /// <returns></returns>
        public static Result Bind(this Result target, Func<Result, Result> func) =>
            target.IsSuccess
            ? func(target)
            : target;


        /// <summary>
        /// Used to pipe Result objects.
        /// R[] -> (() -> R[]) -> R[]
        /// </summary>
        /// <param name="target">Original result</param>
        /// <param name="func">Function to possibly construct next result in sequence</param>
        /// <returns></returns>
        public static async Task<Result> Bind(this Task<Result> target, Func<Result, Task<Result>> func) =>
            (await target).IsSuccess
            ? await func(await target)
            : await target;
    }
}
