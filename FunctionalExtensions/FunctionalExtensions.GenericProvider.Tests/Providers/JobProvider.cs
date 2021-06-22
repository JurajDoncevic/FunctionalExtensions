using FunctionalExtensions.GenericProvider.Tests.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.GenericProvider.Tests.Providers
{
    public class JobProvider : BaseProvider<long, Job, TestDbContext>
    {
        public JobProvider(TestDbContext dbContext) : base(dbContext)
        {
        }
    }
}
