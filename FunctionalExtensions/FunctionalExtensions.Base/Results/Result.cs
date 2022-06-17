using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base.Results;

public enum ResultTypes
{
    SUCCESS,
    FAILURE,
    EXCEPTION
}

/// <summary>
/// Logical result of an operation
/// </summary>
public struct Result
{
    private readonly bool _isSuccess;
    private readonly string _message;
    private readonly Exception _exception;
    private readonly ResultTypes _resultType;

    /// <summary>
    /// Signifies if the operation was successful
    /// </summary>
    public bool IsSuccess => _isSuccess;

    /// <summary>
    /// Operation outcome message
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// The operation's exception
    /// </summary>
    public Exception Exception => _exception;

    /// <summary>
    /// Signifies if an exception was thrown
    /// </summary>
    public bool HasException => _exception != null;

    /// <summary>
    /// Result type
    /// </summary>
    public ResultTypes ResultType => _resultType;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="isSucess"></param>
    /// <param name="message"></param>
    /// <param name="resultType"></param>
    /// <param name="exception"></param>
    internal Result(bool isSucess, string message, ResultTypes resultType, Exception exception)
    {
        _isSuccess = isSucess;
        _message = message;
        _exception = exception;
        _resultType = resultType;
    }

    /// <summary>
    /// Implicit bool operator
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator bool(Result result)
        => result.IsSuccess;

    /// <summary>
    /// Implicit operator to turn a bool into a Result
    /// </summary>
    /// <param name="isSuccess"></param>
    public static implicit operator Result(bool isSuccess)
        => isSuccess
        ? OnSuccess()
        : OnFailure();

    /// <summary>
    /// Creates a successful Result
    /// </summary>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result OnSuccess(string message!! = "Operation successful")
    => new Result(true, message, ResultTypes.SUCCESS, null);

    /// <summary>
    /// Creates a failure Result
    /// </summary>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result OnFailure(string message!! = "Operation failed")
        => new Result(false, message, ResultTypes.FAILURE, null);

    /// <summary>
    /// Creates an exception Result
    /// </summary>
    /// <param name="exception">Thrown exception</param>
    /// <returns></returns>
    public static Result OnException(Exception exception!!)
        => new Result(false, exception.Message, ResultTypes.EXCEPTION, exception);

    public override bool Equals(object obj)
    {
        return obj is Result result &&
               _isSuccess == result._isSuccess &&
               _message == result._message &&
               EqualityComparer<Exception>.Default.Equals(_exception, result._exception) &&
               _resultType == result._resultType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_isSuccess, _message, _exception, _resultType);
    }
}

/// <summary>
/// Data result of an operation
/// </summary>
/// <typeparam name="TData"></typeparam>
public struct Result<TData>
{
    private readonly TData _data;
    private readonly bool _isSuccess;
    private readonly string _message;
    private readonly Exception _exception;
    private readonly ResultTypes _resultType;

    public TData? Data => _data;

    /// <summary>
    /// Signifies if the operation was successful
    /// </summary>
    public bool IsSuccess => _isSuccess && HasData && !HasException;
    
    /// <summary>
    /// Operation outcome message
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// The operation's exception
    /// </summary>
    public Exception Exception => _exception;

    /// <summary>
    /// Result type
    /// </summary>
    public ResultTypes ResultType => _resultType;

    /// <summary>
    /// Signifies if the result has data
    /// </summary>

    public bool HasData => Data != null;

    /// <summary>
    /// Signifies if an exception was thrown
    /// </summary>
    public bool HasException => _exception != null;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data"></param>
    /// <param name="isSucess"></param>
    /// <param name="message"></param>
    /// <param name="resultType"></param>
    /// <param name="exception"></param>
    internal Result(TData data, bool isSucess, string message, ResultTypes resultType, Exception exception)
    {
        _data = data;
        _isSuccess = isSucess;
        _message = message;
        _resultType = resultType;
        _exception = exception;
    }

    /// <summary>
    /// Implicit bool operator
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator bool(Result<TData> result)
        => result.IsSuccess;

    /// <summary>
    /// Implicit operator to turn a value into a Result
    /// </summary>
    /// <param name="data"></param>
    public static implicit operator Result<TData>(TData data)
        => data != null
        ? OnSuccess(data)
        : OnFailure<TData>();

    /// <summary>
    /// Creates a successful operation result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">Result data</param>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result<T> OnSuccess<T>(T data!!, string message!! = "Operation successful")
        => new Result<T>(data, true, message, ResultTypes.SUCCESS, null);

    /// <summary>
    /// Creates a failure operation result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result<T> OnFailure<T>(string message!! = "Operation failed")
        => new Result<T>(default, false, message, ResultTypes.FAILURE, null);

    /// <summary>
    /// Creates an exception operation result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exception">The thrown exception</param>
    /// <param name="message">Outcome message</param>
    /// <returns></returns>
    public static Result<T> OnException<T>(Exception exception!!)
        => new Result<T>(default, false, exception.Message, ResultTypes.EXCEPTION, exception);

    public override bool Equals(object obj)
    {
        return obj is Result<TData> result &&
               _isSuccess == result._isSuccess &&
               _message == result._message &&
               _exception.Equals(result._exception) &&
               _resultType == result._resultType &&
                EqualityComparer<TData>.Default.Equals(_data, result._data);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_data, _isSuccess, _message, _exception, _resultType);
    }
}