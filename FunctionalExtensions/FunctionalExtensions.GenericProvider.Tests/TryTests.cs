using FunctionalExtensions.GenericProvider.Tests.Data;
using static FunctionalExtensions.Base.Try;
using static FunctionalExtensions.Base.ActionExtensions;
using static FunctionalExtensions.Base.FunctionalHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FunctionalExtensions.GenericProvider.Tests
{

    public class TryTests
    {
        private readonly NorthwindDbContext _ctx;

        public TryTests(NorthwindDbContext ctx)
        {
            _ctx = ctx;
        }

        [Fact]
        public void UnitTrySuccessTest()
        {
            var unitTry =
                TryCatch(
                    ((Action)(() => Console.WriteLine())).ToFunc(),
                    (ex) => ex
                    );

            Assert.NotNull(unitTry);
            Assert.False(unitTry.IsData);
            Assert.False(unitTry.IsException);
            Assert.Equal(Unit(), unitTry.ExpectedData);
        }

        [Fact]
        public void DataTrySuccessTest()
        {
            const string testString = "Test";
            var stringTry =
                TryCatch(
                    () => testString,
                    (ex) => ex
                    );

            Assert.NotNull(stringTry);
            Assert.True(stringTry.IsData);
            Assert.False(stringTry.IsException);
            Assert.Equal(testString, stringTry.ExpectedData);
        }

        [Fact]
        public void TryExceptionTest()
        {
            const string exceptionMessage = "Some exception";

            var exceptionTry =
                TryCatch(
                    ((Action)(() =>
                    {
                        throw new Exception(exceptionMessage);
                    })).ToFunc(),
                    (ex) => ex
                    );

            Assert.NotNull(exceptionTry);
            Assert.False(exceptionTry.IsData);
            Assert.True(exceptionTry.IsException);
            Assert.Equal(exceptionMessage, exceptionTry.Exception.Message);
        }
    }
}
