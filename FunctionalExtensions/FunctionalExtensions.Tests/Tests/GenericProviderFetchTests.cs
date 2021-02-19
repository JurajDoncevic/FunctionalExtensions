using FunctionalExtensions.GenericProvider;
using FunctionalExtensions.Tests.Data;
using FunctionalExtensions.Tests.NorthwindModels;
using static FunctionalExtensions.Base.Try;
using static FunctionalExtensions.Base.Results.ResultExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.DependencyInjection;
using FunctionalExtensions.Tests.PeopleModels;

namespace FunctionalExtensions.Tests
{

    public class GenericProviderFetchTests
    {
        private readonly NorthwindDbContext _northwindDbContext;
        private readonly PeopleDbContext _peopleDbContext;
        private readonly CategoryProvider _categoryProvider;
        private readonly FalseProvider _falseProvider;
        private readonly PersonProvider _personProvider;

        public GenericProviderFetchTests([FromServices] NorthwindDbContext northwindDbContext, PeopleDbContext peopleDbContext)
        {
            _northwindDbContext = northwindDbContext;
            _categoryProvider = new CategoryProvider(northwindDbContext);
            _falseProvider = new FalseProvider(northwindDbContext);
            _personProvider = new PersonProvider(peopleDbContext);
        }

        [Fact]
        public async void FetchSuccessTest()
        {
            long existingId = 1;

            var goodResultTask =
                _categoryProvider.Fetch(existingId);

            var goodResult = await goodResultTask;
            Assert.NotNull(goodResult);
            Assert.True(goodResult.IsSuccess);
            Assert.False(goodResult.IsFailure);
            Assert.True(goodResult.HasData);
            Assert.Equal(Base.Results.ErrorType.None, goodResult.ErrorType);
            Assert.NotNull(goodResult.Data);
        }

        [Fact]
        public async void FecthFailWithNoDataTest()
        {
            long noSuchId = 0;

            var noResultTask =
                _categoryProvider.Fetch(noSuchId);

            var noResult = await noResultTask;
            Assert.NotNull(noResult);
            Assert.False(noResult.IsSuccess);
            Assert.True(noResult.IsFailure);
            Assert.False(noResult.HasData);
            Assert.Equal(Base.Results.ErrorType.NoData, noResult.ErrorType);
            Assert.Null(noResult.Data);
        }

        [Fact]
        public async void FecthFailWithExceptionTest()
        {
            int id = 1;

            var resultTask =
                _falseProvider.Fetch(id);

            var result = await resultTask;
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.False(result.HasData);
            Assert.Equal(Base.Results.ErrorType.ExceptionThrown, result.ErrorType);
            Assert.Null(result.Data);
        }

        [Fact]
        public async void FetchAllSuccessTest()
        {

            var goodResultTask =
                _categoryProvider.FetchAll();

            var goodResult = await goodResultTask;
            Assert.NotNull(goodResult);
            Assert.True(goodResult.IsSuccess);
            Assert.False(goodResult.IsFailure);
            Assert.True(goodResult.HasData);
            Assert.Equal(Base.Results.ErrorType.None, goodResult.ErrorType);
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
            Assert.Equal(Base.Results.ErrorType.NoData, result.ErrorType);
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
            Assert.Equal(Base.Results.ErrorType.ExceptionThrown, result.ErrorType);
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
            Assert.Equal(Base.Results.ErrorType.None, result.ErrorType);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Place);
            Assert.NotNull(result.Data.Place.Country);
            Assert.NotNull(result.Data.Job);
        }

        [Fact]
        public async void FetchAllSuccessIncludingAggregate()
        {
            var resultTask =
                _personProvider.FetchAllIncluding( _ => _.Place, _ => _.Place.Country, _ => _.Job);

            var result = await resultTask;

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.HasData);
            Assert.Equal(Base.Results.ErrorType.None, result.ErrorType);
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
