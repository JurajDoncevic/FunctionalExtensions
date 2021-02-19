using System;
using System.Collections.Generic;
using System.Text;
using FunctionalExtensions.Base.Results;
using FunctionalExtensions.Tests.Data;
using static FunctionalExtensions.Base.Try;
using Xunit;

namespace FunctionalExtensions.Tests
{
    public class DataResultTests
    {
        private readonly NorthwindDbContext _ctx;

        public DataResultTests(NorthwindDbContext ctx)
        {
            _ctx = ctx;
        }

        [Fact]
        public void DataResultSuccess()
        {
            var dataResult =
                TryCatch(
                    () => new { Name = "test" },
                    (ex) => ex
                    ).ToDataResult();

            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);
            Assert.False(dataResult.IsFailure);
            Assert.Equal("test", dataResult.Data.Name);
        }

        [Fact]
        public void DataResultExceptionFail()
        {
            const string exceptionMessage = "Exception message";

            var dataResult =
                TryCatch(
                    () => { throw new Exception(exceptionMessage); return new { Name = "test" }; },
                    (ex) => ex
                    ).ToDataResult();

            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);
            Assert.True(dataResult.IsFailure);
            Assert.Equal(exceptionMessage, dataResult.ErrorMessage);
            Assert.Equal(ErrorType.ExceptionThrown, dataResult.ErrorType);
        }
    }
}
