using FunctionalExtensions.GenericProvider.Tests.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace FunctionalExtensions.GenericProvider.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<NorthwindDbContext>();
            services.LoadProviders();
        }
    }

    public static class LoadingHelpers
    {
        public static IServiceCollection LoadProviders (this IServiceCollection services) =>
            System.Reflection.Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      .ToList()
                                      .Where(t => !t.IsAbstract && t.Name.EndsWith("Provider"))
                                      .Aggregate(services, (s, type) => s.AddTransient(type));
    }
}
