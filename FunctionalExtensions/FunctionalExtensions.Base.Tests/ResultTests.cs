using static FunctionalExtensions.Base.Try;
using static FunctionalExtensions.Base.ActionExtensions;
using static FunctionalExtensions.Base.FunctionalHelpers;
using FunctionalExtensions.Base.Results;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FunctionalExtensions.Base;

namespace FunctionalExtensions.Base.Tests
{
    public class ResultTests
    {

        [Fact]
        public void ResultSuccess()
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
        public void ResultExceptionFail()
        {
            const string exceptionMessage = "Exception message";

            var result =
                TryCatch(
                    ((Action)(() => {throw new Exception(exceptionMessage); Console.WriteLine(); })).ToFunc(),
                    (ex) => ex
                    ).ToResult();

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact]
        public void ResultLogicSuccess()
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
        public void ResultLogicFail()
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
    }
}
