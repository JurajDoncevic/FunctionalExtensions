using FunctionalExtensions.GenericProvider.Tests.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.GenericProvider.Tests.Providers
{
    public class PlaceProvider : BaseProvider<long, Place, TestDbContext>
    {
        public PlaceProvider(TestDbContext dbContext) : base(dbContext)
        {
        }
    }
}
