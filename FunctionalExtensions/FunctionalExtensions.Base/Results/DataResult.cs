#define _USE_CONSTRUCTOR_FUNCS
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

    /// <summary>
    /// Extensions methods for public creation of DataResult and Result
    /// </summary>
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Extension method to transform a Try into a DataResult
        /// </summary>
        /// <typeparam name="TResult">Expected return data type</typeparam>
        /// <param name="try">Try object</param>
        /// <returns>DataResult object</returns>
        public static DataResult<TResult> ToDataResult<TResult>(this Try<TResult> @try) =>
            @try switch
            {
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorType.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorType.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorType.Unknown)
            };

        /// <summary>
        /// Async extension method to transform a Try into a DataResult
        /// </summary>
        /// <typeparam name="TResult">Expected return data type</typeparam>
        /// <param name="try">Try object task</param>
        /// <returns>DataResult object</returns>
        public static async Task<DataResult<TResult>> ToDataResultAsync<TResult>(this Task<Try<TResult>> @try) =>
            (await @try) switch
            {
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorType.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorType.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorType.Unknown)
            };

        /// <summary>
        /// Bind resolution extension method for DataResult and Try. Sig: DR[TRY[T]] -> DR[T]
        /// </summary>
        /// <typeparam name="TResult">Result data type</typeparam>
        /// <param name="dataResult">Data result</param>
        /// <returns></returns>
        public static DataResult<TResult> BindTry<TResult>(this DataResult<Try<TResult>> dataResult) =>
            dataResult.HasData
            ? dataResult.Data switch
            {
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorType.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorType.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorType.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorType.Unknown)
            }
            : new DataResult<TResult>(false, "Null result", ErrorType.Unknown);

        /// <summary>
        /// Async bind resolution extension method for DataResult and Try. Sig: DR[TRY[T]] -> DR[T]
        /// </summary>
        /// <typeparam name="TResult">Result data type</typeparam>
        /// <param name="dataResult">Data result</param>
        /// <returns></returns>
        public static async Task<DataResult<TResult>> BindTryAsync<TResult>(this Task<DataResult<Try<TResult>>> dataResult) =>
            await dataResult switch
            {
                DataResult<Try<TResult>> dr when dr.HasData =>
                    dr.Data switch
                    {
                        Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorType.NoData),
                        Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorType.None, t.ExpectedData),
                        Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorType.ExceptionThrown),
                        _ => new DataResult<TResult>(false, "Null result", ErrorType.Unknown)
                    },
                _ => new DataResult<TResult>(false, "Null result", ErrorType.Unknown)
            };

#if USE_CONSTRUCTOR_FUNCS
        
        public static DataResult<TResult> OnException<TResult>(Exception exception) =>
            new DataResult<TResult>(false, exception.Message, ErrorType.ExceptionThrown);

        public static DataResult<TResult> OnFail<TResult>(string message) =>
            new DataResult<TResult>(false, message, ErrorType.Failure);

        public static DataResult<TResult> OnSuccess<TResult>(TResult data) =>
            data != null
            ? new DataResult<TResult>(true, string.Empty, ErrorType.None, data)
            : new DataResult<TResult>(false, "Null result", ErrorType.NoData);
#endif

    }
}
