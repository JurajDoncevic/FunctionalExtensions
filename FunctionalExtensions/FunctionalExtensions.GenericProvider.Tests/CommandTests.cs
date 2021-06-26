using FunctionalExtensions.Base.Results;
using FunctionalExtensions.GenericProvider.Tests.Data;
using FunctionalExtensions.GenericProvider.Tests.Providers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FunctionalExtensions.GenericProvider.Tests
{
    public class CommandTests
    {
        private readonly JobProvider _jobProvider;
        private readonly FalseProvider _falseProvider;
        private readonly PersonProvider _personProvider;
        private readonly CountryProvider _countryProvider;
        private readonly PlaceProvider _placeProvider;
        private readonly TestDbContext _testDbContext;

        public CommandTests(IConfiguration configuration)
        {
            _testDbContext = DatabaseInitialization.CreateDbContext(configuration.GetConnectionString("InMemoryDb"));
            _personProvider = new PersonProvider(_testDbContext);
            _placeProvider = new PlaceProvider(_testDbContext);
            _jobProvider = new JobProvider(_testDbContext);
            _countryProvider = new CountryProvider(_testDbContext);
            _falseProvider = new FalseProvider(_testDbContext);
        }

        [Fact]
        public async void InsertSuccessTest()
        {
            var testCountry = new Country()
            {
                Name = "TestCountry"
            };

            var result =
                await _countryProvider.Insert(testCountry);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorTypes.None, result.ErrorType);

            _testDbContext.Remove(testCountry);
            await _testDbContext.SaveChangesAsync();
        }

        [Fact]
        public async void InsertFailTest()
        {
            var testCountry = new Country()
            {
                Name = null
            };

            var result =
                await _countryProvider.Insert(testCountry);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact]
        public async void DeleteSuccessTest()
        {
            var testCountry = new Country()
            {
                Name = "TestCountry"
            };

            await _testDbContext.AddAsync<Country>(testCountry);
            await _testDbContext.SaveChangesAsync();

            var result =
                await _countryProvider.Delete(testCountry.Id);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorTypes.None, result.ErrorType);
        }

        [Fact]
        public async void DeleteFailureTest()
        {
            const long id = 0;
            var result =
                await _countryProvider.Delete(id);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.Failure, result.ErrorType);
        }

        [Fact]
        public async void UpdateSuccessTest()
        {
            var testCountry = new Country()
            {
                Name = "Original Name"
            };
            await _testDbContext.AddAsync(testCountry);
            await _testDbContext.SaveChangesAsync();
            const string updatedName = "UpdatedName";

            testCountry.Name = updatedName;
            
            var result =
                await _countryProvider.Update(testCountry);
            var countryResult = await _countryProvider.Fetch(testCountry.Id);


            Assert.NotNull(countryResult.Data);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorTypes.None, result.ErrorType);
            Assert.Equal(updatedName, countryResult.Data.Name);

            _testDbContext.Remove(testCountry);
            await _testDbContext.SaveChangesAsync();
        }

        [Fact]
        public async void UpdateFailureTest()
        {
            var country = new Country()
            {
                Name = "Some country name"
            };
            var result =
                await _countryProvider.Update(country);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.Failure, result.ErrorType);
        }

        [Fact]
        public async void UpdateExceptionTest()
        {
            var result =
                await _countryProvider.Update(null);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }
    }
}
