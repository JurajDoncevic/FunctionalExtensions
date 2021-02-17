using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base
{
    /// <summary>
    /// Try-catch resulting object
    /// </summary>
    /// <typeparam name="TExpectedData">Type of expected data if exception not thrown</typeparam>
    public class Try<TExpectedData>
    {
        private readonly Exception _exception;
        private readonly TExpectedData _expectedData;

        /// <summary>
        /// Caught exception
        /// </summary>
        public Exception Exception { get => _exception; }
        /// <summary>
        /// Expected operation return data
        /// </summary>
        public TExpectedData ExpectedData { get => _expectedData; }
        /// <summary>
        /// Was an exception thrown and caught?
        /// </summary>
        public bool IsException { get => _exception != null; }
        /// <summary>
        /// Is there return data?
        /// </summary>
        public bool IsData { get => _expectedData != null; }

        internal Try(TExpectedData expectedData)
        {
            _exception = null;
            _expectedData = expectedData;
        }

        internal Try(Exception exception)
        {
            _exception = exception;
            _expectedData = default(TExpectedData);
        }
    }

    public static class Try
    {
        /// <summary>
        /// Functional try-catch block
        /// </summary>
        /// <typeparam name="T">Expected return data type</typeparam>
        /// <param name="operate">Operations in try block: () -> T</param>
        /// <param name="catchOperate">Operations in catch block: Ex -> Ex</param>
        /// <returns>Try object</returns>
        public static Try<T> TryCatch<T>(Func<T> operate, Func<Exception, Exception> catchOperate)
        {
            try
            { return new Try<T>(operate()); }
            catch (Exception exception)
            { return new Try<T>(catchOperate(exception)); }
        }

        /// <summary>
        /// Async functional try-catch block
        /// </summary>
        /// <typeparam name="T">Expected return data type</typeparam>
        /// <param name="operate">Operations in try block () -> T</param>
        /// <param name="catchOperate">Operations in catch block: Ex -> Ex</param>
        /// <returns></returns>
        public static async Task<Try<T>> TryCatchAsync<T>(Func<Task<T>> operate, Func<Exception, Exception> catchOperate)
        {
            try
            { return new Try<T>(await operate()); }
            catch (Exception exception)
            { return new Try<T>(catchOperate(exception)); }
        }
        
    }
}
