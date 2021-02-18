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
        public ErrorType ErrorType { get; private set; }

        internal Result(bool isSuccess, string errorMessage, ErrorType errorType)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        //public static Result OnException(Exception exception) =>
        //    new Result(false, exception.Message, ErrorType.ExceptionThrown);

        //public static Result OnFail(string message) =>
        //    new Result(false, message, ErrorType.Failure);

        //public static Result OnSuccess() =>
        //    new Result(true, string.Empty, ErrorType.None);
       
    }

    public static class ResultExtensions
    {
        public static Result ToResult(this Try<Unit> @try) =>
            @try switch
            {
                Try<Unit> t when !t.IsException => new Result(true, string.Empty, ErrorType.None),
                Try<Unit> t when t.IsException => new Result(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorType.Unknown)
            };

        public static async Task<Result> ToResultAsync(this Task<Try<Unit>> @try) =>
            (await @try) switch
            {
                Try<Unit> t when !t.IsException => new Result(true, string.Empty, ErrorType.None),
                Try<Unit> t when t.IsException => new Result(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorType.Unknown)
            };

        public static Result ToResult(this Try<bool> @try) =>
            @try switch
            {
                Try<bool> t when !t.IsException => t.ExpectedData ? new Result(true, string.Empty, ErrorType.None)
                                                                  : new Result(false, "Operation failed", ErrorType.Failure),
                Try<bool> t when t.IsException => new Result(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorType.Unknown)
            };

        public static async Task<Result> ToResultAsync(this Task<Try<bool>> @try) =>
            (await @try) switch
            {
                Try<bool> t when !t.IsException => t.ExpectedData ? new Result(true, string.Empty, ErrorType.None)
                                                                  : new Result(false, "Operation failed", ErrorType.Failure),
                Try<bool> t when t.IsException => new Result(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new Result(false, "Null result", ErrorType.Unknown)
            };
    }
}
