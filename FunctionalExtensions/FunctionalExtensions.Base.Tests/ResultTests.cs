using static FunctionalExtensions.Base.Try;
using static FunctionalExtensions.Base.ActionExtensions;
using static FunctionalExtensions.Base.UnitExtensions;
using FunctionalExtensions.Base.Results;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FunctionalExtensions.Base;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base.Tests
{
    public class ResultTests
    {

        [Fact(DisplayName = "Get a success result")]
        public void ResultSuccessTest()
        {
            var result =
                Results.ResultExtensions.ToResult(TryCatch(
                    ((Action)(() => Console.WriteLine())).ToFunc(),
                    (ex) => ex
                    ));

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get an exception on Result")]
        public void ResultExceptionFailTest()
        {
            const string exceptionMessage = "Exception message";

            var result =
                Results.ResultExtensions.ToResult(TryCatch(
                    ((Action)(() => { throw new Exception(exceptionMessage); })).ToFunc(),
                    (ex) => ex
                    ));

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        }

        [Fact(DisplayName = "Get specific type of exception on Result")]
        public void ResultTypedExceptionFailTest()
        {
            var result =
                Results.ResultExtensions.ToResult(TryCatch(
                    ((Action)(() => { throw new ArgumentNullException(); })).ToFunc(),
                    (ex) => ex
                    ));

            Assert.False(result);
            Assert.IsType<ArgumentNullException>(result.Exception);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        }

        [Fact(DisplayName = "Get Result success")]
        public void ResultLogicSuccessTest()
        {

            var result =
                Results.ResultExtensions.ToResult(TryCatch(
                    () => true,
                    (ex) => ex
                    ));

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure")]
        public void ResultLogicFailTest()
        {

            var result =
                Results.ResultExtensions.ToResult(TryCatch(
                    () => false,
                    (ex) => ex
                    ));

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Use multiple Binds with success on Result")]
        public void ResultBindSuccessTest()
        {
            var result =
                GetLogicSuccess()
                .Bind(_ => GetSuccessResult())
                .Bind(_ => GetLogicSuccess())
                .Bind(_ => GetSuccessResult());

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure through Binds")]
        public void ResultBindLogicFailTest()
        {
            var result =
                GetLogicSuccess()
                .Bind(_ => GetSuccessResult())
                .Bind(_ => GetLogicFail())
                .Bind(_ => GetSuccessResult());

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Get Result exception through Binds")]
        public void ResultBindExceptionFailTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";
            var result =
                GetLogicSuccess()
                    .Bind(_ => GetSuccessResult())
                    .Bind(_ => GetExceptionFail(exceptionMessage))
                    .Bind(_ => GetSuccessResult());

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Equal(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Get Result success through async Binds")]
        public async void ResultBindSuccessAsyncTest()
        {
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetLogicSuccessAsync())
                    .Bind(_ => GetSuccessResultAsync());

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure through Binds async")]
        public async void ResultBindLogicFailAsyncTest()
        {
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetLogicFailAsync())
                    .Bind(_ => GetSuccessResultAsync());

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Get Result exception through Binds async")]
        public async void ResultBindExceptionFailAsyncTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetExceptionFailAsync(exceptionMessage))
                    .Bind(_ => GetSuccessResultAsync());

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Equal(exceptionMessage, result.Message);
        }



        [Fact(DisplayName = "Get Result success through Fish chain")]
        public void ResultFishSuccessTest()
        {
            var composition =
                ((Func<bool, Result>)GetResultByLogic)
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetLogicSuccess())
                    .Fish(_ => GetSuccessResult());


            var result = composition(true);

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure through Fish chain")]
        public void ResultFishLogicFailTest()
        {
            var composition =
                ((Func<bool, Result>)GetResultByLogic)
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetLogicSuccess())
                    .Fish(_ => GetSuccessResult());


            var result = composition(false);

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Get Result exception through Fish chain")]
        public void ResultFishExceptionFailTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";

            var composition =
                ((Func<bool, Result>)GetResultByLogic)
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetExceptionFail(exceptionMessage))
                    .Fish(_ => GetSuccessResult());


            var result = composition(true);

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Equal(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Get Result success through Fish chain async")]
        public async void ResultFishSuccessAsyncTest()
        {
            var composition =
                ((Func<bool, Task<Result>>)GetResultByLogicAsync)
                    .Fish(_ => GetSuccessResultAsync())
                    .Fish(_ => GetLogicSuccessAsync())
                    .Fish(_ => GetSuccessResultAsync());


            var result = await composition(true);

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure through Fish chain async")]
        public async void ResultFishLogicFailAsyncTest()
        {
            var composition =
                ((Func<bool, Task<Result>>)GetResultByLogicAsync)
                    .Fish(_ => GetSuccessResultAsync())
                    .Fish(_ => GetLogicSuccessAsync())
                    .Fish(_ => GetSuccessResultAsync());


            var result = await composition(false);

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Get Result exception through Fish chain async")]
        public async void ResultFishExceptionFailAsyncTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";

            var composition =
                ((Func<bool, Task<Result>>)GetResultByLogicAsync)
                    .Fish(_ => GetSuccessResultAsync())
                    .Fish(_ => GetExceptionFailAsync(exceptionMessage))
                    .Fish(_ => GetSuccessResultAsync());


            var result = await composition(true);

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Equal(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Get AsResult success async")]
        public async void AsResultSuccessAsyncTest()
        {
            var result = await ResultExtensions.AsResult(async () => { await Task.Delay(0); return true; });

            Assert.True(result);
        }

        [Fact(DisplayName = "Get AsResult failure async")]
        public async void AsResultFailAsyncTest()
        {
            var result = await ResultExtensions.AsResult(async () => { await Task.Delay(0); return false; });

            Assert.False(result);
        }

        [Fact(DisplayName = "Get AsResult exception async")]
        public async void AsResultExceptionAsyncTest()
        {
            var exceptionMessage = "test";
            var result = await ResultExtensions.AsResult(async () => { await Task.Delay(0); throw new Exception(exceptionMessage); return true; });

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Equal(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Get AsResult success")]
        public void AsResultSuccessTest()
        {
            var result = ResultExtensions.AsResult(() => true);

            Assert.True(result);
        }

        [Fact(DisplayName = "Get AsResult failure")]
        public void AsResultFailTest()
        {
            var result = ResultExtensions.AsResult(() => false);

            Assert.False(result);
        }

        [Fact(DisplayName = "Get AsResult exception")]
        public void AsResultExceptionTest()
        {
            var exceptionMessage = "test";
            var result = ResultExtensions.AsResult(() => { throw new Exception(exceptionMessage); return true; });

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Equal(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Run AsResult with operation returning Result")]
        public void AsResultAcceptingResultTest()
        {
            var successMessage = "Test success!";
            var failureMessage = "Test failure!";
            var exceptionToThrow = new Exception("Exception failure");

            var operationSuccess = () =>
            {
                return Result.OnSuccess(successMessage);
            };

            var operationFailure = () =>
            {
                return Result.OnFailure(failureMessage);
            };

            var operationException = () =>
            {
                return Result.OnException(exceptionToThrow);
            };

            var operationThrownException = () =>
            {
                throw exceptionToThrow;
                return Result.OnSuccess(successMessage); return Result.OnException(exceptionToThrow);
            };

            var successResult = ResultExtensions.AsResult(operationSuccess);
            var failureResult = ResultExtensions.AsResult(operationFailure);
            var exceptionResult = ResultExtensions.AsResult(operationException);
            var exceptionThrownResult = ResultExtensions.AsResult(operationThrownException);

            Assert.Equal(successMessage, successResult.Message);
            Assert.Equal(failureMessage, failureResult.Message);
            Assert.Equal(exceptionToThrow, exceptionResult.Exception);
            Assert.Equal(exceptionToThrow, exceptionThrownResult.Exception);
        }

        [Fact(DisplayName = "Run AsResult async with operation returning Result")]
        public async void AsResultAcceptingResultWithDataTest()
        {
            var successMessage = "Test success!";
            var failureMessage = "Test failure!";
            var exceptionToThrow = new Exception("Exception failure");

            var operationSuccess = async () =>
            {
                return await Task.FromResult(Result<int>.OnSuccess(1, successMessage)); ;
            };

            var operationFailure = async () =>
            {
                return await Task.FromResult(Result<int>.OnFailure(failureMessage));
            };

            var operationException = async () =>
            {
                return await Task.FromResult(Result<int>.OnException(exceptionToThrow));
            };

            var operationThrownException = async () =>
            {
                throw exceptionToThrow;
                return await Task.FromResult(Result<int>.OnSuccess(1, successMessage));
            };

            var successResult = await ResultExtensions.AsResult(operationSuccess);
            var failureResult = await ResultExtensions.AsResult(operationFailure);
            var exceptionResult = await ResultExtensions.AsResult(operationException);
            var exceptionThrownResult = await ResultExtensions.AsResult(operationThrownException);

            Assert.Equal(successMessage, successResult.Message);
            Assert.Equal(1, successResult.Data);
            Assert.Equal(failureMessage, failureResult.Message);
            Assert.Equal(exceptionToThrow, exceptionResult.Exception);
            Assert.Equal(exceptionToThrow, exceptionThrownResult.Exception);
        }

        [Fact(DisplayName = "Run AsResult async with operation returning Result")]
        public async void AsResultAsyncAcceptingResultWithDataTest()
        {
            var successMessage = "Test success!";
            var failureMessage = "Test failure!";
            var exceptionToThrow = new Exception("Exception failure");

            var operationSuccess = async () =>
            {
                return await Task.FromResult(Result<int>.OnSuccess(1, successMessage));
            };

            var operationFailure = async () =>
            {
                return await Task.FromResult(Result<int>.OnFailure(failureMessage));
            };

            var operationException = async () =>
            {
                return await Task.FromResult(Result<int>.OnException(exceptionToThrow));
            };

            var operationThrownException = async () =>
            {
                throw exceptionToThrow;
                return await Task.FromResult(Result<int>.OnSuccess(1, successMessage));
            };

            var successResult = await ResultExtensions.AsResult(operationSuccess);
            var failureResult = await ResultExtensions.AsResult(operationFailure);
            var exceptionResult = await ResultExtensions.AsResult(operationException);
            var exceptionThrownResult = await ResultExtensions.AsResult(operationThrownException);

            Assert.Equal(successMessage, successResult.Message);
            Assert.Equal(1, successResult.Data);
            Assert.Equal(failureMessage, failureResult.Message);
            Assert.Equal(exceptionToThrow, exceptionResult.Exception);
            Assert.Equal(exceptionToThrow, exceptionThrownResult.Exception);
        }

        #region HELPER METHODS
        private Result GetLogicFail()
            => Results.ResultExtensions.ToResult(TryCatch(
                    () => false,
                    (ex) => ex
                    ));

        private Result GetLogicSuccess()
            => Results.ResultExtensions.ToResult(TryCatch(
                    () => true,
                    (ex) => ex
                    ));

        private Result GetSuccessResult()
            => Results.ResultExtensions.ToResult(TryCatch(
                    ((Action)(() => Console.WriteLine())).ToFunc(),
                    (ex) => ex
                    ));

        private Result GetExceptionFail(string exceptionMessage)
            => Results.ResultExtensions.ToResult(TryCatch(
                    ((Action)(() => { throw new Exception(exceptionMessage); })).ToFunc(),
                    (ex) => ex
                    ));

        private async Task<Result> GetLogicFailAsync()
            => await Results.ResultExtensions.ToResult(TryCatch(
                    async () => { await Task.Run(() => 0); return false; },
                    (ex) => ex
                    ));

        private async Task<Result> GetLogicSuccessAsync()
            => await Results.ResultExtensions.ToResult(TryCatch(
                    async () => { await Task.Run(() => 0); return true; },
                    (ex) => ex
                    ));

        private async Task<Result> GetSuccessResultAsync()
            => await Results.ResultExtensions.ToResult(TryCatch(
                    async () => (await Task.Run(() => 0)).Ignore(),
                    (ex) => ex
                    ));

        private async Task<Result> GetExceptionFailAsync(string exceptionMessage)
            => await Results.ResultExtensions.ToResult(TryCatch(
                    async () => { throw new Exception(exceptionMessage); return await Task.Run(() => UnitExtensions.Unit()); },
                    (ex) => ex
                    ));

        private Result GetResultByLogic(bool isSuccess)
            => Results.ResultExtensions.ToResult(TryCatch(
                    () => isSuccess,
                    (ex) => ex
                    ));

        private async Task<Result> GetResultByLogicAsync(bool isSuccess)
            => await Results.ResultExtensions.ToResult(TryCatch(
                    async () => { await Task.Run(() => 0); return isSuccess; },
                    (ex) => ex
                    ));
        #endregion
    }
}
