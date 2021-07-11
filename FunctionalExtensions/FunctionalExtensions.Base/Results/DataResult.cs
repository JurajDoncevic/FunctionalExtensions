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
        private ErrorTypes _errorType;
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
        public ErrorTypes ErrorType { get => _errorType; }
        /// <summary>
        /// The data
        /// </summary>
        public TResult Data { get => _data; }

        internal DataResult(bool isSuccess, string errorMessage, ErrorTypes errorType, TResult data)
        {
            IsSuccess = isSuccess;
            _errorMessage = errorMessage;
            _errorType = errorType;
            _data = data;
        }

        internal DataResult(bool isSuccess, string errorMessage, ErrorTypes errorType)
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
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorTypes.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorTypes.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorTypes.Unknown)
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
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorTypes.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorTypes.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorTypes.Unknown)
            };

        /// <summary>
        /// Extension method to transform a Try into a DataResult
        /// </summary>
        /// <typeparam name="TResult">Expected return data type</typeparam>
        /// <typeparam name="TException">Type of expected exception</typeparam>
        /// <param name="try">Try object</param>
        /// <returns>DataResult object</returns>
        public static DataResult<TResult> ToDataResult<TResult, TException>(this Try<TResult, TException> @try) where TException : Exception =>
            @try switch
            {
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorTypes.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorTypes.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorTypes.Unknown)
            };

        /// <summary>
        /// Async extension method to transform a Try into a DataResult
        /// </summary>
        /// <typeparam name="TResult">Expected return data type</typeparam>
        /// <typeparam name="TException">Type of expected exception</typeparam>
        /// <param name="try">Try object task</param>
        /// <returns>DataResult object</returns>
        public static async Task<DataResult<TResult>> ToDataResultAsync<TResult, TException>(this Task<Try<TResult, TException>> @try)  where TException : Exception =>
            (await @try) switch
            {
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorTypes.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorTypes.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorTypes.Unknown)
            };

        #region MAPS
        /// <summary>
        /// Map on DataResult: D[T]->(T->R)->D[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="dataResult"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static DataResult<R> Map<T, R>(this DataResult<T> dataResult, Func<T, R> func) =>
                dataResult.HasData
                ? new DataResult<R>(dataResult.IsSuccess, dataResult.ErrorMessage, dataResult.ErrorType, func(dataResult.Data))
                : new DataResult<R>(dataResult.IsSuccess, dataResult.ErrorMessage, dataResult.ErrorType);

        /// <summary>
        /// Async Map on DataResult: D[T]->(T->R)->D[R]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="dataResult"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<DataResult<R>> Map<T, R>(this Task<DataResult<T>> dataResult, Func<T, R> func) =>
            await dataResult.Map(result =>
                result.HasData && result.IsSuccess // disable passing default data on fail
                    ? new DataResult<R>(result.IsSuccess, result.ErrorMessage, result.ErrorType, func(result.Data))
                    : new DataResult<R>(result.IsSuccess, result.ErrorMessage, result.ErrorType));
        #endregion

        #region BINDS
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
        public static async Task<DataResult<R>> Bind<T, R>(this Task<DataResult<T>> dataResult, Func<T, Task<DataResult<R>>> func)
        {
            var awaitedResult = await dataResult;
            return
                awaitedResult.IsSuccess && awaitedResult.HasData
                ? await func(awaitedResult.Data)
                : new DataResult<R>(awaitedResult.IsSuccess, awaitedResult.ErrorMessage, awaitedResult.ErrorType);
        }
        #endregion

        #region TRY RESOLVE
        /// <summary>
        /// Bind resolution extension method for DataResult and Try. Sig: DR[TRY[T]] -> DR[T]
        /// </summary>
        /// <typeparam name="TResult">Result data type</typeparam>
        /// <param name="dataResult">Data result</param>
        /// <returns></returns>
        public static DataResult<TResult> TryResolve<TResult>(this DataResult<Try<TResult>> dataResult) =>
            dataResult.HasData
            ? dataResult.Data switch
            {
                Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorTypes.NoData),
                Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorTypes.None, t.ExpectedData),
                Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                _ => new DataResult<TResult>(false, "Null result", ErrorTypes.Unknown)
            }
            : new DataResult<TResult>(false, "Null result", ErrorTypes.Unknown);

        /// <summary>
        /// Async bind resolution extension method for DataResult and Try. Sig: DR[TRY[T]] -> DR[T]
        /// </summary>
        /// <typeparam name="TResult">Result data type</typeparam>
        /// <param name="dataResult">Data result</param>
        /// <returns></returns>
        public static async Task<DataResult<TResult>> TryResolve<TResult>(this Task<DataResult<Try<TResult>>> dataResult) =>
            await dataResult switch
            {
                DataResult<Try<TResult>> dr when dr.HasData =>
                    dr.Data switch
                    {
                        Try<TResult> t when !t.IsException && !t.IsData => new DataResult<TResult>(false, "Null result", ErrorTypes.NoData),
                        Try<TResult> t when !t.IsException => new DataResult<TResult>(true, string.Empty, ErrorTypes.None, t.ExpectedData),
                        Try<TResult> t when t.IsException => new DataResult<TResult>(false, t.Exception.Message, ErrorTypes.ExceptionThrown),
                        _ => new DataResult<TResult>(false, "Null result", ErrorTypes.Unknown)
                    },
                _ => new DataResult<TResult>(false, "Null result", ErrorTypes.Unknown)
            };
        #endregion

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
