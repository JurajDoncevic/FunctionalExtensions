using FunctionalExtensions.GenericProvider.Tests.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.GenericProvider.Tests.Providers
{
    public class PersonProvider : BaseProvider<long, Person, TestDbContext>
    {
        public PersonProvider(TestDbContext dbContext) : base(dbContext)
        {
        }
    }
}
