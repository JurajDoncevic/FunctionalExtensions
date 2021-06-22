using FunctionalExtensions.GenericProvider.Tests.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.GenericProvider.Tests.Providers
{
    public class CountryProvider : BaseProvider<long, Country, TestDbContext>
    {
        public CountryProvider(TestDbContext dbContext) : base(dbContext)
        {
        }
    }
}
