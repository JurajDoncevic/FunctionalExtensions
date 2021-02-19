using FunctionalExtensions.GenericProvider;
using FunctionalExtensions.Tests.Data;
using FunctionalExtensions.Tests.NorthwindModels;
using FunctionalExtensions.Tests.PeopleModels;
using static FunctionalExtensions.Base.Try;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.DependencyInjection;
using FunctionalExtensions.Base.Results;
using Xunit;

namespace FunctionalExtensions.Tests
{
    public class GenericProviderCommandTests
    {
        private readonly NorthwindDbContext _northwindDbContext;
        private readonly PeopleDbContext _peopleDbContext;
        private readonly CategoryProvider _categoryProvider;
        private readonly FalseProvider _falseProvider;
        private readonly PersonProvider _personProvider;
        private readonly CountryProvider _countryProvider;

        public GenericProviderCommandTests([FromServices] NorthwindDbContext northwindDbContext, PeopleDbContext peopleDbContext)
        {
            _northwindDbContext = northwindDbContext;
            _categoryProvider = new CategoryProvider(northwindDbContext);
            _falseProvider = new FalseProvider(northwindDbContext);
            _personProvider = new PersonProvider(peopleDbContext);
            _countryProvider = new CountryProvider(peopleDbContext);
        }

        [Fact]
        public async void InsertSuccessTest()
        {
            var testCountry = new Country()
            {
                Name = "Test Country"
            };

            var result =
                await _countryProvider.Insert(testCountry);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorType.None, result.ErrorType);
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
            Assert.Equal(ErrorType.ExceptionThrown, result.ErrorType);
        }

        [Fact]
        public async void DeleteSuccessTest()
        {
            const long id = 5;
            var result =
                await _countryProvider.Delete(id);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorType.None, result.ErrorType);
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
            Assert.Equal(ErrorType.Failure, result.ErrorType);
        }

        [Fact]
        public async void UpdateSuccessTest()
        {
            const long id = 1;
            const string updatedName = "TEST NAME";
            var countryResult = await _countryProvider.Fetch(id);
            countryResult.Data.Name = updatedName;
            var result =
                await _countryProvider.Update(countryResult.Data);
            countryResult = await _countryProvider.Fetch(id);


            Assert.NotNull(countryResult.Data);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(ErrorType.None, result.ErrorType);
            Assert.Equal(updatedName, countryResult.Data.Name);
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
            Assert.Equal(ErrorType.Failure, result.ErrorType);
        }

        [Fact]
        public async void UpdateExceptionTest()
        {
            var result =
                await _countryProvider.Update(null);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorType.ExceptionThrown, result.ErrorType);
        }

        #region COUNTRY PROVIDER
        private class CountryProvider : BaseProvider<long, Country, PeopleDbContext>
        {
            internal CountryProvider(PeopleDbContext ctx) : base(ctx)
            {

            }
        }
        #endregion

        #region PERSON PROVIDER
        private class PersonProvider : BaseProvider<long, Person, PeopleDbContext>
        {
            internal PersonProvider(PeopleDbContext ctx) : base(ctx)
            {

            }
        }
        #endregion

        #region CATEGORY PROVIDER
        private class CategoryProvider : BaseProvider<long, Category, NorthwindDbContext>
        {
            internal CategoryProvider(NorthwindDbContext dbContext) : base(dbContext)
            {
            }
        }
        #endregion

        #region FALSE PROVIDER AND MODEL

        private class FalseModel : BaseModel<int> { }

        private class FalseProvider : BaseProvider<int, FalseModel, NorthwindDbContext>
        {
            internal FalseProvider(NorthwindDbContext ctx) : base(ctx) { }

            public async System.Threading.Tasks.Task<Base.Results.DataResult<List<FalseModel>>> FetchAllNoData() =>
                TryCatch<List<FalseModel>>(
                    () => null,
                    (ex) => ex
                    ).ToDataResult();
        }
        #endregion
    }


}
