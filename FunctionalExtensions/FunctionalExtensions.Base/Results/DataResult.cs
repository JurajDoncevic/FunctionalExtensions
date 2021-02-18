using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base.Results
{
    /// <summary>
    /// Result that can hold data
    /// </summary>
    /// <typeparam name="TResult">Wrapped data type</typeparam>
    public class DataResult<TResult>
    {
        private bool _isSuccess;
        private string _errorMessage;
        private ErrorType _errorType;
        private TResult _data;

        /// <summary>
        /// Did the operation succeed
        /// </summary>
        public bool IsSuccess { get; private set; }
        /// <summary>
        /// Did the operation fail
        /// </summary>
        public bool IsFailure { get => !IsSuccess; }
        /// <summary>
        /// Does the result have data
        /// </summary>
        public bool HasData { get => Data != null && !Data.Equals(default); }
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get => _errorMessage; }
        /// <summary>
        /// Error type for failure
        /// </summary>
        public ErrorType ErrorType { get => _errorType; }
        /// <summary>
        /// The data
        /// </summary>
        public TResult Data { get => _data; }

        internal DataResult(bool isSuccess, string errorMessage, ErrorType errorType, TResult data)
        {
            IsSuccess = isSuccess;
            _errorMessage = errorMessage;
            _errorType = errorType;
            _data = data;
        }

        internal DataResult(bool isSuccess, string errorMessage, ErrorType errorType)
        {
            IsSuccess = isSuccess;
            _errorMessage = errorMessage;
            _errorType = errorType;
            _data = default;
        }
    }

    public static class DataResult
    {
        public static DataResult<TResult> OnTry<TResult>(Try<TResult> @try) =>
            @try switch
            {
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorType.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorType.Unknown)
            };

        public static DataResult<TResult> ToDataResult<TResult>(this Try<TResult> @try) =>
            @try switch
            {
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorType.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorType.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorType.Unknown)
            };

        public static async Task<DataResult<TResult>> OnTryAsync<TResult>(Task<Try<TResult>> @try) =>
            (await @try) switch
            {
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorType.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorType.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorType.Unknown)
            };

        //public static DataResult<TResult> OnException<TResult>(Exception exception) =>
        //    new DataResult<TResult>(false, exception.Message, ErrorType.ExceptionThrown);

        //public static DataResult<TResult> OnFail<TResult>(string message) =>
        //    new DataResult<TResult>(false, message, ErrorType.Failure);

        //public static DataResult<TResult> OnSuccess<TResult>(TResult data) =>
        //    data != null
        //    ? new DataResult<TResult>(true, string.Empty, ErrorType.None, data)
        //    : new DataResult<TResult>(false, "Null result", ErrorType.NoData);

    }
}
