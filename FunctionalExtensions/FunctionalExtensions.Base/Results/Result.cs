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

public struct Result
{
    private readonly bool _isSuccess;
    private readonly string _message;
    private readonly Exception _exception;
    private readonly ResultTypes _resultType;

    public bool IsSuccess => _isSuccess;

    public string Message => _message;

    public Exception Exception => _exception;

    public ResultTypes ResultType => _resultType;

    internal Result(bool isSucess, string message, ResultTypes resultType, Exception exception)
    {
        _isSuccess = isSucess;
        _message = message;
        _exception = exception;
        _resultType = resultType;
    }

    public static implicit operator bool(Result result)
        => result.IsSuccess;

    public static implicit operator Result(bool isSuccess)
        => isSuccess
        ? OnSuccess()
        : OnFailure();

    public static Result OnSuccess(string message!! = "Operation successful")
    => new Result(true, message, ResultTypes.SUCCESS, null);

    public static Result OnFailure(string message!! = "Operation failed")
        => new Result(false, message, ResultTypes.FAILURE, null);

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


public struct Result<TData>
{
    private readonly TData _data;
    private readonly bool _isSuccess;
    private readonly string _message;
    private readonly Exception _exception;
    private readonly ResultTypes _resultType;

    public TData? Data => _data;

    public bool IsSuccess => _isSuccess && HasData && !HasException;

    public string Message => _message;

    public Exception Exception => _exception;

    public ResultTypes ResultType => _resultType;

    public bool HasData => Data != null;
    public bool HasException => _exception != null;

    internal Result(TData data, bool isSucess, string message, ResultTypes resultType, Exception exception)
    {
        _data = data;
        _isSuccess = isSucess;
        _message = message;
        _resultType = resultType;
        _exception = exception;
    }

    public static implicit operator bool(Result<TData> result)
        => result.IsSuccess;

    public static implicit operator Result<TData>(TData data)
        => data != null
        ? OnSuccess(data)
        : OnFailure<TData>();

    public static Result<T> OnSuccess<T>(T data!!, string message!! = "Operation successful")
        => new Result<T>(data, true, message, ResultTypes.SUCCESS, null);

    public static Result<T> OnFailure<T>(string message!! = "Operation failed")
        => new Result<T>(default, false, message, ResultTypes.FAILURE, null);

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