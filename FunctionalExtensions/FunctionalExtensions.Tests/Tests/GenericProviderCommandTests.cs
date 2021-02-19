﻿using FunctionalExtensions.GenericProvider;
using FunctionalExtensions.Tests.Data;
using FunctionalExtensions.Tests.NorthwindModels;
using FunctionalExtensions.Tests.PeopleModels;
using static FunctionalExtensions.Base.Try;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.DependencyInjection;
using FunctionalExtensions.Base.Results;

namespace FunctionalExtensions.Tests.Tests
{
    public class GenericProviderCommandTests
    {
        private readonly NorthwindDbContext _northwindDbContext;
        private readonly PeopleDbContext _peopleDbContext;
        private readonly CategoryProvider _categoryProvider;
        private readonly FalseProvider _falseProvider;
        private readonly PersonProvider _personProvider;

        public GenericProviderCommandTests([FromServices] NorthwindDbContext northwindDbContext, PeopleDbContext peopleDbContext)
        {
            _northwindDbContext = northwindDbContext;
            _categoryProvider = new CategoryProvider(northwindDbContext);
            _falseProvider = new FalseProvider(northwindDbContext);
            _personProvider = new PersonProvider(peopleDbContext);
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