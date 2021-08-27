﻿using static FunctionalExtensions.Base.Try;
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

        [Fact]
        public void ResultSuccessTest()
        {
            var result =
                TryCatch(
                    ((Action)(() => Console.WriteLine())).ToFunc(),
                    (ex) => ex
                    ).ToResult();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorTypes.None, result.ErrorType);
        }

        [Fact]
        public void ResultExceptionFailTest()
        {
            const string exceptionMessage = "Exception message";

            var result =
                TryCatch(
                    ((Action)(() => {throw new Exception(exceptionMessage); })).ToFunc(),
                    (ex) => ex
                    ).ToResult();

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact]
        public void ResultTypedExceptionFailTest()
        {
            var result =
                TryCatch(
                    ((Action)(() => { throw new ArgumentNullException(); })).ToFunc(),
                    (ex) => ex
                    ).ToResult();

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(new ArgumentNullException().Message, result.ErrorMessage);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact]
        public void ResultLogicSuccessTest()
        {
            const string exceptionMessage = "Exception message";

            var result =
                TryCatch(
                    () => true,
                    (ex) => ex
                    ).ToResult();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(string.Empty, result.ErrorMessage);
            Assert.Equal(ErrorTypes.None, result.ErrorType);
        }

        [Fact]
        public void ResultLogicFailTest()
        {

            var result =
                TryCatch(
                    () => false,
                    (ex) => ex
                    ).ToResult();

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal("Operation failed", result.ErrorMessage);
            Assert.Equal(ErrorTypes.Failure, result.ErrorType);
        }

        [Fact]
        public void ResultBindSuccessTest()
        {
            var result =
                GetLogicSuccess()
                .Bind(_ => GetSuccessResult())
                .Bind(_ => GetLogicSuccess())
                .Bind(_ => GetSuccessResult());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorTypes.None, result.ErrorType);
        }

        [Fact]
        public void ResultBindLogicFailTest()
        {
            var result =
                GetLogicSuccess()
                .Bind(_ => GetSuccessResult())
                .Bind(_ => GetLogicFail())
                .Bind(_ => GetSuccessResult());

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.Failure, result.ErrorType);
        }

        [Fact]
        public void ResultBindExceptionFailTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";
            var result =
                GetLogicSuccess()
                    .Bind(_ => GetSuccessResult())
                    .Bind(_ => GetExceptionFail(exceptionMessage))
                    .Bind(_ => GetSuccessResult());

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }

        [Fact]
        public async void ResultBindSuccessAsyncTest()
        {
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetLogicSuccessAsync())
                    .Bind(_ => GetSuccessResultAsync());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorTypes.None, result.ErrorType);
        }

        [Fact]
        public async void ResultBindLogicFailAsyncTest()
        {
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetLogicFailAsync())
                    .Bind(_ => GetSuccessResultAsync());

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.Failure, result.ErrorType);
        }

        [Fact]
        public async void ResultBindExceptionFailAsyncTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetExceptionFailAsync(exceptionMessage))
                    .Bind(_ => GetSuccessResultAsync());

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }

        #region HELPER METHODS
        private Result GetLogicFail()
            => TryCatch(
                    () => false,
                    (ex) => ex
                    ).ToResult();

        private Result GetLogicSuccess()
            => TryCatch(
                    () => true,
                    (ex) => ex
                    ).ToResult();

        private Result GetSuccessResult()
            => TryCatch(
                    ((Action)(() => Console.WriteLine())).ToFunc(),
                    (ex) => ex
                    ).ToResult();

        private Result GetExceptionFail(string exceptionMessage)
            => TryCatch(
                    ((Action)(() => { throw new Exception(exceptionMessage); })).ToFunc(),
                    (ex) => ex
                    ).ToResult();

        private async Task<Result> GetLogicFailAsync()
            => await TryCatch(
                    async () => { await Task.Run(() => 0); return false; },
                    (ex) => ex
                    ).ToResult();

        private async Task<Result> GetLogicSuccessAsync()
            => await TryCatch(
                    async () => { await Task.Run(() => 0); return true; },
                    (ex) => ex
                    ).ToResult();

        private async Task<Result> GetSuccessResultAsync()
            => await TryCatch(
                    async () => (await Task.Run(() => 0)).Ignore(),
                    (ex) => ex
                    ).ToResult();

        private async Task<Result> GetExceptionFailAsync(string exceptionMessage)
            => await TryCatch(
                    async () => { throw new Exception(exceptionMessage); return await Task.Run(() => Unit()); },
                    (ex) => ex
                    ).ToResult();
        #endregion
    }
}
