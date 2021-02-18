using FunctionalExtensions.Tests.Data;
using static FunctionalExtensions.Base.Try;
using static FunctionalExtensions.Base.ActionExtensions;
using static FunctionalExtensions.Base.FunctionalHelpers;
using FunctionalExtensions.Base.Results;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FunctionalExtensions.Base;

namespace FunctionalExtensions.Tests
{
    public class ResultTests
    {
        private readonly NorthwindDbContext _ctx;

        public ResultTests(NorthwindDbContext ctx)
        {
            _ctx = ctx;
        }

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
            Assert.Equal(ErrorType.None, result.ErrorType);
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
            Assert.Equal(ErrorType.ExceptionThrown, result.ErrorType);
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
            Assert.Equal(ErrorType.None, result.ErrorType);
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
            Assert.Equal(ErrorType.Failure, result.ErrorType);
        }
    }
}
