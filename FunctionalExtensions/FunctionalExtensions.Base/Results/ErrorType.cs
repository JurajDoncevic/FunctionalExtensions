using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.Base.Results
{
    /// <summary>
    /// Possible error types for Results
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// An exception was thrown
        /// </summary>
        ExceptionThrown,
        /// <summary>
        /// Some logical failure occured
        /// </summary>
        Failure,
        /// <summary>
        /// No data was returned when expected
        /// </summary>
        NoData,
        /// <summary>
        /// Unknown error occured
        /// </summary>
        Unknown,
        /// <summary>
        /// No error occured, operation successful
        /// </summary>
        None
    }
}
