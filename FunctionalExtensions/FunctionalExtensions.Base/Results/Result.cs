#define USE_CONSTRUCTOR_FUNCS
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static FunctionalExtensions.Base.UnitExtensions;

namespace FunctionalExtensions.Base.Results
{
    /// <summary>
    /// Operation result
    /// </summary>
    public struct Result
    {
        private readonly bool _isSuccess;
        private readonly string _errorMessage;
        private readonly ErrorTypes _errorType;

        /// <summary>
        /// Did the operation succeed
        /// </summary>
        public bool IsSuccess => _isSuccess;
        /// <summary>
        /// Did the operation fail
        /// </summary>
        public bool IsFailure => !IsSuccess; 
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage => _errorMessage;
        /// <summary>
        /// Error type for failure
        /// </summary>
        public ErrorTypes ErrorType => _errorType; 

        internal Result(bool isSuccess, string errorMessage, ErrorTypes errorType)
        {
            _isSuccess = isSuccess;
            _errorMessage = errorMessage;
            _errorType = errorType;
        }

        public static implicit operator bool(Result result)
        {
            return result.IsSuccess;
        }

        public override bool Equals(object obj)
        {
            return obj is Result result &&
                   _isSuccess == result._isSuccess &&
                   _errorType == result._errorType &&
                   _errorMessage == result._errorMessage;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_isSuccess, _errorMessage, _errorType);
        }

#if USE_CONSTRUCTOR_FUNCS
        internal static Result OnException(Exception exception) =>
            new Result(false, exception.Message, ErrorTypes.ExceptionThrown);

        internal static Result OnFail(string message) =>
            new Result(false, message, ErrorTypes.Failure);

        internal static Result OnSuccess() =>
            new Result(true, string.Empty, ErrorTypes.None);
#endif
    }

    /// <summary>
    /// Extensions methods for public creation of DataResult and Result
    /// </summary>
    public static partial class ResultExtensions
    {
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
        /// Extension method to transform Try[Unit] into a Result 
        /// </summary>
        /// <param name="try">Try object</param>
        /// <typeparam name="TException">Type of expected exception from Try</typeparam>
        /// <returns>Result</returns>
        public static Result ToResult<TExpection>(this Try<Unit, TExpection> @try) where TExpection : Exception =>
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
        /// <typeparam name="TException">Type of expected exception from Try</typeparam>
        /// <returns>Result object</returns>
        public static async Task<Result> ToResult<TExpection>(this Task<Try<Unit, TExpection>> @try) where TExpection : Exception =>
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
        /// <typeparam name="TException">Type of expected exception from Try</typeparam>
        /// <returns>Result object</returns>
        public static async Task<Result> ToResult<TException>(this Task<Try<bool, TException>> @try) where TException : Exception =>
            (await @try) switch
            {
                Try<bool> t when !t.IsException => t.ExpectedData ? new Result(true, string.Empty, ErrorTypes.None)
                                                                  : new Result(false, "Operation failed", ErrorTypes.Failure),
                Try<bool> t when t.IsException => new Result(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorTypes.Unknown)
            };
        #endregion

        #region BINDS

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

        #endregion

        #region KLEISLI FISH OPERATOR

        public static Func<T1, Result> Fish<T1>(this Func<T1, Result> before, Func<Result, Result> after)
            => _1 => before(_1) switch
                {
                    Result{ IsSuccess: true} r => after(r),
                    Result r => r
                };

        public static Func<T1, Task<Result>> Fish<T1>(this Func<T1, Task<Result>> before, Func<Result, Task<Result>> after)
            => async  _1 => await before(_1) switch
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

        #endregion

    }
}
