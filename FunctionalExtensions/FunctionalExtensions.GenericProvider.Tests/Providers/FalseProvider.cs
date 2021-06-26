using FunctionalExtensions.GenericProvider.Tests.Data;
using System;
using System.Collections.Generic;
using System.Text;
using FunctionalExtensions.Base.Results;
using static FunctionalExtensions.Base.Try;
using System.Threading.Tasks;

namespace FunctionalExtensions.GenericProvider.Tests.Providers
{
    public class FalseProvider : BaseProvider<int, FalseModel, TestDbContext>
    {
        public FalseProvider(TestDbContext ctx) : base(ctx) { }

        public async Task<DataResult<List<FalseModel>>> FetchAllNoData() =>
            TryCatch(
                (Func<List<FalseModel>>)(() => null),
                (ex) => ex
                ).ToDataResult();
    }
}
