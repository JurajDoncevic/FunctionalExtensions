using FunctionalExtensions.GenericProvider.Tests.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalExtensions.GenericProvider.Tests
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(provider => new ConfigurationBuilder().
                                        AddJsonFile("appsettings.json")
                                        .Build());
        }
    }

    public static class LoadingHelpers
    {
        public static IServiceCollection LoadProviders(this IServiceCollection services) =>
            System.Reflection.Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      .ToList()
                                      .Where(t => !t.IsAbstract && t.Name.EndsWith("Provider"))
                                      .Aggregate(services, (s, type) => s.AddTransient(type));


    }
}
