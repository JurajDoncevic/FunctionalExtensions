using System;
using System.Collections.Generic;
using System.Text;
using FunctionalExtensions.Base.Results;
using static FunctionalExtensions.Base.Try;
using Xunit;
using FunctionalExtensions.Base;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base.Tests
{
    public class DataResultTests
    {

        [Fact]
        public void DataResultSuccessTest()
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
        public void DataResultExceptionFailTest()
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
            Assert.Equal(ErrorTypes.ExceptionThrown, dataResult.ErrorType);
        }

        [Fact]
        public void DataResultTypedExceptionFailTest()
        {

            var dataResult =
                TryCatch(
                    () => { throw new ArgumentNullException(); return new { Name = "test" }; },
                    (ex) => ex
                    ).ToDataResult();

            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);
            Assert.True(dataResult.IsFailure);
            Assert.Equal(new ArgumentNullException().Message, dataResult.ErrorMessage);
            Assert.Equal(ErrorTypes.ExceptionThrown, dataResult.ErrorType);
        }

        [Fact]
        public void DataResultBindingSuccessTest()
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
        public void DataResultBindingExceptionFailTest()
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
            Assert.Equal(ErrorTypes.ExceptionThrown, dataResult2.ErrorType);
        }

        [Fact]
        public void DataResultBinding4LevelSuccessTest()
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
        public void DataResultBinding4LevelFailTest()
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
            Assert.Equal(ErrorTypes.ExceptionThrown, finalDataResult.ErrorType);
        }

        [Fact]
        public async void DataResultSuccessAsyncMapTest()
        {
            var result =
                await TryCatch(
                    async () => { await Task.Delay(100); return 1; },
                    (ex) => ex)
                .ToDataResult()
                .Map((int _) => _.ToString());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.HasData);
            Assert.Equal("1", result.Data);
        }

        [Fact]
        public async void DataResultFailureAsyncMapTest()
        {
            const string exceptionMessage = "Exception occured";
            var result =
                await TryCatch(
                    async () => { await Task.Delay(100); throw new Exception(exceptionMessage); return 1; },
                    (ex) => ex
                    ).ToDataResult()
                    .Map(_ => _ + 1);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact]
        public async void DataResultSuccessBindAsyncTest()
        {
            var result =
                await GetSuccessResultAsync()
                    .Bind(_ => GetSuccessResultAsync2())
                    .Bind(_ => GetSuccessResultAsync());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.HasData);
            Assert.Equal(new PersonStruct { FirstName = "John", LastName = "Doe" }, result.Data);
        }

        [Fact]
        public async void DataResultFailBindAsyncTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";
            var result =
                await GetSuccessResultAsync()
                    .Bind(_ => GetSuccessResultAsync2())
                    .Bind(_ => GetExceptionFailAsync(exceptionMessage))
                    .Bind(_ => GetSuccessResultAsync());

            Assert.NotNull(result);
            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void DataResultSuccessFishTest()
        {

            var composition =
                ((Func<bool, DataResult<PersonStruct>>)GetResult)
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetSuccessResult());

            var result = composition(false);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.HasData);
            Assert.Equal(new PersonStruct { FirstName = "John", LastName = "Doe" }, result.Data);
        }

        [Fact]
        public void DataResultFailFishTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";

            var composition =
                ((Func<bool, DataResult<PersonStruct>>)GetResult)
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetExceptionFail(exceptionMessage))
                    .Fish(_ => GetSuccessResult());

            var result = composition(true);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async void DataResultSuccessFishAsyncTest()
        {

            var composition =
                ((Func<bool, Task<DataResult<PersonStruct>>>)GetResultAsync)
                    .Fish(_ => GetSuccessResultAsync())
                    .Fish(_ => GetSuccessResultAsync2())
                    .Fish(_ => GetSuccessResultAsync());

            var result = await composition(false);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.HasData);
            Assert.Equal(new PersonStruct { FirstName = "John", LastName = "Doe" }, result.Data);
        }

        [Fact]
        public async void DataResultFailFishAsyncTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";

            var composition =
                ((Func<bool, Task<DataResult<PersonStruct>>>)GetResultAsync)
                    .Fish(_ => GetSuccessResultAsync())
                    .Fish(_ => GetExceptionFailAsync(exceptionMessage))
                    .Fish(_ => GetSuccessResultAsync());

            var result = await composition(true);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async void AsDataResultSuccessAsyncTest()
        {
            var person = new PersonStruct { FirstName = "John", LastName = "Smith" };
            var result = await ResultExtensions.AsDataResult(async () => { await Task.Delay(0); return person; });

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.HasData);
            Assert.Equal(person, result.Data);
        }

        [Fact]
        public async void AsDataResultExceptionAsyncTest()
        {
            var person = new PersonStruct { FirstName = "John", LastName = "Smith" };
            var exceptionMessage = "test";
            var result = await ResultExtensions.AsDataResult(async () => { await Task.Delay(0); throw new Exception(exceptionMessage) ; return person; });

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }

        [Fact]
        public void AsDataResultSuccessTest()
        {
            var person = new PersonStruct { FirstName = "John", LastName = "Smith" };
            var result = ResultExtensions.AsDataResult(() =>  person);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.HasData);
            Assert.Equal(person, result.Data);
        }

        [Fact]
        public void AsDataResultExceptionTest()
        {
            var person = new PersonStruct { FirstName = "John", LastName = "Smith" };
            var exceptionMessage = "test";
            var result = ResultExtensions.AsDataResult(() => { throw new Exception(exceptionMessage); return person; });

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }

        [Fact]
        public void ImplicitOperatorSuccessReturn()
        {
            int? someValue = 5;

            var result =
                ResultExtensions.AsDataResult(() => someValue);

            Assert.True(result);
        }

        [Fact]
        public void ImplicitOperatorNullSuccessReturn()
        {
            int? someValue = null;

            var result =
                ResultExtensions.AsDataResult(() => someValue);

            Assert.False(result);
            Assert.Equal(ErrorTypes.NoData, result.ErrorType);
        }

        #region HELPER METHODS

        private DataResult<PersonStruct> GetResult(bool throwsException)
            => TryCatch(
                    () => { if (throwsException) throw new Exception(); return new PersonStruct { FirstName = "John", LastName = "Doe" }; },
                    (ex) => ex
                    ).ToDataResult();

        private async Task<DataResult<PersonStruct>> GetResultAsync(bool throwsException)
            => await TryCatch(
                    async () => { if (throwsException) throw new Exception(); return await Task.Run(() => new PersonStruct { FirstName = "John", LastName = "Doe" }); },
                    (ex) => ex
                    ).ToDataResult();

        private DataResult<PersonStruct> GetSuccessResult()
            => TryCatch(
                    () => new PersonStruct { FirstName = "John", LastName = "Doe" },
                    (ex) => ex
                    ).ToDataResult();

        private DataResult<PersonStruct> GetSuccessResult2()
            => TryCatch(
                    () => new PersonStruct { FirstName = "John", LastName = "Smith" },
                    (ex) => ex
                    ).ToDataResult();

        private DataResult<PersonStruct> GetExceptionFail(string exceptionMessage)
            => TryCatch(
                    () => { throw new Exception(exceptionMessage); return new PersonStruct { FirstName = "John", LastName = "Doe" }; },
                    (ex) => ex
                    ).ToDataResult();

        private async Task<DataResult<PersonStruct>> GetSuccessResultAsync()
            => await TryCatch(
                    async () => await Task.Run(() => new PersonStruct { FirstName = "John", LastName = "Doe" }),
                    (ex) => ex
                    ).ToDataResult();

        private async Task<DataResult<PersonStruct>> GetSuccessResultAsync2()
            => await TryCatch(
                    async () => await Task.Run(() => new PersonStruct { FirstName = "John", LastName = "Smith" }),
                    (ex) => ex
                    ).ToDataResult();

        private async Task<DataResult<PersonStruct>> GetExceptionFailAsync(string exceptionMessage)
            => await TryCatch(
                    async () => { throw new Exception(exceptionMessage); return await Task.Run(() => new PersonStruct { FirstName = "John", LastName = "Doe" }); },
                    (ex) => ex
                    ).ToDataResult();

        private struct PersonStruct
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        #endregion
    }
}
