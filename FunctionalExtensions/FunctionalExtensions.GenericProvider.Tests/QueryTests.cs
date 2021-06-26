using FunctionalExtensions.GenericProvider.Tests.Data;
using FunctionalExtensions.GenericProvider.Tests.Providers;
using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using static FunctionalExtensions.Base.Try;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FunctionalExtensions.GenericProvider.Tests
{
    public class QueryTests
    {
        private readonly PersonProvider _personProvider;
        private readonly PlaceProvider _placeProvider;
        private readonly JobProvider _jobProvider;
        private readonly CountryProvider _countryProvider;
        private readonly FalseProvider _falseProvider;


        public QueryTests(IConfiguration configuration)
        {
            var context = DatabaseInitialization.CreateDbContext(configuration.GetConnectionString("InMemoryDb"));
            _personProvider = new PersonProvider(context);
            _placeProvider = new PlaceProvider(context);
            _jobProvider = new JobProvider(context);
            _countryProvider = new CountryProvider(context);
            _falseProvider = new FalseProvider(context);
        }

        [Fact]
        public async void FetchSuccessTest()
        {
            long existingId = 1;

            var goodResultTask =
                _countryProvider.Fetch(existingId);

            var goodResult = await goodResultTask;
            Assert.NotNull(goodResult);
            Assert.True(goodResult.IsSuccess);
            Assert.False(goodResult.IsFailure);
            Assert.True(goodResult.HasData);
            Assert.Equal(ErrorTypes.None, goodResult.ErrorType);
            Assert.NotNull(goodResult.Data);
        }

        [Fact]
        public async void FetchFailWithNoDataTest()
        {
            long noSuchId = 0;

            var noResultTask =
                _countryProvider.Fetch(noSuchId);

            var noResult = await noResultTask;
            Assert.NotNull(noResult);
            Assert.False(noResult.IsSuccess);
            Assert.True(noResult.IsFailure);
            Assert.False(noResult.HasData);
            Assert.Equal(ErrorTypes.NoData, noResult.ErrorType);
            Assert.Null(noResult.Data);
        }

        [Fact]
        public async void FetchFailWithExceptionTest()
        {
            int id = 1;

            var resultTask =
                _falseProvider.Fetch(id);

            var result = await resultTask;
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.False(result.HasData);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
            Assert.Null(result.Data);
        }

        [Fact]
        public async void FetchAllSuccessTest()
        {

            var goodResultTask =
                _countryProvider.FetchAll();

            var goodResult = await goodResultTask;
            Assert.NotNull(goodResult);
            Assert.True(goodResult.IsSuccess);
            Assert.False(goodResult.IsFailure);
            Assert.True(goodResult.HasData);
            Assert.Equal(ErrorTypes.None, goodResult.ErrorType);
            Assert.NotNull(goodResult.Data);
            Assert.NotEmpty(goodResult.Data);
        }

        [Fact]
        public async void FetchAllFailWithNoDataTest()
        {
            var resultTask =
                _falseProvider.FetchAllNoData();

            var result = await resultTask;
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.False(result.HasData);
            Assert.Equal(ErrorTypes.NoData, result.ErrorType);
            Assert.Null(result.Data);
        }

        [Fact]
        public async void FetchAllFailWithExceptionTest()
        {
            var resultTask =
                _falseProvider.FetchAll();

            var result = await resultTask;
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.False(result.HasData);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
            Assert.Null(result.Data);
        }

        [Fact]
        public async void FetchSuccessIncludingAggregate()
        {
            long id = 2;
            var resultTask =
                _personProvider.FetchIncluding(id, _ => _.Place, _ => _.Place.Country, _ => _.Job);

            var result = await resultTask;

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.HasData);
            Assert.Equal(ErrorTypes.None, result.ErrorType);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Place);
            Assert.NotNull(result.Data.Place.Country);
            Assert.NotNull(result.Data.Job);
        }

        [Fact]
        public async void FetchAllSuccessIncludingAggregate()
        {
            var resultTask =
                _personProvider.FetchAllIncluding(_ => _.Place, _ => _.Place.Country, _ => _.Job);

            var result = await resultTask;

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.HasData);
            Assert.Equal(ErrorTypes.None, result.ErrorType);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.All(result.Data,
                    (item) =>
                    {
                        Assert.NotNull(item.Job);
                        Assert.NotNull(item.Place);
                        Assert.NotNull(item.Place.Country);
                    }
                    );
        }


        [Fact]
        public void FetchChainedWithBind()
        {
            var chainedResults =
                _personProvider.FetchAll().Result
                               .Bind(_ => _placeProvider.Fetch(_.First().PlaceId.Value).Result)
                               .Bind(_ => _countryProvider.Fetch(_.CountryId).Result)
                               .Map(_ => _.Name);

            Assert.NotNull(chainedResults);
            Assert.True(chainedResults.IsSuccess);
            Assert.False(chainedResults.IsFailure);
            Assert.True(chainedResults.HasData);
            Assert.True(chainedResults.Data.Length > 0);

        }

        [Fact]
        public async void FetchChainedWithBindAsync()
        {
            var chainedResults =
                await _personProvider.FetchAll()
                                     .Bind(_ => _placeProvider.Fetch(_.First().PlaceId.Value))
                                     .Bind(_ => _countryProvider.Fetch(_.CountryId))
                                     .Map(_ => _.Name);

            Assert.NotNull(chainedResults);
            Assert.True(chainedResults.IsSuccess);
            Assert.False(chainedResults.IsFailure);
            Assert.True(chainedResults.HasData);
            Assert.True(chainedResults.Data.Length > 0);

        }


    }
}
