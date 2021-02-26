using System;
using System.Collections.Generic;
using System.Text;
using FunctionalExtensions.Base.Results;
using FunctionalExtensions.Tests.Data;
using static FunctionalExtensions.Base.Try;
using Xunit;
using FunctionalExtensions.Base;
using System.Threading.Tasks;

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

        [Fact]
        public void DataResultBindingSuccess()
        {

            var dataResult1 =
                TryCatch(
                    () => 2,
                    (ex) => ex
                    ).ToDataResult();

            var dataResult2 =
                dataResult1.Bind(x => 
                    TryCatch(
                            () => "TEST" + x.ToString(),
                            (ex) => ex
                        ).ToDataResult());

            Assert.NotNull(dataResult2);
            Assert.True(dataResult2.IsSuccess);
            Assert.True(dataResult2.HasData);
            Assert.False(dataResult2.IsFailure);
            Assert.NotNull(dataResult2.Data);
            Assert.Equal("TEST2", dataResult2.Data);
        }

        [Fact]
        public void DataResultBindingExceptionFail()
        {
            const string exceptionMessage = "Exception message";

            var dataResult1 =
                TryCatch(
                    () => { throw new Exception(exceptionMessage); return 2; },
                    (ex) => ex
                    ).ToDataResult();

            var dataResult2 =
                dataResult1.Bind(x => 
                    TryCatch(
                        () => "TEST" + x.ToString(),
                        (ex) => ex
                        ).ToDataResult());

            Assert.NotNull(dataResult2);
            Assert.False(dataResult2.IsSuccess);
            Assert.True(dataResult2.IsFailure);
            Assert.Equal(exceptionMessage, dataResult2.ErrorMessage);
            Assert.Equal(ErrorType.ExceptionThrown, dataResult2.ErrorType);
        }

        [Fact]
        public void DataResultBinding4LevelSuccess()
        {

            var finalDataResult =
                TryCatch(
                    () => 1,
                    (ex) => ex
                    ).ToDataResult()
                .Bind(x => 
                    TryCatch(
                        () => x.ToString() + "2",
                        (ex) => ex
                        ).ToDataResult())
                .Bind(x =>
                    TryCatch(
                        () => x.ToString() + "3",
                        (ex) => ex
                        ).ToDataResult())
                .Bind(x =>
                    TryCatch(
                        () => x.ToString() + "4",
                        (ex) => ex
                        ).ToDataResult()
                );

            Assert.NotNull(finalDataResult);
            Assert.True(finalDataResult.IsSuccess);
            Assert.False(finalDataResult.IsFailure);
            Assert.True(finalDataResult.HasData);
            Assert.Equal("1234", finalDataResult.Data);         
        }

        [Fact]
        public void DataResultBinding4LevelFail()
        {
            const string exceptionMessage = "Exception at level 3";

            // Fails at level 3
            var finalDataResult =
                TryCatch(
                    () => 1,
                    (ex) => ex
                    ).ToDataResult()
                .Bind(x =>
                    TryCatch(
                        () => x.ToString() + "2",
                        (ex) => ex
                        ).ToDataResult())
                .Bind(x =>
                    TryCatch(
                        () => { throw new Exception(exceptionMessage); return "3"; },
                        (ex) => ex
                        ).ToDataResult())
                .Bind(x =>
                    TryCatch(
                        () => x.ToString() + "4",
                        (ex) => ex
                        ).ToDataResult()
                );

            Assert.NotNull(finalDataResult);
            Assert.False(finalDataResult.IsSuccess);
            Assert.True(finalDataResult.IsFailure);
            Assert.False(finalDataResult.HasData);
            Assert.Equal(exceptionMessage, finalDataResult.ErrorMessage);
            Assert.Equal(ErrorType.ExceptionThrown, finalDataResult.ErrorType);
        }

        [Fact]
        public async void DataResultSuccessAsyncMap()
        {
            var result =
                await TryCatchAsync(
                    async () => { await Task.Delay(100); return 1; },
                    (ex) => ex)
                .ToDataResultAsync()
                .MapAsync(_ => _.ToString());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.HasData);
            Assert.Equal("1", result.Data);
        }

        [Fact]
        public async void DataResultFailureAsyncMap()
        {
            const string exceptionMessage = "Exception occured";
            var result =
                await TryCatchAsync(
                    async () => { await Task.Delay(100); throw new Exception(exceptionMessage); return 1; },
                    (ex) => ex
                    ).ToDataResultAsync()
                    .MapAsync(_ => _ + 1);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.ExceptionThrown, result.ErrorType);
        }
    }
}
