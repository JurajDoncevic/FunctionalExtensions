using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.GenericProvider.Tests.Data
{
    public static class DatabaseInitialization
    {
        public static TestDbContext CreateDbContext(string connString)
        {
            SqliteConnection connection = new SqliteConnection(connString);
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new TestDbContext(options);
            context.Database.EnsureCreated();
            context.PopulateDatabase();
            return context;
        }

        private static void PopulateDatabase(this TestDbContext dbContext)
        {

            List<Country> countries = new List<Country>
            {
                new Country()
                {
                    Name = "Croatia"
                },
                new Country()
                {
                    Name = "United Kingdom"
                },
                new Country()
                {
                    Name = "France"
                },
                new Country()
                {
                    Name = "Slovenia"
                }
            };

            List<Place> places = new List<Place>
            {
                new Place()
                {
                    Name = "Zagreb",
                    CountryId = 1
                },
                new Place()
                {
                    Name = "London",
                    CountryId = 2
                },
                new Place()
                {
                    Name = "Rijeka",
                    CountryId = 1
                },
                new Place()
                {
                    Name = "Paris",
                    CountryId = 3
                },
                new Place()
                {
                    Name = "Ljubljana",
                    CountryId = 4
                }
            };

            List<Job> jobs = new List<Job>
            {
                new Job()
                {
                    Name = "Baker"
                },
                new Job()
                {
                    Name = "Programmer"
                },
                new Job()
                {
                    Name = "Teacher"
                }
            };

            List<Person> people = new List<Person>
            {
                new Person()
                {
                    FirstName = "Ivo",
                    LastName = "Ivic",
                    JobId = 1,
                    PlaceId = 1
                },
                new Person()
                {
                    FirstName = "Pero",
                    LastName = "Peric",
                    JobId = 2,
                    PlaceId = 1
                },
                new Person()
                {
                    FirstName = "John",
                    LastName = "Smith",
                    JobId = 2,
                    PlaceId = 2
                },
                new Person()
                {
                    FirstName = "Jean Luc",
                    LastName = "Picard",
                    JobId = 3,
                    PlaceId = 3
                },
                new Person()
                {
                    FirstName = "Jeremy",
                    LastName = "Jefferson",
                    JobId = 3,
                    PlaceId = 2
                },
            };

            dbContext.AddRange(countries);
            dbContext.SaveChanges();
            dbContext.AddRange(places);
            dbContext.SaveChanges();
            dbContext.AddRange(jobs);
            dbContext.SaveChanges();
            dbContext.AddRange(people);
            dbContext.SaveChanges();

        }
    }
}
