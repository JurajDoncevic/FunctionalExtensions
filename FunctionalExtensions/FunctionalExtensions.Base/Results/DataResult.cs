#define USE_CONSTRUCTOR_FUNCS
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
    public struct DataResult<TResult>
    {
        private readonly bool _isSuccess;
        private readonly string _errorMessage;
        private readonly ErrorTypes _errorType;
        private readonly TResult _data;

        /// <summary>
        /// Did the operation succeed
        /// </summary>
        public bool IsSuccess => _isSuccess;
        /// <summary>
        /// Did the operation fail
        /// </summary>
        public bool IsFailure => !IsSuccess;
        /// <summary>
        /// Does the result have data
        /// </summary>
        public bool HasData { get => Data != null && !Data.Equals(default); }
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage => _errorMessage;
        /// <summary>
        /// Error type for failure
        /// </summary>
        public ErrorTypes ErrorType => _errorType;
        /// <summary>
        /// The data
        /// </summary>
        public TResult Data => _data;

        internal DataResult(bool isSuccess, string errorMessage, ErrorTypes errorType, TResult data)
        {
            _isSuccess = isSuccess;
            _errorMessage = errorMessage;
            _errorType = errorType;
            _data = data;
        }

        internal DataResult(bool isSuccess, string errorMessage, ErrorTypes errorType)
        {
            _isSuccess = isSuccess;
            _errorMessage = errorMessage;
            _errorType = errorType;
            _data = default;
        }

#if USE_CONSTRUCTOR_FUNCS

        internal static DataResult<R> OnException<R>(Exception exception) =>
            new DataResult<R>(false, exception.Message, ErrorTypes.ExceptionThrown);

        internal static DataResult<R> OnFail<R>(string message) =>
            new DataResult<R>(false, message, ErrorTypes.Failure);

        internal static DataResult<R> OnSuccess<R>(R data) =>
            data != null
            ? new DataResult<R>(true, string.Empty, ErrorTypes.None, data)
            : new DataResult<R>(false, "Null result", ErrorTypes.NoData);
#endif
    }

    /// <summary>
    /// Extensions methods for public creation of DataResult and Result
    /// </summary>
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Extension method to generate a DataResult over an operation using TryCatch
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static DataResult<TResult> AsDataResult<TResult>(Func<TResult> operation)
            => Try.TryCatch(
                () => operation(),
                (ex) => ex
                ).ToDataResult();

        /// <summary>
        /// Extension method to generate a DataResult over an operation using TryCatch
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static async Task<DataResult<TResult>> AsDataResult<TResult>(Func<Task<TResult>> operation)
            => await Try.TryCatch(
                () => operation(),
                (ex) => ex
                ).ToDataResult();

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
        public static async Task<DataResult<TResult>> ToDataResult<TResult>(this Task<Try<TResult>> @try) =>
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
        public static async Task<DataResult<TResult>> ToDataResult<TResult, TException>(this Task<Try<TResult, TException>> @try) where TException : Exception =>
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

        #region KLEISLI FISH OPERATOR

        public static Func<T1, DataResult<R2>> Fish<T1, R1, R2>(this Func<T1, DataResult<R1>> before, Func<R1, DataResult<R2>> after)
            => _1 => before(_1) switch
                {
                    DataResult<R1> { IsSuccess: true, HasData: true } dr => after(dr.Data),
                    DataResult<R1> dr => new DataResult<R2>(dr.IsSuccess, dr.ErrorMessage, dr.ErrorType)
                };

        public static Func<T1, Task<DataResult<R2>>> Fish<T1, R1, R2>(this Func<T1, Task<DataResult<R1>>> before, Func<R1, Task<DataResult<R2>>> after)
            => async _1 => await before(_1) switch
                {
                    DataResult<R1> { IsSuccess: true, HasData: true } dr => await after(dr.Data),
                    DataResult<R1> dr => new DataResult<R2>(dr.IsSuccess, dr.ErrorMessage, dr.ErrorType)
                };

        public static Func<DataResult<R2>> Fish<R1, R2>(this Func<DataResult<R1>> before, Func<R1, DataResult<R2>> after)
            => () => before() switch
                {
                    DataResult<R1> { IsSuccess: true, HasData: true } dr => after(dr.Data),
                    DataResult<R1> dr => new DataResult<R2>(dr.IsSuccess, dr.ErrorMessage, dr.ErrorType)
                };

        public static Func<Task<DataResult<R2>>> Fish<R1, R2>(this Func<Task<DataResult<R1>>> before, Func<R1, Task<DataResult<R2>>> after)
            => async () => await before() switch
            {
                DataResult<R1> { IsSuccess: true, HasData: true } dr => await after(dr.Data),
                DataResult<R1> dr => new DataResult<R2>(dr.IsSuccess, dr.ErrorMessage, dr.ErrorType)
            };

        #endregion

    }
}
